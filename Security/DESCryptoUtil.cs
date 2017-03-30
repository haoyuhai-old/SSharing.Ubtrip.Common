using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace SSharing.Ubtrip.Common
{
    /// <summary>
    /// DES加密算法的辅助类
    /// </summary>
    public static class DESCryptoUtil
    {
        private const string DefaultKey = "s6m5v4a1";
        private const string DefaultIV = "7y2e3h89";

        /// <summary>
        /// 生成随机的Key和初始化向量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        public static void GenerateKeyIV(out string key, out string iv)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.GenerateKey();
            des.GenerateIV();
            key = Encoding.UTF8.GetString(des.Key);
            iv = Encoding.UTF8.GetString(des.IV);
        }

        /// <summary>
        /// 采用DES算法进行加密，使用默认的key, iv及密码模式(CBC)
        /// </summary>
        /// <param name="plainString">将要加密的文本</param>
        /// <returns>加密结果的Base64编码串</returns>
        public static string Encrypt(string plainString)
        {
            return Encrypt(plainString, DefaultKey, DefaultIV, CipherMode.CBC);
        }

        /// <summary>
        /// 采用DES算法进行解密，使用默认的key, iv及密码模式(CBC)
        /// </summary>
        /// <param name="encryptBase64String">加密结果的Base64编码串</param>
        /// <returns>解密之后的原始文本</returns>
        public static string Decrypt(string encryptBase64String)
        {
            return Decrypt(encryptBase64String, DefaultKey, DefaultIV, CipherMode.CBC);
        }
   
        /// <summary>
        /// 采用DES算法进行加密，并指定key, iv及密码模式
        /// </summary>
        /// <param name="plainString">将要加密的文本</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">初始化向量</param>
        /// <param name="cipherMode">指定用于加密的块密码模式, .Net默认情况下指定为CBC, 如果加密模式为ECB则iv无效，指定与key相同的值即可</param>
        /// <returns>加密结果的Base64编码串</returns>
        public static string Encrypt(string plainString, string key, string iv, CipherMode cipherMode)
        {
            if (key == null || key.Length < 8) throw new ArgumentException("key不能为空且至少为8位");
            if (iv == null || iv.Length < 8) throw new ArgumentException("iv不能为空且至少为8位");

            byte[] rgbKey         = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            byte[] rgbIV          = Encoding.UTF8.GetBytes(iv.Substring(0, 8));
            byte[] inputByteArray = Encoding.UTF8.GetBytes(plainString);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Mode = cipherMode;

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            string result = Convert.ToBase64String(ms.ToArray());

            ms.Close();
            cs.Close();
            return result;
        }

        /// <summary>
        /// 采用DES算法进行解密，并指定key, iv及密码模式
        /// </summary>
        /// <param name="encryptBase64String">加密结果的Base64编码串</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">初始化向量</param>
        /// <param name="cipherMode">指定用于加密的块密码模式, .Net默认情况下指定为CBC，如果加密模式为ECB则iv无效，指定与key相同的值即可</param>
        /// <returns>解密之后的原始文本</returns>
        public static string Decrypt(string encryptBase64String, string key, string iv, CipherMode cipherMode)
        {
            if (key == null || key.Length < 8) throw new ArgumentException("key不能为空且至少为8位");
            if (iv == null || iv.Length < 8) throw new ArgumentException("iv不能为空且至少为8位");

            byte[] rgbKey         = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            byte[] rgbIV          = Encoding.UTF8.GetBytes(iv.Substring(0, 8));
            byte[] inputByteArray = Convert.FromBase64String(encryptBase64String);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Mode = cipherMode;

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            string result = Encoding.UTF8.GetString(ms.ToArray());

            ms.Close();
            cs.Close();
            return result;
        }
    }
}
