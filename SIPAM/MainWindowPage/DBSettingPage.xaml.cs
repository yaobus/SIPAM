using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SIPAM.GlobalFunction;
using SIPAM.GlobalVariable;
using MySqlConnector;
using SIPAM.DbOperation;
using ControlzEx.Standard;

namespace SIPAM.MainWindowPage
{
    /// <summary>
    /// DBSettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class DBSettingPage : UserControl
    {
        public DBSettingPage()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 窗口加载状态
        /// </summary>
        public bool WindowLoadedStatus = false;


        /// <summary>
        /// 修改了数据库地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DbHost_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            

            SaveConfig();



        }



        /// <summary>
        /// 窗口加载完毕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DBSettingPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            WindowLoadedStatus = true;
            LoadConfig();
        }


        /// <summary>
        /// 修改了数据库端口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DbPort_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            SaveConfig();
        }


        /// <summary>
        /// 修改了数据库名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DbName_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            SaveConfig();
        }


        /// <summary>
        /// 修改了数据库用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DbUser_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            SaveConfig();
        }


        /// <summary>
        /// 修改了数据库密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DbPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            SaveConfig();
        }

        /// <summary>
        /// 加载配置信息到编辑框
        /// </summary>
        private void LoadConfig()
        {
            DbConfigOperate.ReadConfig();



            if (Variable.JObject != null)
            {


                try
                {
                    DbHost.Text = Variable.JObject["ServerAddress"].ToString();
                    DbPort.Text = Variable.JObject["ServerPort"].ToString();
                    DbUser.Text = Variable.JObject["DbUser"].ToString();
                    DbPassword.Password = Variable.JObject["DbPassword"].ToString();
                    DbName.Text = Variable.JObject["DbName"].ToString();

                }
                catch (Exception)
                {

                }



            }
            else
            {
                MessageBox.Show("未发现数据库配置文件！\r 点击“确定”立刻配置数据库!","数据库配置文件不存在",MessageBoxButton.OK,MessageBoxImage.Warning);
            }


        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public void SaveConfig()
        {

            if (WindowLoadedStatus == true)
            {
                JObject jObject = new JObject();

                jObject.Add("ServerAddress", DbHost.Text);
                jObject.Add("ServerPort", DbPort.Text);
                jObject.Add("DbName", DbName.Text);
                jObject.Add("DbUser", DbUser.Text);
                jObject.Add("DbPassword", DbPassword.Password);


                //获取程序启动目录
                string path = System.IO.Directory.GetCurrentDirectory() + @"\config\db_config.json";
                string outStr = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);

                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\config"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\config");
                }
                else
                {
                    if (!File.Exists(path))
                    {
                        StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8);

                        streamWriter.WriteLine(outStr);

                        streamWriter.Close();

                        // MessageBox.Show("保存成功!");

                    }
                    else
                    {
                        StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8);

                        streamWriter.WriteLine(outStr);

                        streamWriter.Close();

                        // MessageBox.Show("保存成功!");
                    }
                }




            }



        }


        /// <summary>
        /// 初始表单创建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitializationButton_OnClick(object sender, RoutedEventArgs e)
        {
            GlobalFunction.DbConfigOperate.AnalysisDatabaseConfig();//解析数据库配置
            //LoadBar.Visibility = Visibility.Visible;

            //第一步,连接数据库

            if (Variable.DbConnectInfo != null)
            {
                try
                {
                    DbClass.MySqlConnection = DbClass.ConnectionMysql(Variable.DbConnectInfo);
                    DbClass.MySqlConnection.Open(); //连接数据库
                    CheckDatabase();
                }
                catch (Exception)
                {

                    MessageBox.Show("数据库连接失败!请检查数据库配置！", "数据库连接失败!", MessageBoxButton.OK, MessageBoxImage.Warning);
				}

            }

            
        }


        /// <summary>
        /// 检查数据库中表是否存在，不存在则创建表
        /// </summary>
        /// <returns></returns>
        private void CheckDatabase()
        {
            string[] tableNames = { "users", "network", "logs", "groups", "department" };

            string databaseName = DbName.Text;

            int x = 0;

            foreach (string tableName in tableNames)
            {

                string query = string.Format("SELECT 1 FROM `{0}` LIMIT 1", tableName);


                MySqlCommand command = new MySqlCommand(query, DbClass.MySqlConnection);

                // 尝试执行查询
                try
                {
                    command.ExecuteScalar();
                    Console.WriteLine("表单已存在");
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1146) // MySQL 错误码：表不存在
                    {
                        Console.WriteLine("表单不存在,即将创建表单！");

                        // 创建表
                        CreateNewTable(DbClass.MySqlConnection, tableName);
                        x++;

                    }
                    else
                    {
                        throw; // 如果是其他 MySQL 异常，抛出异常
                    }
                }


            }

            if (x == 5)
            {
                
                Console.WriteLine("表单创建完成，即将装载初始数据!");
                //装载初始数据
                InsertDefaultData();
                Console.WriteLine("初始数据装载完毕!");
            }

            MessageBox.Show("初始表单创建完毕，请返回登录页面！", "完成", MessageBoxButton.OK, MessageBoxImage.Information);

			// LoadBar.Visibility = Visibility.Hidden;
		}






        /// 创建新表的方法
       private static void CreateNewTable(MySqlConnection connection, string tableName)
        {
            string query = "";

            // 创建表的 SQL 语句
            if (tableName == "users")
            {
                query = "CREATE TABLE `users`  ( `id` int NOT NULL, `user` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `name` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `password` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `department` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL, `group` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `email` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL, `tel` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `phone` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `date` datetime NULL DEFAULT NULL, `status` int NULL DEFAULT NULL,  PRIMARY KEY (`id`) USING BTREE)";

            }
            else if (tableName == "network")
            {
                query = "CREATE TABLE `network`(`id` int NOT NULL, `tableName` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,  `network` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `netmask` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL, `description` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `authority` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '授权用户组',  `creator` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `date` datetime NULL DEFAULT NULL,  `status` int NULL DEFAULT NULL,  `attention` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '注意事项',  PRIMARY KEY (`id`) USING BTREE) ";
            }
            else if (tableName == "logs")
            {
                query = "CREATE TABLE `logs`  (  `id` int NOT NULL AUTO_INCREMENT,  `applyUser` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `applyNetwork` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `tableName` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `ip` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,  `userName` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `userDepartment` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `userTel` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `userPhone` varchar(11) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `deviceType` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `deviceModel` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `deviceMac` varchar(12) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `deviceAddress` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `applyDate` datetime NULL DEFAULT NULL COMMENT '申请日期',  `applyStatus` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '申请状态，0：已申请，但还未批准，1已批准，2已驳回',  `ratifyUser` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '批准人',  `ratifyDate` datetime NULL DEFAULT NULL COMMENT '批准日期',  `ratifyNote` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '批准备注',  PRIMARY KEY (`id`) USING BTREE) \r\n";
            }
            else if (tableName == "groups")
            {
                query = "CREATE TABLE `groups`  (  `id` int NOT NULL COMMENT '管理员群组必须是1号群组，待审核是2号群组，请勿修改',  `group` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `description` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `authority` varchar(9) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '权限',  `creator` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `date` datetime NULL DEFAULT NULL,  `status` int NULL DEFAULT NULL,  PRIMARY KEY (`id`) USING BTREE) ";
            }
            else if (tableName == "department")
            {
                query = "CREATE TABLE `department`  (  `id` int NOT NULL,  `department` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,  `description` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `network` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `creator` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,  `date` datetime NULL DEFAULT NULL,  `status` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL) ";
            }

            // 执行 SQL 命令来创建新表
            MySqlCommand command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery();
        }


        /// <summary>
        /// 装载初始数据
        /// </summary>
        private void InsertDefaultData()
        {

            //装载用户群组初始数据
            //管理员
            string sql = string.Format("INSERT INTO `groups` (id，`group`,description,authority,creator,date,status) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6})", 1, "admin", "管理员用户组", "111111111", "system", DateTime.Now, 0);
            DbClass.ModifySql(sql);

            //游客
             sql = string.Format("INSERT INTO `groups` (id，`group`,description,authority,creator,date,status) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6})", 2, "guest", "已注册待审核用户组", "100000000", "system", DateTime.Now, 0);
             DbClass.ModifySql(sql);

            //普通用户组
            sql = string.Format("INSERT INTO `groups` (id，`group`,description,authority,creator,date,status) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6})", 3, "user", "一般用户组", "100000000", "system", DateTime.Now, 0);
            DbClass.ModifySql(sql);


            //装载部门初始数据
            sql = string.Format("INSERT INTO department (id，department,description,network,creator,date,status) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6})", 1, "system", "系统用户组", "", "system", DateTime.Now, 0);
            DbClass.ModifySql(sql);

            //装载管理员初始数据
            sql = string.Format("INSERT INTO users (`id`, `user`, `name`, `password`, `department`, `group`, `email`, `tel`, `phone`, `date`, `status`) VALUES (1, 'admin', 'admin', '375EF066B038516A', 'system', '1', 'admin@admin.com', '', '', '{0}', 0)",DateTime.Now);
            DbClass.ModifySql(sql);

        }

    }
}
