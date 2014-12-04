using Cbw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CbwSever
{
    internal class InMemoryCbwContext : ICbwContext
    {
        private List<Channel> channels;
        private ReaderWriterLockSlim channelsLock = new ReaderWriterLockSlim();

        public InMemoryCbwContext()
        {
            var channel0 = new Channel { Id = 0, Title = "Public Channel", Description = "Welcome to CBW" };
            channel0.Captions.Add(new Caption { Text = "show what?" });

            channels = new List<Channel> { channel0 };
        }

        public IQueryable<Channel> GetChannels()
        {
            channelsLock.EnterReadLock();
            try
            {
                return channels.AsQueryable();
            }
            finally
            {
                channelsLock.ExitReadLock();
            }
        }

        public Channel GetChannel(int channelId)
        {
            return getChannelById(channelId);
        }

        public IQueryable<Caption> GetCaptions(int channelId)
        {
            return getChannelById(channelId).Captions.AsQueryable();
        }

        public void AddChannel(Channel channel)
        {
            channelsLock.EnterWriteLock();
            var id = this.channels.Max(c => c.Id);
            channel.Id = id + 1;
            this.channels.Add(channel);
            channelsLock.ExitWriteLock();
        }

        public void AddCaption(int channelId, Caption caption)
        {
            var channel = getChannelById(channelId);
            caption.Time = DateTimeOffset.Now;
            channel.Captions.Add(caption);
        }

        private Channel getChannelById(int channelId)
        {
            channelsLock.EnterReadLock();
            try
            {
                var channel = channels.SingleOrDefault(c => c.Id == channelId);
                if (channel == null)
                {
                    throw new Exception(string.Format("Channel with key {0} not found", channelId));
                }

                return channel;
            }
            finally
            {
                channelsLock.ExitReadLock();
            }
        }
    }
}