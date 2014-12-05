using Cbw.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cbw.Show.Con
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new CbwClient(new Uri("http://localhost.fiddler:3338/cbw"));

            ctx.OnCaptionArrive += ctx_OnCaptionArrive;

            ctx.StartPush();
            Console.ReadLine();

        }

        static void ctx_OnCaptionArrive(Caption obj)
        {
            Console.WriteLine(obj.Text);
        }
    }
}
