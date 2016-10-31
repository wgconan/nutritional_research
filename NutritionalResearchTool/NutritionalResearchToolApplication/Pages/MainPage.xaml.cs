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
            //App.Current.Properties["CurrentRecordId"] = Guid.Parse("f02a6307-9042-4424-b239-d96efd650a57");
            //Frame myframe = App.Current.Properties["MyFrame"] as Frame;
            //myframe.Navigate(new Uri(@"Pages\QuestionPage.xaml", UriKind.Relative));
            if (string.IsNullOrEmpty(textBoxQueueID.Text))
            {
                textBlock_Required1.Visibility = Visibility.Visible;
                return;
            }
            if (string.IsNullOrEmpty(textBoxHealthbookID.Text))
            {
                textBlock_Required2.Visibility = Visibility.Visible;
                return;
            }
            if (string.IsNullOrEmpty(textBoxInvestigatorName.Text))
            {
                textBlock_Required3.Visibility = Visibility.Visible;
                return;
            }
            if (string.IsNullOrEmpty(textBoxName.Text))
            {
                textBlock_Required4.Visibility = Visibility.Visible;
                return;
            }
            if (string.IsNullOrEmpty(textBoxBirthday.Text))
            {
                textBlock_Required5.Visibility = Visibility.Visible;
                return;
            }
            if (string.IsNullOrEmpty(textBoxWeek.Text))
            {
                textBlock_Required6.Visibility = Visibility.Visible;
                return;
            }
            if (string.IsNullOrEmpty(textBoxHeight.Text))
            {
                textBlock_Required7.Visibility = Visibility.Visible;
                return;
            }
            if (string.IsNullOrEmpty(textBoxBeforeWeight.Text))
            {
                textBlock_Required8.Visibility = Visibility.Visible;
                return;
            }
            if (string.IsNullOrEmpty(textBoxCurrentWeight.Text))
            {
                textBlock_Required9.Visibility = Visibility.Visible;
                return;
            }
            if (!textBoxQueueID.Text.IsNumeric())
            {
                MessageBox.Show("队列编码必须为数字");
                return;
            }
            if (!textBoxHealthbookID.Text.IsNumeric())
            {
                MessageBox.Show("围产保健手册编码必须为数字");
                return;
            }
            if (!textBoxBirthday.Text.IsDateTime())
            {
                MessageBox.Show("生日应为日期格式");
                return;
            }
            if (!textBoxWeek.Text.IsNumeric())
            {
                MessageBox.Show("孕周必须为数字");
                return;
            }
            if (!textBoxHeight.Text.IsFloat())
            {
                MessageBox.Show("身高必须为数字");
                return;
            }
            if (!textBoxBeforeWeight.Text.IsFloat())
            {
                MessageBox.Show("体重必须为数字");
                return;
            }
            if (!textBoxCurrentWeight.Text.IsFloat())
            {
                MessageBox.Show("体重必须为数字");
                return;
            }
            INRMainService myMainService = BusinessStaticInstances.GetSingleMainServiceInstance();
            try
            {
                NewInvestigationRecordDto newRecord = new NewInvestigationRecordDto()
                {
                    BeforeWeight = double.Parse(textBoxBeforeWeight.Text),
                    Birthday = DateTime.Parse(textBoxBirthday.Text),
                    CurrentWeight = double.Parse(textBoxCurrentWeight.Text),
                    HealthBookId = textBoxHealthbookID.Text,
                    Height = double.Parse(textBoxHeight.Text),
                    InvestigatorName = textBoxInvestigatorName.Text,
                    Name = textBoxName.Text,
                    QueueId = textBoxQueueID.Text,
                    Week = int.Parse(textBoxWeek.Text)
                };
                Guid myID = myMainService.CreateNewInvestigationRecord(newRecord);
                App.Current.Properties["CurrentRecordId"] = myID;
                Frame myframe = App.Current.Properties["MyFrame"] as Frame;
                myframe.Navigate(new Uri(@"Pages\QuestionPage.xaml", UriKind.Relative),1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonViewRecords_Click(object sender, RoutedEventArgs e)
        {
            Frame myframe = App.Current.Properties["MyFrame"] as Frame;
            myframe.Navigate(new Uri(@"Pages\RecordListPage.xaml", UriKind.Relative));
        }

        private void labelRecordsCount_Loaded(object sender, RoutedEventArgs e)
        {
            INRMainService myMainService = BusinessStaticInstances.GetSingleMainServiceInstance();
            try
            {
                labelRecordsCount.Content = myMainService.GetFinishedInvestigationRecordCount().ToString() + " 份";
            }
            catch (Exception)
            {
                labelRecordsCount.Content = "0 份";
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = e.Source as TextBox;
            if (tb != null)
            {
                switch (tb.Name)
                {
                    case "textBoxQueueID":
                        textBlock_Required1.Visibility = Visibility.Hidden;
                        break;
                    case "textBoxHealthbookID":
                        textBlock_Required2.Visibility = Visibility.Hidden;
                        break;
                    case "textBoxInvestigatorName":
                        textBlock_Required3.Visibility = Visibility.Hidden;
                        break;
                    case "textBoxName":
                        textBlock_Required4.Visibility = Visibility.Hidden;
                        break;
                    case "textBoxBirthday":
                        textBlock_Required5.Visibility = Visibility.Hidden;
                        break;
                    case "textBoxWeek":
                        textBlock_Required6.Visibility = Visibility.Hidden;
                        break;
                    case "textBoxHeight":
                        textBlock_Required7.Visibility = Visibility.Hidden;
                        break;
                    case "textBoxBeforeWeight":
                        textBlock_Required8.Visibility = Visibility.Hidden;
                        break;
                    case "textBoxCurrentWeight":
                        textBlock_Required9.Visibility = Visibility.Hidden;
                        break;
                    default:
                        return;
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.Current.Properties["CurrentRecordId"] = Guid.Empty;
        }
    }
}
