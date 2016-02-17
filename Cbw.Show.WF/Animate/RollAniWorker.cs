using System.Drawing;
using System.Windows.Forms;

namespace Cbw.Show.WF
{
    internal class RollAniWorker : AniWorker
    {
        private static RowHelper rowHelper = new RowHelper();
        private static RowHelper reverseRowHelper = new RowHelper();

        private int row = 0;
        private bool updatedRow = false;
        private Label label;

        public RollAniWorker(AniConfig config, Control container, string text)
            : base(config, container)
        {
            this.label = new MyLabel(text, config);
        }

        protected override void Create()
        {
            Logger.Log("Add lab: {0}", label.Text);

            if (Config.Direction)
            {
                this.row = rowHelper.Get();
                this.label.Left = this.Container.Right;
            }
            else
            {
                this.row = reverseRowHelper.Get();
                this.label.Left = this.Container.Left - this.label.PreferredWidth;
            }

            this.label.Top = this.row * this.Config.RowHeight;
            this.Container.Controls.Add(this.label);

            this.State = AniWorkerState.Active;
        }

        protected override void Move()
        {
            // Logger.Log("Go lab: {0}", label.Text);
            this.label.Left -= this.Config.DeltaMove;

            if (this.label.Right < this.Container.Left)
            {
                this.State = AniWorkerState.Completed;
            }

            if (!updatedRow)
            {
                if (this.Config.Direction)
                {
                    if (this.label.Right < this.Container.Right)
                    {
                        rowHelper.Release(this.row);
                        this.updatedRow = true;
                    }
                }
                else if (this.label.Left > this.Container.Left)
                {
                    reverseRowHelper.Release(this.row);
                    this.updatedRow = true;
                }
            }
        }

        public override void Complete()
        {
            Logger.Log("Remove lab: {0}", label.Text);
            this.Container.Controls.Remove(this.label);
        }
    }
}
