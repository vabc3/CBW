using Cbw.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cbw.Input.Con
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new CbwClient();
            string text = Console.ReadLine();
            while (!string.IsNullOrEmpty(text))
            {
                ctx.PushTest(text);
                text = Console.ReadLine();
            }
        }
    }
}
