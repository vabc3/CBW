using Cbw.Client;
using System.Windows.Forms;

namespace Cbw.Show.WF
{
    internal enum AniWorkerState
    {
        Ready, Active, Free, Completed
    }

    internal abstract class AniWorker
    {
        public static AniWorker CreateWorker(AniConfig config, Control container, Caption caption)
        {
            if (caption.Config != null && caption.Config.Display.HasValue)
            {
                switch (caption.Config.Display.Value)
                {
                    case DisplayMode.Fade:
                        return new FadeAniWorker(config, container, caption.Text);
                    case DisplayMode.Roll:
                        return new RollAniWorker(config, container, caption.Text);
                }
            }

            return new RollAniWorker(config, container, caption.Text);
        }


        public AniWorker(AniConfig config, Control container)
        {
            this.Config = config;
            this.Container = container;

            this.State = AniWorkerState.Ready;
        }

        public bool Completed
        {
            get
            {
                return this.State == AniWorkerState.Completed;
            }
        }

        public AniWorkerState State { get; set; }

        protected AniConfig Config { get; private set; }

        protected Control Container { get; private set; }

        public void Go()
        {
            switch (this.State)
            {
                case AniWorkerState.Ready:
                    this.Create();
                    break;
                case AniWorkerState.Active:
                    this.Move();
                    break;
                default:
                    break;
            }
        }

        public abstract void Complete();

        protected abstract void Create();
        protected abstract void Move();
    }
}
