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


            NutritiveElementIdList.Add(new Guid("97a83f2a-716c-44cb-a244-4b051cc28948"));
            NutritiveElementIdList.Add(new Guid("99568846-6f48-455b-9190-ece79e614c38"));
            NutritiveElementIdList.Add(new Guid("42b9a89f-3fa8-43a1-854d-b7c33b4cc7d3"));
            NutritiveElementIdList.Add(new Guid("e5745dfc-59c6-437a-99d7-d9fa5d0cda2b"));
            NutritiveElementIdList.Add(new Guid("77137cfc-454c-4e65-9150-67c3b7bf0eca"));
            NutritiveElementIdList.Add(new Guid("47cd8c7e-64a0-414c-976f-6aa570eaf03c"));
            NutritiveElementIdList.Add(new Guid("77a52568-236a-46fc-b2a0-5679608248b7"));
            NutritiveElementIdList.Add(new Guid("4c305cd2-2798-4305-8351-82eeb9f878c0"));
            NutritiveElementIdList.Add(new Guid("109f1b16-759f-4c55-888a-90c44a828d75"));
            NutritiveElementIdList.Add(new Guid("3f8303b3-5c1f-427e-a7e1-90ba5438ef89"));
            NutritiveElementIdList.Add(new Guid("29d6ed1d-89b8-48b2-a54d-4a4aab664578"));
            NutritiveElementIdList.Add(new Guid("f47dc264-c932-40fe-b2c1-20b9180001e2"));
            NutritiveElementIdList.Add(new Guid("1353190b-26e3-4989-913f-7c99c4237b57"));
            NutritiveElementIdList.Add(new Guid("7b5aa0f9-b2ec-4990-826e-7fcd3bd34fdf"));
            NutritiveElementIdList.Add(new Guid("23a40a5d-5afd-43d1-876b-337970260aeb"));
            NutritiveElementIdList.Add(new Guid("947f346f-ad85-4ecb-8471-d9e8fd0f2493"));
            NutritiveElementIdList.Add(new Guid("c91def7e-edb6-46dc-808b-368b5770a93f"));
            NutritiveElementIdList.Add(new Guid("60747f55-5cf6-43df-a4fb-7c0e4ef8d228"));
            NutritiveElementIdList.Add(new Guid("6d578288-beac-43ff-a72b-dc5cc86316fc"));
            NutritiveElementIdList.Add(new Guid("1e28e117-2afd-48c4-8dae-541089bd107a"));
            NutritiveElementIdList.Add(new Guid("0cc17275-bf63-4196-90df-97fbeaded769"));
            NutritiveElementIdList.Add(new Guid("0d47b309-5267-42b2-ac14-1c9848f0cd37"));
            NutritiveElementIdList.Add(new Guid("8125918e-014c-4db6-aeb5-d9b99a43194d"));
            NutritiveElementIdList.Add(new Guid("74f2bf4d-7058-4743-91f2-f674546e6ebb"));
            NutritiveElementIdList.Add(new Guid("6c3bf476-4c94-43fe-ae66-1c031b6c3861"));
            NutritiveElementIdList.Add(new Guid("7f03ba0f-599e-418a-a6fe-9ead3faa9dcf"));
            NutritiveElementIdList.Add(new Guid("929830cd-c275-4d98-b239-33efc70b2f74"));
            NutritiveElementIdList.Add(new Guid("1437ec32-27a6-4d36-a857-f8c400410ba3"));
            NutritiveElementIdList.Add(new Guid("5fe79ebd-54e3-4e64-ac10-72c8f5fd379e"));
            NutritiveElementIdList.Add(new Guid("ca6b7d89-0e6c-49fc-a5d7-47f97b3ccb3e"));
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
        List<Guid> NutritiveElementIdList = new List<Guid>();

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            if (cb_Foods.SelectedItem == null)
            {
                MessageBox.Show("先选一种食物!");
                return;
            }
            Foods foods = cb_Foods.SelectedItem as Foods;
            if (foods == null)
            {
                MessageBox.Show("foods转换出错！");
                return;
            }

            List<FoodNutritionsPostDto> datas = new List<FoodNutritionsPostDto>();
            string inputElements = txtInputElements.Text;
            List<string> ElementsList = inputElements.Split(',').ToList();
            if(ElementsList.Count()!=31)
            {
                MessageBox.Show("输入错误");
                return;
            }

            for (int i = 0; i < 30; i++)
            {
                FoodNutritionsPostDto newData = new FoodNutritionsPostDto()
                {
                    FoodId = foods.Id,
                    NutritiveElementId = NutritiveElementIdList[i],
                    Value =(string.IsNullOrEmpty(ElementsList[i+1]))?0:double.Parse(ElementsList[i+1])
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
                txtInputElements.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
