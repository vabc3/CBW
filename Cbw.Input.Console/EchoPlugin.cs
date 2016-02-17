using Cbw.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cbw.Input.Con
{
    class EchoPlugin : ICbwClientPlugin
    {
        private const string name = "echo";
        private static readonly string[] pingResponses = new string[] { "私は氏のエコーだ", "I'm echo man" };
        private Random rand = new Random();

        public string Name
        {
            get { return name; }
        }

        public Task<string> GetResponse(string input)
        {
            return Task.FromResult(input == CbwPluggableClient.Ping ? GetPingResponse() : answer(input));
        }

        private string GetPingResponse()
        {
            return pingResponses[rand.Next() % pingResponses.Length];
        }

        private string answer(string text)
        {
            return text;
        }
    }
}
