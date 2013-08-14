using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Reflection;
using System.Drawing.Design;
using Alias = System.Windows.Forms;
using System.ComponentModel.Design;

namespace CustomControls
{
    public interface IDisplayOrEdit
    {
        [Bindable(true), Category("Binding"), DefaultValue("")]
        string BindingMember { get; set; }
        [Bindable(true), Category("Binding"), DefaultValue("")]
        string BindingSource { get; set; }
        [Bindable(true), Category("Binding")]
        BindingModes BindingMode { get; set; }
        string AssociatedRowId { get; set; }
        string EditGroup { get; set; }
        string EditRole { get; set; }

        string GetTextValue();
        bool ChangedStatus { get; set; }
        bool HadBeenChanged { get; set; }
        void SetEditUpdateButton(EditUpdateButton bt);
        bool Editing { get; }
    }
}
