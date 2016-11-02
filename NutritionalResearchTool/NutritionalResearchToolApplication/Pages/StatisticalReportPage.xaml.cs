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
using System.Threading.Tasks;
using NutritionalResearchToolApplication.Windows;

namespace NutritionalResearchToolApplication.Pages
{
    /// <summary>
    /// StatisticalReportPage.xaml 的交互逻辑
    /// </summary>
    public partial class StatisticalReportPage : Page
    {
        NutritionalResearchStatisticalReportViewDto report = null;

        public StatisticalReportPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(DataContext == null || DataContext as NutritionalResearchStatisticalReportViewDto == null)
            {
                throw new ArgumentNullException("报告数据为空");
            }
            report = DataContext as NutritionalResearchStatisticalReportViewDto;
            dg_StructureOfMeals.ItemsSource = report.StructureOfMeals.OrderBy(nObj => nObj.StructureCode).ToList();
            dg_NutrtiveElementIntakeStatistics.ItemsSource = report.NutrtiveElementIntakeStatistics.OrderBy(nObj => nObj.NutritiveName).ToList();
            dg_FoodFirstCategory.ItemsSource = report.FillingRecords.GroupBy(nObj => nObj.FirstCategoryCode).OrderBy(nObj => nObj.Key).Select(nObj => new FoodFirstCategorySummary { Code = nObj.Key, Name = nObj.First().FirstCategoryName }).ToList();
            dg_FoodFirstCategory.SelectedIndex = 0;
        }

        private void dg_FoodFirstCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FoodFirstCategorySummary cate = e.AddedItems[0] as FoodFirstCategorySummary;
            if(cate != null)
            {
                dg_FoodAnswerRecord.ItemsSource = report.FillingRecords.Where(nObj => nObj.FirstCategoryCode == cate.Code).OrderBy(nObj => nObj.SecondCategoryCode).ToList();
            }
        }
    }

    public class FoodFirstCategorySummary
    {
        public string Code { get; set; }

        public string Name { get; set; }
    }
}
