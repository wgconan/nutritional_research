using NutritionalResearchBusiness.Dtos;
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
using NutritionalResearchToolApplication.Windows;
using NutritionalResearchBusiness;
using NutritionalResearchBusiness.Enums;
using NutritionalResearchBusiness.Extensions;

namespace NutritionalResearchToolApplication.Controls
{
    /// <summary>
    /// OptionControl_Type1.xaml 的交互逻辑
    /// </summary>
    public partial class OptionControl_Type1 : UserControl
    {
        QuestionViewDto _questionObj = null;
        AnswerType currentAnswerType = AnswerType.Month;
        int currentFrequency = 1;

        public event EventHandler<EventArgs> FinishedInputEvent;

        public OptionControl_Type1()
        {
            InitializeComponent();
        }

        public OptionControl_Type1(QuestionViewDto questionObj)
        {
            InitializeComponent();
            _questionObj = questionObj;
        }

        private void cb_OptionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems[0] as ComboBoxItem;
            switch (int.Parse(item.Tag.ToString()))
            {
                case 1:
                    rb_Option1.Visibility = Visibility.Visible;
                    rb_Option2.Visibility = Visibility.Visible;
                    rb_Option3.Visibility = Visibility.Visible;
                    rb_Option4.Visibility = Visibility.Collapsed;
                    rb_Option5.Visibility = Visibility.Collapsed;
                    rb_Option6.Visibility = Visibility.Collapsed;
                    rb_OptionN.Visibility = Visibility.Visible;
                    tb_OptionN.Visibility = Visibility.Visible;
                    tb_OptionN.Text = string.Empty;
                    break;
                case 2:
                    rb_Option1.Visibility = Visibility.Visible;
                    rb_Option2.Visibility = Visibility.Visible;
                    rb_Option3.Visibility = Visibility.Visible;
                    rb_Option4.Visibility = Visibility.Visible;
                    rb_Option5.Visibility = Visibility.Visible;
                    rb_Option6.Visibility = Visibility.Visible;
                    rb_OptionN.Visibility = Visibility.Visible;
                    tb_OptionN.Visibility = Visibility.Visible;
                    tb_OptionN.Text = string.Empty;
                    break;
                case 3:
                    rb_Option1.Visibility = Visibility.Visible;
                    rb_Option2.Visibility = Visibility.Visible;
                    rb_Option3.Visibility = Visibility.Visible;
                    rb_Option4.Visibility = Visibility.Visible;
                    rb_Option5.Visibility = Visibility.Visible;
                    rb_Option6.Visibility = Visibility.Collapsed;
                    rb_OptionN.Visibility = Visibility.Visible;
                    tb_OptionN.Visibility = Visibility.Visible;
                    tb_OptionN.Text = string.Empty;
                    break;
                default:
                    return;
            }
        }

        private void rb_Option_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb_Option = e.Source as RadioButton;
            int.TryParse(rb_Option.Content.ToString(), out currentFrequency);
            ComboBoxItem item = cb_OptionType.SelectedItem as ComboBoxItem;
            string _tempStr = "";
            switch (int.Parse(item.Tag.ToString()))
            {
                case 1:
                    currentAnswerType = AnswerType.Month;
                    _tempStr = "近4周" + rb_Option.Content.ToString() + "次";
                    break;
                case 2:
                    currentAnswerType = AnswerType.Week;
                    _tempStr = "每周" + rb_Option.Content.ToString() + "次";
                    break;
                case 3:
                    currentAnswerType = AnswerType.Day;
                    _tempStr = "每天" + rb_Option.Content.ToString() + "次";
                    break;
                default:
                    return;
            }
            textblock_Frequency.Text = _tempStr;
        }

        private void OptionType1_Loaded(object sender, RoutedEventArgs e)
        {
            #region InitControls
            rb_Option1.Visibility = Visibility.Visible;
            rb_Option2.Visibility = Visibility.Visible;
            rb_Option3.Visibility = Visibility.Visible;
            rb_Option4.Visibility = Visibility.Collapsed;
            rb_Option5.Visibility = Visibility.Collapsed;
            rb_Option6.Visibility = Visibility.Collapsed;
            rb_OptionN.Visibility = Visibility.Visible;
            tb_OptionN.Visibility = Visibility.Visible;
            tb_OptionN.Text = string.Empty;
            rb_Option1.Checked += rb_Option_Checked;
            rb_Option2.Checked += rb_Option_Checked;
            rb_Option3.Checked += rb_Option_Checked;
            rb_Option4.Checked += rb_Option_Checked;
            rb_Option5.Checked += rb_Option_Checked;
            rb_Option6.Checked += rb_Option_Checked;
            rb_OptionN.Checked += rb_Option_Checked;
            tb_OptionN.TextChanged += tb_OptionN_TextChanged;
            cb_OptionType.SelectionChanged += cb_OptionType_SelectionChanged;
            cb_OptionType.SelectedIndex = 0;
            rb_Option1.IsChecked = true;
            #endregion


            if (_questionObj.ReferenceDiagramList.Count > 0)
            {
                textblock_diagram.Visibility = Visibility.Visible;
                grid_diagram.Visibility = Visibility.Visible;
                img_Diagram1.Visibility = Visibility.Visible;
                textblock_Diagram1.Visibility = Visibility.Visible;
                img_Diagram1.Source = new BitmapImage(new Uri(_questionObj.ReferenceDiagramList[0].IconPath, UriKind.Relative));
                img_Diagram1.Tag = _questionObj.ReferenceDiagramList[0].Value;
                textblock_Diagram1.Text = _questionObj.ReferenceDiagramList[0].Value.ToString() + _questionObj.FoodUnit;
            }
            else
            {
                img_Diagram1.Visibility = Visibility.Collapsed;
                grid_diagram.Visibility = Visibility.Collapsed;
                textblock_Diagram1.Visibility = Visibility.Hidden;
                textblock_diagram.Visibility = Visibility.Hidden;
            }
            if (_questionObj.ReferenceDiagramList.Count > 1)
            {
                img_Diagram2.Visibility = Visibility.Visible;
                textblock_Diagram2.Visibility = Visibility.Visible;
                img_Diagram2.Source = new BitmapImage(new Uri(_questionObj.ReferenceDiagramList[1].IconPath, UriKind.Relative));
                img_Diagram2.Tag = _questionObj.ReferenceDiagramList[1].Value;
                textblock_Diagram2.Text = _questionObj.ReferenceDiagramList[1].Value.ToString() + _questionObj.FoodUnit;
            }
            else
            {
                img_Diagram2.Visibility = Visibility.Hidden;
                textblock_Diagram2.Visibility = Visibility.Hidden;
            }
            if (_questionObj.ReferenceDiagramList.Count > 2)
            {
                img_Diagram3.Visibility = Visibility.Visible;
                textblock_Diagram3.Visibility = Visibility.Visible;
                img_Diagram3.Source = new BitmapImage(new Uri(_questionObj.ReferenceDiagramList[2].IconPath, UriKind.Relative));
                img_Diagram3.Tag = _questionObj.ReferenceDiagramList[2].Value;
                textblock_Diagram3.Text = _questionObj.ReferenceDiagramList[2].Value.ToString() + _questionObj.FoodUnit;
            }
            else
            {
                img_Diagram3.Visibility = Visibility.Hidden;
                textblock_Diagram3.Visibility = Visibility.Hidden;
            }

            #region 如果为已答题，则加载当前答案
            if (_questionObj.CurrentAnswer != null)
            {
                if(_questionObj.CurrentAnswer.AnswerValue1.HasValue)
                {
                    switch (_questionObj.CurrentAnswer.Answer_Type)
                    {
                        case AnswerType.Month:
                            cb_OptionType.SelectedIndex = 0;
                            switch (_questionObj.CurrentAnswer.AnswerValue1.Value)
                            {
                                case 1:
                                    rb_Option1.IsChecked = true;
                                    break;
                                case 2:
                                    rb_Option2.IsChecked = true;
                                    break;
                                case 3:
                                    rb_Option3.IsChecked = true;
                                    break;
                                default:
                                    rb_OptionN.IsChecked = true;
                                    tb_OptionN.Text = _questionObj.CurrentAnswer.AnswerValue1.Value.ToString();
                                    break;
                            }
                            break;
                        case AnswerType.Week:
                            cb_OptionType.SelectedIndex = 1;
                            switch (_questionObj.CurrentAnswer.AnswerValue1.Value)
                            {
                                case 1:
                                    rb_Option1.IsChecked = true;
                                    break;
                                case 2:
                                    rb_Option2.IsChecked = true;
                                    break;
                                case 3:
                                    rb_Option3.IsChecked = true;
                                    break;
                                case 4:
                                    rb_Option4.IsChecked = true;
                                    break;
                                case 5:
                                    rb_Option5.IsChecked = true;
                                    break;
                                case 6:
                                    rb_Option6.IsChecked = true;
                                    break;
                                default:
                                    rb_OptionN.IsChecked = true;
                                    tb_OptionN.Text = _questionObj.CurrentAnswer.AnswerValue1.Value.ToString();
                                    break;
                            }
                            break;
                        case AnswerType.Day:
                            cb_OptionType.SelectedIndex = 2;
                            switch (_questionObj.CurrentAnswer.AnswerValue1.Value)
                            {
                                case 1:
                                    rb_Option1.IsChecked = true;
                                    break;
                                case 2:
                                    rb_Option2.IsChecked = true;
                                    break;
                                case 3:
                                    rb_Option3.IsChecked = true;
                                    break;
                                case 4:
                                    rb_Option4.IsChecked = true;
                                    break;
                                case 5:
                                    rb_Option5.IsChecked = true;
                                    break;
                                default:
                                    rb_OptionN.IsChecked = true;
                                    tb_OptionN.Text = _questionObj.CurrentAnswer.AnswerValue1.Value.ToString();
                                    break;
                            }
                            break;
                        case AnswerType.Other:
                        default:
                            break;
                    } 
                }
                tb_Intake.Text = (_questionObj.CurrentAnswer.AnswerValue2.HasValue) ? _questionObj.CurrentAnswer.AnswerValue2.Value.ToString() : string.Empty;
            }
            #endregion
        }

        private void tb_OptionN_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = e.Source as TextBox;
            int.TryParse(tb.Text, out currentFrequency);
            ComboBoxItem item = cb_OptionType.SelectedItem as ComboBoxItem;
            string _tempStr = "";
            switch (int.Parse(item.Tag.ToString()))
            {
                case 1:
                    _tempStr = "近4周" + (string.IsNullOrEmpty(tb.Text) ? "n" : tb.Text) + "次";
                    break;
                case 2:
                    _tempStr = "每周" + (string.IsNullOrEmpty(tb.Text) ? "n" : tb.Text) + "次";
                    break;
                case 3:
                    _tempStr = "每天" + (string.IsNullOrEmpty(tb.Text) ? "n" : tb.Text) + "次";
                    break;
                default:
                    return;
            }
            textblock_Frequency.Text = _tempStr;
        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            if(rb_OptionN.IsChecked == true && string.IsNullOrEmpty(tb_OptionN.Text) || string.IsNullOrEmpty(tb_Intake.Text))
            {
                MessageBox.Show("请完成必要数据项填写");
                return;
            }
            if(rb_OptionN.IsChecked == true && !tb_OptionN.Text.IsNumeric())
            {
                MessageBox.Show("输入数据需是整数");
                return;
            }
            if (!tb_Intake.Text.IsNumeric())
            {
                MessageBox.Show("输入数据需是整数");
                return;
            }
            try
            {
                SubmitAnswerToDB(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现数据异常：" + ex.Message +"，程序返回首页");
                ReturnMainPage();
            }
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

        private void ReturnMainPage()
        {
            Frame myframe = App.Current.Properties["MyFrame"] as Frame;
            myframe.Navigate(new Uri(@"Pages\MainPage.xaml", UriKind.Relative));
        }

        private void textblock_NoAnswer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SubmitAnswerToDB(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现数据异常：" + ex.Message + "，程序返回首页");
                ReturnMainPage();
            }
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

        private void img_Diagram_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image _image = e.Source as Image;
            if(_image != null)
            {
                tb_Intake.Text = _image.Tag.ToString();
                ReferenceDiagramDetailWindow window = new ReferenceDiagramDetailWindow(_image.Source);
                window.ShowDialog();
            }
        }

        private void SubmitAnswerToDB(bool isAnswered)
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
            if(isAnswered)
            {
                answer.AnswerValue1 = currentFrequency;
                answer.AnswerValue2 = int.Parse(tb_Intake.Text); 
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
                throw ex;
            }
        }
    }
}
