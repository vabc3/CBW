using Cbw.Client;
using System;
using System.Configuration;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cbw.Show.WF
{
    public partial class Form1 : Form
    {
        private CbwClient client;
        private AniManager aniManager = new AniManager();
        private bool showOsd;
        private bool fs;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.timer2.Interval = 20;
            this.timer2.Tick += Animate;
            this.timer2.Start();

            this.textBox1.BackColor = Color.Black;

            this.showOsd = false;

            string uri = ConfigurationManager.AppSettings["cbwServiceUri"];
            this.client = new CbwClient(new Uri(uri), 0);
            this.client.OnCaptionArrive += (caption) => this.aniManager.Register(caption, this.panel1);
            this.client.OnSpeedChange += this.aniManager.UpdateSpeed;
            this.client.Start();
            this.Welcome();
        }

        private void Welcome()
        {
            Task.Run(async () => await this.client.Push(new Caption()
            {
                Text = "Watching " + this.client.Channel.Title,
                Config = new CaptionConfig
                {
                    Display = DisplayMode.Fade
                }
            }));
        }

        private void Animate(object sender, EventArgs e)
        {
            if (showOsd)
            {
                this.label1.Text = getOsd();
            }

            this.aniManager.Run();
        }

        private string getOsd()
        {
            return string.Format("{0} {1} {2}x",
                this.client.IsLive ? "LIVE" : "RECO",
                this.client.StageTime.ToString("HH:mm:ss.f"),
                this.client.Speed == 1 ? "1/2" : (this.client.Speed / 2).ToString());
        }

        private void Panel1_Resize(object sender, EventArgs e)
        {
            this.aniManager.UpdateConfig(this.panel1.Height, this.panel1.Width);
        }

        private async void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.L:
                    await this.client.Push(new Caption() { Text = "@" });
                    break;
                case Keys.O:
                    this.showOsd = !this.label1.Visible;
                    this.label1.Visible = this.showOsd;
                    break;
                case Keys.R:
                    this.client.StageTime = this.client.StageTime.AddSeconds(-10);
                    break;
                case Keys.E:
                    this.client.IsLive = true;
                    break;
                case Keys.F:
                    if (!this.fs)
                    {
                        if (this.WindowState == FormWindowState.Maximized)
                        {
                            this.Hide();
                            this.WindowState = FormWindowState.Normal;
                        }
                        this.FormBorderStyle = FormBorderStyle.None;
                        this.WindowState = FormWindowState.Maximized;
                        this.Show();

                        this.fs = true;
                    }
                    else
                    {
                        this.WindowState = FormWindowState.Normal;
                        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                        this.fs = false;
                    }
                    break;
                case Keys.Q:
                    Application.Exit();
                    break;
                case Keys.Up:
                    this.client.SpeedShiftUp();
                    break;
                case Keys.Down:
                    this.client.SpeedShiftDown();
                    break;

                case Keys.Enter:
                    textBox1.Visible = true;
                    textBox1.Focus();
                    break;

                case Keys.ShiftKey:
                    if (this.aniManager.Config.Direction)
                    {
                        this.aniManager.Config.Direction = false;
                        this.client.Direction = false;
                    }
                    break;
                default:
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                this.aniManager.Config.Direction = true;
                this.client.Direction = true;
            }
        }

        private async void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
            {
                this.textBox1.Visible = false;
                this.Focus();

                if (e.KeyCode == Keys.Enter)
                {
                    await command(textBox1.Text);
                }

                this.textBox1.Text = string.Empty;
            }
        }

        private async Task command(string text)
        {
            string cmd;
            string dat = null;
            int idx = text.IndexOf(' ');
            if (idx < 0 || idx==text.Length)
            {
                cmd = text;
            }
            else
            {
                cmd = text.Substring(0, idx);
                dat = text.Substring(idx + 1);
            }

            if (cmd == "s")
            {
                if (string.IsNullOrWhiteSpace(dat)) return; //clientbug?
                await this.client.Push(new Caption()
                {
                    Text = dat,
                    Config = new CaptionConfig
                    {
                        Display = DisplayMode.Fade
                    }
                });
            }
            else if (cmd == "q")
            {
                Application.Exit();
            }
            else if (cmd == "j")
            {
                DateTimeOffset da;
                if (DateTimeOffset.TryParse(dat, out da))
                {
                    this.client.StageTime = da;
                }
            }
            else if (cmd == "c")
            {
                this.aniManager.Clear();
            }
        }


    }
}
