using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NacosConfig.Util
{
    internal class MD5Util
    {
        /// <summary>
        /// 获取MD5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] source = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < source.Length; i++)
            {
                sBuilder.Append(source[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

    }
}
