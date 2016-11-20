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

using NutritionalResearchBusiness.Dtos;
using NutritionalResearchBusiness;
using NutritionalResearchBusiness.Extensions;
using NutritionalResearchToolApplication.Windows;
using NutritionalResearchBusiness.Enums;
using Microsoft.Win32;
using System.Threading.Tasks;

namespace NutritionalResearchToolApplication.Pages
{
    /// <summary>
    /// RecordListPage.xaml 的交互逻辑
    /// </summary>
    public partial class RecordListPage : Page
    {
        InvestigationRecordQueryConditions condition = new InvestigationRecordQueryConditions();

        public RecordListPage()
        {
            InitializeComponent();
        }

        private void myPager_PagerIndexChanged(object sender, EventArgs e)
        {
            DgUserDataBind();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            condition.HealthBookId = string.Empty;
            condition.Name = string.Empty;
            condition.QueueId = string.Empty;
            condition.QueryStartTime = DateTime.Now.AddYears(-1);
            condition.QueryEndTime = DateTime.Now;
            myPager.PageSize = 5;
            DgUserDataBind();
        }

        private void DgUserDataBind()
        {
            PageQueryInput<InvestigationRecordQueryConditions> conditions = new PageQueryInput<InvestigationRecordQueryConditions>()
            {
                PageSize = myPager.PageSize,
                PageIndex = myPager.PageIndex,
                QueryConditions = condition
            };
            try
            {
                INRMainService myMainService = BusinessStaticInstances.GetSingleMainServiceInstance();
                var result = myMainService.QueryInvestigationRecordListByConditions(conditions);
                myPager.TotalCount = result.TotalCount;
                dg_Record.DataContext = result.QueryResult;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ReturnMainPage();
            }
        }

        private void ReturnMainPage()
        {
            Frame myframe = App.Current.Properties["MyFrame"] as Frame;
            myframe.Navigate(new Uri(@"Pages\MainPage.xaml", UriKind.Relative));
        }

        private void btn_Query_Click(object sender, RoutedEventArgs e)
        {
            QueryConditionsWindow querywindow = new QueryConditionsWindow(condition);
            querywindow.Closed += Querywindow_Closed;
            querywindow.ShowDialog();
        }

        private void Querywindow_Closed(object sender, EventArgs e)
        {
            QueryConditionsWindow querywindow = sender as QueryConditionsWindow;
            if (querywindow != null)
            {
                if (querywindow.IsCancel == false)
                {
                    myPager.PageIndex = 1;
                    DgUserDataBind();
                }
                querywindow.Closed -= Querywindow_Closed;
            }
        }

        private void tb_ContinueAnswer_Loaded(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            InvestigationRecordViewDto record = btn.Tag as InvestigationRecordViewDto;
            switch (record.State)
            {
                case InvestigationRecordStateType.NoFinish:
                    btn.Visibility = Visibility.Visible;
                    break;
                case InvestigationRecordStateType.FinishedAndNoAudit:
                    btn.Visibility = Visibility.Collapsed;
                    break;
                case InvestigationRecordStateType.FinishedAndAudited:
                    btn.Visibility = Visibility.Collapsed;
                    break;
                default:
                    return;
            }
        }

        private void tb_ModifyAnswer_Loaded(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            InvestigationRecordViewDto record = btn.Tag as InvestigationRecordViewDto;
            switch (record.State)
            {
                case InvestigationRecordStateType.NoFinish:
                    btn.Visibility = Visibility.Collapsed;
                    break;
                case InvestigationRecordStateType.FinishedAndNoAudit:
                    btn.Visibility = Visibility.Visible;
                    break;
                case InvestigationRecordStateType.FinishedAndAudited:
                    btn.Visibility = Visibility.Collapsed;
                    break;
                default:
                    return;
            }
        }

        private void tb_ViewReport_Loaded(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            InvestigationRecordViewDto record = btn.Tag as InvestigationRecordViewDto;
            switch (record.State)
            {
                case InvestigationRecordStateType.NoFinish:
                    btn.Visibility = Visibility.Collapsed;
                    break;
                case InvestigationRecordStateType.FinishedAndNoAudit:
                    btn.Visibility = Visibility.Visible;
                    break;
                case InvestigationRecordStateType.FinishedAndAudited:
                    btn.Visibility = Visibility.Visible;
                    break;
                default:
                    return;
            }
        }

        private void tb_ContinueAnswer_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            InvestigationRecordViewDto record = btn.Tag as InvestigationRecordViewDto;
            if (record.State != InvestigationRecordStateType.NoFinish)
            {
                MessageBox.Show("该调查已完成");
                return;
            }
            App.Current.Properties["CurrentRecordId"] = record.Id;
            Frame myframe = App.Current.Properties["MyFrame"] as Frame;
            myframe.Navigate(new Uri(@"Pages\QuestionPage.xaml", UriKind.Relative), (record.LastFinishQuestionSN > 0) ? record.LastFinishQuestionSN : 1);
        }

        private void tb_ModifyAnswer_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            InvestigationRecordViewDto record = btn.Tag as InvestigationRecordViewDto;
            if (record.State != InvestigationRecordStateType.FinishedAndNoAudit)
            {
                MessageBox.Show("该调查已审核,不能修改");
                return;
            }
            App.Current.Properties["CurrentRecordId"] = record.Id;
            Frame myframe = App.Current.Properties["MyFrame"] as Frame;
            myframe.Navigate(new Uri(@"Pages\QuestionPage.xaml", UriKind.Relative), 1);
        }

        private void tb_Audit_Loaded(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            InvestigationRecordViewDto record = btn.Tag as InvestigationRecordViewDto;
            switch (record.State)
            {
                case InvestigationRecordStateType.NoFinish:
                    btn.Visibility = Visibility.Collapsed;
                    break;
                case InvestigationRecordStateType.FinishedAndNoAudit:
                    btn.Visibility = Visibility.Visible;
                    break;
                case InvestigationRecordStateType.FinishedAndAudited:
                    btn.Visibility = Visibility.Collapsed;
                    break;
                default:
                    return;
            }
        }

        private void tb_Audit_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            InvestigationRecordViewDto record = btn.Tag as InvestigationRecordViewDto;
            if (record.State != InvestigationRecordStateType.FinishedAndNoAudit)
            {
                MessageBox.Show("该调查已审核或还未完成");
                return;
            }
            AuditRecordWindow auditWindow = new AuditRecordWindow(record.Id);
            auditWindow.Closed += AuditWindow_Closed;
            auditWindow.ShowDialog();
        }

        private void AuditWindow_Closed(object sender, EventArgs e)
        {
            AuditRecordWindow auditWindow = sender as AuditRecordWindow;
            if (auditWindow != null)
            {
                if (auditWindow.isOk == true)
                {
                    DgUserDataBind();
                }
                auditWindow.Closed -= AuditWindow_Closed;
            }
        }

        private void tb_ViewReport_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            InvestigationRecordViewDto record = btn.Tag as InvestigationRecordViewDto;
            if (record.State == InvestigationRecordStateType.NoFinish)
            {
                MessageBox.Show("该调查还未完成，没有任何报告");
                return;
            }
            StatisticalReportWindow window = new StatisticalReportWindow(record.Id);
            window.Show();
        }

        private void btn_Export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel文件|*.xlsx";
            if (sfd.ShowDialog() == true)
            {
                INRDataProcessService myDataProcessService = BusinessStaticInstances.GetSingleDataProcessServiceInstance();
                bool exportResult = false;
                Exception exportExcepiton = null;
                int exportCount = 0;
                Task task = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        exportCount = myDataProcessService.ExportNutritionalInvestigationRecords2Excel(condition, sfd.FileName);
                        //exportCount = myDataProcessService.ExportNutritionalResearchReport2Excel(condition, sfd.FileName);
                        exportResult = true;
                    }
                    catch (Exception ex)
                    {
                        exportResult = false;
                        exportExcepiton = ex;
                    }
                });
                WatingWindow waitwindow = new WatingWindow(task);
                waitwindow.ShowDialog();
                if (exportResult)
                {
                    MessageBox.Show("已导出" + exportCount.ToString() + "条已审核的记录");
                }
                else
                {
                    MessageBox.Show("导出失败:" + exportExcepiton.Message);
                }
            }
        }

        private void tb_ExportReport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel文件|*.xlsx";
            Button btn = e.Source as Button;
            InvestigationRecordViewDto record = btn.Tag as InvestigationRecordViewDto;
            if (sfd.ShowDialog() == true)
            {
                INRDataProcessService myDataProcessService = BusinessStaticInstances.GetSingleDataProcessServiceInstance();
                bool exportResult = false;
                Exception exportExcepiton = null;
                Task task = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        myDataProcessService.ExportNutritionalAnalysisReport2Excel(record.Id, sfd.FileName);
                        exportResult = true;
                    }
                    catch (Exception ex)
                    {
                        exportResult = false;
                        exportExcepiton = ex;
                    }
                });
                WatingWindow waitwindow = new WatingWindow(task);
                waitwindow.ShowDialog();
                if (exportResult)
                {
                    MessageBox.Show("已导出" + record.QueueId + "的营养分析报告");
                }
                else
                {
                    MessageBox.Show("导出失败:" + exportExcepiton.Message);
                }
            }
        }

        private void tb_ExportReport_Loaded(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            InvestigationRecordViewDto record = btn.Tag as InvestigationRecordViewDto;
            switch (record.State)
            {
                case InvestigationRecordStateType.NoFinish:
                    btn.Visibility = Visibility.Collapsed;
                    break;
                case InvestigationRecordStateType.FinishedAndNoAudit:
                    btn.Visibility = Visibility.Collapsed;
                    break;
                case InvestigationRecordStateType.FinishedAndAudited:
                    btn.Visibility = Visibility.Visible;
                    break;
                default:
                    return;
            }
        }

        private void btn_Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //SaveFileDialog sfd = new SaveFileDialog();
            ofd.Filter = "Excel文件|*.xlsx";
            if (ofd.ShowDialog() == true)
            {
                INRDataProcessService myDataProcessService = BusinessStaticInstances.GetSingleDataProcessServiceInstance();
                bool importResult = false;
                Exception exportExcepiton = null;
                int importCount = 0;
                int existCount = 0;
                Task task = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        myDataProcessService.ImportIntakeRecordsExcel(ofd.FileName, out importCount, out existCount);
                        importResult = true;
                    }
                    catch (Exception ex)
                    {
                        importResult = false;
                        exportExcepiton = ex;
                    }
                });
                WatingWindow waitwindow = new WatingWindow(task);
                waitwindow.ShowDialog();
                if (importResult)
                {
                    MessageBox.Show("已导入" + importCount.ToString() + "条记录，有" + existCount + "条记录已存在，没有导入。");
                }
                else
                {
                    MessageBox.Show("导入记录失败:" + exportExcepiton.Message);
                }
            }
        }

        private void tb_PrintReport_Loaded(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            InvestigationRecordViewDto record = btn.Tag as InvestigationRecordViewDto;
            switch (record.State)
            {
                case InvestigationRecordStateType.NoFinish:
                    btn.Visibility = Visibility.Collapsed;
                    break;
                case InvestigationRecordStateType.FinishedAndNoAudit:
                    btn.Visibility = Visibility.Collapsed;
                    break;
                case InvestigationRecordStateType.FinishedAndAudited:
                    btn.Visibility = Visibility.Visible;
                    break;
                default:
                    return;
            }
        }

        private void tb_PrintReport_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            InvestigationRecordViewDto record = btn.Tag as InvestigationRecordViewDto;
            INRDataProcessService myDataProcessService = BusinessStaticInstances.GetSingleDataProcessServiceInstance();
            bool exportResult = false;
            Exception exportExcepiton = null;
            Task task = Task.Factory.StartNew(() =>
            {
                try
                {
                    myDataProcessService.ExportNutritionalAnalysisReport2Print(record.Id);
                    exportResult = true;
                }
                catch (Exception ex)
                {
                    exportResult = false;
                    exportExcepiton = ex;
                }
            });
            WatingWindow waitwindow = new WatingWindow(task);
            waitwindow.ShowDialog();
            if (!exportResult)
            {
                MessageBox.Show("报告导出失败:" + exportExcepiton.Message);
            }
            else
            {
                MessageBox.Show("请在Excel中打印报告。\r\n请关闭excel后，再打印下一份报告。");
            }
        }
    }
}
