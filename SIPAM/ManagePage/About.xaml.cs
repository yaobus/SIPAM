using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
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

namespace SIPAM.ManagePage
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
        }


        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/yaobus/SIPAM.git");
        }

        private void IpamNoteTextBlock_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/yaobus/IPAM-NOTE");
        }
    }
}
