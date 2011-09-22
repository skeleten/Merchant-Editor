using HändlerEditor.XAML;

namespace HändlerEditor.Code
{
    public static class Extentions
    {
        public static bool HasFlagSet(this Message.VisibleButtons value, Message.VisibleButtons flag)
        {
            return (value & flag) == flag;
        }
    }
}
