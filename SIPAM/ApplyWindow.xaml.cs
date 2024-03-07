using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySqlConnector;
using SIPAM.DbOperation;
using SIPAM.GlobalVariable;
using static SIPAM.ViewModes.ViewModes;

namespace SIPAM
{
    /// <summary>
    /// ApplyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ApplyWindow : Window
    {
        public ApplyWindow()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //窗口最大化
            // this.WindowState = WindowState.Maximized;

            //加载已登录用户信息
            TbUser.Text = Variable.UserInfo.User;
            TbName.Text = Variable.UserInfo.Name;
            TbDepartment.Text = Variable.UserInfo.Department;
            SelectIpUser.Text = TbUser.Text;

            string group = Variable.UserInfo.Group;


            TbGroup.Text = group;


            AddressListView.ItemsSource = AddressInfos;
            CbUseTo.ItemsSource = useList;
            CbUserDepartment.ItemsSource = departmentList;
            CbDeviceType.ItemsSource = deviceTypeList;
            //加载对应群组地址数据
            LoadAddressData(Convert.ToInt32(group));

            LoadInitialData();
        }


        private List<string> departmentList = new List<string>();
        private List<string> useList = new List<string>();
        private List<string> deviceTypeList = new List<string>();

        /// <summary>
        /// 加载初始数据
        /// </summary>
        private void LoadInitialData()
        {
            LoadDepartmentList();

            #region 用途列表
            useList.Add("个人办公");
            useList.Add("视频监控");
            useList.Add("WEB服务");
            useList.Add("邮件服务");
            useList.Add("安全监控");
            useList.Add("安全管理");
            useList.Add("安全认证");
            useList.Add("安全防护");
            useList.Add("网络分析");
            useList.Add("数据库服务");
            useList.Add("虚拟服务");
            useList.Add("云服务");
            useList.Add("核心路由");
            useList.Add("汇聚交换");
            useList.Add("网络接入");
            useList.Add("FTP服务");
            useList.Add("数据备份");
            useList.Add("教学考试");
            useList.Add("应用服务");
            useList.Add("KMS服务");
            useList.Add("NTP服务");
            useList.Add("其他");
            #endregion


            #region 设备类型列表
            deviceTypeList.Add("台式计算机");
            deviceTypeList.Add("便携式计算机");
            deviceTypeList.Add("瘦客户机");
            deviceTypeList.Add("云桌面");
            deviceTypeList.Add("虚拟机");
            deviceTypeList.Add("服务器");
            deviceTypeList.Add("核心路由器");
            deviceTypeList.Add("接入交换机");
            deviceTypeList.Add("防火墙");
            deviceTypeList.Add("行为管理器");
            deviceTypeList.Add("网络音视频设备");
            deviceTypeList.Add("一体化视频会议终端");
            deviceTypeList.Add("视频会议录播服务器");
            deviceTypeList.Add("MCU服务器");
            deviceTypeList.Add("无线路由器");
            deviceTypeList.Add("无线AP");
            deviceTypeList.Add("智能终端");
            deviceTypeList.Add("手机");
            deviceTypeList.Add("IP电话");
            deviceTypeList.Add("POS终端");
            deviceTypeList.Add("打印机");
            deviceTypeList.Add("复印机");
            deviceTypeList.Add("小型机");
            deviceTypeList.Add("视频监控平台");
            deviceTypeList.Add("硬盘录像机");
            deviceTypeList.Add("监控摄像头");
            deviceTypeList.Add("传真机");
            deviceTypeList.Add("扫描仪");
            deviceTypeList.Add("考勤机");
            deviceTypeList.Add("考勤系统");
            deviceTypeList.Add("门禁");
            deviceTypeList.Add("UPS主机");
            deviceTypeList.Add("智能家居设备");
            deviceTypeList.Add("NAS系统");
            deviceTypeList.Add("其他设备");



            #endregion
        }

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






        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }




        /// <summary>
        /// 调整尺寸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {

            // 获取桌面工作区域大小
            double workAreaWidth = SystemParameters.WorkArea.Width;
            double workAreaHeight = SystemParameters.WorkArea.Height;

            // 将窗口大小设置为工作区域大小
            this.Width = workAreaWidth;
            this.Height = workAreaHeight;
            this.Left = SystemParameters.WorkArea.Left;
            this.Top = SystemParameters.WorkArea.Top;

        }


        /// <summary>
        /// 结束程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 记录当前选中的网段对应的表单名称和网段
        /// </summary>
        private string tableName, network;

        /// <summary>
        /// 加载地址图形列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            int index = AddressListView.SelectedIndex;

            TbAttention.Text = AddressInfos[index].Attention;


            GraphicsPlan.Children.Clear();

            if (CountIp() > 0)
            {
                foreach (Button button in IpSelectPanel.Children)
                {

                    IpSelectPanel.UnregisterName("bt" + button.Content);

                }

            }


            IpSelectPanel.Children.Clear();


            if (index != -1)
            {
                tableName = AddressInfos[index].TableName;
                network = AddressInfos[index].Network;
                LoadAddressConfig(tableName);
            }

        }





        /// <summary>
        /// 加载IP地址图形化表
        /// </summary>
        private void LoadAddressConfig(string tableName)
        {


            string sql = string.Format("SELECT * FROM {0} ORDER BY Address ASC", tableName);

            MySqlDataReader reader = DbClass.CarrySqlCmd(sql);

            List<ViewModes.ViewModes.IpAddressInfo> ipAddressInfos = new List<ViewModes.ViewModes.IpAddressInfo>();

            while (reader.Read())
            {
                int address = reader.GetInt32("Address");
                int addressStatus = reader.GetInt32("AddressStatus");
                string description = DbClass.GetReaderString(reader, "Description");
                string applyUser = DbClass.GetReaderString(reader, "ApplyUser");


                string useTo = "";
                string userDepartment = "";

                string email = "";
                string userTel = "";
                string phoneNumber = "";
                string deviceType = "";

                string deviceModel = "";
                string deviceMac = "";
                string deviceAddress = "";
                DateTime applyTime = DateTime.MinValue;

                string ratify = "";
                DateTime ratifyTime = DateTime.MinValue;

                ViewModes.ViewModes.IpAddressInfo ipAddress = new IpAddressInfo(address, addressStatus, description,
                    applyUser, useTo, userDepartment, email, userTel, phoneNumber, deviceType, deviceModel, deviceMac,
                    deviceAddress, applyTime, ratify, ratifyTime);

                ipAddressInfos.Add(ipAddress);
            }

            reader.Dispose();

            WriteAddressConfig(ipAddressInfos);
        }

        /// <summary>
        /// 配置图形化显示
        /// </summary>
        /// <param name="ipAddressInfos"></param>
        private void WriteAddressConfig(List<ViewModes.ViewModes.IpAddressInfo> ipAddressInfos)
        {

            int x = ipAddressInfos.Count;

            for (int i = 0; i < x; i++)
            {
                //bool status = false;

                string description = null;
                Brush colorBrush = null;
                int status = ipAddressInfos[i].AddressStatus;

                switch (status)
                {
                    case 0:
                        colorBrush = Brushes.DimGray;
                        description = "网段IP地址";

                        break;

                    case 1:
                        colorBrush = Brushes.DarkCyan;
                        description = "保留IP地址";

                        break;

                    case 2:
                        colorBrush = Brushes.LimeGreen;
                        description = "可用IP地址";
                        //status = true;

                        break;

                    case 3:
                        colorBrush = Brushes.DarkOrange;
                        description = "锁定IP地址" + "\r" + ipAddressInfos[i].Description + "\r" + ipAddressInfos[i].ApplyUser;


                        break;

                    case 4:
                        colorBrush = Brushes.OrangeRed;
                        description = "已用IP地址" + "\r" + ipAddressInfos[i].Description + "\r" + ipAddressInfos[i].ApplyUser;


                        break;


                    case 5:
                        colorBrush = Brushes.LightSlateGray;
                        description = "广播IP地址";


                        break;

                }


                Button newButton = new Button()
                {
                    Width = 55,
                    Height = 30,
                    Background = colorBrush,
                    Foreground = Brushes.AliceBlue,
                    Content = i.ToString(),
                    ToolTip = description,
                    Tag = status,
                    Style = (Style)this.FindResource("MaterialDesignRaisedLightButton"),
                    Margin = new Thickness(3),

                };



                newButton.Click += NewButton_Click;





                //显示到图形化区域
                GraphicsPlan.Children.Add(newButton);

            }

        }


        /// <summary>
        /// IP地址被选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {

                int tag = Convert.ToInt32(button.Tag);



                if (tag == 2)
                {
                    string text = button.Content.ToString();

                    if (button.Background == Brushes.LimeGreen)
                    {
                        //紫色代表被选中
                        button.Background = Brushes.BlueViolet;

                        Button bt = new Button();


                        string name = "bt" + button.Content;

                        bt.Content = button.Content;
                        bt.Background = button.Background;
                        bt.Height = button.Height;
                        bt.Width = button.Width;
                        bt.Margin = button.Margin;
                        bt.Style = (Style)this.FindResource("MaterialDesignRaisedLightButton");
                        IpSelectPanel.Children.Add(bt);
                        IpSelectPanel.RegisterName(name, bt);


                    }
                    else
                    {
                        //绿色代表未被选中
                        button.Background = Brushes.LimeGreen;

                        Button bt = IpSelectPanel.FindName("bt" + button.Content.ToString()) as Button;

                        if (bt != null)
                        {
                            IpSelectPanel.Children.Remove(bt);
                            IpSelectPanel.UnregisterName("bt" + button.Content.ToString());
                        }

                    }

                }

                //统计已选择IP数量
                SelectIpNum.Text = CountIp().ToString();
            }
        }

        /// <summary>
        /// 统计已选择IP数量
        /// </summary>
        public int CountIp()
        {

            int num = IpSelectPanel.Children.Count;


            return num;


        }

        /// <summary>
        /// 提交申请
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BT_ApplyIP_Click(object sender, RoutedEventArgs e)
        {

            //已选IP数量大于0
            if (Convert.ToInt32(SelectIpNum.Text) > 0)
            {


                if (CheckInput() == 0)
                {

                    MessageBoxResult result =
                        MessageBox.Show("确认提交申请？", "确认？", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        //统计要申请的IP地址
                        List<int> ipList = StatisticsIp();


                        //查询当前网段现有被锁定的IP数量sql语句
                        string queryNum = string.Format("SELECT COUNT(*) FROM {0} WHERE AddressStatus = 3", tableName);

                        //查询当前网段现有被锁定的IP数量
                        int beforeNum = DbClass.ExecuteScalarTableNum(queryNum);


                        //锁定需要申请的IP地址
                        LockIp(ipList);


                        string ipAddress = "";

                        for (int i = 0; i < ipList.Count; i++)
                        {
                            ipAddress += ipList[i] + "|";
                        }


                        //发送申请请求给管理员
                        string sql = string.Format("INSERT INTO logs (`applyUser`,`applyNetwork`,`tableName`,`ip`,`userName`,`userDepartment`,`userTel`,`userPhone`,`deviceType`,`deviceModel`,`deviceMac`,`deviceAddress`,`applyDate`,`applyStatus`) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',{13})", SelectIpUser.Text, network, tableName, ipAddress, IpUser.Text, CbUserDepartment.SelectionBoxItem.ToString(), IpUserTel.Text, IpUserPhone.Text, CbDeviceType.SelectionBoxItem.ToString(), IpUserModel.Text, IpUserModelMac.Text, IpDeviceAddress.Text, DateTime.Now, 0);


                        DbClass.ModifySql(sql);

                        //循环次数
                        int x = 0;

                        //查询到更新数据条数符合预期或者2秒后进行下一步操作
                        while (DbClass.ExecuteScalarTableNum(queryNum) - beforeNum != ipList.Count || x != 4)
                        {

                            x += 1;

                            Thread.Sleep(500);

                        }

                        //重新装载数据
                        AddressListView_SelectionChanged(null, null);

                    }

                }


            }
            else
            {
                MessageBox.Show("请先选择要申请的IP地址！");
            }


        }






        /// <summary>
        /// 统计已选IP地址
        /// </summary>
        private List<int> StatisticsIp()
        {
            List<int> ipList = new List<int>();

            foreach (Button button in IpSelectPanel.Children)
            {
                int ip = Convert.ToInt32(button.Content);
                ipList.Add(ip);

            }


            return ipList;
        }


        private void LockIp(List<int> ipList)
        {
           

            for (int i = 0; i < ipList.Count; i++)
            {

                string sql = string.Format("UPDATE `{0}` SET `AddressStatus` = 3 WHERE `Address` = {1}", tableName, ipList[i]);


                DbClass.ExecuteSql(sql);

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

            //1.校验用途
            if (CbUseTo.SelectedIndex == -1)
            {
                index += 1;
                message += index + ":请选择地址用途\r";

            }


            //2.校验IP地址使用人是否为空
            if (IpUser.Text.Length < 1)
            {
                index += 1;
                message += index + ":IP地址使用人姓名长度需大于1位字符\r";

            }


            //3.判断用户部门是否已选择
            if (CbUserDepartment.SelectedIndex == -1)
            {
                index += 1;
                message += index + ":请选择使用人部门\r";
            }


            //4.校验是否输入了格式正确的手机号码
            if (!IsValidPhoneNumber(IpUserPhone.Text))
            {

                index += 1;
                message += index + ":请输入正确的手机号码\r";

            }


            //5.校验是否选择了设备类型
            if (CbDeviceType.SelectedIndex == -1)
            {
                index += 1;
                message += index + ":请选择设备类型\r";

            }

            //6.校验是否输入了正确的设备位置
            if (IpDeviceAddress.Text.Length < 2)
            {
                index += 1;
                message += index + ":请输入正确的设备位置\r";

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


        private void BT_ApplySegment_Click(object sender, RoutedEventArgs e)
        {

        }


        private void BT_PasswordModify_Click(object sender, RoutedEventArgs e)
        {

        }


        //已分配网段
        ObservableCollection<AddressInfo> AddressInfos = new ObservableCollection<AddressInfo>();

        /// <summary>
        /// 从数据库获取网段信息
        /// </summary>
        /// <param name="sql"></param>
        private void LoadAddressData(int groupId)
        {

            AddressInfos.Clear();


            string sql = string.Format("SELECT * FROM network WHERE authority LIKE '%g{0}p%'", groupId);


            // 查询IP段信息
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


                AddressInfos.Add(new AddressInfo(id, tableName, network, netmask, attention, description, authority, creator, date));
            }

            reader.Dispose();


        }




        private void SizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }
    }
}
