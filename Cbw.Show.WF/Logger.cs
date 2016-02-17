using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cbw.Show.WF
{
    public static class Logger
    {
        public static void Log(string content, params object[] para)
        {
            Console.WriteLine(content, para);
        }
    }
}
