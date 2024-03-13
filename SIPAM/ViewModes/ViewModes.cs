using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPAM.ViewModes
{
	internal class ViewModes
	{


		/// <summary>
		/// ip地址信息
		/// </summary>
		public class IpAddressInfo
		{
			//IP地址
			public int Address { get; set; }

			//地址状态
			public int AddressStatus { get; set; }

			//注释
			public string Description { get; set; }

			//IP地址申请人
			public string ApplyUser { get; set; }

			//地址用途
			public string UseTo { get; set; }

			//使用人部门
			public string UserDepartment { get; set; }

			//使用人EMAIL
			public string Email { get; set; }

			//使用人电话
			public string UserTel { get; set; }

			// 使用人手机号
			public string PhoneNumber { get; set; }

			//设备类型
			public string DeviceType { get; set; }

			//设备型号
			public string DeviceModel { get; set; }

			//设备MAC地址
			public string DeviceMac { get; set; }

			//设备所在位置
			public string DeviceAddress { get; set; }

			//地址申请时间
			public DateTime ApplyTime { get; set; }

			// 批准人
			public string Ratify { get; set; }

			// 批准时间
			public DateTime RatifyTime { get; set; }


			public IpAddressInfo(int address, int addressStatus, string description, string applyUser, string useTo,
				string userDepartment, string email, string userTel, string phoneNumber, string deviceType, string deviceModel,
				string deviceMac, string deviceAddress, DateTime applyTime, string ratify, DateTime ratifyTime)
			{
				Address = address;
				AddressStatus = addressStatus;
				Description = description;
				ApplyUser = applyUser;
				UseTo = useTo;
				UserDepartment = userDepartment;
				Email = email;
				UserTel = userTel;
				PhoneNumber = phoneNumber;
				DeviceType = deviceType;
				DeviceModel = deviceModel;
				DeviceMac = deviceMac;
				DeviceAddress = deviceAddress;
				ApplyTime = applyTime;
				Ratify = ratify;
				RatifyTime = ratifyTime;

			}
		}


		/// <summary>
		/// 用户信息
		/// </summary>
		public class UserInfo
		{
			public int Id { get; set; }

			public string User { get; set; }

			public string Name { get; set; }

			public string Password { get; set; }

			public string Department { get; set; }

			public string Group { get; set; }

			public string Email { get; set; }

			public string Tel { get; set; }

			public string Phone { get; set; }

			public DateTime? Date { get; set; }

			public int Status { get; set; }

			public UserInfo(int id, string user, string name, string password, string department, string group, string email, string tel, string phone, DateTime? date, int status)
			{
				Id = id;
				User = user;
				Name = name;
				Password = password;
				Department = department;
				Group = group;
				Email = email;
				Tel = tel;
				Phone = phone;
				Date = date;
				Status = status;
			}

		}


		/// <summary>
		/// 用户组信息
		/// </summary>
		public class GroupInfo
		{
			public int Id { get; set; }

			public string GroupName { get; set; }

			public string Description { get; set; }

			//网段
			public string Authority { get; set; }

			public string Creator { get; set; }

			public DateTime Date { get; set; }

			public GroupInfo(int id, string groupName, string description, string authority, string creator, DateTime date)
			{
				Id = id;
				GroupName = groupName;
				Description = description;
				Authority = authority;
				Creator = creator;
				Date = date;

			}

		}


		/// <summary>
		/// 网段信息
		/// </summary>
		public class AddressInfo
		{
			public int Id { get; set; }

			public string TableName { get; set; }

			public string Network { get; set; }

			public string Netmask { get; set; }

			public string Attention { get; set; }

			public string Description { get; set; }

			public string Authority { get; set; }


			public string Creator { get; set; }

			public DateTime Date { get; set; }



			public AddressInfo(int id, string tableName, string network, string netmask, string attention, string description, string authority, string creator, DateTime date)
			{
				Id = id;
				TableName = tableName;
				Network = network;
				Netmask = netmask;
				Attention = attention;
				Description = description;
				Authority = authority;
				Creator = creator;
				Date = date;

			}

		}


		/// <summary>
		/// 部门信息
		/// </summary>
		public class DepartmentInfo
		{
			public int Id { get; set; }

			public string Department { get; set; }

			public string Description { get; set; }

			public string Network { get; set; }

			public string Creator { get; set; }

			public DateTime Date { get; set; }

			public DepartmentInfo(int id, string department, string description, string network, string creator, DateTime date)
			{
				Id = id;
				Department = department;
				Description = description;
				Network = network;
				Creator = creator;
				Date = date;

			}

		}



		public class ApproveInfo
		{
			//序号
			public int Id { get; set; }

			//申请用户
			public string ApplyUser { get; set; }

			//申请网段
			public string ApplyNetwork { get; set; }

			//申请的Ip
			public string ApplyIp { get; set; }

			//使用人
			public string UserName { get; set; }

			//使用人部门
			public string UserDepartment { get; set; }

			//使用人电话
			public string UserPhone { get; set; }

			//设备类型
			public string DeviceType { get; set; }

			//设备位置
			public string DeviceAddress { get; set; }

			//网段表名
			public string TableName { get; set; }


			public ApproveInfo(int id, string applyUser, string applyNetwork, string applyIp, string userName, string userDepartment, string userPhone, string deviceType, string deviceAddress, string tableName)
			{
				Id = id;
				ApplyUser = applyUser;
				ApplyNetwork = applyNetwork;
				ApplyIp = applyIp;
				UserName = userName;
				UserDepartment = userDepartment;
				UserPhone = userPhone;
				DeviceType = deviceType;
				DeviceAddress = deviceAddress;
				TableName = tableName;

			}
		}


		/// <summary>
		/// 我已申请的IP地址信息
		/// </summary>
		public class MyAddressInfo
		{
			public int Index { get; set; }

			public string Network { get; set; }

			public int Address { get; set; }

			public string ApplyUser { get; set; }

			public string Description { get; set; }

			public string UserDepartment { get; set; }

			public string UserPhone { get; set; }

			public string DeviceAddress { get; set; }


			public MyAddressInfo(int index, string network, int address, string applyUser, string description, string userDepartment, string userPhone, string deviceAddress)
			{
				Index = index;
				Network = network;
				Address = address;
				ApplyUser = applyUser;
				Description = description;
				UserDepartment = userDepartment;
				UserPhone = userPhone;
				DeviceAddress = deviceAddress;
			}
		}
	}
}
