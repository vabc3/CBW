using Cbw.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cbw.Show.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new CbwClient();
            System.Console.WriteLine(ctx.GetLast());
        }
    }
}
