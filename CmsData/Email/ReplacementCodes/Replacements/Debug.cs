using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
#if DEBUG
        public static void ReCodes()
        {
            var list = new List<string>
            {
                "{address}",
                "{address2}",
                "{amtdue}",
                "{amtpaid}",
                "{amount}",
                "{barcode}",
                "{birthdate}",
                "{campus}",
                "{cellphone}",
                "{city}",
                "{csz}",
                "{country}",
                "{createaccount}",
                "{church}",
                "{churchphone}",
                "{cmshost}",
                "{dob}",
                "{estatement}",
                "{emailhref}",
                "{first}",
                "{fromemail}",
                "{homephone}",
                "{last}",
                "{name}",
                "{nextmeetingtime}",
                "{nextmeetingtime0}",
                "{occupation}",
                "{orgname}",
                "{org}",
                "{orgmembercount}",
                "{paylink}",
                "{peopleid}",
                "{receivesms}",
                "{salutation}",
                "{state}",
                "{statementtype}",
                "{email}",
                "{toemail}",
                "{today}",
                "{title}",
                "{track}",
                "{unsubscribe}",
                MatchAddSmallGroupRe,
                MatchPledgeRe,
                MatchPledgeFundRe,
                MatchSettingRe,
                MatchExtraValueRe,
                MatchFirstOrSubstituteRe,
                MatchSubGroupsRe,
                MatchOrgExtraRe,
                MatchSubGroupRe,
                MatchOrgMemberRe,
                MatchOrgBarCodeRe,
                MatchRegTextRe,
                MatchRegisterLinkRe,
                MatchRegisterLinkHrefRe,
                MatchRegisterTagRe,
                MatchRsvpLinkRe,
                MatchSendLinkRe,
                MatchSupportLinkRe,
                MatchMasterLinkRe,
                MatchVolReqLinkRe,
                MatchVolSubLinkRe,
                MatchVoteLinkRe,
            };
            foreach(var i in list)
                Debug.WriteLine(i);
        }
#endif
    }
}
