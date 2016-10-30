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
    /// ReferenceDiagramDetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ReferenceDiagramDetailWindow : Window
    {
        ImageSource image = null;
        public ReferenceDiagramDetailWindow()
        {
            InitializeComponent();
        }

        public ReferenceDiagramDetailWindow(ImageSource _image)
        {
            InitializeComponent();
            image = _image;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(image != null)
            {
                img_diagram.Source = image;
            }
        }
    }
}
