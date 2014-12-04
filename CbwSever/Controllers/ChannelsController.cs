using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.OData;
using Cbw;
using System.Threading.Tasks;

namespace CbwSever
{
    [EnableQuery]
    public class ChannelsController : ODataController
    {
        public async Task<IQueryable<Channel>> Get()
        {
           return await Task.Run(() =>
            {
                var a = new Channel() { Id = 0, Title = "5" };
                var b = new Channel() { Id = 1, Title = "6" };

                var ocl = new[] { a, b };

                return ocl.AsQueryable();
            });
            
        }

        public void Post(Channel channel)
        {
            if (ModelState.IsValid)
            {

            }
        }

    }
}