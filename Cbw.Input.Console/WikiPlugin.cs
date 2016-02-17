using Cbw.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cbw.Input.Con
{
    class WikiPlugin : ICbwClientPlugin
    {
        private const string name = "wiki";
        private const string pingResponse = "维基君欢迎@wiki咨询哦";
        private const int maxLen = 120;
        private readonly Regex eng = new Regex("^[A-Za-z]*$");

        public string Name
        {
            get { return name; }
        }

        public async Task<string> GetResponse(string input)
        {
            if (input == CbwPluggableClient.Ping)
            {
                return pingResponse;
            }

            
            string uri = string.Format(
                "http://{0}.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&explaintext=1&titles={1}", 
                eng.IsMatch(input) ? "en" : "zh",
                input);
            WebClient client = new WebClient();
            string result = "";
            try
            {
                using (var stream = await client.OpenReadTaskAsync(uri))
                using (var sr = new StreamReader(stream))
                {
                    JsonSerializer ser = new JsonSerializer();
                    Result data = ser.Deserialize<Result>(new JsonTextReader(sr));
                    result = data.query.pages.Values.First().extract;
                    if (result!=null && result.Length > maxLen)
                    {
                        result = result.Substring(0, maxLen);
                    }
                }
            }
            catch (Exception ex)
            {
                result = "wiki error :( " + ex.Message;
            }

            return string.IsNullOrEmpty(result) ? "I don't know that" : result;
        }
    }

    internal class Result
    {
        public Query query { get; set; }
    }

    internal class Query
    {
        public Dictionary<string, Page> pages { get; set; }
    }

    internal class Page
    {
        public string extract { get; set; }
    }
}
