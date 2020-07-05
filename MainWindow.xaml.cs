using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InternTest.ViewModels;

namespace InternTest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel mainViewModel;
        public MainWindow()
        {

            InitializeComponent();
            mainViewModel = (ViewModel)this.FindResource("mainViewModel");

        }

        private void ColumnBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = (CheckBox)sender;
            mainViewModel?.FilterColumns.Add(checkbox.Name);
        }
    }
}
