using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace Cbw.Show.WF
{
    class MyLabel : Label
    {
        private readonly static int check = 0xFF / 2;
        private readonly static Color checkc = Color.FromArgb(0xF00F); //Color.FromArgb(0xFFFF + 1);

        public MyLabel(string text, AniConfig config)
        {
            this.Text = text;
            this.AutoSize = true;
            this.Font = config.Font;

            this.ForeColor = ComputeColor();
        }

        private Color ComputeColor()
        {
            if (this.Text.Length >= 5)
            {
                int res = check;
                if (this.Text.Substring(0, 5).Any(ch => (res ^= ch) < 0) || res == 0) return checkc;
            }

            return Color.WhiteSmoke;
        }
    }
}
