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
            string serviceUri = args.Length > 0 ? args[0] : "http://localhost:3338/cbw";

            var ctx = new CbwPluggableClient(new Uri(serviceUri));
            ctx.RegisterPlugin(new EchoPlugin()).Wait();
            // ctx.RegisterPlugin(new WikiPlugin()).Wait();
            ctx.Start();

            Console.WriteLine("Connected to '{0}'", ctx.Channel.Title);
            Console.WriteLine(ctx.Channel.Description);

            string text = Console.ReadLine();
            while (!string.IsNullOrEmpty(text))
            {
                var bol =ctx.Push(new Caption() { Text = text }).Result;
                text = Console.ReadLine();
            }
        }
    }
}
