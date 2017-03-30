using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// 加密解密工具类
    /// </summary>
    public class EncryptUtil
    {
        #region MD5加密

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="FMD5Value">加密字符串</param>
        /// <returns></returns>
        public static string MD5(string FMD5Value)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] b = Encoding.GetEncoding("utf-8").GetBytes(FMD5Value);
            b = md5.ComputeHash(b);
            string s = null;
            for (int i = 0; i <= b.Length - 1; i++)
            {
                s += b[i].ToString("x").PadLeft(2, '0'); ;
            }
            md5 = null;
            return s;
        }

        /// <summary>
        /// 标准MD5字符加密，采用.NET提供的方法
        /// </summary>
        /// <param name="Str">需要加密字符串</param>
        /// <returns></returns>
        public static string DoNetMD5(string sStr)
        {
            if (!string.IsNullOrEmpty(sStr))
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sStr, "MD5");
            }
            else return "";
        }

        #endregion

        #region 其他加密

        /// <summary>
        /// 加密key:不允许修改
        /// </summary>
        private string encryptKey
        {
            get { return "s6m5v4a1"; }
        }
        /// <summary>
        ///  加密初始化向量:不允许修改
        /// </summary>
        private string encryptIV
        {
            get { return "7y2e3h89"; }
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string Encrypt(string encryptString)
        {
            System.IO.MemoryStream mStream = null;
            System.Security.Cryptography.CryptoStream cStream = null;
            try
            {
                EncryptUtil des = new EncryptUtil();
                byte[] rgbKey = Encoding.UTF8.GetBytes(des.encryptKey.Substring(0, 8));
                byte[] rgbIV = Encoding.UTF8.GetBytes(des.encryptIV.Substring(0, 8));
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                System.Security.Cryptography.DESCryptoServiceProvider dCSP = new System.Security.Cryptography.DESCryptoServiceProvider();
                mStream = new System.IO.MemoryStream();
                cStream = new System.Security.Cryptography.CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), System.Security.Cryptography.CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch (Exception)
            {
                return encryptString;
            }
            finally
            {
                if (mStream != null) { mStream.Close(); mStream.Dispose(); }
                if (cStream != null) { cStream.Close(); cStream.Dispose(); }
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="decryptString"></param>
        /// <returns></returns>
        public static string Decrypt(string decryptString)
        {
            System.IO.MemoryStream mStream = null;
            System.Security.Cryptography.CryptoStream cStream = null;
            try
            {
                EncryptUtil des = new EncryptUtil();
                byte[] rgbKey = Encoding.UTF8.GetBytes(des.encryptKey.Substring(0, 8));
                byte[] rgbIV = Encoding.UTF8.GetBytes(des.encryptIV.Substring(0, 8));
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                System.Security.Cryptography.DESCryptoServiceProvider DCSP = new System.Security.Cryptography.DESCryptoServiceProvider();
                mStream = new System.IO.MemoryStream();
                cStream = new System.Security.Cryptography.CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), System.Security.Cryptography.CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
            finally
            {
                if (mStream != null) { mStream.Close(); mStream.Dispose(); }
                if (cStream != null) { cStream.Close(); cStream.Dispose(); }
            }
        }

        /// <summary>
        /// 生成随机字符码
        /// </summary>
        /// <param name="codeLen">随机码长度</param>
        /// <returns>产生的随机码</returns>
        public static string CreateVerifyCode()
        {
            int codeLen = length;
            string[] arr = codeSerial.Split(',');

            string code = string.Empty;

            int randValue = -1;

            //Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            Random rand = new Random(GetRandomSeed());

            for (int i = 0; i < codeLen; i++)
            {
                randValue = rand.Next(0, arr.Length - 1);

                code += arr[randValue];
            }

            return code;
        }

        #endregion

        #region 随机码

        #region 自定义随机码字符串序列(使用逗号分隔)#region 自定义随机码字符串序列(使用逗号分隔)

        static string codeSerial = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";

        #endregion

        #region 验证码长度(默认8个验证码的长度)#region 验证码长度(默认8个验证码的长度)

        static int length = 8;

        #endregion


        public static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        #endregion
    }// 类结束
}// 命名空间结束
