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
using System.Windows.Shapes;
using NutritionalResearchToolApplication.Pages;
using NutritionalResearchBusiness;
using NutritionalResearchBusiness.Dtos;
using System.Threading.Tasks;

namespace NutritionalResearchToolApplication.Windows
{
    /// <summary>
    /// StatisticalReportWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StatisticalReportWindow : Window
    {
        Guid _recordId;
        NutritionalResearchStatisticalReportViewDto report = null;

        public StatisticalReportWindow()
        {
            InitializeComponent();
        }

        public StatisticalReportWindow(Guid recordId)
        {
            InitializeComponent();
            _recordId = recordId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            INRDataProcessService myDataProcessService = BusinessStaticInstances.GetSingleDataProcessServiceInstance();
            Task task = Task.Factory.StartNew(() =>
            {
                try
                {
                    report = myDataProcessService.GetSomeoneRecordStatisticalReport(_recordId);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("查询报告出错：" + ex.Message);
                    this.Close();
                }
            });
            WatingWindow waitwindow = new WatingWindow(task);
            waitwindow.ShowDialog();
            textblock_Age.Text = report.Age.ToString();
            textblock_BeforeBMI.Text = Math.Round(report.BeforeBMI,1).ToString();
            textblock_BeforeWeight.Text = Math.Round(report.BeforeWeight,1).ToString();
            textblock_BookId.Text = report.HealthBookId;
            textblock_CurrentWeight.Text = Math.Round(report.CurrentWeight,1).ToString();
            textblock_Height.Text = Math.Round(report.Height,1).ToString();
            textblock_InvestigationTime.Text = report.InvestigationTime.ToString("yyyy-MM-dd");
            textblock_Name.Text = report.Name;
            textblock_Week.Text = report.Week.ToString();
            StatisticalReportPage page = new StatisticalReportPage();
            page.DataContext = report;
            frame_Report.Navigate(page);  
        }
    }
}
