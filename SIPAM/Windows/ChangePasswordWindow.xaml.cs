using MySqlConnector;
using SIPAM.DbOperation;
using SIPAM.EncryptionDecryptionFunction;
using SIPAM.GlobalVariable;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SIPAM.Windows
{
    /// <summary>
    /// ChangePasswordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        public ChangePasswordWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            Window mainWindow = new MainWindow();
            var window = Window.GetWindow(this); //关闭父窗体
            window?.Close();

            //打开新窗口
            mainWindow.Show();
        }



        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordButton_OnClick(object sender, RoutedEventArgs e)
        {
            //输入输入无误，进行下一步操作
            if (CheckInput()==0)
            {
                //解析数据库配置
                GlobalFunction.DbConfigOperate.AnalysisDatabaseConfig();

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

                    //第二步,查询比对用户登录信息
                    string sql = string.Format("SELECT * FROM users WHERE user='{0}'", TbUserName.Text);

                    MySqlDataReader reader = DbClass.CarrySqlCmd(sql);
                    if (reader.Read())
                    {

                        string password = reader.GetString("PASSWORD");
                        string newPassword = Md5EncryptionDecryption.PasswordMd5(UserPasswordBox.Password);

                        if (password == Md5EncryptionDecryption.PasswordMd5(UserOldPassword.Password))//密码正确
                        {

                            sql = string.Format("UPDATE `users` SET `password` = '{0}' WHERE `user` = '{1}'", newPassword, TbUserName.Text);

                            Console.WriteLine(sql);

                            DbClass.ModifySql(sql);

                            MessageBoxResult result = MessageBox.Show("密码已修改，请使用新密码登录！", "密码修改成功", MessageBoxButton.OK,
                                MessageBoxImage.Information);

                            if (result == MessageBoxResult.OK)
                            {
                                BackButton_OnClick(null, null);
                            }



                            //释放连接资源
                            reader.Dispose();




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



        }

        /// <summary>
        /// 检查用户输入是否合法
        /// </summary>
        /// <returns></returns>
        private int CheckInput()
        {

            string message = "";
            int index = 0;

            //1.校验用户名
            if (TbUserName.Text.Length < 3)
            {
                index += 1;
                message += index + ":用户名长度需要大于3位字符\r";

            }


            //2.校验密码长度是否小于6位
            if (UserPasswordBox.Password.Length < 6)
            {
                index += 1;
                message += index + ":密码长度需要大于等于6位字符\r";

            }


            //3.判断两次输入的密码是否一致
            if (UserPasswordBoxAgain.Password != UserPasswordBox.Password)
            {
                index += 1;
                message += index + ":两次密码输入不一致\r";
            }



            if (index > 0)
            {
                MessageBox.Show(message, "发现" + index + "项内容需要注意！", MessageBoxButton.OK, MessageBoxImage.Warning);
                return index;
            }
            else
            {
                return 0;
            }



        }

    }
}
