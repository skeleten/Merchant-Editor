using HändlerEditor.XAML;

namespace HändlerEditor.Code
{
    public static class Extentions
    {
        public static bool HasFlagSet(this Message.VisibleButtons value, Message.VisibleButtons flag)
        {
            return (value & flag) == flag;
        }

        public static Message.VisibleButtons SetFlag(this Message.VisibleButtons value, Message.VisibleButtons flag, bool b)
        {
            if (b)
                value |= flag;
            else if (value.HasFlagSet(flag))
                value ^= flag;

            return value;
        }
    }
}
