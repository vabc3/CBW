using Cbw.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace CbwShow.WF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string uri = ConfigurationManager.AppSettings["cbwServiceUri"] ?? "http://localhost:3338/cbw/"; 
            client = new CbwClient(new Uri(uri), 0);
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
                str = client.GetLast();
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
            if (label1.Right < 0)
            {
                t2.Stop();
                label1.Left = this.Right;
                timer1.Start();
            }
        }

        private Timer t2;
        private CbwClient client;
    }
}
