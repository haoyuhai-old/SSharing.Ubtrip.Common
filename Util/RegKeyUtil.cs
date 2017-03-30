using System;
using System.Collections.Generic;
using System.Text;

namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// ��������ַ���������
    /// </summary>
    public class RegKeyUtil
    {
        private string _strin;
        private string _strout;
        private Random _rnd;

        public RegKeyUtil(string strin)
        {
            //_rnd = new Random(System.Environment.TickCount);//System.Environment.TickCount��ʾϵͳ���������ĺ����� 
            _rnd = new Random(System.Guid.NewGuid().GetHashCode());
            _strin = strin; 
        }

        /// <summary>
        /// ���ݸ�ʽȡ����ַ�
        /// </summary>
        /// <param name="strformat"></param>
        /// <returns></returns>
        private string GetOneRandomNum(string strformat)
        {
            string strtemp;
            switch (strformat)
            {
                case "*":// ��д��ĸ������������,��strformat = "****-****-****-****"
                    {
                        int itmp = _rnd.Next(36);
                        if (itmp < 10)
                            strtemp = _rnd.Next(10).ToString();
                        else
                            strtemp = Convert.ToChar(_rnd.Next(26) + 'A').ToString();
                        break;
                    }
                case "#":// ����,��strformat = "####-####-####-####"
                    {
                        strtemp = _rnd.Next(10).ToString();
                        break;
                    }
                case "$":// ��д��ĸ,��strformat = "$$$$-$$$$-$$$$-$$$$"
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
        /// ��������ַ���
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

    }// �����
}// �����ռ����
