using MySqlConnector;
using SIPAM.DbOperation;
using SIPAM.EncryptionDecryptionFunction;
using SIPAM.GlobalVariable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static SIPAM.ViewModes.ViewModes;

namespace SIPAM
{
    /// <summary>
    /// RegisterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 点击提交申请按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitButton_OnClick(object sender, RoutedEventArgs e)
        {


            //输入无误，进行下一步操作
            if (CheckInput() == 0)
            {
                //查询用户名是否重复
                string sql = string.Format("SELECT COUNT(*) FROM users WHERE user='{0}'", UserNameBox.Text);

                int num = DbClass.ExecuteScalarTableNum(sql);

                //查询用户名是否没有重复
                if (num == 0)
                {
                    //查询用户总数量sql
                    sql = "SELECT COUNT(*) FROM users";

                    num = DbClass.ExecuteScalarTableNum(sql);

                    string password = Md5EncryptionDecryption.PasswordMd5(UserPasswordBox.Password);

                    string department = departmentList[DepartmentComboBox.SelectedIndex];

                    string tel = TelBox.Text;

                    if (TelBox.Text == "")
                    {
                        tel = "";
                    }
                    else
                    {
                        tel = TelBox.Text;
                    }


                    //组合插入新用户的sql语句
                    sql = string.Format("INSERT INTO users ( id, `user`, `name`, `password`, department, `group`, email, tel, phone, date, `status` ) VALUES ({0},'{1}','{2}','{3}','{4}',{5},'{6}','{7}','{8}','{9}',{10})", num + 1, UserNameBox.Text, RealNameBox.Text, password, department, 2, EmailBox.Text, tel, PhoneBox.Text, DateTime.Now, 0);

                    //Console.WriteLine(sql);
                    DbClass.ModifySql(sql);



                    MessageBoxResult result = MessageBox.Show("注册申请已提交，请等待管理员审核！\r即将返回登录页面！", "注册成功，等待审核", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (result == MessageBoxResult.OK)
                    {
                        BackButton_OnClick(null, null);
                    }

                }
                else
                {
                    //用户名已存在
                    MessageBox.Show("用户名已存在");
                }



            }


            //string sql = "SELECT COUNT(*) FROM users";




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
            if (UserNameBox.Text.Length < 3)
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


            //4.校验用户姓名是否大于等于2位
            if (RealNameBox.Text.Length < 2)
            {
                index += 1;
                message += index + ":姓名应大于2位字符\r";

            }


            //5.校验是否选择了部门
            if (DepartmentComboBox.SelectedIndex == -1)
            {
                index += 1;
                message += index + ":请选择你所在的部门\r";

            }

            //6.校验是否输入了格式正确的EMAIL
            if (!IsValidEmail(EmailBox.Text))
            {
                index += 1;
                message += index + ":请输入正确的邮箱地址\r";

            }

            //7.校验是否输入了格式正确的手机号码
            if (!IsValidPhoneNumber(PhoneBox.Text))
            {

                index += 1;
                message += index + ":请输入正确的手机号码\r";

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

        /// <summary>
        /// 检查是否是瞎几把输入的邮箱地址
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private static bool IsValidEmail(string email)
        {
            // 定义邮箱地址的正则表达式模式
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            // 使用 Regex 类来进行匹配
            Regex regex = new Regex(pattern);

            // 调用 IsMatch 方法检查邮箱地址是否匹配模式
            return regex.IsMatch(email);
        }

        /// <summary>
        /// 检查是否是瞎几把输入的手机号码
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            // 定义手机号码的正则表达式
            string pattern = @"^1[3456789]\d{9}$";

            // 使用正则表达式进行匹配
            return Regex.IsMatch(phoneNumber, pattern);
        }


        /// <summary>
        /// 允许窗口任意拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }

        }

        /// <summary>
        /// 结束程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// 返回登录界面
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
        /// 窗口加载完毕，检查数据库连接是否正常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            GlobalFunction.DbConfigOperate.AnalysisDatabaseConfig(); //解析数据库配置


            //第一步,连接数据库

            if (Variable.DbConnectInfo != null)
            {
                try
                {
                    DbClass.MySqlConnection = DbClass.ConnectionMysql(Variable.DbConnectInfo);
                    DbClass.MySqlConnection.Open(); //连接数据库
                    DepartmentComboBox.ItemsSource = departmentList;
                    LoadDepartmentList();

                }
                catch (Exception)
                {
                    MessageBox.Show("数据库连接失败!请检查数据库配置");
                }
            }
        }


        private List<string> departmentList = new List<string>();

        /// <summary>
        /// 加载部门数据
        /// </summary>
        private void LoadDepartmentList()
        {
            departmentList.Clear();

            string sql = "SELECT * FROM department";

            MySqlDataReader reader = DbClass.CarrySqlCmd(sql);


            while (reader.Read())
            {

                string department = reader.GetString("department");




                departmentList.Add(department);

            }

            reader.Dispose();




        }
    }
}
