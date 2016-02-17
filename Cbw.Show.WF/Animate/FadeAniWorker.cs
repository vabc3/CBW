using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cbw.Show.WF
{
    class FadeAniWorker : AniWorker
    {
        private static RowHelper rowHelper = new RowHelper();
        private Label label;
        private int row;

        public FadeAniWorker(AniConfig config, Control container, string text)
            : base(config, container)
        {
            this.label = new MyLabel(text, config);
        }

        public override void Complete()
        {
            Logger.Log("Remove fade: {0}", label.Text);
            rowHelper.Release(this.row);
            this.Container.Controls.Remove(this.label);
        }

        protected override void Create()
        {
            Logger.Log("Create fade: {0}", label.Text);
            this.row = rowHelper.Get();
            this.label.Top = this.Container.Bottom - (this.row + 1) * this.Config.RowHeight;
            this.label.Left = this.Container.Left + this.Config.BoardWidth / 2 - this.label.PreferredWidth / 2;

            this.Container.Controls.Add(this.label);

            Task.Run(() => Update());
            this.State = AniWorkerState.Free;
        }

        protected override void Move()
        {
            Logger.Log("Move fade: {0}", label.Text);
        }

        private async void Update()
        {
            await Task.Delay(this.Config.FadeTime);
            this.State = AniWorkerState.Completed;
        }
    }
}
