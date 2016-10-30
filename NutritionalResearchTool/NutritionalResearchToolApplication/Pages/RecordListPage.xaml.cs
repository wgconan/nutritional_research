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
            if(querywindow != null)
            {
                if(querywindow.IsCancel == false)
                {
                    myPager.PageIndex = 1;
                    DgUserDataBind();
                }
                querywindow.Closed -= Querywindow_Closed;
            }
        }
    }
}
