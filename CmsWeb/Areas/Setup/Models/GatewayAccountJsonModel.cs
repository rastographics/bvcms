using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Areas.Setup.Models
{
    public class GatewayAccountJsonModel
    {
        public int? ProcessId { get; set; }
        public int? GatewayAccountId { get; set; }
        public string GatewayAccountName { get; set; }
        public int GatewayId { get; set; }
        public string[] GatewayAccountInputs { get; set; }
        public string[] GatewayAccountValues { get; set; }
        public bool UseForAll { get; set; }
    }
}
