using CmsData;
using CmsData.Codes;
using CmsWeb.Models;
using Newtonsoft.Json;
using SeatsioDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models.Ticketing
{
    public class TicketingModel : IDbBinder
    {
        public CMSDataContext CurrentDatabase { get; set; }

        // Binding Items
        public int MeetingId { get; set; }
        public int OrderId { get; set; }
        public string SelectedSeats { get; set; }
        public int? PeopleId { get; set; }

        // Items set in Initialize
        public string Pricing { get; set; }
        public string Workspacekey { get; set; }
        public string Eventkey { get; set; }
        public string Secretkey { get; set; }
        public int? CurrentUserPeopleId { get; set; }
        private Meeting Event { get; set; }

        // Helpers
        public HtmlString HtmlPricing => new HtmlString(Pricing);

        public TicketingModel(CMSDataContext db, int meetingId)
        {
            CurrentDatabase = db;
            MeetingId = meetingId;
            Initialize();
        }

        // ReSharper disable once UnusedMember.Global
        public TicketingModel()
        {
        }

        private void Initialize()
        {
            Event = CurrentDatabase.Meetings.Single(vv => vv.MeetingId == MeetingId);
            if (!Event.MeetingDate.HasValue)
                throw new Exception($"TicketingModel: meeting date missing for meetingId {MeetingId}");
            Eventkey = Event.EventKey;
            Secretkey = CurrentDatabase.Setting("TicketingWorkspaceSecretKey", "na");
            Workspacekey = CurrentDatabase.Setting("TicketingWorkspaceKey", "na");
            Pricing = Meeting.GetExtra(CurrentDatabase, MeetingId, "Pricing");
            if(!Pricing.HasValue())
                Pricing = Organization.GetExtra(CurrentDatabase, Event.OrganizationId, "Pricing");
        }

        public void BookSeats()
        {
            if (SelectedSeats == null)
                throw new Exception("No Seats Selected");
            Initialize();
            var seatList = SelectedSeats.SplitStr(",");
            var client = new SeatsioClient(Secretkey);
            var booking = client.Events.Book(Event.EventKey, seatList, orderId: OrderId.ToString());
            var dict = JsonConvert.DeserializeObject<List<CatPrice>>(Pricing)
                .ToDictionary(k => k.category, v => v.price);

            var order = new TicketingOrder()
            {
                Count = seatList.Length,
                MeetingId = MeetingId,
                SelectedSeats = SelectedSeats,
                CreatedDate = DateTime.Now,
                Status = "Booked",
                PeopleId = PeopleId  // may be null until after login or create account
            };
            CurrentDatabase.TicketingOrders.InsertOnSubmit(order);
            foreach (var seat in seatList)
            {
                var i = booking.Objects[seat];
                order.TicketingSeats.Add(new TicketingSeat
                {
                    SeatLabel = i.Label,
                    Price = dict[i.CategoryKey ?? 0],
                    Category = i.CategoryLabel,
                    CategoryKey = i.CategoryKey,
                    Section = i.Section,
                    Row = i.Labels.Parent.Label,
                    Seat = i.Labels.Own.Label.ToInt2(),
                });
            }
            order.TotalPrice = order.TicketingSeats.Sum(vv => vv.Price ?? 0);
            CurrentDatabase.SubmitChanges();
            OrderId = order.OrderId;
            CurrentUserPeopleId = CurrentDatabase.UserPeopleId ?? 1;
        }

        public void PaymentSucceeded()
        {
            Initialize();
            if(!PeopleId.HasValue)
                throw new Exception("no peopleid on ticketing payment");
            var order = CurrentDatabase.TicketingOrders.Single(vv => vv.OrderId == OrderId);
            order.Status = "Paid";
            order.PurchaseDate = DateTime.Now;
            order.PeopleId = PeopleId;
            var member = OrganizationMember.InsertOrgMembers(
                CurrentDatabase, Event.OrganizationId, PeopleId.Value, MemberTypeCode.Member, Util.Now);
            member.AddToExtraText("orders", OrderId.ToString());
            CurrentDatabase.SubmitChanges();
        }

        public void AbandonSeats()
        {
            Initialize();
            var order = CurrentDatabase.TicketingOrders.Single(vv => vv.OrderId == OrderId);
            var seats = order.TicketingSeats;
            var listseats = seats.Select(vv => vv.SeatLabel).ToList();
            var client = new SeatsioClient(Secretkey);
            client.Events.Release(Event.EventKey, listseats, orderId:OrderId.ToString());
            order.Status = "Abandoned";
            order.PeopleId = PeopleId;
            CurrentDatabase.SubmitChanges();
        }
    }
}
