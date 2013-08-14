using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using UtilityExtensions;

namespace CustomControls
{
    public enum BindingModes
    {
        TwoWay,
        OneWay
    }

    public enum EditType : int
    {
        Text,
        Number,
        Date,
        Code,
        Phone
    }
    public class EditUpdateButton : Button
    {
        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresControlState(this);
            base.OnInit(e);
            //RecursiveShareControls(Page);
        }
        protected Button CancelButton;
        private void CreateCancelButton()
        {
            if (DesignMode || (Editing && CancelButton == null))
            {
                CancelButton = new Button();
                CancelButton.Text = "Cancel";
                CancelButton.ID = "CancelButton";
                CancelButton.Click += new EventHandler(CancelButton_Click);
                CancelButton.CausesValidation = false;
                Controls.Add(CancelButton);
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateCancelButton();
            if (this.Enabled)
                this.Enabled = !CheckRole || Page.User.IsInRole("Edit");
        }
        private const string EditText = "Edit";
        [DefaultValue(EditText)]
        public new string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public EditUpdateButton()
            : base()
        {
            this.Text = EditText;
            EnableViewState = false;
            CheckRole = true;
        }

        private List<IDisplayOrEdit> _Controls = new List<IDisplayOrEdit>();

        public void RegisterControl(IDisplayOrEdit c)
        {
            _Controls.Add(c);
        }
        public override void DataBind()
        {
            foreach (Control c in _Controls)
                c.DataBind();
        }
        public bool KeepEditing { get; set; }
        public bool Editing
        {
            get
            {
                if (Page.IsPostBack)
                    return Page.Request.Form[UniqueID] == "Edit" || KeepEditing;
                else
                    return Text == "Update";
            }
            set
            {
                Text = "Update";
            }
        }
        public bool Updating
        {
            get { return Page.Request.Form[UniqueID] == "Update"; }
        }
        public int Changes { get; set; }
        public bool NeedToSave
        {
            get { return Changes > 0; }
        }
        public bool CheckRole { get; set; }
        protected override void OnPreRender(EventArgs e)
        {
            Text = (Editing ? "Update" : EditText);
            DisableHyperlinks();
            base.OnPreRender(e);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            CreateCancelButton();
            base.RenderChildren(writer);
        }
        public void DisableHyperlinks()
        {
            DisableHyperlinksRecursive(Page, CancelButton, this, Editing);
        }
        public static void DisableHyperlinksRecursive(Control cc, Control CancelButton, Control UpdateButton, bool Editing)
        {
            foreach (Control c in cc.Controls)
                if (c is LinkButtonConfirm)
                    ((LinkButtonConfirm)c).Editing = Editing;
                else if (c is HyperLink || (c is IButtonControl && c != UpdateButton && c != CancelButton))
                {
                    var b = c as WebControl;
                    if (Editing && !b.Attributes["nodisable"].HasValue())
                        b.Attributes.Add("onclick", "alert('Must update or cancel first'); return false");
                    else
                        b.Attributes.Remove("onclick");
                }
                else
                    if (c.HasControls())
                        DisableHyperlinksRecursive(c, CancelButton, UpdateButton, Editing);
        }
        private bool canceling = false;
        protected override void OnClick(EventArgs e)
        {
            if (Editing)
                CreateCancelButton();
            else
                Controls.Remove(CancelButton);
            base.OnClick(e);
        }
        void CancelButton_Click(object sender, EventArgs e)
        {
            canceling = true;
            Controls.Remove(CancelButton);
            OnClick(e);
        }
        internal string DataBindValue(IDisplayOrEdit c)
        {
            return DataBindObject(c).ToString();
        }
        internal object DataBindObject(IDisplayOrEdit c)
        {
            object o = GetBindingSourceObject(c);
            return Util.GetPropertyEx(o, c.BindingMember) ?? "";
        }

        public void UpdateBinding(IDisplayOrEdit c)
        {
            if (!canceling && c.ChangedStatus && c.BindingMode == BindingModes.TwoWay)
            {
                object o = GetBindingSourceObject(c);
                Util.SetPropertyFromText(o, c.BindingMember, c.GetTextValue());
                Changes++;
                c.ChangedStatus = false;
                c.HadBeenChanged = true;
            }
        }
        private object GetBindingSourceObject(IDisplayOrEdit c)
        {
            if (string.IsNullOrEmpty(c.BindingMember))
                c.BindingMember = ((Control)c).ID;
            object BindingSourceObject;
            if (c.BindingSource == "this")
                BindingSourceObject = Page;
            else
                BindingSourceObject = Util.GetPropertyEx(Page, c.BindingSource);

            return BindingSourceObject;
        }

        [DefaultValue(false)]
        public override bool EnableViewState
        {
            get { return base.EnableViewState; }
            set { base.EnableViewState = value; }
        }
        public string EditGroup { get; set; }
    }
}
