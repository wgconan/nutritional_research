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
            myframe.Navigate(new Uri(@"Pages\QuestionPage.xaml", UriKind.Relative), (record.LastFinishQuestionSN > 0)? record.LastFinishQuestionSN : 1);
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
    }
}
