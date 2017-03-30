using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// RSA加密算法的辅助类
    /// </summary>
    public static class RSACryptoUtil
    {
        /// <summary>
        /// 产生RSA算法的密钥
        /// </summary>
        /// <param name="xmlKeys">包含公钥和私钥的Xml字符串</param>
        /// <param name="xmlPublicKey">只包含公钥的Xml字符串</param>
        public static void GenerateRSAKeys(out string xmlKeys, out string xmlPublicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            xmlKeys = rsa.ToXmlString(true);
            xmlPublicKey = rsa.ToXmlString(false);
        } 

        /// <summary>
        /// 执行RSA加密，返回加密结果的Base64编码
        /// </summary>
        /// <param name="xmlString">包含 System.Security.Cryptography.RSA 密钥信息的 XML 字符串</param>
        /// <param name="plainString">要加密的文本</param>
        /// <returns>加密结果的Base64编码</returns>
        public static string Encrypt(string xmlString, string plainString)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlString);
            
            byte[] result = rsa.Encrypt(Encoding.UTF8.GetBytes(plainString), false);
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// 执行RSA解密，返回原始的文本
        /// </summary>
        /// <param name="xmlString">包含 System.Security.Cryptography.RSA 密钥信息的 XML 字符串</param>
        /// <param name="encryptBase64String">加密结果的Base64编码</param>
        /// <returns>原始的文本</returns>
        public static string Decrypt(string xmlString, string encryptBase64String)
        {   
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlString);
            
            byte[] result = rsa.Decrypt(Convert.FromBase64String(encryptBase64String), false);
            return Encoding.UTF8.GetString(result);
        }
    }
}
