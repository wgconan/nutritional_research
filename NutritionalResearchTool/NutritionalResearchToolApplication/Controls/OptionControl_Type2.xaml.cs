using NutritionalResearchBusiness;
using NutritionalResearchBusiness.Dtos;
using NutritionalResearchBusiness.Enums;
using NutritionalResearchBusiness.Extensions;
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

namespace NutritionalResearchToolApplication.Controls
{
    /// <summary>
    /// OptionControl_Type2.xaml 的交互逻辑
    /// </summary>
    public partial class OptionControl_Type2 : UserControl
    {
        QuestionViewDto _questionObj = null;
        AnswerType currentAnswerType = AnswerType.Other;
        int currentChoice = 1;

        public event EventHandler<EventArgs> FinishedInputEvent;

        public OptionControl_Type2()
        {
            InitializeComponent();
        }
        public OptionControl_Type2(QuestionViewDto questionObj)
        {
            InitializeComponent();
            _questionObj = questionObj;
        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            if (rb_Option_Yes.IsChecked == true && string.IsNullOrEmpty(tb_OptionN.Text))
            {
                MessageBox.Show("请完成必要数据项填写");
                return;
            }
            if (rb_Option_Yes.IsChecked == true && !tb_OptionN.Text.IsNumeric())
            {
                MessageBox.Show("输入数据需是整数");
                return;
            }
            SubmitAnswerToDB();
            if (FinishedInputEvent != null)
            {
                FinishedInputEvent(_questionObj.Id, new EventArgs());
            }
            else
            {
                MessageBox.Show("出现数据异常，程序返回首页");
                ReturnMainPage();
            }
        }

        private void OptionType2_Loaded(object sender, RoutedEventArgs e)
        {
            #region InitControls
            rb_Option_Yes.Checked += Rb_Option_Yes_Checked; ;
            rb_Option_No.Checked += Rb_Option_No_Checked; ;
            #endregion
        }

        private void Rb_Option_No_Checked(object sender, RoutedEventArgs e)
        {
            tb_OptionN.Visibility = Visibility.Hidden;
            textblock_unit.Visibility = Visibility.Hidden;
            currentChoice = 0;
        }

        private void Rb_Option_Yes_Checked(object sender, RoutedEventArgs e)
        {
            tb_OptionN.Visibility = Visibility.Visible;
            textblock_unit.Visibility = Visibility.Visible;
            tb_OptionN.Text = "";
            currentChoice = 1;
        }

        private void ReturnMainPage()
        {
            Frame myframe = App.Current.Properties["MyFrame"] as Frame;
            myframe.Navigate(new Uri(@"Pages\MainPage.xaml", UriKind.Relative));
        }

        private void SubmitAnswerToDB()
        {
            INRMainService myMainService = BusinessStaticInstances.GetSingleMainServiceInstance();
            InvestigationAnswerInputDto answer = new InvestigationAnswerInputDto()
            {
                QuestionId = _questionObj.Id,
                Question_Type = _questionObj.Type,
                QuestionSerialNumber = _questionObj.SerialNumber,
                Answer_Type = currentAnswerType,
                InvestigationRecordId = (Guid)App.Current.Properties["CurrentRecordId"]
            };
            if (currentChoice == 1)
            {
                answer.AnswerValue1 = 1;
                answer.AnswerValue2 = int.Parse(tb_OptionN.Text);
            }
            else
            {
                answer.AnswerValue1 = null;
                answer.AnswerValue2 = null;
            }
            try
            {
                myMainService.AddOrUpdateSomeoneInvestigationAnswer(answer);
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现数据异常：" + ex.Message + "，程序返回首页");
                ReturnMainPage();
            }
        }
    }
}
