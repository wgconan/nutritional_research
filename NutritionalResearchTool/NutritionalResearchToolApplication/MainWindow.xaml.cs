using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using NutritionalResearchToolApplication.Pages;

namespace NutritionalResearchToolApplication
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void mainWindows_Loaded(object sender, RoutedEventArgs e)
        {
            App.Current.Properties["MyFrame"] = myFrame;
        }

        private void btn_GotoMainPage_Click(object sender, RoutedEventArgs e)
        {
            if(!myFrame.CurrentSource.OriginalString.Contains("MainPage"))
            {
                myFrame.Navigate(new Uri(@"Pages\MainPage.xaml",UriKind.Relative));
            }
            //else
            //{
            //    MessageBox.Show("当前就是主页");
            //}
        }
    }
}
