using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.OData.Builder;

namespace Cbw
{
    public class Channel
    {
        public Channel()
        {
            this.Captions = new List<Caption>();
        }

        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        [Contained]
        public virtual ICollection<Caption> Captions { get; set; }
    }
}