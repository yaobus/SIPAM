using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SIPAM.GlobalFunction
{
    internal class IpAddressConvert
    {

        /// <summary>
        /// 将IP地址转换为十进制
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static uint IpToDecimal(string ip)
        {


            // 将 IP 地址字符串解析为 IPAddress 对象
            IPAddress ipAddress = IPAddress.Parse(ip);

            // 获取 IP 地址的字节数组
            byte[] ipAddressBytes = ipAddress.GetAddressBytes();

            // 将字节数组转换为 10 进制数
            uint decimalIpAddress = (uint)(ipAddressBytes[0] << 24 | ipAddressBytes[1] << 16 | ipAddressBytes[2] << 8 | ipAddressBytes[3]);


            return decimalIpAddress;
        }

    }
}
