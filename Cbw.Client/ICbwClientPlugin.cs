using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cbw.Client
{
    public interface ICbwClientPlugin
    {
        string Name { get;}

        Task<string> GetResponse(string input);
    }
}
