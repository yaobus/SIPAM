using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using MySqlConnector;
using SIPAM.DbOperation;
using SIPAM.EncryptionDecryptionFunction;
using SIPAM.GlobalVariable;

namespace SIPAM.MainWindowPage
{
    /// <summary>
    /// LoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoginPage : UserControl
    {
        public LoginPage()
        {
            InitializeComponent();
        }





        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalFunction.DbConfigOperate.AnalysisDatabaseConfig();//解析数据库配置


            //第一步,连接数据库

            if (Variable.DbConnectInfo != null)
            {
                try
                {
                    DbClass.MySqlConnection = DbClass.ConnectionMysql(Variable.DbConnectInfo);
                    DbClass.MySqlConnection.Open(); //连接数据库

                }
                catch (Exception)
                {
                    MessageBox.Show("数据库连接失败!请检查数据库配置");
                }

                //第二步,查询比对管理员登录信息
                string sql = string.Format("SELECT * FROM users WHERE user='{0}'", UserNameBox.Text);

                MySqlDataReader reader = DbClass.CarrySqlCmd(sql);
                if (reader.Read())
                {

                    string password = reader.GetString("PASSWORD");

                    if (password == Md5EncryptionDecryption.PasswordMd5(UserPasswordBox.Password))//密码正确
                    {
                        //查看用户群组
                        int group = Convert.ToInt32(reader.GetString("group"));


                        GetUserInfo(reader);



                        //释放连接资源
                        reader.Dispose();

                        //判断用户类型，弹出相应的页面
                        switch (group)
                        {
                            //管理员组
                            case 1:

                                Window mainWindow = new ManageWindow();
                                var window = Window.GetWindow(this);//关闭父窗体
                                window?.Close();

                                //打开新窗口
                                mainWindow.Show();
                                break;

                            //游客组
                            case 2:

                                MessageBox.Show("用户正在等待审核！","等待审核",MessageBoxButton.OK,MessageBoxImage.Exclamation);

                                break;

                            //普通用户组
                            default:

                                mainWindow = new ApplyWindow();
                                window = Window.GetWindow(this);//关闭父窗体
                                window?.Close();

                                //打开新窗口
                                mainWindow.Show();

                                break;
                        }


                    }
                    else
                    {
                        MessageBox.Show("用户名或密码错误!");
                    }

                }
                else
                {
                    MessageBox.Show("用户名或密码错误!");
                }

            }


        }

        //检查输入的数据是否为空，为空则输出空字符串，否则原样输出
        public Func<string, string> processInput = input => string.IsNullOrEmpty(input) ? "" : input;





        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="reader"></param>
        public void GetUserInfo(MySqlDataReader reader)
        {


            Variable.UserInfo.Id = reader.GetInt32("id");

            //MessageBox.Show(Variable.UserInfo.Id.ToString());

            Variable.UserInfo.User = reader.GetString("user");
            Variable.UserInfo.Name = reader.GetString("name");
            Variable.UserInfo.Password = reader.GetString("password");
            Variable.UserInfo.Department = reader.GetString("department");
            Variable.UserInfo.Group = reader.GetString("group");


            Variable.UserInfo.Email = processInput(reader.GetString("email"));
            Variable.UserInfo.Tel = processInput(reader.GetString("tel"));
            Variable.UserInfo.Phone = processInput(reader.GetString("phone"));
            Variable.UserInfo.Date = reader.GetDateTime("date");
            Variable.UserInfo.Status = reader.GetInt32("status");



        }




        /// <summary>
        /// 点击注册按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Window registerWindow = new RegisterWindow();
            var window = Window.GetWindow(this);//关闭父窗体
            window?.Close();

            //打开新窗口
            registerWindow.Show();



        }

    }
}
