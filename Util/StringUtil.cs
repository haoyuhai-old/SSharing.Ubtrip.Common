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
    /// �ַ���������
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// ���Դ�Сд�Ϳո���ַ����Ƚ�
        /// </summary>
        public static bool EqualsIgnoreCaseAndSpace(string name1, string name2)
        {
            if (name1 == null || name2 == null) return false;
            return string.Equals(name1.Trim(), name2.Trim(), StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// ɾ����������1���Ŀ��ַ�(�ո�س������У�tab)
        /// src����100��ƽ��ʱ��2.9΢��
        /// </summary>
        /// <param name="src">�������ַ�</param>
        /// <returns>�ַ�</returns>
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
        /// ���ݸ������ַ���ģ�弰���ݶ���̬�����ַ�����֧�����ԣ��������ü���������(�����ʽ�а�������ʱ����֧������). 
        /// </summary>
        /// <param name="stringTemplate">�ַ���ģ��</param>
        /// <param name="dataObject">���ݶ��󣬱�������ַ���ģ����ָ��������</param>  
        /// <returns>�������ɵ��ַ���</returns>
        public static string BuilderString(string stringTemplate, object dataObject)
        {
            return new StringConstructor(stringTemplate, dataObject).GetResult();            
        }

        /// <summary>
        /// �Ƿ���������ַ�
        /// </summary>
        public static bool ContainsCnChar(string str)
        {
            //\u4e00-\u9fa5 ���ֵķ�Χ�� 
            //^[\u4e00-\u9fa5]$ ���ֵķ�Χ������                
            if (str == null) return false;

            Regex rx = new Regex("[\u4e00-\u9fa5]");
            return rx.IsMatch(str);
        }

        /// <summary>
        /// �ж��ַ����Ƿ�Base64��Я�����⴦���ַ���
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

        #region �ڲ��࣬����ʵ���ַ������滻

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
                this.context.Imports.AddType(typeof(Math));                                         //����Math�µľ�̬����                
                this.context.Imports.AddMethod("Format", typeof(StringConstructor), string.Empty);  //����string.Format����
            }

            public string GetResult()
            {
                if (string.IsNullOrWhiteSpace(stringTemplate)) return stringTemplate;

                //������<%...%>֮�е����Ա��ʽ,eg:<%Order.OrderNo%>, <%Order.OrdeDate:[yyyy-MM-dd]%
                Regex regex = new Regex("<%([^<>%]+)%>");
                string result = regex.Replace(stringTemplate, this.ReplaceMatch);

                //������<#...#>֮�е����Ա��ʽ,eg:<#Order.OrderNo#>, <# Order.OrdeDate.ToString("yyyy-MM-dd")#>
                Regex regex2 = new Regex("<#(([^#]|(#(?!>)))+)#>"); //�м���ַ�Ҫô����#,Ҫô��#�����治��>
                result = regex2.Replace(result, this.ReplaceMatchByFlee);
                return result;
            }

            /// <summary>
            /// ÿ���ҵ�������ʽƥ��ʱ�����õķ���
            /// </summary>
            private string ReplaceMatchByFlee(Match match)
            {
                string expression = match.Groups[1].Value;
                if (string.IsNullOrWhiteSpace(expression)) return expression;

                if (ContainsCnChar(expression))
                {
                    //������������ַ�������ciloci���ʽ�в�֧�����ģ������������Լ��ķ������棬�����Ͳ�֧����������
                    return MyEvaluateExpression(expression);
                }
                else
                {
                    //������������ģ���ʹ��ciloci�ķ�ʽ�������
                    IDynamicExpression e = context.CompileDynamic(expression);
                    object result = e.Evaluate();
                    return result.ToString();
                }
            }           

            //cilociʹ�õ�format����
            public static string Format(string format, params object[] args)
            {
                return string.Format(format, args);
            }

            /// <summary>
            /// ÿ���ҵ�������ʽƥ��ʱ�����õķ���
            /// </summary>
            private string ReplaceMatch(Match match)
            {
                string expression = match.Groups[1].Value;
                if (string.IsNullOrWhiteSpace(expression)) return expression;

                return MyEvaluateExpression(expression);
            }

            //����ʵ�ֵı���ʽ��ⷽ��
            private string MyEvaluateExpression(string expression)
            {
                string[] strs = expression.Trim().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (strs.Length == 0) return expression;

                //���ζ��������                
                object tempObj = this.dataObject;
                for (int i = 0; i < strs.Length; i++)
                {
                    string propertyName = strs[i].Trim();
                    if (propertyName.EndsWith(")"))   //��������(���������޲�,���з���ֵ)
                    {
                        tempObj = InvokeMethodWithoutArgument(tempObj, propertyName.TrimEnd(new char[] { '(', ')', ' ' }));
                    }
                    else       //ȡ����ֵ
                    {
                        int startIndex = propertyName.IndexOf(":[");
                        if (startIndex >= 0)
                        {
                            //����ʽ��������
                            int endIndex = propertyName.IndexOf("]");
                            if (endIndex < 0) throw new Exception("������Ч:" + propertyName);

                            tempObj = GetPropertyOrFieldValue(tempObj, propertyName.Substring(0, startIndex));

                            string format = propertyName.Substring(startIndex + 2).TrimEnd(new char[] { ']', ' ' });
                            tempObj = string.Format("{0:" + format + "}", tempObj);
                        }
                        else
                        {
                            //������
                            tempObj = GetPropertyOrFieldValue(tempObj, propertyName);
                        }
                    }
                }
                return tempObj == null ? string.Empty : tempObj.ToString();                
            }

            private static object InvokeMethodWithoutArgument(object obj, string methodName)
            {
                if (obj == null) throw new ArgumentException(string.Format("����Ϊnull,�޷����÷���{0}", methodName), "obj");

                Type type = obj.GetType();
                MethodInfo mi = type.GetMethod(methodName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (mi != null)
                {
                    return mi.Invoke(obj, null);
                }

                throw new ArgumentException(string.Format("����{0}�в��淽��{1}�Ķ���", type.Name, methodName), "methodName");
            }

            private static object GetPropertyOrFieldValue(object obj, string propertyOrFieldName)
            {
                if (obj == null) throw new ArgumentException(string.Format("����Ϊnull,�޷���ȡ����{0}��ֵ", propertyOrFieldName), "obj");

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

                throw new ArgumentException(string.Format("����{0}�в������Ի��ֶ�{1}�Ķ���", type.Name, propertyOrFieldName), "propertyOrFieldName");
            }
        }
        #endregion
    }// �����
}// �����ռ����
