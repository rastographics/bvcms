using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;
using UtilityExtensions;

namespace CustomControls
{
    public class EmailHyperlink : HyperLink
    {
        protected override void Render(HtmlTextWriter writer)
        {
            NavigateUrl = Email.EmailHref(Name);
            base.Render(writer);
        }
        [Bindable(true)]
        public string Email { get; set; }
        [Bindable(true)]
        public string Name { get; set; }
    }
}
