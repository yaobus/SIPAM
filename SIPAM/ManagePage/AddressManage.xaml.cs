using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlzEx.Theming;
using MaterialDesignThemes.Wpf;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SIPAM.DbOperation;
using SIPAM.EncryptionDecryptionFunction;
using SIPAM.GlobalVariable;
using SIPAM.ViewModes;
using static SIPAM.ViewModes.ViewModes;

namespace SIPAM.ManagePage
{
    /// <summary>
    /// AddressManage.xaml 的交互逻辑
    /// </summary>
    public partial class AddressManage : UserControl
    {
        public AddressManage()
        {
            InitializeComponent();


        }






        /// <summary>
        /// 页面加载状态（页面未完成加载时不计算IP）
        /// </summary>
        public bool LoadStatus = false;

        /// <summary>
        /// 初始化加载状态,并加载初始状态IP地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressManage_OnLoaded(object sender, RoutedEventArgs e)
        {
            //页面加载完成，标志位
            LoadStatus = true;

            CalculateIpAddress();

            AddressListView.ItemsSource = AddressInfos;

            LoadAddressData();


        }



        /// <summary>
        /// 点击IP地址项时，加载IP地址配置图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //用于标识不自动生成配置
            selectStatus = 1;

            ClearInput();

            int index = AddressListView.SelectedIndex;

            string tableName;

            GraphicsPlan.Children.Clear();

            if (index != -1)
            {
                tableName = AddressInfos[index].TableName;
                LoadAddressConfig(tableName);

                IpTextBox.Text = AddressInfos[index].Network;
                MaskText.Text = AddressInfos[index].Netmask;

                CalculateNetMaskLocated();
                CalculateIpAddress();

                TbAttention.Text = AddressInfos[index].Attention;

                DelButton.Visibility = Visibility.Visible;

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


                switch (ipAddressInfos[i].AddressStatus)
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
                        description = "锁定IP地址" + "\r" + ipAddressInfos[i].Description + "\r" +
                                      ipAddressInfos[i].ApplyUser;


                        break;

                    case 4:
                        colorBrush = Brushes.OrangeRed;
                        description = "已用IP地址" + "\r" + ipAddressInfos[i].Description + "\r" +
                                      ipAddressInfos[i].ApplyUser;


                        break;


                    case 5:
                        colorBrush = Brushes.LightSlateGray;
                        description = "广播IP地址";


                        break;

                }



                //显示到图形化区域
                GraphicsPlan.Children.Add(new Button()
                {
                    Width = 53,
                    Height = 30,
                    Background = colorBrush,
                    Foreground = Brushes.AliceBlue,
                    Content = i.ToString(),
                    ToolTip = description,
                    //IsEnabled = status,
                    Style = (Style)this.FindResource("MaterialDesignFlatLightBgButton"),
                    Margin = new Thickness(5)

                });

            }

        }



        /// <summary>
        /// 判断是否通过改变掩码位滑动条来生成图形化配置，0为生成，否则不生成
        /// </summary>
        private int selectStatus = 0;

        /// <summary>
        /// 调整子网掩码位数后计算IP地址数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaskSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //加载完成之后才加载配置图示
            if (LoadStatus == true)
            {
                //添加网段的时候才显示配置图示
                if (selectStatus == 0)
                {
                    GraphicsConfig();
                }

            }

        }



        /// <summary>
        /// 页面尺寸调整
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            YLine.Width = SystemParameters.WorkArea.Size.Height - 60;
        }




        /// <summary>
        /// IP地址图形化显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphicsButton_OnClick(object sender, RoutedEventArgs e)
        {
            GraphicsConfig();
        }




        /// <summary>
        /// 配置图形化显示
        /// </summary>
        public void GraphicsConfig()
        {

            GraphicsPlan.Children.Clear();
            CalculateIpAddress();
            string num = NumBox.Text;
            int num2 = Int32.Parse(num);
            GraphicsIpAddress(num2);


        }



        /// <summary>
        /// 计算IP地址数量
        /// </summary>
        public void CalculateIpAddress()
        {
            int value = (int)MaskSlider.Value;
            int num = 32 - value;
            string str = null;
            int tempStr = 8 - num;

            //计算子网掩码二进制数
            for (int i = 0; i < tempStr; i++)
            {
                str += "1";
            }

            for (int i = 0; i < num; i++)
            {
                str += "0";
            }

            int t = Convert.ToInt32(str, 2);
            MaskText.Text = "255.255.255." + t.ToString();
            NumBox.Text = (256 - t).ToString();

            string ipStr = IpTextBox.Text;
            int index = ipStr.LastIndexOf(".") + 1;

            string strTemp = ipStr.Substring(0, index);




        }


        /// <summary>
        /// 计算子网掩码位数
        /// </summary>
        /// <param name="num"></param>
        private void CalculateNetMaskLocated()
        {
            string ipStr = MaskText.Text;
            int index = ipStr.LastIndexOf(".") + 1;

            string strTemp = ipStr.Substring(index, ipStr.Length - index);

            if (strTemp == "0")
            {
                strTemp = "256";
            }

            string str = Convert.ToString(Convert.ToInt32(strTemp), 2);



            index = str.IndexOf("0");


            string str2 = str.Substring(index, str.Length - index);

            // MessageBox.Show(str2);



            MaskSlider.Value = 32 - str2.Length;
        }



        /// <summary>
        /// IP地址图形化到显示区
        /// </summary>
        /// <param name="x"></param>
        public void GraphicsIpAddress(int x)
        {
            List<ViewModes.ViewModes.IpAddressInfo> ipAddress = new List<ViewModes.ViewModes.IpAddressInfo>();
            List<string> lockList = new List<string>();


            //是否锁定其他IP地址
            if (LockBoxOther.IsChecked == true)
            {
                string[] Ls = null;
                Ls = LockedIp.Text.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                lockList = new List<string>(Ls);
            }

            for (int i = 0; i < x; i++)
            {
                Brush colorBrush;
                List<string> LockIp = new List<string>();
                string description;
                //int status;


                if (i == 0)
                {
                    description = "网段IP地址";
                    colorBrush = Brushes.DimGray;
                    //status = 0;
                }
                else
                {
                    if (i == 1 & LockBoxFirst.IsChecked == true)
                    {
                        description = "保留IP地址";
                        colorBrush = Brushes.DarkCyan;
                        //status = 1;

                    }
                    else
                    {
                        if (i == x - 2 & LockBoxLast.IsChecked == true)
                        {

                            description = "保留IP地址";
                            colorBrush = Brushes.DarkCyan;
                            //status = 1;

                        }
                        else
                        {
                            if (i == x - 1)
                            {
                                description = "广播IP地址";
                                colorBrush = Brushes.LightSlateGray;
                                //status = 5;
                            }
                            else
                            {
                                description = "未分配IP地址";
                                colorBrush = Brushes.LimeGreen;
                                //status = 2;
                            }


                        }




                    }


                }


                //手动锁定需要保留的IP地址
                if (lockList.Contains(i.ToString()))
                {
                    description = "保留IP地址";
                    colorBrush = Brushes.DarkCyan;
                    //status = 1;
                }


                //显示到图形化区域
                GraphicsPlan.Children.Add(new Button()
                {
                    Width = 55,
                    Height = 30,
                    Background = colorBrush,
                    Foreground = Brushes.AliceBlue,
                    Content = i.ToString(),
                    ToolTip = description,
                    Style = (Style)this.FindResource("MaterialDesignFlatLightBgButton"),
                    Margin = new Thickness(3)


                });


            }



        }




        /// <summary>
        /// 重新生成配置图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LockBoxFirst_OnClick(object sender, RoutedEventArgs e)
        {
            GraphicsConfig();
        }

        /// <summary>
        /// 重新生成配置图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LockBoxLast_OnClick(object sender, RoutedEventArgs e)
        {
            GraphicsConfig();
        }

        /// <summary>
        /// 重新生成配置图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LockBoxOther_OnClick(object sender, RoutedEventArgs e)
        {
            GraphicsConfig();
        }



        private void IpTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {

        }



        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            selectStatus = 0;
            UnlockInput();
        }


        /// <summary>
        /// 保存网段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

            //判断是不是瞎几把写的IP地址
            if (IsValidIp(IpTextBox.Text) == true)
            {
                //规范化IP输入
                string ip = IpTextBox.Text.Substring(0, IpTextBox.Text.LastIndexOf(".") + 1) + "*";


                if (IpDescription.Text != "")
                {
                    string tableName;

                    string sqlTemp =
                        string.Format("SELECT COUNT(*) FROM network WHERE network = '{0}' AND netmask = '{1}'", ip,
                            MaskText.Text);

                    int num = DbClass.ExecuteScalarTableNum(sqlTemp);

                    //IP地址段已存在
                    if (num > 0)
                    {
                        string msg = string.Format("已存在同配置网段{0}个，是否继续添加同配置网段？", num.ToString());

                        MessageBoxResult result = MessageBox.Show(msg, "确认", MessageBoxButton.YesNo,
                            MessageBoxImage.Information);

                        if (result == MessageBoxResult.Yes)
                        {
                            // 用户点击了"是"按钮，执行相关操作
                            tableName = CreateTableName(ip) + "_" + (num + 1).ToString();

                            int index = DbClass.ExecuteScalarTableNum("SELECT COUNT(*) FROM network");



                            //插入网段信息总表的数据
                            string sql = string.Format(
                                "INSERT INTO network (id,tableName,network,netmask,description,authority,creator,date,status,attention) VALUES ({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},'{9}')",
                                index + 1, tableName, ip, MaskText.Text, IpDescription.Text, "g1p",
                                Variable.UserInfo.User, DateTime.Now, 0, TbAttention.Text);

                            //Console.WriteLine(sql);
                            DbClass.ModifySql(sql);



                            //创建表
                            CreateTable(tableName);


                            //清空输入框
                            ClearInput();

                            //重新加载地址列表
                            LoadAddressData();

                            //装载初始数据
                            InitializedData(tableName);



                        }
                        else if (result == MessageBoxResult.No)
                        {
                            // 用户点击了"否"按钮，取消操作或进行其他处理


                        }


                    }
                    else
                    {
                        tableName = CreateTableName(ip) + "_1";



                        int index = DbClass.ExecuteScalarTableNum("SELECT COUNT(*) FROM network");



                        //插入网段信息总表的数据
                        string sql = string.Format(
                            "INSERT INTO network (id,tableName,network,netmask,description,authority,creator,date,status,attention) VALUES ({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},'{9}')",
                            index + 1, tableName, ip, MaskText.Text, IpDescription.Text, "g1p", Variable.UserInfo.User,
                            DateTime.Now, 0, TbAttention.Text);

                        //Console.WriteLine(sql);
                        DbClass.ModifySql(sql);

                        //创建表
                        CreateTable(tableName);

                        //清空输入
                        ClearInput();

                        //重新加载地址列表
                        LoadAddressData();

                        //装载初始数据
                        InitializedData(tableName);


                    }


                }
                else
                {

                    MessageBox.Show("为了便于后期管理，必须填写网段说明!", "确定", MessageBoxButton.OK, MessageBoxImage.Information);


                }

            }
            else
            {
                MessageBox.Show("IP地址不合法，请检查IP地址是否正确!", "确定", MessageBoxButton.OK, MessageBoxImage.Information);
            }


        }



        /// <summary>
        /// 生成表名
        /// </summary>
        /// <returns></returns>
        private string CreateTableName(string address)
        {
            string name = address.Substring(0, IpTextBox.Text.LastIndexOf(".") + 1) + "1";
            name = "tb_" + name.Replace(".", "_");

            return name;
        }


        /// <summary>
        /// 创建一张数据表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private void CreateTable(string tableName)
        {
            string sql = string.Format(
                "CREATE TABLE IF NOT EXISTS {0} (Address INT NOT NULL ,AddressStatus INT,Description VARCHAR ( 40 ),ApplyUser VARCHAR ( 40 ),UseTo VARCHAR ( 40 ),UserDepartment VARCHAR ( 40 ),Email VARCHAR ( 40 ),UserTel VARCHAR ( 12 ),PhoneNumber VARCHAR ( 14 ),DeviceType VARCHAR ( 40 ),DeviceModel VARCHAR ( 40 ),DeviceMac VARCHAR ( 20 ),DeviceAddress VARCHAR ( 40 ),ApplyTime VARCHAR ( 40 ),Ratify VARCHAR ( 40 ),RatifyTime DATE );",
                tableName);

            MySqlDataReader reader = DbClass.CarrySqlCmd(sql);

            reader.Dispose();
        }




        /// <summary>
        /// 填充网段表单初始数据
        /// </summary>
        private void InitializedData(string tableName)
        {
            //需要写入的IP数量
            int x = Convert.ToInt32(NumBox.Text);

            List<string> lockList = new List<string>();

            string[] ls = null;

            ls = LockedIp.Text.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            lockList = new List<string>(ls);




            for (int i = 0; i < x; i++)
            {
                //IP地址锁定状态：0网段IP，1锁定IP，5广播IP
                int addressStatus = 0;

                //string description;


                List<string> lockip = new List<string>();

                if (i == 0)
                {
                    addressStatus = 0;


                }
                else
                {
                    if (i == 1 & LockBoxFirst.IsChecked == true)
                    {
                        addressStatus = 1;
                    }
                    else
                    {
                        if (i == x - 2 & LockBoxLast.IsChecked == true)
                        {
                            addressStatus = 1;
                        }
                        else
                        {
                            if (i == x - 1)
                            {
                                addressStatus = 5;

                            }
                            else
                            {
                                addressStatus = 2;
                            }

                        }

                    }
                }

                if (LockBoxOther.IsChecked == true)
                {
                    //手动锁定需要保留的IP
                    if (lockList.Contains(i.ToString()))
                    {

                        addressStatus = 1;

                    }
                }




                string sql = string.Format("INSERT INTO `{0}` (`Address`, `AddressStatus`) VALUES ({1}, {2})",
                    tableName, i, addressStatus);


                //DbClass.ExecuteSql(sql);

                //异步执行
                DbClass.ExecuteSqlAsync(sql);


            }

        }





        /// <summary>
        /// 解除按钮禁用
        /// </summary>
        private void UnlockInput()
        {
            IpTextBox.IsEnabled = true;
            IpTextBox.Text = "";
            MaskSlider.IsEnabled = true;
            LockBoxFirst.IsEnabled = true;
            LockBoxLast.IsEnabled = true;
            LockBoxOther.IsEnabled = true;
            LockedIp.IsEnabled = true;
            IpDescription.IsEnabled = true;
            GraphicsButton.IsEnabled = true;
            TbAttention.IsEnabled = true;
            SaveButton.Visibility = Visibility.Visible;
            DelButton.Visibility = Visibility.Hidden;
            GraphicsPlan.Children.Clear();

            TbAttention.Text = "默认网关：\r\n子网掩码：\r\nDNS：\r\nNTP：\r\nKMS：";


        }

        private void ClearInput()
        {
            IpTextBox.Text = "";
            IpDescription.Text = "";

            IpTextBox.IsEnabled = false;
            MaskSlider.IsEnabled = false;
            LockBoxFirst.IsEnabled = false;
            LockBoxLast.IsEnabled = false;
            LockBoxOther.IsEnabled = false;
            LockedIp.IsEnabled = false;
            IpDescription.IsEnabled = false;
            GraphicsButton.IsEnabled = false;
            TbAttention.IsEnabled = false;
            SaveButton.Visibility = Visibility.Collapsed;
            TbAttention.Text = "";
        }


        /// <summary>
        /// 判断是否是合法IP
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        static bool IsValidIp(string ipAddress)
        {
            IPAddress address;
            return IPAddress.TryParse(ipAddress, out address);
        }



        ObservableCollection<AddressInfo> AddressInfos = new ObservableCollection<AddressInfo>();


        /// <summary>
        /// 从数据库获取IP段信息
        /// </summary>
        private void LoadAddressData()
        {
            AddressInfos.Clear();

            //查询IP段信息
            string sql = "SELECT * FROM network WHERE status!=1";

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


                AddressInfos.Add(new AddressInfo(id, tableName, network, netmask, attention, description, authority,
                    creator, date));


            }



            reader.Dispose();



        }


        /// <summary>
        /// 替换中文逗号为英文逗号方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReplaceChineseComma(string input)
        {
            // 使用 Replace 方法将中文逗号替换为英文逗号
            return input.Replace("，", ",");
        }

        /// <summary>
        /// 及时替换用户输入的中文逗号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LockedIp_OnTextChanged(object sender, TextChangedEventArgs e)
        {

            if (LockedIp.Text.IndexOf("，") != -1)
            {
                LockedIp.Text = ReplaceChineseComma(LockedIp.Text);
                LockedIp.Select(LockedIp.Text.Length, 0);
            }



        }



        /// <summary>
        /// 删除网段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelButton_OnClick(object sender, RoutedEventArgs e)
        {


            int index = AddressListView.SelectedIndex;

            MessageBoxResult result = MessageBox.Show("危险！此操作将删除此网段！是否继续？", "你最好清楚自己在做什么", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
               string sql = string.Format("UPDATE `network` SET `status` = '1' WHERE `tableName` = '{0}'", AddressInfos[index].TableName);

                Console.WriteLine(sql);

                DbClass.ModifySql(sql);

                DelButton.Visibility = Visibility.Collapsed;


                //重新加载地址列表
                LoadAddressData();

               



            }
        }
    }
}
