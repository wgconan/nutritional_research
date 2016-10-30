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
using System.Windows.Shapes;

namespace NutritionalResearchToolApplication.Windows
{
    /// <summary>
    /// QueryConditionsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class QueryConditionsWindow : Window
    {
        InvestigationRecordQueryConditions _conditions;
        public bool IsCancel { get; set; }
        public QueryConditionsWindow()
        {
            InitializeComponent();
        }

        public QueryConditionsWindow(InvestigationRecordQueryConditions conditions)
        {
            InitializeComponent();
            _conditions = conditions;
            tb_BookId.Text = conditions.HealthBookId;
            tb_Name.Text = conditions.Name;
            tb_QueueId.Text = conditions.QueueId;
            dp_QueryStartTime.SelectedDate = conditions.QueryStartTime;
            dp_QueryEndTime.SelectedDate = conditions.QueryEndTime;
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsCancel = true;
            this.Close();
        }

        private void btn_Query_Click(object sender, RoutedEventArgs e)
        {
            _conditions.QueueId = tb_QueueId.Text;
            _conditions.HealthBookId = tb_BookId.Text;
            _conditions.Name = tb_Name.Text;
            _conditions.QueryStartTime = dp_QueryStartTime.SelectedDate;
            _conditions.QueryEndTime = dp_QueryEndTime.SelectedDate.HasValue? dp_QueryEndTime.SelectedDate.Value.Add(new TimeSpan(23,59,59)) : dp_QueryEndTime.SelectedDate;
            IsCancel = false;
            this.Close();
        }
    }
}
