using MySqlConnector;
using SIPAM.DbOperation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static SIPAM.ViewModes.ViewModes;

namespace SIPAM.ManagePage
{
    /// <summary>
    /// User.xaml 的交互逻辑
    /// </summary>
    public partial class User : UserControl
    {
        public User()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 加载用户列表,部门表项，群组表项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void User_OnLoaded(object sender, RoutedEventArgs e)
        {
            UserListView.ItemsSource = userInfos;
            CbGroup.ItemsSource = groupList;
            //CbDepartment.ItemsSource = departmentList;

           // LoadDepartment();

            LoadGroupData();

            LoadUserData();
        }


        /// <summary>
        /// 点击选择用户，加载用户信息到编辑框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserListView.SelectedIndex != -1)
            {
                EditButton.IsEnabled = true;
                CbGroup.IsEnabled = true;
                TbUser.Text = userInfos[UserListView.SelectedIndex].User;
                TbName.Text = userInfos[UserListView.SelectedIndex].Name;
                CbDepartment.Text= userInfos[UserListView.SelectedIndex].Department;


				//CbDepartment.SelectedIndex = GetValueKey(userInfos[UserListView.SelectedIndex].Department, departmentDictionary) +1;

				




				TbTel.Text = userInfos[UserListView.SelectedIndex].Tel;
                TbPhone.Text = userInfos[UserListView.SelectedIndex].Phone;


                CbGroup.SelectedIndex = GetValueKey(userInfos[UserListView.SelectedIndex].Group, groupDic) - 1;

                TbEmail.Text = userInfos[UserListView.SelectedIndex].Email;
            }
            else
            {
                EditButton.IsEnabled = false;
                CbGroup.IsEnabled = false;
            }




        }

        private Dictionary<int, string> departmentDictionary = new Dictionary<int, string>();

        private List<string> departmentList = new List<string>();

        /// <summary>
        /// 加载部门数据,并查询当前输入的键值对应的KEY
        /// </summary>
        private void LoadDepartment()
        {

            departmentDictionary.Clear();
            departmentList.Clear();

            string sql = "SELECT * FROM department";


            MySqlDataReader reader = DbClass.CarrySqlCmd(sql);


            while (reader.Read())
            {
                int id = reader.GetInt32("id");
                string department = reader.GetString("department");


                departmentDictionary.Add(id, department);
                departmentList.Add(department);

            }

            reader.Dispose();




        }







        /// <summary>
        /// 调整页面高度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            YLine.Width = SystemParameters.WorkArea.Size.Height - 60;
        }




        ObservableCollection<UserInfo> userInfos = new ObservableCollection<UserInfo>();

        private List<string> groupList = new List<string>();

        private Dictionary<int, string> groupDic = new Dictionary<int, string>();


        /// <summary>
        /// 从数据库获取用户信息
        /// </summary>
        private void LoadUserData()
        {
            userInfos.Clear();



            //查询IP段信息,status=1则用户被删除
            string sql = "SELECT * FROM users WHERE status!=1";

            MySqlDataReader reader = DbClass.CarrySqlCmd(sql);


            while (reader.Read())
            {
                int id = reader.GetInt32("id");
                string user = reader.GetString("user");
                string name = reader.GetString("name");
                string department = reader.GetString("department");
                string group = reader.GetString("group");
                string email = reader.GetString("email");
                string tel = reader.GetString("tel");
                string phone = reader.GetString("phone");
                DateTime date = reader.GetDateTime("date");
                int status = reader.GetInt32("status");

                int index = Convert.ToInt32(group);
                string groupName = groupDic[index];


                userInfos.Add(new UserInfo(id, user, name, "", department, groupName, email, tel, phone, date, status));


            }

            reader.Dispose();


        }


        /// <summary>
        /// 从数据库获取群组信息
        /// </summary>
        private void LoadGroupData()
        {
            groupList.Clear();

            groupDic.Clear();


            //查询群组
            string sql = "SELECT * FROM `groups` WHERE status!=1";



            MySqlDataReader reader = DbClass.CarrySqlCmd(sql);


            while (reader.Read())
            {
                int id = reader.GetInt32("id");
                string group = reader.GetString("group");

                groupList.Add(group);
                groupDic.Add(id, group);

            }

            reader.Dispose();


        }


        /// <summary>
        /// 通过键值获取KEY
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private int GetValueKey(string groupName, Dictionary<int, string> dictionary)
        {
            int index = -1;
            foreach (var pair in dictionary)
            {
                if (pair.Value == groupName)
                {
                    index = pair.Key;

                }
            }

            return index;
        }



        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveButton.Visibility = Visibility.Visible;
            ResetPassword.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

            if (TbUser.Text=="admin")
            {
                MessageBox.Show("Admin用户组无法更改，请勿骚操作！", "快住手", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (CbGroup.SelectedIndex != -1)
                {
                    int groupId = CbGroup.SelectedIndex + 1;

                    string sql = string.Format("UPDATE `users` SET `group` = '{0}' WHERE `id` = {1}", groupId, userInfos[UserListView.SelectedIndex].Id);

                    //DbClass.CarrySqlCmd(sql);


                    DbClass.ExecuteSql(sql);

                    SaveButton.Visibility = Visibility.Hidden;
                    ResetPassword.Visibility = Visibility.Hidden;

                    LoadUserData();
                }
                else
                {
                    MessageBox.Show("用户群组为必选项", "用户群组未设定", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }







        }


        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetPassword_OnClick(object sender, RoutedEventArgs e)
        {




            MessageBoxResult result = MessageBox.Show("该用户密码将被重置为123456\r是否继续?", "重置用户密码", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                string sql = string.Format("UPDATE `users` SET `password` = '375EF066B038516A' WHERE `id` = {0}", userInfos[UserListView.SelectedIndex].Id);

                //DbClass.CarrySqlCmd(sql);
                DbClass.ExecuteSql(sql);
                SaveButton.Visibility = Visibility.Hidden;
                ResetPassword.Visibility = Visibility.Hidden;
                MessageBox.Show("密码重置成功！", "密码重置成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }




        }

    }
}
