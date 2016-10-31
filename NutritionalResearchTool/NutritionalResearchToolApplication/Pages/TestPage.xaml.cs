using NutritionalResearchBusiness;
using NutritionalResearchBusiness.DAL;
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

namespace NutritionalResearchToolApplication.Pages
{
    /// <summary>
    /// TestPage.xaml 的交互逻辑
    /// </summary>
    public partial class TestPage : Page
    {
        public TestPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            INRDataProcessService myDataService = BusinessStaticInstances.GetSingleDataProcessServiceInstance();
            cb_Foods.ItemsSource = myDataService.GetFoodsList();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            if(cb_Foods.SelectedItem == null)
            {
                MessageBox.Show("先选一种食物!");
                return;
            }
            Foods foods = cb_Foods.SelectedItem as Foods;
            if(foods == null)
            {
                MessageBox.Show("foods转换出错！");
                return;
            }
            List<FoodNutritionsPostDto> datas = new List<FoodNutritionsPostDto>();
            List<TextBox> listTextBox = GetChildObjects<TextBox>(myWrapPanel, "tb_Element");
            foreach (var item in listTextBox)
            {
                FoodNutritionsPostDto newData = new FoodNutritionsPostDto()
                {
                    FoodId = foods.Id,
                    NutritiveElementId = Guid.Parse(item.Tag.ToString()),
                    Value = (string.IsNullOrEmpty(item.Text) ? 0 : double.Parse(item.Text))
                };
                datas.Add(newData);
            }
            try
            {
                INRDataProcessService myDataService = BusinessStaticInstances.GetSingleDataProcessServiceInstance();
                myDataService.CreateFoodNutritionsForTesting(datas);
                List<Foods> tempsource = cb_Foods.ItemsSource as List<Foods>;
                tempsource.Remove(foods);
                cb_Foods.ItemsSource = tempsource;
                cb_Foods.SelectedIndex = -1;
                MessageBox.Show("提交成功!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public List<T> GetChildObjects<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && (((T)child).Name.Contains(name) | string.IsNullOrEmpty(name)))
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects<T>(child, name));
            }
            return childList;
        }
    }
}
