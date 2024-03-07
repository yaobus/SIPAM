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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SIPAM
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadLoginPage();
        }


        /// <summary>
        /// 程序初始页面为登录页面时状态为0
        /// </summary>
        public int LoadStatus = 0;




        /// <summary>
        /// 加载程序初始界面为登录界面
        /// </summary>
        public void LoadLoginPage()
        {
            MainWindowPlan.Children.Clear();
            MainWindowPlan.Children.Add(new MainWindowPage.LoginPage());

        }





        /// <summary>
        /// 切换程序语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanguageSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }




        /// <summary>
        /// 切换数据库设置和登录面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {


            if (LoadStatus == 0)
            {
                MainWindowPlan.Children.Clear();
                MainWindowPlan.Children.Add(new MainWindowPage.DBSettingPage());
                LoadStatus = 1;
            }
            else
            {
                MainWindowPlan.Children.Clear();
                MainWindowPlan.Children.Add(new MainWindowPage.LoginPage());
                LoadStatus = 0;
            }


        }



        /// <summary>
        /// 关闭程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// 允许程序任意位置拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }

        }
    }
}
