using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace SSharing.Ubtrip.Common
{
    public static class DropDownListExtension
    {
        /// <summary>
        /// 设置下拉项选定项的值为指定的值，如果不包含指定的值，则为空
        /// </summary>
        public static void SetSelectedValueOrEmpty(this DropDownList dropDownList, string selectedValue)
        {
            dropDownList.SelectedIndex = dropDownList.Items.IndexOf(dropDownList.Items.FindByValue(selectedValue));
        }
    }
}
