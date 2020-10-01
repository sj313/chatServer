using System;
using System.Threading.Tasks;

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

        public static byte[] asBtyes(this int numToChange)
        {
            return BitConverter.GetBytes((long)numToChange);
        }

        public static long asLong(this byte[] byteArrayToChange)
        {
            return BitConverter.ToInt64(byteArrayToChange);
        }
        public static void DoNTimesAsync(this Action theThingToDoNumberOfTimesTo, int N)
        {
            for (int i = 0; i < N; i++)
            {
                new Task(theThingToDoNumberOfTimesTo).Start();
            }
        }
    }
}


