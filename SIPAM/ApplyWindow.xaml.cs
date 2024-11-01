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
            //SelectIpUser.Text = TbUser.Text;
            TbGroup.Text = Variable.UserInfo.Group;
            MySqlKeepAlive.Start();
            RbApply_OnClick(null, null);


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


        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BT_LoginOut_OnClick(object sender, RoutedEventArgs e)
        {


			Window mainWindow = new MainWindow();
			var window = Window.GetWindow(this); //关闭父窗体
			window?.Close();

			//打开新窗口
			mainWindow.Show();


		}


        /// <summary>
        /// 加载申请界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RbApply_OnClick(object sender, RoutedEventArgs e)
        {
			ApplyOptionPlan.Children.Clear();
			ApplyOptionPlan.Children.Add(new UserPage.ApplyPage());

		}

        /// <summary>
        /// 关于
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void RbAbout_OnClick(object sender, RoutedEventArgs e)
        {
	        ApplyOptionPlan.Children.Clear();
	        ApplyOptionPlan.Children.Add(new ManagePage.About());
		}



        private void RbMyAddress_OnClick(object sender, RoutedEventArgs e)
        {
			ApplyOptionPlan.Children.Clear();
			ApplyOptionPlan.Children.Add(new UserPage.MyAddress());
		}
    }
}
