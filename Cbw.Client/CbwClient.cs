using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cbw.Client
{
    public class CbwClient
    {
        private CbwContainer context;
        private int channel;

        public CbwClient(Uri serviceUri, int channel)
        {
            this.context = new CbwContainer(serviceUri)
            {
                MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges
            };

            this.channel = channel;
        }

        public string GetLast()
        {
            return this.context.Channels.ByKey(0).Captions.OrderByDescending(c => c.Time).Take(1).Single().Text;
        }
    }
}
