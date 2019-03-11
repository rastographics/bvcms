using CmsData;
using CmsData.Codes;
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable RedundantDefaultMemberInitializer
namespace CmsWeb.Areas.Public.Models.MobileAPIv2
{
    public class MobileGivingStatement
    {
        private readonly int peopleID = 0;
        private readonly int spouseID = 0;

        private readonly DateTime fromDate;
        private readonly DateTime toDate;

        private readonly bool jointStatement = false;

        private List<MobileGivingContribution> giftsNormal = new List<MobileGivingContribution>();
        private List<MobileGivingContribution> giftsInKind = new List<MobileGivingContribution>();
        private List<MobileGivingContribution> giftsPledge = new List<MobileGivingContribution>();
        private List<MobileGivingContribution> giftsTaxable = new List<MobileGivingContribution>();

        public MobileGivingStatement(CMSDataContext db, int peopleID, DateTime fromDate, DateTime toDate)
        {
            this.peopleID = peopleID;
            this.fromDate = fromDate;
            this.toDate = toDate;

            Person person = db.LoadPersonById(peopleID);
            spouseID = person.SpouseId ?? 0;

            if (person.PositionInFamilyId == PositionInFamily.PrimaryAdult)
            {
                if (person.ContributionOptionsId != null)
                {
                    jointStatement = person.ContributionOptionsId == StatementOptionCode.Joint;
                }
                else
                {
                    jointStatement = person.SpouseId != null && person.SpouseId != 0;
                }
            }
        }

        public void loadContributions(CMSDataContext db)
        {
            List<int> peopleMatch = new List<int> { peopleID };
            // If joint, find spouse entries also
            if (jointStatement)
            {
                peopleMatch.Add(spouseID);
            }

            IQueryable<MobileGivingContribution> contributions = from contribution in db.Contributions
                                                                 join fund in db.ContributionFunds on contribution.FundId equals fund.FundId
                                                                 where peopleMatch.Contains(contribution.PeopleId ?? 0)
                                                                 // Recorded
                                                                 where contribution.ContributionStatusId == 0
                                                                 // Returned or reverses (6, 7)
                                                                 where contribution.ContributionTypeId != 6 && contribution.ContributionTypeId != 7
                                                                 // Date range
                                                                 where contribution.ContributionDate != null && contribution.ContributionDate >= fromDate && contribution.ContributionDate <= toDate
                                                                 select new MobileGivingContribution
                                                                 {
                                                                     id = contribution.ContributionId,
                                                                     typeID = contribution.ContributionTypeId,
                                                                     amount = contribution.ContributionAmount ?? 0,
                                                                     date = contribution.ContributionDate ?? DateTime.Today,
                                                                     checkNumber = contribution.CheckNo,
                                                                     fundID = fund.FundId,
                                                                     fundName = fund.FundName,
                                                                     fundTaxDeductible = !fund.NonTaxDeductible ?? true,
                                                                     fundPledge = fund.FundPledgeFlag
                                                                 };

            foreach (MobileGivingContribution contribution in contributions)
            {
                switch (contribution.getType())
                {
                    case ContributionTypeCode.Stock:
                    case ContributionTypeCode.GiftInKind:
                        {
                            giftsInKind.Add(contribution);
                            break;
                        }

                    case ContributionTypeCode.Pledge:
                        {
                            giftsPledge.Add(contribution);
                            break;
                        }

                    case ContributionTypeCode.NonTaxDed:
                        {
                            giftsTaxable.Add(contribution);
                            break;
                        }

                    default:
                        {
                            giftsNormal.Add(contribution);
                            break;
                        }
                }
            }
        }
    }
}
