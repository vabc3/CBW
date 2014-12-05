using Cbw;
using System.Collections.Generic;
using System.Linq;

namespace Cbw
{
    internal interface ICbwContext
    {
        IQueryable<Channel> GetChannels();
        Channel GetChannel(int channelId);

        IQueryable<Caption> GetCaptions(int channelId);
        void AddChannel(Channel channel);
        void AddCaption(int channelId, Caption caption);
    }
}