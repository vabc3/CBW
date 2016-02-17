using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cbw.Client
{
    public class CbwClient
    {
        private const int BasicSpeedStack = 2;
        private const int MaxSpeed = 5; // 16x;
        private const int MinSpeed = 1; // 1/2x;

        private const int UpdataInterval = 3000;
        private static readonly DateTimeOffset basicOffset = DateTimeOffset.MinValue.AddYears(1);

        private int speed = 1;
        private bool isLive = true;
        private bool direction = true;

        private int maxId = -1;
        private CbwContainer context;
        private DateTimeOffset stageTime;
        private DateTimeOffset stageTimeReal;
        private DateTimeOffset previous;
        private DataServiceQuery<Caption> captionQuery;
        private object lockObj = new object();

        #region Constructor
        public CbwClient(Uri serviceUri, int channel = 0)
        {
            this.context = new CbwContainer(serviceUri)
            {
                MergeOption = MergeOption.OverwriteChanges
            };

            var channelQuery = this.context.Channels.ByKey(channel);
            this.Channel = Task.Factory.FromAsync<Channel>(
                channelQuery.BeginGetValue(a => { }, null),
                channelQuery.EndGetValue).Result;

            this.captionQuery = context.Channels.ByKey(channel).Captions;
            this.previous = basicOffset;
            this.stageTime = this.stageTimeReal = DateTimeOffset.Now;
        }
        #endregion

        public event Action<Caption> OnCaptionArrive;

        #region Speed Control
        public event Action<int> OnSpeedChange;

        // 2 based, slowest is 1/2
        public int Speed { get { return 1 << this.speed; } }

        public void SpeedShiftUp()
        {
            if (!this.IsLive) this.SpeedChange(this.speed + 1);
        }

        public void SpeedShiftDown()
        {
            if (!this.IsLive) this.SpeedChange(this.speed - 1);
        }

        public void SpeedReset()
        {
            this.SpeedChange(1);
        }

        private void SpeedChange(int newSpeed)
        {
            if (this.speed == newSpeed || newSpeed < MinSpeed || newSpeed > MaxSpeed) return;

            lock (lockObj)
            {
                UpdateDT();
                this.speed = newSpeed;
            }

            if (this.OnSpeedChange != null)
            {
                this.OnSpeedChange(this.Speed);
            }
        }
        #endregion

        public Channel Channel { get; private set; }

        public bool Direction
        {
            get { return this.direction; }
            set
            {
                if (this.direction != value)
                {
                    lock (lockObj)
                    {
                        this.UpdateDT();
                        this.direction = value;
                    }

                    this.previous = basicOffset;
                    this.IsLive = false;
                }
            }
        }

        public bool IsLive
        {
            get { return isLive; }
            set
            {
                if (this.isLive != value)
                {
                    this.isLive = value;

                    this.previous = basicOffset;
                    if (this.IsLive)
                    {
                        this.SpeedReset();
                        this.maxId = -1;
                    }
                }
            }
        }

        private void UpdateDT()
        {

            DateTimeOffset actualNow;
            var now = DateTimeOffset.Now;

            if (!this.IsLive)
            {
                var delta = now.Subtract(this.stageTimeReal);
                actualNow = this.stageTime + new TimeSpan(delta.Ticks * this.Speed / BasicSpeedStack * (this.Direction ? 1 : -1));
                if (actualNow > now) // catch up with real time.
                {
                    this.IsLive = true;

                    actualNow = now;
                }

                this.stageTime = actualNow;
                this.stageTimeReal = now;
            }
            else
            {
                this.stageTimeReal = this.stageTime = now;
            }

        }

        public DateTimeOffset StageTime
        {
            get
            {
                if (this.IsLive)
                {
                    return DateTimeOffset.Now;
                }

                lock (this.lockObj) { UpdateDT(); }

                return this.stageTime;
            }
            set
            {
                this.stageTime = value;
                this.stageTimeReal = DateTimeOffset.Now;
                this.IsLive = false;
            }
        }

        public void Start()
        {
            Task.Run(() => UpdateQueue());
        }

        public async Task<bool> Push(Caption caption)
        {
            this.context.AddRelatedObject(this.Channel, "Captions", caption);
            return await this.context.SaveChangesAsync().ContinueWith(res => res.Result.First().StatusCode == 201);
        }

        protected virtual async Task FireEvent(IEnumerable<Caption> captions)
        {
            if (this.OnCaptionArrive != null)
            {
                await Task.Run(() => { foreach (var item in captions) this.OnCaptionArrive(item); });
            }
        }

        private async void UpdateQueue()
        {
            while (true)
            {
                lock (lockObj)
                {
                    UpdateDT();
                }

                var stime = this.StageTime;
                DataServiceQuery<Caption> query;
                var prev = (this.previous == basicOffset)
                    ? stime.Subtract(new TimeSpan(0, 0, 0, 0, UpdataInterval * Speed / BasicSpeedStack))
                    : this.previous;

                DateTimeOffset begin, end;
                if (stime > prev)
                {
                    begin = prev; end = stime;
                }
                else
                {
                    begin = stime; end = prev;
                }

                if (this.IsLive)
                {
                    query = this.captionQuery
                         .Where(c =>
                             c.Id > this.maxId
                             && c.Time > begin.Subtract(new TimeSpan(0, 0, 0, 0, UpdataInterval * Speed / BasicSpeedStack / 2))
                             && c.Time <= end)
                         .OrderBy(c => c.Time)
                         as DataServiceQuery<Caption>;
                }
                else
                {
                    query = this.captionQuery
                         .Where(c =>
                             c.Time > begin
                             && c.Time <= end)
                         .OrderBy(c => c.Time)
                         as DataServiceQuery<Caption>;
                }

                var captions = (await query.ExecuteAsync()).ToList();

                this.previous = stime;

                if (captions.Any())
                {
                    if (IsLive)
                    {
                        this.maxId = captions.Max(c => c.Id);
                    }

                    await this.FireEvent(captions);
                }

                await Task.Delay(UpdataInterval);
            }
        }
    }
}
