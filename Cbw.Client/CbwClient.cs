using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cbw.Client
{
    public class CbwClient
    {
        private CbwContainer context;
        private int channel;
        private Queue<Caption> captionQueue;
        private int maxId;

        public CbwClient() :
            this(new Uri("http://localhost:3338/cbw"), 0)
        {
        }

        public CbwClient(Uri serviceUri, int channel = 0)
        {
            this.context = new CbwContainer(serviceUri)
            {
                MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges
            };

            this.channel = channel;
            this.maxId = -1;

            this.captionQueue = new Queue<Caption>();
            this.UpdataInterval = 3000;
        }

        public event Action<Caption> OnCaptionArrive;

        public int UpdataInterval { get; set; }

        public void StartPush()
        {
            Task.Run(() => UpdateQueue());
            Task.Run(() => PushWork());
        }

        public string GetLast()
        {
            var query = this.context.Channels.ByKey(0).Captions.OrderByDescending(c => c.Time).Take(1) as DataServiceQuery<Caption>;
            var captions = query.ExecuteAsync();
            captions.Wait();
            var text = captions.Result.Single().Text;

            return text;
        }

        private async void UpdateQueue()
        {
            while (true)
            {
                var query = context
                    .Channels
                    .ByKey(0)
                    .Captions
                    .Where(c => c.Id > maxId && c.Time > DateTime.Now.Subtract(new TimeSpan(0, 0, 0, UpdataInterval * 2)))
                    .OrderBy(c => c.Time)
                    as DataServiceQuery<Caption>;
                var captions = await query.ExecuteAsync();

                if (captions.Any())
                {
                    this.maxId = captions.Last().Id;
                    lock (this.captionQueue)
                    {
                        foreach (var caption in captions)
                        {
                            this.captionQueue.Enqueue(caption);
                        }
                    }
                }
                await Task.Delay(UpdataInterval);
            }
        }

        private async void PushWork()
        {
            while (true)
            {
                if (this.captionQueue.Any())
                {
                    lock (this.captionQueue)
                    {
                        while (this.captionQueue.Any())
                        {
                            var item = this.captionQueue.Dequeue();
                            this.OnCaptionArrive(item);
                        }
                    }
                }

                await Task.Delay(500);
            }
        }

    }
}
