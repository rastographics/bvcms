using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;
using UtilityExtensions;

namespace CustomControls
{
    public class DisplayHyperlink : HyperLink, IDisplayOrEdit
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.RegisterEditButton(Page);
        }
        public DisplayHyperlink()
            : base()
        {
            BindingMode = BindingModes.OneWay;
        }
        public override void DataBind()
        {
            base.DataBind();
            if (BindingFormat.HasValue())
                Text = BindingFormat.Fmt(EditUpdateButton.DataBindObject(this));
            else
                Text = EditUpdateButton.DataBindValue(this);
            NavigateUrl = DataBindUrl();
        }
        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Title, BindingMember);
            base.Render(writer);
        }

        public string BindingUrlMember { get; set; }
        public string BindingUrlFormat { get; set; }
        private string DataBindUrl()
        {
            object o;
            if (BindingSource == "this")
                o = Page;
            else
                o = Util.GetPropertyEx(Page, BindingSource);
            object v = Util.GetPropertyEx(o, BindingUrlMember) ?? "";
            return BindingUrlFormat.Fmt(v);
        }
        public string BindingFormat { get; set; }

        #region IDisplayOrEdit Members

        private EditUpdateButton EditUpdateButton;
        public void SetEditUpdateButton(EditUpdateButton bt)
        {
            EditUpdateButton = bt;
        }
        private string _BindingMember;
        public string BindingMember
        {
            get
            {
                if (!_BindingMember.HasValue())
                    return this.ID;
                return _BindingMember;
            }
            set { _BindingMember = value; }
        }
        public string BindingSource { get; set; }
        [Browsable(false)]
        public BindingModes BindingMode { get; set; }
        public string GetTextValue()
        {
            return Text;
        }
        public bool ChangedStatus
        {
            get { return false; }
            set { }
        }
        public bool HadBeenChanged
        {
            get { return false; }
            set { }
        }

        public bool HasEditButton
        {
            get { return EditUpdateButton != null; }
        }

        public bool Editing
        {
            get { return EditUpdateButton.Editing; }
        }

        public string AssociatedRowId { get; set; }
        public string EditGroup { get; set; }
        public string EditRole { get; set; }
        #endregion
    }
}
