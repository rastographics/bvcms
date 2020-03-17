using System;

namespace CmsWeb.Areas.Dialog.Models
{
    public class RemoteDeposit
    {
        public string AccountNumber { get; set; }
        public int BundleHeaderId { get; set; }
        public DateTime? DepositDate { get; set; }
    }
}
