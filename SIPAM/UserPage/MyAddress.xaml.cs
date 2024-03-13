using MySqlConnector;
using SIPAM.DbOperation;
using SIPAM.GlobalVariable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

namespace SIPAM.UserPage
{
	/// <summary>
	/// MyAddress.xaml 的交互逻辑
	/// </summary>
	public partial class MyAddress : UserControl
	{
		public MyAddress()
		{
			InitializeComponent();
		}



		/// <summary>
		/// 加载用户已申请IP
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MyAddress_OnLoaded(object sender, RoutedEventArgs e)
		{
			MyAddressListView.ItemsSource = myAddressInfos;


			//加载对应群组地址数据
			LoadAddressData(Convert.ToInt32(Variable.UserInfo.Group));
			
			
		}


		//已分配网段
		ObservableCollection<AddressInfo> AddressInfos = new ObservableCollection<AddressInfo>();

		//已申请地址
		ObservableCollection<MyAddressInfo> myAddressInfos = new ObservableCollection<MyAddressInfo>();

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
				string network = reader.GetString("Network");
				DateTime date=DateTime.Now;


				AddressInfos.Add(new AddressInfo(id, tableName, network, "", "", "", "", "", date));
			}

			reader.Dispose();

			int index = 0;
			for (int i = 0; i < AddressInfos.Count; i++)
			{
				sql = string.Format("SELECT * FROM `{0}` WHERE `ApplyUser`='{1}'", AddressInfos[i].TableName, GlobalVariable.Variable.UserInfo.User);
				//Console.WriteLine(sql);

				MySqlDataReader	reader2 = DbClass.CarrySqlCmd(sql);

				while (reader2.Read())
				{
					index++;
					string network = AddressInfos[i].Network;
					int address = reader2.GetInt32("Address");
					string applyUser = reader2.GetString("ApplyUser");
					string description = reader2.GetString("Description");
					string userDepartment = reader2.GetString("UserDepartment");
					string userPhone = reader2.GetString("PhoneNumber");
					string deviceAddress = reader2.GetString("DeviceAddress");


					myAddressInfos.Add(new MyAddressInfo(index,network, address, applyUser, description, userDepartment, userPhone, deviceAddress));
				}

				reader2.Dispose();
			}

			


		}

	}
}
