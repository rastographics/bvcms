using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CustomControls
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:PostTextBox runat=server></{0}:PostTextBox>")]
    public class PostTextBox : TextBox
    {
        public bool ChangedStatus { get; set; }
        public bool HadBeenChanged { get; set; }
        protected override bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            ChangedStatus = base.LoadPostData(postDataKey, postCollection);
            return ChangedStatus;
        }
    }
}
