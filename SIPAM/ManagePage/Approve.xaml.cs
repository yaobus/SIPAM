using MySqlConnector;
using SIPAM.DbOperation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static SIPAM.ViewModes.ViewModes;

namespace SIPAM.ManagePage
{
    /// <summary>
    /// Overview.xaml 的交互逻辑
    /// </summary>
    public partial class Approve : UserControl
    {
        public Approve()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 启动时加载待批准申请
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Approve_OnLoaded(object sender, RoutedEventArgs e)
        {
            ApproveListView.ItemsSource = approveInfos;
            

            //加载申请列表
            LoadApproveInfos();


        }


        ObservableCollection<ApproveInfo> approveInfos = new ObservableCollection<ApproveInfo>();
        


        /// <summary>
        /// 加载申请信息列表
        /// </summary>
        private void LoadApproveInfos()
        {
            approveInfos.Clear();

            //查询IP段信息,status=1则用户被删除
            string sql = "SELECT * FROM `logs` WHERE applyStatus=0";


            MySqlDataReader reader = DbClass.CarrySqlCmd(sql);

            int x = 0;

            while (reader.Read())
            {
                x++;
                int id = x;
                string applyUser = reader.GetString("applyUser");
                string applyNetwork = reader.GetString("applyNetwork");
                string ip = reader.GetString("ip");
                string userName = reader.GetString("userName");
                string userDepartment = reader.GetString("userDepartment");
                string userPhone = reader.GetString("userPhone");
                string deviceType = reader.GetString("deviceType");
                string deviceAddress = reader.GetString("deviceAddress");
                string applyStatus = reader.GetString("applyStatus");
                string tableName = reader.GetString("tableName");
                

                approveInfos.Add(new ApproveInfo(id, applyUser, applyNetwork, ip, userName, userDepartment, userPhone, deviceType, deviceAddress,tableName));


            }

            reader.Dispose();

        }



        /// <summary>
        /// 批准申请
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApproveButton_OnClick(object sender, RoutedEventArgs e)
        {

            MessageBoxResult result =
                MessageBox.Show("确认批准申请？", "确认？", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                //要批准的IP地址
                List<string> ipList = StatisticsIp(TbApplyIp.Text);
                
                LockIp(ipList);


            }


            //重新加载申请信息列表
            LoadApproveInfos();
        }


        /// <summary>
        /// 分配IP地址(锁定)，更新LOG
        /// </summary>
        /// <param name="ipList"></param>
        private void LockIp(List<string> ipList)
        {
            string tableName = approveInfos[ApproveListView.SelectedIndex].TableName;

            for (int i = 0; i < ipList.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(ipList[i]))
                {
                    string sql = string.Format("UPDATE `{0}` SET `AddressStatus` = 4 WHERE `Address` = {1}", tableName, ipList[i]);


                    DbClass.ExecuteSql(sql);
                    // Console.WriteLine(sql);


                    //applyStatus=0，待批准；applyStatus=1，已批准；applyStatus=2，已驳回；
                    sql = string.Format("UPDATE `logs` SET `applyStatus` = '1', `ratifyUser` = '{0}', `ratifyDate` = '{1}' , `ratifyNote` = '{2}' WHERE `tableName` = '{3}' AND `ip` = '{4}'",GlobalVariable.Variable.UserInfo.Name,DateTime.Now,TbNote.Text, tableName, TbApplyIp.Text);
                    DbClass.ExecuteSql(sql);
                   

                }






            }


        }



        /// <summary>
        /// 统计已选IP地址
        /// </summary>
        private List<string> StatisticsIp(string ipSub)
        {
            List<string> ipList = new List<string>(ipSub.Split('|'));



            return ipList;
        }


        /// <summary>
        /// 驳回申请
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RejectButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show("确认驳回申请？IP地址将被释放！", "确认？", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                //要驳回的IP地址
                List<string> ipList = StatisticsIp(TbApplyIp.Text);

                ReleaseIp(ipList);


            }


            //重新加载申请信息列表
            LoadApproveInfos();
        }


        /// <summary>
        /// 释放IP地址(解除锁定)，更新LOG
        /// </summary>
        /// <param name="ipList"></param>
        private void ReleaseIp(List<string> ipList)
        {
            string tableName = approveInfos[ApproveListView.SelectedIndex].TableName;

            for (int i = 0; i < ipList.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(ipList[i]))
                {
                    string sql = string.Format("UPDATE `{0}` SET `AddressStatus` = 2 WHERE `Address` = {1}", tableName, ipList[i]);


                    DbClass.ExecuteSql(sql);
                    // Console.WriteLine(sql);


                    //applyStatus=0，待批准；applyStatus=1，已批准；applyStatus=2，已驳回；
                    sql = string.Format("UPDATE `logs` SET `applyStatus` = '2', `ratifyUser` = '{0}', `ratifyDate` = '{1}' , `ratifyNote` = '{2}' WHERE `tableName` = '{3}' AND `ip` = '{4}'", GlobalVariable.Variable.UserInfo.Name, DateTime.Now, TbNote.Text, tableName, TbApplyIp.Text);
                    DbClass.ExecuteSql(sql);


                }






            }


        }




        /// <summary>
        /// 点击加载当前所选申请内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApproveListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ApproveListView.SelectedIndex;
            if (index != -1)
            {
                ApproveButton.Visibility = Visibility.Visible;

                RejectButton.Visibility= Visibility.Visible;

                TbApplyUser.Text = approveInfos[ApproveListView.SelectedIndex].ApplyUser;
                TbApplyNetwork.Text = approveInfos[ApproveListView.SelectedIndex].ApplyNetwork;
                TbApplyIp.Text = approveInfos[ApproveListView.SelectedIndex].ApplyIp;
                UserName.Text = approveInfos[ApproveListView.SelectedIndex].UserName;
                TbDepartment.Text = approveInfos[ApproveListView.SelectedIndex].UserDepartment;
                TbPhone.Text = approveInfos[ApproveListView.SelectedIndex].UserPhone;
                TbDeviceType.Text = approveInfos[ApproveListView.SelectedIndex].DeviceType;
                TbDeviceAddress.Text = approveInfos[ApproveListView.SelectedIndex].DeviceAddress;

                TbIpNumber.Text = GetIpCount(TbApplyIp.Text).ToString();






            }
            else
            {
                ApproveButton.Visibility = Visibility.Hidden;

                RejectButton.Visibility = Visibility.Hidden;
            }





        }



        /// <summary>
        /// 获取该用户申请的IP地址总数量
        /// </summary>
        /// <param name="applyIps"></param>
        /// <returns></returns>
        private int GetIpCount(string applyIps)
        {
            List<string> ipList = StatisticsIp(applyIps);
            List<string> newList=new List<string>();
            for (int i = 0; i < ipList.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(ipList[i]))
                {
                    newList.Add(ipList[i]);
                }

            }

            return newList.Count;

        }


        private void Approve_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
			YLine.Width = SystemParameters.WorkArea.Size.Height - 60;
		}
    }
}
