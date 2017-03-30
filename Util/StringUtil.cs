using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using Ciloci.Flee;
using System.Globalization;

namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// 字符处理工具类
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// 忽略大小写和空格的字符串比较
        /// </summary>
        public static bool EqualsIgnoreCaseAndSpace(string name1, string name2)
        {
            if (name1 == null || name2 == null) return false;
            return string.Equals(name1.Trim(), name2.Trim(), StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 删除连续多于1个的空字符(空格回车，换行，tab)
        /// src长度100，平均时间2.9微秒
        /// </summary>
        /// <param name="src">待处理字符</param>
        /// <returns>字符</returns>
        public static string Suppress(string src)
        {
            if (src == null || src == "")
            {
                return src;
            }

            StringBuilder dest = new StringBuilder();

            //char[] cs = src.ToCharArray();
            bool inBlank = true;

            foreach (char c in src)
            {
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                {
                    inBlank = true;
                }
                else
                {
                    if (inBlank == true)
                    {
                        dest.Append(" ");
                    }
                    inBlank = false;
                    dest.Append(c);
                }
            }

            return dest.ToString(1,dest.Length - 1);
        }

        /// <summary>
        /// 根据给定的字符串模板及数据对象动态生成字符串。支持属性，方法调用及算术运算(当表达式中包含中文时将不支持运算). 
        /// </summary>
        /// <param name="stringTemplate">字符串模板</param>
        /// <param name="dataObject">数据对象，必须包含字符串模板中指定的属性</param>  
        /// <returns>返回生成的字符串</returns>
        public static string BuilderString(string stringTemplate, object dataObject)
        {
            return new StringConstructor(stringTemplate, dataObject).GetResult();            
        }

        /// <summary>
        /// 是否包含中文字符
        /// </summary>
        public static bool ContainsCnChar(string str)
        {
            //\u4e00-\u9fa5 汉字的范围。 
            //^[\u4e00-\u9fa5]$ 汉字的范围的正则                
            if (str == null) return false;

            Regex rx = new Regex("[\u4e00-\u9fa5]");
            return rx.IsMatch(str);
        }

        /// <summary>
        /// 判断字符串是否Base64（携瑞特殊处理字符）
        /// </summary>
        public static bool IsBase64Format(string strInput)
        {
            strInput = strInput.Replace("*", "+").Replace("-", "/").Replace("_", "=");
            try
            {
                Convert.FromBase64String(strInput);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region 内部类，用来实现字符串的替换

        public class StringConstructor
        {            
            private string stringTemplate;
            private object dataObject;
            private ExpressionContext context;

            public StringConstructor(string stringTemplate, object dataObject)
            {
                this.stringTemplate = stringTemplate;
                this.dataObject = dataObject;

                this.context = new ExpressionContext(this.dataObject);
                this.context.Options.ParseCulture = CultureInfo.GetCultureInfo("zh-CN");
                this.context.Imports.AddType(typeof(Math));                                         //导入Math下的静态方法                
                this.context.Imports.AddMethod("Format", typeof(StringConstructor), string.Empty);  //导入string.Format方法
            }

            public string GetResult()
            {
                if (string.IsNullOrWhiteSpace(stringTemplate)) return stringTemplate;

                //包含在<%...%>之中的属性表达式,eg:<%Order.OrderNo%>, <%Order.OrdeDate:[yyyy-MM-dd]%
                Regex regex = new Regex("<%([^<>%]+)%>");
                string result = regex.Replace(stringTemplate, this.ReplaceMatch);

                //包含在<#...#>之中的属性表达式,eg:<#Order.OrderNo#>, <# Order.OrdeDate.ToString("yyyy-MM-dd")#>
                Regex regex2 = new Regex("<#(([^#]|(#(?!>)))+)#>"); //中间的字符要么不是#,要么是#但后面不是>
                result = regex2.Replace(result, this.ReplaceMatchByFlee);
                return result;
            }

            /// <summary>
            /// 每当找到正则表达式匹配时都调用的方法
            /// </summary>
            private string ReplaceMatchByFlee(Match match)
            {
                string expression = match.Groups[1].Value;
                if (string.IsNullOrWhiteSpace(expression)) return expression;

                if (ContainsCnChar(expression))
                {
                    //如果包含中文字符，由于ciloci表达式中不支持中文，所以这里用自己的方法代替，这样就不支持算术运算
                    return MyEvaluateExpression(expression);
                }
                else
                {
                    //如果不包含中文，则使用ciloci的方式，更灵活
                    IDynamicExpression e = context.CompileDynamic(expression);
                    object result = e.Evaluate();
                    return result.ToString();
                }
            }           

            //ciloci使用的format方法
            public static string Format(string format, params object[] args)
            {
                return string.Format(format, args);
            }

            /// <summary>
            /// 每当找到正则表达式匹配时都调用的方法
            /// </summary>
            private string ReplaceMatch(Match match)
            {
                string expression = match.Groups[1].Value;
                if (string.IsNullOrWhiteSpace(expression)) return expression;

                return MyEvaluateExpression(expression);
            }

            //自已实现的表求式求解方法
            private string MyEvaluateExpression(string expression)
            {
                string[] strs = expression.Trim().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (strs.Length == 0) return expression;

                //依次对属性求解                
                object tempObj = this.dataObject;
                for (int i = 0; i < strs.Length; i++)
                {
                    string propertyName = strs[i].Trim();
                    if (propertyName.EndsWith(")"))   //方法调用(方法必须无参,且有返回值)
                    {
                        tempObj = InvokeMethodWithoutArgument(tempObj, propertyName.TrimEnd(new char[] { '(', ')', ' ' }));
                    }
                    else       //取属性值
                    {
                        int startIndex = propertyName.IndexOf(":[");
                        if (startIndex >= 0)
                        {
                            //带格式串的属性
                            int endIndex = propertyName.IndexOf("]");
                            if (endIndex < 0) throw new Exception("参数无效:" + propertyName);

                            tempObj = GetPropertyOrFieldValue(tempObj, propertyName.Substring(0, startIndex));

                            string format = propertyName.Substring(startIndex + 2).TrimEnd(new char[] { ']', ' ' });
                            tempObj = string.Format("{0:" + format + "}", tempObj);
                        }
                        else
                        {
                            //简单属性
                            tempObj = GetPropertyOrFieldValue(tempObj, propertyName);
                        }
                    }
                }
                return tempObj == null ? string.Empty : tempObj.ToString();                
            }

            private static object InvokeMethodWithoutArgument(object obj, string methodName)
            {
                if (obj == null) throw new ArgumentException(string.Format("对象为null,无法调用方法{0}", methodName), "obj");

                Type type = obj.GetType();
                MethodInfo mi = type.GetMethod(methodName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (mi != null)
                {
                    return mi.Invoke(obj, null);
                }

                throw new ArgumentException(string.Format("类型{0}中不存方法{1}的定义", type.Name, methodName), "methodName");
            }

            private static object GetPropertyOrFieldValue(object obj, string propertyOrFieldName)
            {
                if (obj == null) throw new ArgumentException(string.Format("对象为null,无法获取属性{0}的值", propertyOrFieldName), "obj");

                Type type = obj.GetType();
                PropertyInfo pi = type.GetProperty(propertyOrFieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (pi != null)
                {
                    return pi.GetValue(obj, null);
                }

                FieldInfo fi = type.GetField(propertyOrFieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fi != null)
                {
                    return fi.GetValue(obj);
                }

                throw new ArgumentException(string.Format("类型{0}中不存属性或字段{1}的定义", type.Name, propertyOrFieldName), "propertyOrFieldName");
            }
        }
        #endregion
    }// 类结束
}// 命名空间结束
