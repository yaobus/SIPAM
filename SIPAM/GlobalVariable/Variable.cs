using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SIPAM.GlobalVariable
{
    internal class Variable
    {


        //数据库路径
        public static string DbPath = Directory.GetCurrentDirectory() + @"\config\db_config.json";//指定配置文件路径


        //数据库配置信息
        public static JObject JObject;







        //----------------------Database-Info-------------------------
        //Remote-Server-Core服务器ip地址
        //public  IPAddress IpAddress;

        //Remote-Server-Core服务器端口
        //public  Int32 ServerPort;

        //数据库连接信息
        public static string DbConnectInfo;



        //当前登录的用户信息
        public static ViewModes.ViewModes.UserInfo UserInfo = new ViewModes.ViewModes.UserInfo(0, "", "", "", "", "", "", "", "", null, 0);




    }
}
