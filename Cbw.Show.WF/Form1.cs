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
using System.Threading;

namespace CbwShow.WF
{
    class Capt
    {
        public Label Label { get; set; }
    }

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
            this.labels = new List<Label>();
            this.queue = new Queue<Capt>();
            string uri = ConfigurationManager.AppSettings["cbwServiceUri"] ?? "http://localhost.fiddler:3338/cbw/";
            this.client = new CbwClient(new Uri(uri), 0);
            this.client.OnCaptionArrive += client_OnCaptionArrive;
            this.client.StartPush();
        }

        void client_OnCaptionArrive(Caption caption)
        {
            var t = new Label();
            t.Text = caption.Text;
            t.Left = panel1.Right;
            lock (this.queue)
            {
                this.queue.Enqueue(new Capt { Label = t });
            }
        }

        void timer1_Tick(object sender, System.EventArgs e)
        {
            if (this.queue.Any())
            {
                lock (this.queue)
                {
                    while (queue.Any())
                    {
                        var cap = this.queue.Dequeue();
                        this.labels.Add(cap.Label);
                        this.panel1.Controls.Add(cap.Label);
                    }
                }
            }

            IList<Label> removes = new List<Label>();

            foreach(var lab in this.labels){
                if (lab.Right < panel1.Left)
                {
                    removes.Add(lab);
                }
                else
                {
                    lab.Left -= 20;
                }
            }

            foreach (var rem in removes)
            {
                this.labels.Remove(rem);
                this.panel1.Controls.Remove(rem);
            }

            //timer1.Stop();
            //string str = "null";
            //try
            //{
            //    str = client.GetLast();
            //}
            //catch (Exception ex)
            //{
            //    str = "Err!!" + ex.Message;
            //}

            //process(str);
        }

        private Queue<Capt> queue;
        private IList<Label> labels;
        private CbwClient client;
    }
}
