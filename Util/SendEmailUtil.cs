using System;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using System.Net.Mime;

namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// SMTP�ʼ����͹�����
    /// </summary>
    public class SendEmailUtil
    {
        private Encoding _Encoding = Encoding.UTF8;

        public void SendEmail(MyMailMessage myMailMessage)
        {
            #region ��ȡ����֤���Ͳ���

            myMailMessage.To = (myMailMessage.To == null ? string.Empty : myMailMessage.To.Trim());
            myMailMessage.Cc = (myMailMessage.Cc == null ? string.Empty : myMailMessage.Cc.Trim());
            myMailMessage.Bcc = (myMailMessage.Bcc == null ? string.Empty : myMailMessage.Bcc.Trim());

            if (myMailMessage.To.Length == 0 && myMailMessage.Cc.Length == 0 && myMailMessage.Bcc.Length == 0)
                throw new Exception("�ռ����ʼ���ַ����ȫΪ�գ�");

            string REG_EMAIL = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";            
            if (myMailMessage.To.Length > 0 && !Regex.IsMatch(myMailMessage.To, REG_EMAIL))
                throw new Exception("�ռ����ʼ���ַ����ȷ��");

            if (myMailMessage.Cc.Length > 0 && !Regex.IsMatch(myMailMessage.Cc, REG_EMAIL))
                throw new Exception("�����ʼ���ַ����ȷ��");

            if (myMailMessage.Bcc.Length > 0 && !Regex.IsMatch(myMailMessage.Bcc, REG_EMAIL))
                throw new Exception("�����ʼ���ַ����ȷ��");

            myMailMessage.To = myMailMessage.To.Replace(";", ",").TrimEnd(',');
            myMailMessage.Cc = myMailMessage.Cc.Replace(";", ",").TrimEnd(',');
            myMailMessage.Bcc = myMailMessage.Bcc.Replace(";", ",").TrimEnd(',');
            myMailMessage.Body = myMailMessage.Body + "\n\n";
            
            #endregion

            #region �����ʼ���Ϣ������

            MailMessage mailMessage = new MailMessage();           
            mailMessage.From = new MailAddress(myMailMessage.ServiceAccount, myMailMessage.DisplayName);
            if (myMailMessage.To.Length > 0)
            {
                mailMessage.To.Add(myMailMessage.To);
            }
            if (myMailMessage.Cc.Length > 0)
            {
                mailMessage.CC.Add(myMailMessage.Cc);
            }
            if (myMailMessage.Bcc.Length > 0)
            {
                mailMessage.Bcc.Add(myMailMessage.Bcc);
            }

            mailMessage.Subject = myMailMessage.Subject;
            mailMessage.Body = myMailMessage.Body;
            mailMessage.SubjectEncoding = _Encoding;
            mailMessage.BodyEncoding = _Encoding; 
            mailMessage.IsBodyHtml = true;
                                    
            List<Attachment> attachments = GetAttachments(myMailMessage);
            foreach (Attachment item in attachments)
            {
                mailMessage.Attachments.Add(item);
            }
            
            SmtpClient smtpClient = new SmtpClient(myMailMessage.SmtpServer);
            smtpClient.DeliveryMethod        = SmtpDeliveryMethod.Network;
            smtpClient.Port                  = myMailMessage.Port;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials           = new NetworkCredential(myMailMessage.ServiceAccount, myMailMessage.ServiceAccountPwd);
            smtpClient.EnableSsl             = myMailMessage.EnableSsl;
            smtpClient.Timeout               = GetTimeoutMilliSeconds(myMailMessage);
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                //д��־
                LogUtil.Write(ex.ToString(), LogLevel.Error, "SendEmail");
            }
            
            smtpClient.Dispose();
            mailMessage.Dispose();
            #endregion
        }

        #region private methods 

        //��ȡ�ʼ���ʱʱ��
        private static int GetTimeoutMilliSeconds(MyMailMessage myMailMessage)
        {
            if (myMailMessage.TimeoutSeconds > 0)
            {
                return myMailMessage.TimeoutSeconds * 1000;
            }
            else
            {
                return 300 * 1000; //5����
            }
        }

        private List<Attachment> GetAttachments(MyMailMessage myMailMessage)
        {
            List<Attachment> result = new List<Attachment>();
            if (!string.IsNullOrEmpty(myMailMessage.Attachments))
            {
                Regex regex = new Regex("<([^<>]+)>");  //������<...>֮�е��ַ���
                foreach (Match match in regex.Matches(myMailMessage.Attachments))
                {
                    Attachment attachment = GetAttachment(match);
                    if (attachment != null)
                    {
                        result.Add(attachment);
                    }
                }
            }
            return result;
        }

        private Attachment GetAttachment(Match match)
        {
            string expression = match.Groups[1].Value;
            if (string.IsNullOrWhiteSpace(expression)) return null;

            string[] strs = expression.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length == 1)
            {
                return new Attachment(strs[0], GetMediaType(strs[0]));         //���ֻ��һ������������Ϊ���ļ�����·��
            }
            else if (strs.Length == 2)
            {
                //<������|�ļ�����·��> 
                Attachment attachment = new Attachment(strs[1], GetMediaType(strs[1]));
                attachment.NameEncoding = _Encoding;
                attachment.TransferEncoding = TransferEncoding.Base64;                
                attachment.ContentDisposition.FileName = string.Format("=?{0}?B?{1}?=", _Encoding.HeaderName, GetBase64String(strs[0], _Encoding));
                return attachment;                
            }
            else
            {
                return null;
            }
        }

        private string GetBase64String(string str, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        private static string GetMediaType(string fileName)
        {
            if (fileName.IsNullOrEmpty())
            {
                return MediaTypeNames.Application.Octet;
            }
            else
            {
                if (fileName.EndsWith(".pdf", StringComparison.CurrentCultureIgnoreCase))
                {
                    return MediaTypeNames.Application.Pdf;
                }
                else if (fileName.EndsWith(".zip", StringComparison.CurrentCultureIgnoreCase))
                {
                    return MediaTypeNames.Application.Zip;
                }
                else if (fileName.EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase))
                {
                    return MediaTypeNames.Image.Gif;
                }
                else if (fileName.EndsWith(".jpeg", StringComparison.CurrentCultureIgnoreCase))
                {
                    return MediaTypeNames.Image.Jpeg;
                }
                else if (fileName.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase))
                {
                    return MediaTypeNames.Image.Jpeg;
                }
                else if (fileName.EndsWith(".tiff", StringComparison.CurrentCultureIgnoreCase))
                {
                    return MediaTypeNames.Image.Tiff;
                }
                else if (fileName.EndsWith(".html", StringComparison.CurrentCultureIgnoreCase))
                {
                    return MediaTypeNames.Text.Html;
                }
                else if (fileName.EndsWith(".htm", StringComparison.CurrentCultureIgnoreCase))
                {
                    return MediaTypeNames.Text.Html;
                }
                else if (fileName.EndsWith(".txt", StringComparison.CurrentCultureIgnoreCase))
                {
                    return MediaTypeNames.Text.Plain;
                }
                else if (fileName.EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
                {
                    return MediaTypeNames.Text.Xml;
                }
                else
                {
                    return MediaTypeNames.Application.Octet;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// �����ʼ��������Ϣ
    /// </summary>
    public class MyMailMessage
    {
        private string m_SmtpServer = string.Empty;
        public string SmtpServer
        {
            get { return m_SmtpServer; }
            set { m_SmtpServer = value; }
        }

        private string m_ServiceAccount = string.Empty;
        public string ServiceAccount
        {
            get { return m_ServiceAccount; }
            set { m_ServiceAccount = value; }
        }

        private string m_ServiceAccountPwd = string.Empty;
        public string ServiceAccountPwd
        {
            get { return m_ServiceAccountPwd; }
            set { m_ServiceAccountPwd = value; }
        }

        private string m_DisplayName = string.Empty;
        public string DisplayName
        {
            get { return m_DisplayName; }
            set { m_DisplayName = value; }
        }

        private string m_To = string.Empty;
        public string To
        {
            get { return m_To; }
            set { m_To = value; }
        }

        private string m_Cc = string.Empty;
        public string Cc
        {
            get { return m_Cc; }
            set { m_Cc = value; }
        }

        private string m_Bcc = string.Empty;
        public string Bcc
        {
            get { return m_Bcc; }
            set { m_Bcc = value; }
        }

        private string m_Subject = string.Empty;
        public string Subject
        {
            get { return m_Subject; }
            set { m_Subject = value; }
        }

        private string m_Body = string.Empty;
        public string Body
        {
            get { return m_Body; }
            set { m_Body = value; }
        }

        private bool m_EnableSsl;
        public bool EnableSsl
        {
            get { return m_EnableSsl; }
            set { m_EnableSsl = value; }
        }

        private int m_Port;
        public int Port
        {
            get { return m_Port; }
            set { m_Port = value; }
        }

        /// <summary>
        /// �����ַ���(�ļ���|�ļ�����·��)
        /// </summary>
        public string Attachments { get; set; }

        /// <summary>
        /// �ʼ����ͳ�ʱʱ��(��������û�����Ϊ�գ�Ĭ��Ϊ300s)
        /// </summary>
        public int TimeoutSeconds { get; set; }
    }
}// �����ռ����
