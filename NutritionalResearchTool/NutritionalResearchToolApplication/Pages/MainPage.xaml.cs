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

namespace NutritionalResearchToolApplication.Pages
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        
        public MainPage()
        {
            InitializeComponent();
        }

        private void buttonBegin_Click(object sender, RoutedEventArgs e)
        {
            INRMainService myMainService = BusinessStaticInstances.GetSingleMainServiceInstance();
            try
            {
                NewInvestigationRecord newRecord = new NewInvestigationRecord()
                {
                    BeforeWeight = double.Parse(textBoxBeforeWeight.Text),
                    Birthday = DateTime.Parse(textBoxBirthday.Text),
                    CurrentWeight = double.Parse(textBoxCurrentWeight.Text),
                    HealthBookId = textBoxHealthbookID.Text,
                    Height = double.Parse(textBoxHeight.Text),
                    InvestigatorName = textBoxInvestigatorName.Text,
                    Name = textBoxName.Text,
                    QueneId = textBoxQueueID.Text,
                    Week = int.Parse(textBoxWeek.Text)
                };
                Guid myID = myMainService.CreateNewInvestigationRecord(newRecord);
                MessageBox.Show(myID.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            
        }

        private void buttonViewRecords_Click(object sender, RoutedEventArgs e)
        {
            INRMainService myMainService = BusinessStaticInstances.GetSingleMainServiceInstance();
            try
            {
                labelRecordsCount.Content = myMainService.GetInvestigationRecordCount().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
