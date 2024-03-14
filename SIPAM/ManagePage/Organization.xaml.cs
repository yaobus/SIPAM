using ControlzEx.Standard;
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
    /// Organization.xaml 的交互逻辑
    /// </summary>
    public partial class Organization : UserControl
    {
        public Organization()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 页面加载完毕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Organization_OnLoaded(object sender, RoutedEventArgs e)
        {

            OrgListView.ItemsSource = departmentInfos;

            LoadDepartmentData();
        }

        /// <summary>
        /// 窗口尺寸被改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            YLine.Width = SystemParameters.WorkArea.Size.Height - 60;
        }


        private string SelectDepartment = "";

        /// <summary>
        /// 部门被选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrgListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = OrgListView.SelectedIndex;
            if (index != -1)
            {
                 SelectDepartment = departmentInfos[index].Department;
                 
                 DelButton.Visibility = Visibility.Visible;
                 EditButton.Visibility = Visibility.Visible;

                 GroupTextBox.Text = departmentInfos[OrgListView.SelectedIndex].Department;
                 DescriptionText.Text = departmentInfos[OrgListView.SelectedIndex].Description;
            }

            




        }






        /// <summary>
        /// 保存部门信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {   //显示保存按钮

            GroupTextBox.IsEnabled = true;
            DescriptionText.IsEnabled = true;
            SaveButton.Visibility = Visibility.Visible;
            
            //0新增部门，1编辑部门
            EditStatus = 0;

        }



        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            //新增
			if (EditStatus == 0)
			{
				if (GroupTextBox.Text != "" && DescriptionText.Text != "")
				{
					string sqlTemp = string.Format("SELECT COUNT(*) FROM department WHERE department ='{0}' AND status = '0'", GroupTextBox.Text);

					int x = DbClass.ExecuteScalarTableNum(sqlTemp);

					if (x == 0)
					{
						int num = DbClass.ExecuteScalarTableNum("SELECT COUNT(*) FROM department");


						string sql = string.Format("INSERT INTO `department` (`id`, `department`, `description`, `creator`, `date`, `status`) VALUES ({0}, '{1}', '{2}', '{3}', '{4}', '0')", num + 1, GroupTextBox.Text, DescriptionText.Text, GlobalVariable.Variable.UserInfo.User, DateTime.Now);

						DbClass.ModifySql(sql);

						LoadDepartmentData();

						SaveButton.Visibility = Visibility.Collapsed;
						GroupTextBox.IsEnabled = false;
						DescriptionText.IsEnabled = false;
					}
					else
					{

						MessageBox.Show("名称已存在，请勿重复添加！", "重复", MessageBoxButton.OK, MessageBoxImage.Information);

					}



				}
				else
				{
					MessageBox.Show("请完整填写部门信息！", "确定", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
            
			else//编辑
			{
				if (GroupTextBox.Text != "" && DescriptionText.Text != "")
				{
					string sql = string.Format("UPDATE `department` SET `department` = '{0}',`description`='{1}' WHERE `department` = '{2}' AND `description` ='{3}'", GroupTextBox.Text,DescriptionText.Text, departmentInfos[OrgListView.SelectedIndex].Department, departmentInfos[OrgListView.SelectedIndex].Description);

					Console.WriteLine(sql);

					DbClass.ModifySql(sql);

					LoadDepartmentData();

					SaveButton.Visibility = Visibility.Collapsed;
					EditButton.Visibility = Visibility.Collapsed;
					GroupTextBox.IsEnabled = false;
					DescriptionText.IsEnabled = false;


				}
				else
				{
					MessageBox.Show("请完整填写部门信息！", "确定", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}







        }


        ObservableCollection<DepartmentInfo> departmentInfos = new ObservableCollection<DepartmentInfo>();

        private int EditStatus;

        /// <summary>
        /// 从数据库获取部门信息
        /// </summary>
        private void LoadDepartmentData()
        {
            departmentInfos.Clear();

			//查询IP段信息
			string sql = "SELECT * FROM department WHERE status!=1";

			MySqlDataReader reader = DbClass.CarrySqlCmd(sql);

			int id = 0;
            while (reader.Read())
            {
	            id++;
                string department = reader.GetString("department");
                string description = reader.GetString("description");
                string network = DbClass.GetReaderString(reader, "network");
                string creator = reader.GetString("creator");
                DateTime date = reader.GetDateTime("date");


                departmentInfos.Add(new DepartmentInfo(id, department, description, network, creator, date));


            }

            reader.Dispose();


        }




        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelButton_OnClick(object sender, RoutedEventArgs e)
        {


            MessageBoxResult result = MessageBox.Show("确认删除该数据吗？注意此操作不可逆！", "你最好知道自己在做什么！", MessageBoxButton.OKCancel,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.OK)
            {
                string sql = string.Format("UPDATE `department` SET `Status` = 1 WHERE `department` = '{0}'", SelectDepartment);
                DbClass.ModifySql(sql);
                LoadDepartmentData();
                SaveButton.Visibility = Visibility.Collapsed;

            }
        }


        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditButton_OnClick(object sender, RoutedEventArgs e)
		{
			//0新增部门，1编辑部门
			EditStatus = 1;
            GroupTextBox.IsEnabled=true;
            DescriptionText.IsEnabled=true;
            SaveButton.Visibility=Visibility.Visible;



	        
        }
    }
}
