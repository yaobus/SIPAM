using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SIPAM
{
    /// <summary>
    /// ManageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ManageWindow : Window
    {
        public ManageWindow()
        {
            InitializeComponent();
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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                

                try
                {

                    DragMove();
                }
                catch (Exception exception)
                {
                    //Console.WriteLine(exception);
                    //throw;
                }
            }
        }



        /// <summary>
        /// 最大化和标准窗口大小切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {



	        // 获取屏幕的工作区大小
	        double screenWidth = SystemParameters.WorkArea.Width;
	        double screenHeight = SystemParameters.WorkArea.Height;

	        // 获取任务栏的高度
	        double taskbarHeight = SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Height;

	        // 设置窗口样式为无边框
	        WindowStyle = WindowStyle.None;

	        // 设置窗口大小为屏幕的宽度，高度为屏幕工作区的高度减去任务栏的高度
	        Width = screenWidth;
	        Height = screenHeight - taskbarHeight;

	        // 设置窗口位置为屏幕的左上角
	        Left = 0;
	        Top = 0;

		}


        /// <summary>
        /// 窗口加载完毕后载入功能页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           // this.WindowState=WindowState.Maximized;
            ManageOptionPlan.Children.Clear();
            ManageOptionPlan.Children.Add(new ManagePage.Approve());
            



        }



        /// <summary>
        /// 加载地址管理页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RB_AddressManage_Click(object sender, RoutedEventArgs e)
        {
            ManageOptionPlan.Children.Clear();
            ManageOptionPlan.Children.Add(new ManagePage.AddressManage());
        }




        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }




        /// <summary>
        /// 群组管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RB_Group_Click(object sender, RoutedEventArgs e)
        {
            ManageOptionPlan.Children.Clear();
            ManageOptionPlan.Children.Add(new ManagePage.GroupManage());
        }

        /// <summary>
        /// 部门管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RB_Org_Click(object sender, RoutedEventArgs e)
        {
            ManageOptionPlan.Children.Clear();
            ManageOptionPlan.Children.Add(new ManagePage.Organization());
        }


        private void RB_User_Click(object sender, RoutedEventArgs e)
        {
            ManageOptionPlan.Children.Clear();
            ManageOptionPlan.Children.Add(new ManagePage.User());
        }


        /// <summary>
        /// 加载审批页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RB_approve_OnClick(object sender, RoutedEventArgs e)
        {
            ManageOptionPlan.Children.Clear();
            ManageOptionPlan.Children.Add(new ManagePage.Approve());
        }



        /// <summary>
        /// 改变窗口尺寸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// 显示关于页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RB_About_OnClick(object sender, RoutedEventArgs e)
        {
            ManageOptionPlan.Children.Clear();
            ManageOptionPlan.Children.Add(new ManagePage.About());
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
    }
}
