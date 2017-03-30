using System;
using System.Collections.Generic;
using System.Text;

namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// 生成随机字符串工具类
    /// </summary>
    public class RegKeyUtil
    {
        private string _strin;
        private string _strout;
        private Random _rnd;

        public RegKeyUtil(string strin)
        {
            //_rnd = new Random(System.Environment.TickCount);//System.Environment.TickCount表示系统启动以来的毫秒数 
            _rnd = new Random(System.Guid.NewGuid().GetHashCode());
            _strin = strin; 
        }

        /// <summary>
        /// 根据格式取随机字符
        /// </summary>
        /// <param name="strformat"></param>
        /// <returns></returns>
        private string GetOneRandomNum(string strformat)
        {
            string strtemp;
            switch (strformat)
            {
                case "*":// 大写字母和数字随机组合,如strformat = "****-****-****-****"
                    {
                        int itmp = _rnd.Next(36);
                        if (itmp < 10)
                            strtemp = _rnd.Next(10).ToString();
                        else
                            strtemp = Convert.ToChar(_rnd.Next(26) + 'A').ToString();
                        break;
                    }
                case "#":// 数字,如strformat = "####-####-####-####"
                    {
                        strtemp = _rnd.Next(10).ToString();
                        break;
                    }
                case "$":// 大写字母,如strformat = "$$$$-$$$$-$$$$-$$$$"
                    {
                        strtemp = Convert.ToChar(_rnd.Next(26) + 'A').ToString();
                        break;
                    }
                default:
                    {
                        strtemp = strformat;
                        break;
                    }
            }
            return strtemp;
        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <returns></returns>
        public string GetRandomNum()
        {
            _strout = String.Empty;
            for (int i = 0; i < _strin.Length; i++)
            {
                _strout += this.GetOneRandomNum(_strin[i].ToString());
            }
            return _strout;
        } 

    }// 类结束
}// 命名空间结束
