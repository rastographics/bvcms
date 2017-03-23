using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private string UnsubscribeReplacement(EmailQueueTo emailqueueto)
        {
            var qs = "OptOut/UnSubscribe/?enc=" + Util.EncryptForUrl($"{emailqueueto.PeopleId}|{@from.Address}");
            var url = db.ServerLink(qs);
            return $@"<a href=""{url}"">Unsubscribe</a>";
        }
    }
}