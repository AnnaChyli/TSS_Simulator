using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShamirGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SimViewModel _vm = new SimViewModel();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = _vm;
        }

        private void CreateSharesButton_OnClick(object sender, RoutedEventArgs e)
        {
            DisplaySharesPanel.Visibility = Visibility.Visible;
            _vm.CreateShares();
            EnterSharesPanel.Visibility = Visibility.Visible;
            CreateSharesButton.IsEnabled = false;
            NumOfShares.IsEnabled = false;
            Threshold.IsEnabled = false;
        }

        private void RecoverSecret_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxImage icon = MessageBoxImage.Information;
            try
            {               
                if (_vm.RecoverSecret())
                {
                    RecoverSecretPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show(this, String.Format("Please provide {0} shares to recover your secret", _vm.Threshold), "Information", MessageBoxButton.OK, icon);
                }
            }catch(InvalidOperationException ex)
            {
                MessageBox.Show(this, "Provided shares are invalid and do not allow you to restore your secret.", "Information", MessageBoxButton.OK, icon);
            }
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string input = e.Text;
            Regex regex = new Regex("[^0-9]");
            bool isDigit = regex.IsMatch(input);

            if (isDigit)
            {
                int number = Int32.Parse(input);
                if (number > 9999)
                {
                    isDigit = false;
                }
            }

            e.Handled = isDigit;
        }


        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TryMore_OnClick(object sender, RoutedEventArgs e)
        {
            DisplaySharesPanel.Visibility = Visibility.Collapsed;
            EnterSharesPanel.Visibility = Visibility.Collapsed;
            RecoverSecretPanel.Visibility = Visibility.Collapsed;

            CreateSharesButton.IsEnabled = true;
            NumOfShares.IsEnabled = true;
            Threshold.IsEnabled = true;

            this.InitializeComponent();

            _vm.Clear();
            DataContext = _vm;
        }

        private void CopySecret_Click(object sender, RoutedEventArgs e)
        {
            var data = (((Button)sender).DataContext as ShareName).DisplayName;
            Clipboard.SetText(data);
        }
    }
}