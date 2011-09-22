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

namespace HändlerEditor.XAML
{
    /// <summary>
    /// Interaction logic for RenameTabWindow.xaml
    /// </summary>
    public partial class RenameTabWindow : Window
    {
        public bool IsNameSelected { get; private set; }
        public string NewName { get; private set; }

        public RenameTabWindow()
        {
            InitializeComponent();
            tbNewTabName.Focus();
        }

        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            IsNameSelected = true;
            NewName = tbNewTabName.Text;
            this.Close();
        }

        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            IsNameSelected = false;
            this.Close();
        }
    }
}
