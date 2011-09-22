using System.Windows;
using System.Windows.Input;

namespace HändlerEditor.XAML
{
    /// <summary>
    /// Interaction logic for RenameTabWindow.xaml
    /// </summary>
    public partial class RenameTabWindow
    {
        #region Shortcuts

        public static readonly RoutedCommand CloseCommand = new RoutedCommand();

        #endregion

        public bool IsNameSelected { get; private set; }
        public string NewName { get; private set; }

        public RenameTabWindow()
        {
            InitializeComponent();
            InitializeShortcuts();
            tbNewTabName.Focus();
        }

        private static void InitializeShortcuts()
        {
            CloseCommand.InputGestures.Add(new KeyGesture(Key.Escape, ModifierKeys.None));
        }

        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            IsNameSelected = true;
            NewName = tbNewTabName.Text;
            Close();
        }

        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            IsNameSelected = false;
            Close();
        }

        private void CloseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            IsNameSelected = false;
            Close();
        }
    }
}
