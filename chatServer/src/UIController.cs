using System;

namespace ChatServer
{
    public abstract class UIController
    {
        // public UIController

        public static Action<string> Display = null;

        public static Func<string> Input = null;

        public static void getInput(Action<string> doSomething)
        {
            while (true)
            {
                doSomething(Input());
            }
        }
    }

}
