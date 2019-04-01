using CmsData;
using CmsData.Registration;
using CmsWeb.Code;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Areas.Org.Models
{
    public class SettingsMessagesModel
    {
        public Organization Org;

        public int Id
        {
            get { return Org != null ? Org.OrganizationId : 0; }
            set
            {
                if (Org == null)
                {
                    Org = DbUtil.Db.LoadOrganizationById(value);
                }
            }
        }

        public SettingsMessagesModel()
        {
        }

        public SettingsMessagesModel(int id, bool formatHtml = false)
        {
            Id = id;
            this.CopyPropertiesFrom(Org, typeof(OrgAttribute));
            this.CopyPropertiesFrom(RegSettings, typeof(RegAttribute));
            //            if (formatHtml)
            //            {
            //                FormatHtml(x => x.Body);
            //                FormatHtml(x => x.ReminderBody);
            //                FormatHtml(x => x.SenderBody);
            //                FormatHtml(x => x.SupportBody);
            //                FormatHtml(x => x.InstructionSpecial);
            //                FormatHtml(x => x.InstructionFind);
            //                FormatHtml(x => x.InstructionLogin);
            //                FormatHtml(x => x.InstructionOptions);
            //                FormatHtml(x => x.InstructionSelect);
            //                FormatHtml(x => x.InstructionSorry);
            //                FormatHtml(x => x.InstructionSubmit);
            //            }
        }

        //        void FormatHtml(Expression<Func<SettingsMessagesModel, string>> o)
        //        {
        //            var expr = (MemberExpression)o.Body;
        //            var prop = (PropertyInfo)expr.Member;
        //            var s = prop.GetValue(this) as string;
        //            if (!s.HasValue())
        //                return;
        //            prop.SetValue(this, TidyLib.FormatHtml(s));
        //        }
        public void Update()
        {
            this.CopyPropertiesTo(Org, typeof(OrgAttribute));
            this.CopyPropertiesTo(RegSettings, typeof(RegAttribute));
            var os = DbUtil.Db.CreateRegistrationSettings(RegSettings.ToString(), Id);
            Org.UpdateRegSetting(os);
            DbUtil.Db.SubmitChanges();
        }

        private Settings RegSettings => _regsettings ?? (_regsettings = DbUtil.Db.CreateRegistrationSettings(Id));
        private Settings _regsettings;

        [Reg, Display(Description = ConfirmationDescription)]
        [DisplayName("Confirmation Subject")]
        public string Subject { get; set; }

        [Reg, DisplayName("Confirmation Body")]
        public string Body { get; set; }

        [Org, Display(Description = NotifyIdsDescription)]
        [DisplayName("Online Notify Persons")]
        public string NotifyIds { get; set; }

        [Reg, Display(Description = LoginDescription)]
        [DisplayName("Login Instructions")]
        public string InstructionLogin { get; set; }

        [Reg, Display(Description = SelectDescription)]
        [DisplayName("Select Instructions")]
        public string InstructionSelect { get; set; }

        [Reg, Display(Description = FindDescription)]
        [DisplayName("Find Instructions")]
        public string InstructionFind { get; set; }

        [Reg, Display(Description = OptionsDescription)]
        [DisplayName("Options Instructions")]
        public string InstructionOptions { get; set; }

        [Reg, Display(Description = SpecialDescription)]
        [DisplayName("Special Instructions")]
        public string InstructionSpecial { get; set; }

        [Reg, Display(Description = SubmitDescription)]
        [DisplayName("Submit Instructions")]
        public string InstructionSubmit { get; set; }

        [Reg, Display(Description = SorryDescription)]
        [DisplayName("Sorry Instructions")]
        public string InstructionSorry { get; set; }

        [Reg, Display(Description = ThankYouDescription)]
        public string ThankYouMessage { get; set; }

        [Reg, Display(Description = TermsDescription)]
        public string Terms { get; set; }

        [Reg, Display(Description = ReminderDescription)]
        public string ReminderSubject { get; set; }

        [Reg]
        public string ReminderBody { get; set; }

        [Org, Display(Description = GiftNotifyIdsDescription)]
        [DisplayName("Notify Persons for Gift")]
        public string GiftNotifyIds { get; set; }

        [Reg, Display(Description = SupportDescription)]
        public string SupportSubject { get; set; }

        [Reg]
        public string SupportBody { get; set; }

        [Reg, Display(Description = SenderDescription)]
        public string SenderSubject { get; set; }

        [Reg]
        public string SenderBody { get; set; }

        [Reg, Display(Description = SenderDescription)]
        public string ConfirmationTrackingCode { get; set; }


        #region Descriptions

        private const string ConfirmationDescription = @"
This is the email that is sent as a confirmation of a successful registration.

Put DO NOT SEND as the Subject if you do not need a confirmation.
";
        private const string NotifyIdsDescription = @"
These are the users that will be notified when a registration occurs.

The top most user will be the 'sender' of the confirmation email.
";
        private const string LoginDescription = @"
####These are for special instructions that will show up at the top of the registration in the appropriate context.

Instructions for the Login page.
";
        private const string SelectDescription = @"
Instructions when a list of Family Members to select one to register are displayed.
";
        private const string FindDescription = @"
When you are searching for a person.

Either when you are not logged in, or adding a guest.
";
        private const string OptionsDescription = @"
Allows you to further explain the questions, dropdowns, checkboxes etc.
";
        private const string SpecialDescription = @"
For Managed Giving, Manage Subscribtions etc.
";
        private const string SubmitDescription = @"
For when you have the choice to register another or continue the registration.
";
        private const string SorryDescription = @"
This message appears on the registration page whenever someone cannot register: 

* The Maximum has been reached
* When it is checked as Filled 
* It is prior to the Registration Start Date.

see [this help article](https://docs.touchpointsoftware.com/OnlineRegistration/MessagesSettings.html)
";
        private const string TermsDescription = @"
Some registrations like a camp, or sports team, or a trip require you to agree to terms (indemnification).

They would have to click an 'Agree to terms' checkbox to complete the registration.

You could also include a link to a PDF file in the Confirmation email.
";
        private const string ThankYouDescription = @"
This is displayed when you complete the registration.

If left blank, the default will be used.
";
        private const string ReminderDescription = @"
This is the email that is sent as a reminder for members of an event.
";
        private const string GiftNotifyIdsDescription = @"
These are the users that will be notified when a gift occurs.

The top most user will be the 'sender' of the confirmation email.
";
        private const string SupportDescription = @"
This is the email that is sent to solicit supporters.
";
        private const string SenderDescription = @"
This is the email that is sent to notify senders.
";
        private const string ConfirmationTrackingDescription = @"
You put the name of the special content that contains the javascript code here.
";

        #endregion

    }
}
