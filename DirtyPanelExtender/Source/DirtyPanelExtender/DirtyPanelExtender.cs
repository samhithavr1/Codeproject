using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Text;
using AjaxControlToolkit;

[assembly: System.Web.UI.WebResource("DirtyPanelExtender.DirtyPanelExtenderBehavior.js", "text/javascript")]

namespace DirtyPanelExtender
{
    [Designer(typeof(DirtyPanelExtenderDesigner))]
    [ClientScriptResource("DirtyPanelExtender.DirtyPanelExtenderBehavior", "DirtyPanelExtender.DirtyPanelExtenderBehavior.js")]
    [TargetControlType(typeof(Panel))]
    [TargetControlType(typeof(UpdatePanel))]
    public class DirtyPanelExtender : ExtenderControlBase
    {
        [ExtenderControlProperty]
        [DefaultValue("Data has not been saved.")]
        public string OnLeaveMessage
        {
            get
            {
                return GetPropertyValue("OnLeaveMessage", "");
            }
            set
            {
                SetPropertyValue("OnLeaveMessage", value);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager.RegisterHiddenField(this, string.Format("{0}_Values", TargetControlID), String.Join(",", GetValuesArray()));
            base.OnPreRender(e);
        }

        private string[] GetValuesArray()
        {
            return GetValuesArray(TargetControl.Controls);
        }

        private string[] GetValuesArray(ControlCollection coll)
        {
            List<string> values = new List<string>();
            foreach (Control control in coll)
            {
                if (control is DropDownList || control is RadioButtonList)
                {
                    values.Add(string.Format("{0}:{1}", control.ClientID, ((ListControl)control).SelectedIndex));
                }
                else if (control is IEditableTextControl)
                {
                    values.Add(string.Format("{0}:{1}", control.ClientID, Uri.EscapeDataString(((IEditableTextControl) control).Text)));
                }
                else if (control is ICheckBoxControl)
                {
                    values.Add(string.Format("{0}:{1}", control.ClientID, ((ICheckBoxControl) control).Checked.ToString().ToLower()));
                }

                values.AddRange(GetValuesArray(control.Controls));
            }
            return values.ToArray();
        }

        public void ResetDirtyFlag()
        {
            ScriptManager.RegisterClientScriptBlock(TargetControl, TargetControl.GetType(), 
                string.Format("{0}_Values_Update", TargetControlID), string.Format("document.getElementById('{0}').value = '{1}';",
                    string.Format("{0}_Values", TargetControlID), String.Join(",", GetValuesArray())), true);
        }
    }
}