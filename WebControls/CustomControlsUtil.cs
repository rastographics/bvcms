using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web;

namespace CustomControls
{
    public static class CustomControlsUtil
    {
        private static void RegisterEditButtonExtracted(IDisplayOrEdit from, EditUpdateButton bt)
        {
            from.SetEditUpdateButton(bt);
            bt.RegisterControl(from);
        }
        public static void RegisterEditButton(this IDisplayOrEdit from, Control cc)
        {
            if (HttpContext.Current == null)
                return;
            const string STR_EditButton = "EditButton";
            var bt = HttpContext.Current.Items[from.EditGroup + STR_EditButton] as EditUpdateButton;
            if (bt != null)
            {
                RegisterEditButtonExtracted(from, bt);
                return;
            }
            foreach (Control c in cc.Controls)
            {
                if (c is EditUpdateButton && ((EditUpdateButton)c).EditGroup == from.EditGroup)
                {
                    RegisterEditButtonExtracted(from, (EditUpdateButton)c);
                    HttpContext.Current.Items[from.EditGroup + STR_EditButton] = c;
                    break;
                }
                else if (c.HasControls())
                    from.RegisterEditButton(c);
            }
        }
    }
}
