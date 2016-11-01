using NutritionalResearchBusiness;
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
    /// AuditRecordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AuditRecordWindow : Window
    {
        Guid _recordId = Guid.Empty;
        public bool isOk = false;

        public AuditRecordWindow()
        {
            InitializeComponent();
        }

        public AuditRecordWindow(Guid recordId)
        {
            InitializeComponent();
            _recordId = recordId;
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(tb_Auditor.Text))
            {
                MessageBox.Show("请填写审核人姓名!");
                return;
            }
            INRMainService myMainService = BusinessStaticInstances.GetSingleMainServiceInstance();
            try
            {
                myMainService.AuditSomeoneInvestigationRecord(_recordId,tb_Auditor.Text);
                isOk = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("审核出错：" + ex.Message);
            }
            this.Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            isOk = false;
            this.Close();
        }
    }
}
