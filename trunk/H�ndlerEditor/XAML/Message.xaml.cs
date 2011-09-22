using System;
using System.Windows;
using HändlerEditor.Code;

namespace HändlerEditor.XAML
{
    /// <summary>
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class Message
    {
        [Flags]
        public enum VisibleButtons : byte
        {
            None = 0x0,
            Yes = 0x2,
            No = 0x4,
            Ok = 0x8,
            Cancel = 0xF,
            YesNo = Yes | No,
            OkCancel = Ok | Cancel,
        }

        public VisibleButtons Result { get; private set; }

        public Message(string message, VisibleButtons visibleButtons)
        {
            Result = VisibleButtons.None;
            InitializeComponent();
            MessageText.Text = message;
            SetButtonVisibilities(visibleButtons);
        }

        public void SetButtonVisibilities(VisibleButtons visibleButtons)
        {
            if (visibleButtons.HasFlagSet(VisibleButtons.Yes))
                YesButton.Visibility = Visibility.Visible;
            if (visibleButtons.HasFlagSet(VisibleButtons.No))
                NoButton.Visibility = Visibility.Visible;
            if(visibleButtons.HasFlagSet(VisibleButtons.Ok))
                OkButton.Visibility = Visibility.Visible;
            if (visibleButtons.HasFlagSet(VisibleButtons.Cancel))
                CancelButton.Visibility = Visibility.Visible;
        }

        public new VisibleButtons ShowDialog()
        {
            base.ShowDialog();
            return Result;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            Result = VisibleButtons.Ok;
            Close();
        }

        private void YesButtonClick(object sender, RoutedEventArgs e)
        {
            Result = VisibleButtons.Yes;
            Close();
        }

        private void NoButtonClick(object sender, RoutedEventArgs e)
        {
            Result = VisibleButtons.No;
            Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Result = VisibleButtons.Cancel;
            Close();
        }
    }
}
