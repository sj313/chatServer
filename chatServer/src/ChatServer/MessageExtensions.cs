using System;

namespace ChatServer
{
    public static class MessageExtensions
    {
        public static string asStringMessage(this byte[] bytes)
        {
            return System.Text.Encoding.Default.GetString(bytes);
        }

        public static byte[] asBtyeMessage(this string stringToChange)
        {
            return System.Text.Encoding.Default.GetBytes(stringToChange);
        }
    }
}


