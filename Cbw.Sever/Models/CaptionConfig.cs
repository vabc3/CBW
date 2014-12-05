using System.ComponentModel.DataAnnotations;
namespace Cbw
{
    public enum DisplayMode
    {
        Roll,
        Fade
    }

    public class CaptionConfig
    {
        public DisplayMode? Display { get; set; }
    }
}