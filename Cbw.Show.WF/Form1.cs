using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CbwShow.WF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.timer1.Interval = 500;
            this.timer1.Tick += timer1_Tick;
            this.timer1.Start();

            t2 = new Timer();
            t2.Interval = 15;
            t2.Tick += t2_Tick;


        }

        void timer1_Tick(object sender, System.EventArgs e)
        {
            timer1.Stop();
            string str = "null";
            try
            {
                //ctx = new CbwContainer(new Uri("http://localhost.fiddler:3338/cbw/"));
                //var query = ctx.Channels
                //    .ByKey(new Dictionary<string, object>() { { "Id", 0 } })
                //    .Captions
                //    .OrderByDescending(c => c.Time)
                //    .Take(1)
                //    .Single();
                //str = query.Text;
            }
            catch (Exception ex)
            {
                str = "Err!!" + ex.Message;
            }

            process(str);
        }

        void process(string text)
        {
            label1.Text = text;

            t2.Start();
        }

        void t2_Tick(object sender, System.EventArgs e)
        {
            label1.Left = label1.Left - 3;
            if (label1.Left < 0)
            {
                t2.Stop();
                label1.Left = this.Right;
                timer1.Start();
            }
        }

        private Timer t2;
    }
}
