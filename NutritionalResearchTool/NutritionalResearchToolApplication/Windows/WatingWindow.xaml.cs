using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace NutritionalResearchToolApplication.Windows
{
    /// <summary>
    /// WatingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WatingWindow : Window
    {
        Task _waitTask = null;
        public WatingWindow()
        {
            InitializeComponent();
        }

        public WatingWindow(Task waitTask)
        {
            InitializeComponent();
            _waitTask = waitTask;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pb_wait.IsIndeterminate = true;
            Task.Factory.StartNew(() =>
            {
                _waitTask.Wait(60 * 1000);
                this.Dispatcher.BeginInvoke(new Action(() => { this.Close(); }));
            });
        }
    }
}
