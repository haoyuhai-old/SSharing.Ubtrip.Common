using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace SSharing.Ubtrip.Common.Helper
{
    /// <summary>
    /// 商旅系统辅助类
    /// </summary>
    public class UbtripHelper
    {
        /// <summary>
        /// 复姓库，目前共81个
        /// </summary>
        private static readonly List<string> complexNames = new List<string> { "欧阳","太史","端木","上官","司马","东方","独孤",
            "南宫","万俟","闻人","夏侯","诸葛","尉迟","公羊","赫连","澹台","皇甫","宗政","濮阳","公冶","太叔","申屠","公孙","慕容",
            "仲孙","钟离","长孙","宇文","司徒","鲜于","司空","闾丘","子车","亓官","司寇","巫马","公西","颛孙","壤驷","公良","漆雕",
            "乐正","宰父","谷梁","拓跋","夹谷","轩辕","令狐","段干","百里","呼延","东郭","南门","羊舌","微生","公户","公玉","公仪",
            "梁丘","公仲","公上","公门","公山","公坚","左丘","公伯","西门","公祖","第五","公乘","贯丘","公皙","南荣","东里","东宫",
            "仲长","子书","子桑","即墨","达奚","褚师","吴铭"};

        /// <summary>
        /// 姓名拆成姓和名,已经考虑复姓
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static Tuple<string, string> SplitFullName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                throw new ArgumentNullException("fullName");
            }

            fullName = fullName.Trim();
            var lastName = string.Empty;//姓
            var firstName = string.Empty;//名
            if (fullName.Length > 2)//考虑复姓
            {
                var preTwoWords = fullName.Substring(0, 2);
                if (complexNames.Contains(preTwoWords))
                {
                    lastName = fullName.Substring(0, 2);
                    firstName = fullName.Substring(2);
                }
                else
                {
                    lastName = fullName.Substring(0, 1);
                    firstName = fullName.Substring(1);
                }
            }
            else if (fullName.Length == 2)//两个字，第一个字符作为姓，第二个作为名
            {
                lastName = fullName.Substring(0, 1);
                firstName = fullName.Substring(1);
            }
            else//只有一个字，就作为姓
            {
                lastName = fullName;
            }

            return new Tuple<string, string>(lastName, firstName);
        }

        /// <summary>
        /// 对接顺丰，通用调用webService方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="systemId"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string CallWebServiceBySF(string url, string operation, string data)
        {
            var result = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?o=" + operation);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Timeout = 60000;
            request.Method = "POST";
            request.ContentType = "text/xml;charset=UTF-8";

            var byteData = Encoding.UTF8.GetBytes(data);
            request.ContentLength = byteData.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(byteData, 0, byteData.Length);
            }

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    result = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        /// <summary>
        /// 依据出发日期，出发时间和到达时间获取到达日期
        /// 是否+1
        /// </summary>
        /// <param name="departDate"></param>
        /// <param name="departTime"></param>
        /// <param name="arriveTime"></param>
        /// <returns></returns>
        public static string GetArriveDateByDepartDate(DateTime departDate, string departTime, string arriveTime)
        {
            if (string.Compare(departTime, arriveTime) > 0)
            {
                return departDate.AddDays(1).ToString("yyyy-MM-dd");
            }
            else
            {
                return departDate.ToString("yyyy-MM-dd");
            }
        }

        /// <summary>
        /// 定义哪些网络异常可以重试
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool CanRetry(WebExceptionStatus status)
        {
            return status == WebExceptionStatus.Timeout ||
                   status == WebExceptionStatus.ConnectionClosed ||
                   status == WebExceptionStatus.ConnectFailure ||
                   status == WebExceptionStatus.SendFailure ||
                   status == WebExceptionStatus.ReceiveFailure ||
                   status == WebExceptionStatus.RequestCanceled ||
                   status == WebExceptionStatus.KeepAliveFailure;
        }


    }
}
