using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cbw.Client
{
    public class CbwPluggableClient : CbwClient
    {
        public const string Ping = "ping";

        private Dictionary<string, ICbwClientPlugin> map = new Dictionary<string, ICbwClientPlugin>();

        public CbwPluggableClient(Uri serviceUri)
            : base(serviceUri)
        {
        }

        public async Task RegisterPlugin(ICbwClientPlugin plugin)
        {
            this.map.Add("@" + plugin.Name, plugin);
            await this.Push(new Caption()
            {
                Text = "New friend " + plugin.Name + " comes online.",
                Config = new CaptionConfig { Display = DisplayMode.Fade }
            });
        }

        protected override async Task FireEvent(IEnumerable<Caption> captions)
        {
            await base.FireEvent(captions);

            string content;
            foreach (var p in map)
            {
                foreach (var caption in captions)
                {
                    content = null;
                    if (caption.Text.StartsWith(p.Key))
                    {
                        if (caption.Text.Length == p.Key.Length)
                        {
                            content = Ping;
                        }
                        else
                        {
                            content = caption.Text.Substring(p.Key.Length + 1);
                        }
                    }
                    else if (caption.Text == "@")
                    {
                        content = Ping;
                    }

                    if (content == null) continue;


                    var text = await p.Value.GetResponse(content);
                    await this.Push(new Caption { Text = text });
                }
            }
        }
    }
}
