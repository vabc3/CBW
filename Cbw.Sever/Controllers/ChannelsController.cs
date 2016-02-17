using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Cbw.Sever
{
    public class ChannelsController : ODataController
    {
        private ICbwContext context = CbwContextFactory.CreateCbwContext();

        [EnableQuery]
        public async Task<IHttpActionResult> Get()
        {
            return await Task.FromResult(Ok(context.GetChannels()));
        }

        [EnableQuery]
        public async Task<IHttpActionResult> Get(int key)
        {
            return await Task.FromResult(Ok(context.GetChannel(key)));
        }

        [EnableQuery]
        public IHttpActionResult GetCaptions(int key)
        {
            return Ok(context.GetCaptions(key));
        }

        public async Task<IHttpActionResult> Post(Channel channel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return await Task.Run(() =>
            {
                context.AddChannel(channel);
                return Created(channel);
            });
        }

        public IHttpActionResult PostToCaptionsFromChannel(int key, Caption caption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            context.AddCaption(key, caption);
            return Created(caption);
        }
    }
}