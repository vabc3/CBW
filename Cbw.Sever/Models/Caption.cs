using System;
using System.ComponentModel.DataAnnotations;

namespace Cbw
{
    public class Caption
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTimeOffset Time { get; set; }

        public CaptionConfig Config { get; set; }
    }
}