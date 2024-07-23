using MySqlConnector;
using SIPAM.DbOperation;
using SIPAM.GlobalVariable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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
using static SIPAM.ViewModes.ViewModes;

namespace SIPAM.ManagePage
{
    /// <summary>
    /// GroupManage.xaml 的交互逻辑
    /// </summary>
    public partial class GroupManage : UserControl
    {
        public GroupManage()
        {
            InitializeComponent();
        }







        private void GroupListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupListView.SelectedIndex != -1)
            {

                LoadUserData();

                //授权标记
                string authorityTag = "g" + groupInfos[GroupListView.SelectedIndex].Id.ToString() + "p";


                //查询群组可用IP段信息
                string sql = string.Format("SELECT * FROM network WHERE authority Like '%{0}%' AND status!=1", authorityTag);
                LoadAddressData(sql, 1);


                sql = string.Format("SELECT * FROM network WHERE authority NOT Like '%{0}%' AND status!=1", authorityTag);
                LoadAddressData(sql, 2);


                //群组进入待编辑状态
                EditButton.IsEnabled = true;



                //加载当前选中群组信息
                // int groupId = groupInfos[GroupListView.SelectedIndex].Id;

                GroupTextBox.Text = groupInfos[GroupListView.SelectedIndex].GroupName;
                DescriptionTextBox.Text = groupInfos[GroupListView.SelectedIndex].Description;


            }
            else
            {
                EditButton.IsEnabled = false;
            }

        }


        private void LoadUserData()
        {
            int index = groupInfos[GroupListView.SelectedIndex].Id;

            userInfos.Clear();

            string sql = string.Format("SELECT * FROM users WHERE `group`={0}", index);


            //查询IP段信息

            MySqlDataReader reader = DbClass.CarrySqlCmd(sql);


            while (reader.Read())
            {
                int id = reader.GetInt32("id");
                string name = reader.GetString("name");
                string department = reader.GetString("department");
                string phone = reader.GetString("phone");



                userInfos.Add(new UserInfo(id, "", name, "", department, "", "", "", phone, DateTime.Now, 0));

            }

            reader.Dispose();


        }



        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            YLine.Width = SystemParameters.WorkArea.Size.Height - 60;
        }

        /// <summary>
        /// 判断是新建群组还是编辑群组，0为新建，1为编辑
        /// </summary>
        private int SaveStatus = 0;

        /// <summary>
        /// 点击新建按钮，启用保存和取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {//保存按钮为新建模式
            SaveStatus = 0;

            GroupTextBox.IsEnabled = true;
            DescriptionTextBox.IsEnabled = true;

            //启用编辑和保存按钮
            CancelButton.IsEnabled = true;
            SaveButton.IsEnabled = true;

            //禁用编辑按钮
            EditButton.IsEnabled = false;

            GroupListView.IsEnabled = false;
        }


        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            //新建群组后保存
            if (SaveStatus == 0)
            {
                if (GroupTextBox.Text != "" && DescriptionTextBox.Text != "")
                {
                    //先查询该名称的群组是否存在,=1则存在
                    string sql = string.Format("SELECT COUNT(*) FROM `groups` WHERE `group` = '{0}'", GroupTextBox.Text);

                    int num = DbClass.ExecuteScalarTableNum(sql);

                    //待写入的群组不存在，则写入新群组
                    if (num == 0)
                    {

                        string sql2 = "SELECT COUNT(*) FROM `groups`";

                        //查询现有共有多少个群组
                        int num2 = DbClass.ExecuteScalarTableNum(sql2);


                        //插入新的群组信息
                        string sql3 = string.Format("INSERT INTO `groups` (id，`group`,description,authority,creator,date,status) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6})", num2 + 1, GroupTextBox.Text, DescriptionTextBox.Text, "100000000", Variable.UserInfo.User, DateTime.Now, 0);
                        //Console.WriteLine(sql3);
                        DbClass.ModifySql(sql3);



                        GroupTextBox.IsEnabled = false;
                        GroupTextBox.Text = "";

                        DescriptionTextBox.IsEnabled = false;
                        DescriptionTextBox.Text = "";

                        SaveButton.IsEnabled = false;
                        CancelButton.IsEnabled = false;

                        //重新加载群组列表
                        LoadGroupData();

                    }
                    else
                    {
                        MessageBox.Show("已存在相同的群组，请勿重复添加！");
                    }


                }
                else
                {

                    MessageBox.Show("请完整填写群组信息！");

                }

            }
            //编辑群组后保存
            else
            {
                if (GroupTextBox.Text != "" && DescriptionTextBox.Text != "")
                {
                    //先查询该名称的群组是否存在,小于或等于1则正常，大于1则和其他群组名称冲突
                    string sql = string.Format("SELECT COUNT(*) FROM `groups` WHERE `group` = '{0}'", GroupTextBox.Text);

                    int num = DbClass.ExecuteScalarTableNum(sql);

                    //待写入的群组不存在或只有当前一个，则更新群组信息
                    if (num <= 1)
                    {
                        //获取当前群组ID
                        int groupId = groupInfos[GroupListView.SelectedIndex].Id;


                        //TODO
                        //更新群组信息
                        string sql3 = string.Format("UPDATE `groups` SET `group`='{0}',description='{1}' WHERE id={2}", GroupTextBox.Text,DescriptionTextBox.Text,groupId);
                        Console.WriteLine(sql3);
                        DbClass.ModifySql(sql3);



                        GroupTextBox.IsEnabled = false;
                        GroupTextBox.Text = "";

                        DescriptionTextBox.IsEnabled = false;
                        DescriptionTextBox.Text = "";

                        SaveButton.IsEnabled = false;
                        CancelButton.IsEnabled = false;


                        //重新加载群组列表
                        LoadGroupData();

                    }
                    else
                    {
                        MessageBox.Show("已存在其他相同的群组名，请勿重复添加！");
                    }


                }
                else
                {

                    MessageBox.Show("请完整填写群组信息！");

                }

            }







        }




        /// <summary>
        /// 窗口加载完毕，加载群组信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupManage_OnLoaded(object sender, RoutedEventArgs e)
        {
            GroupListView.ItemsSource = groupInfos;
            SegmentListView.ItemsSource = AddressInfos;
            SegmentListViewUn.ItemsSource = AddressInfos2;
            UserListView.ItemsSource = userInfos;
            LoadGroupData();

        }

        ObservableCollection<GroupInfo> groupInfos = new ObservableCollection<GroupInfo>();

        //已分配网段
        ObservableCollection<AddressInfo> AddressInfos = new ObservableCollection<AddressInfo>();

        //未分配网段
        ObservableCollection<AddressInfo> AddressInfos2 = new ObservableCollection<AddressInfo>();

        //用户列表
        ObservableCollection<UserInfo> userInfos = new ObservableCollection<UserInfo>();


        /// <summary>
        /// 从数据库获取群组信息
        /// </summary>
        private void LoadGroupData()
        {
            groupInfos.Clear();



            //查询群组
            string sql = "SELECT * FROM `groups` WHERE status!=1";



            MySqlDataReader reader = DbClass.CarrySqlCmd(sql);


            while (reader.Read())
            {
                int id = reader.GetInt32("id");
                string group = reader.GetString("group");
                string description = reader.GetString("description");
                string authority = DbClass.GetReaderString(reader, "authority");
                string creator = reader.GetString("creator");
                DateTime date = reader.GetDateTime("date");



                groupInfos.Add(new GroupInfo(id, group, description, authority, creator, date));


            }

            reader.Dispose();


        }


        /// <summary>
        /// 从数据库获取网段信息
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="index">1，已分配；0，未分配</param>
        private void LoadAddressData(string sql, int index = 0)
        {

            if (index == 1)
            {
                AddressInfos.Clear();

                //查询IP段信息

                MySqlDataReader reader = DbClass.CarrySqlCmd(sql);


                while (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    string tableName = reader.GetString("tableName");
                    string network = reader.GetString("network");
                    string netmask = reader.GetString("netmask");
                    string attention = reader.GetString("attention");
                    string description = reader.GetString("description");
                    string authority = reader.GetString("authority");
                    string creator = reader.GetString("creator");
                    DateTime date = reader.GetDateTime("date");


                    AddressInfos.Add(new AddressInfo(id, tableName, network, netmask,attention, description, authority, creator, date));

                }

                reader.Dispose();
            }
            else
            {
                AddressInfos2.Clear();

                //查询IP段信息

                MySqlDataReader reader = DbClass.CarrySqlCmd(sql);


                while (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    string tableName = reader.GetString("tableName");
                    string network = reader.GetString("network");
                    string netmask = reader.GetString("netmask");
                    string attention = reader.GetString("attention");
                    string description = reader.GetString("description");
                    string authority = reader.GetString("authority");
                    string creator = reader.GetString("creator");
                    DateTime date = reader.GetDateTime("date");


                    AddressInfos2.Add(new AddressInfo(id, tableName, network, netmask,attention, description, authority, creator, date));

                }
                reader.Dispose();
            }


        }







        /// <summary>
        /// 点击移除被选网段访问权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SegmentListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            int group = groupInfos[GroupListView.SelectedIndex].Id;

            if (SegmentListView.SelectedIndex != -1 && group != 1)
            {

                //获取当前选中的网段ID
                int index = AddressInfos[SegmentListView.SelectedIndex].Id;

                //获取当前选中的网段的授权清单
                string oldAuthority = AddressInfos[SegmentListView.SelectedIndex].Authority;

                //组合标签
                string tag = "g" + group + "p";

                //形成新授权清单
                string newAuthority = oldAuthority.Replace(tag, "");

                //构建sql语句
                string sql = string.Format("UPDATE network SET authority='{0}' WHERE id={1}", newAuthority, index);


                //执行SQL语句
                DbClass.ModifySql(sql);

                //更新显示
                GroupListView_SelectionChanged(null, null);


            }
        }



        /// <summary>
        /// 点击添加被选网段访问权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SegmentListViewUn_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int group = groupInfos[GroupListView.SelectedIndex].Id;

            //1为管理员组默认授权全部网段，不可修改；2为已注册未审核用户组，不授权任何网段，不可修改
            if (SegmentListViewUn.SelectedIndex != -1 && group != 1 && group != 2)
            {

                //获取当前选中的网段ID
                int index = AddressInfos2[SegmentListViewUn.SelectedIndex].Id;

                //获取当前选中的网段的授权清单
                string oldAuthority = AddressInfos2[SegmentListViewUn.SelectedIndex].Authority;

                //组合标签
                string tag = "g" + group + "p";

                //形成新授权清单
                string newAuthority = oldAuthority + tag;

                //构建sql语句
                string sql = string.Format("UPDATE network SET authority='{0}' WHERE id={1}", newAuthority, index);


                //执行SQL语句
                DbClass.ModifySql(sql);

                //更新显示
                GroupListView_SelectionChanged(null, null);


            }
        }



        /// <summary>
        /// 进入修改用户组信息模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            //保存按钮为编辑模式
            SaveStatus = 1;

            //禁用新建用户组按钮
            AddButton.IsEnabled = false;
            //SaveLabel.Content = "保存修改";

            //启用相关按钮
            CancelButton.IsEnabled = true;
            SaveButton.IsEnabled = true;

            //启用编辑框
            GroupTextBox.IsEnabled = true;
            DescriptionTextBox.IsEnabled = true;

        }


        /// <summary>
        /// 点击取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            EditButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            AddButton.IsEnabled = true;

            GroupTextBox.Text = "";
            DescriptionTextBox.Text = "";

            //禁用编辑框
            GroupTextBox.IsEnabled = false;
            DescriptionTextBox.IsEnabled = false;

            //启用群组列表
            GroupListView.IsEnabled = true;
        }
    }
}
