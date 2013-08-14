using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;
using UtilityExtensions;

namespace CustomControls
{
    public class DisplayLabel : Label, IDisplayOrEdit
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.RegisterEditButton(Page);
        }
        public DisplayLabel()
            : base()
        {
            BindingMode = BindingModes.OneWay;
        }
        public override void DataBind()
        {
            base.DataBind();
            Text = EditUpdateButton.DataBindValue(this);
            if (FormatString.HasValue())
                Text = FormatString.Fmt(Text);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Title, BindingMember);
            base.Render(writer);
        }
        public string FormatString { get; set; }

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
