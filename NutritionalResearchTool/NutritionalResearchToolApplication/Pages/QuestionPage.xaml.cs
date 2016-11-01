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
using NutritionalResearchToolApplication.Controls;
using NutritionalResearchBusiness.Enums;
using System.Threading.Tasks;
using NutritionalResearchToolApplication.Windows;
using System.Threading;

namespace NutritionalResearchToolApplication.Pages
{
    /// <summary>
    /// QuestionPage.xaml 的交互逻辑
    /// </summary>
    public partial class QuestionPage : Page
    {
        int serialNumber = 1;
        Guid recordId = Guid.Empty;
        QuestionViewDto questionObj = null;

        public QuestionPage()
        {
            Frame myframe = App.Current.Properties["MyFrame"] as Frame;
            myframe.Navigated += Myframe_Navigated;
            InitializeComponent();
        }

        private void Myframe_Navigated(object sender, NavigationEventArgs e)
        {
            if(e.ExtraData != null)
            {
                serialNumber = (int)e.ExtraData;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            recordId = (Guid)App.Current.Properties["CurrentRecordId"];
            //serialNumber = 1;
            textblock_FirstCategory.Text = recordId.ToString();
            LoadQuestionInfo(serialNumber);
        }

        private void LoadQuestionInfo(int serialNumber)
        {
            try
            {
                INRMainService myMainService = BusinessStaticInstances.GetSingleMainServiceInstance();
                questionObj = myMainService.GetQuestionViewBySerialNumber(serialNumber,recordId);
                pb_QuestionProcess.Value = questionObj.CurrentProgress * 100;
                textblock_FirstCategory.Text = questionObj.FirstCategoryName;
                textblock_SecondCategory.Text = questionObj.SecondCategoryName;
                textblock_QuestionDescription.Text = questionObj.Description;
                switch (questionObj.Type)
                {
                    case QuestionType.Standard:
                        textblock_FirstCategory.Foreground = new SolidColorBrush(Color.FromRgb(112,237,145));
                        textblock_SecondCategory.Foreground = new SolidColorBrush(Color.FromRgb(112, 237, 145));
                        textblock_QuestionDescription.Foreground = new SolidColorBrush(Color.FromRgb(112, 237, 145));
                        pb_QuestionProcess.Foreground = new SolidColorBrush(Color.FromRgb(112, 237, 145));
                        var control1 = new OptionControl_Type1(questionObj);
                        control1.FinishedInputEvent += Control_FinishedInputEvent;
                        grid_Options.Children.Add(control1);
                        break;
                    case QuestionType.Optional:
                        textblock_FirstCategory.Foreground = new SolidColorBrush(Color.FromRgb(249, 81, 114));
                        textblock_SecondCategory.Foreground = new SolidColorBrush(Color.FromRgb(249, 81, 114));
                        textblock_QuestionDescription.Foreground = new SolidColorBrush(Color.FromRgb(249, 81, 114));
                        pb_QuestionProcess.Foreground = new SolidColorBrush(Color.FromRgb(249, 81, 114));
                        var control2 = new OptionControl_Type2(questionObj);
                        control2.FinishedInputEvent += Control_FinishedInputEvent;
                        grid_Options.Children.Add(control2);
                        break;
                    case QuestionType.Choice:
                        textblock_FirstCategory.Foreground = new SolidColorBrush(Color.FromRgb(249, 81, 114));
                        textblock_SecondCategory.Foreground = new SolidColorBrush(Color.FromRgb(249, 81, 114));
                        textblock_QuestionDescription.Foreground = new SolidColorBrush(Color.FromRgb(249, 81, 114));
                        pb_QuestionProcess.Foreground = new SolidColorBrush(Color.FromRgb(249, 81, 114));
                        var control3 = new OptionControl_Type3(questionObj);
                        control3.FinishedInputEvent += Control_FinishedInputEvent;
                        grid_Options.Children.Add(control3);
                        break;
                    default:
                        ReturnMainPage();
                        return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载问题失败：" + ex.Message);
                ReturnMainPage();
            }
        }

        private void Control_FinishedInputEvent(object sender, EventArgs e)
        {
            Guid questionId = (Guid)sender;
            if(questionId == questionObj.Id)
            {
                switch (questionObj.Type)
                {
                    case QuestionType.Standard:
                        OptionControl_Type1 control1 = grid_Options.Children[0] as OptionControl_Type1;
                        control1.FinishedInputEvent -= Control_FinishedInputEvent;
                        break;
                    case QuestionType.Optional:
                        OptionControl_Type2 control2 = grid_Options.Children[0] as OptionControl_Type2;
                        control2.FinishedInputEvent -= Control_FinishedInputEvent;
                        break;
                    case QuestionType.Choice:
                        OptionControl_Type3 control3 = grid_Options.Children[0] as OptionControl_Type3;
                        control3.FinishedInputEvent -= Control_FinishedInputEvent;
                        break;
                    default:
                        ReturnMainPage();
                        return;
                }
                grid_Options.Children.Clear();
                if (questionObj.CurrentProgress < 1)
                {
                    serialNumber++;
                    LoadQuestionInfo(serialNumber);
                }
                else
                {
                    var task = Task.Factory.StartNew(() =>
                    {
                        INRMainService myMainService = BusinessStaticInstances.GetSingleMainServiceInstance();
                        try
                        {
                            myMainService.FinishSomeoneInvestigationRecord(recordId);
                        }
                        catch (Exception ex)
                        {
                            this.Dispatcher.BeginInvoke(new Action(() => { MessageBox.Show("完成调查发送错误：" + ex.Message); }));
                        }
                    });
                    WatingWindow waitingWindow = new WatingWindow(task);
                    waitingWindow.ShowDialog();
                    Frame myframe = App.Current.Properties["MyFrame"] as Frame;
                    myframe.Navigate(new Uri(@"Pages\RecordListPage.xaml", UriKind.Relative));
                }
            }
            else
            {
                MessageBox.Show("发生未知数据错误，程序返回首页");
                ReturnMainPage();
            }
        }

        private void ReturnMainPage()
        {
            Frame myframe = App.Current.Properties["MyFrame"] as Frame;
            myframe.Navigate(new Uri(@"Pages\MainPage.xaml", UriKind.Relative));
        }
    }
}
