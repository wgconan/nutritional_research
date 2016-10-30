using NutritionalResearchBusiness;
using NutritionalResearchBusiness.Dtos;
using NutritionalResearchBusiness.Enums;
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
    /// OptionControl_Type3.xaml 的交互逻辑
    /// </summary>
    public partial class OptionControl_Type3 : UserControl
    {
        QuestionViewDto _questionObj = null;
        AnswerType currentAnswerType = AnswerType.Other;
        int currentChoice = 1;

        public event EventHandler<EventArgs> FinishedInputEvent;

        public OptionControl_Type3()
        {
            InitializeComponent();
        }

        public OptionControl_Type3(QuestionViewDto questionObj)
        {
            InitializeComponent();
            _questionObj = questionObj;
        }

        private void btn_Finish_Click(object sender, RoutedEventArgs e)
        {
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

        private void OptionType3_Loaded(object sender, RoutedEventArgs e)
        {
            #region InitControls
            rb_Option_A.Checked += Rb_Option_A_Checked; 
            rb_Option_B.Checked += Rb_Option_B_Checked;
            rb_Option_C.Checked += Rb_Option_C_Checked;
            #endregion
        }

        private void Rb_Option_C_Checked(object sender, RoutedEventArgs e)
        {
            currentChoice = 3;
        }

        private void Rb_Option_B_Checked(object sender, RoutedEventArgs e)
        {
            currentChoice = 2;
        }

        private void Rb_Option_A_Checked(object sender, RoutedEventArgs e)
        {
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
                InvestigationRecordId = (Guid)App.Current.Properties["CurrentRecordId"],
                AnswerValue1 = currentChoice,
                AnswerValue2 = null
            };
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
