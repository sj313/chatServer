using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ChatServer
{
    public class UIController
    {
        // public UIController

        public Action<string> Display = null;

        public Func<string> Input = null;

        public void getInput(Action<string> doSomething)
        {
            while (true)
            {
                doSomething(this.Input());
            }
        }
    }

}
