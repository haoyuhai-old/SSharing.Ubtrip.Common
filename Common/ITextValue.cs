using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// 表示包含显示的文本及值的数据项,可以绑定到下拉框控件
    /// </summary>
    public interface ITextValue
    {
        /// <summary>
        /// 显示的文本
        /// </summary>
        string Text { get; }

        /// <summary>
        /// 值
        /// </summary>
        string Value { get; }
    }
}
