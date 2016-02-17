using Cbw.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Cbw.Show.WF
{
    internal class AniManager
    {
        private AniConfig config = new AniConfig();
        private List<AniWorker> workers = new List<AniWorker>();
        private ReaderWriterLockSlim workersLock = new ReaderWriterLockSlim();

        public AniConfig Config { get { return config; } }

        public void Register(Caption caption, Control container)
        {
            Logger.Log("Caption arrived:{0}", caption.Text);
            var worker = AniWorker.CreateWorker(this.config, container, caption);
            this.workersLock.EnterWriteLock();
            this.workers.Add(worker);
            this.workersLock.ExitWriteLock();
        }

        public void UpdateConfig(int height, int width)
        {
            this.config.BoardHeight = height;
            this.config.BoardWidth = width;
        }

        public void Run()
        {
            List<AniWorker> removes;
            this.workersLock.EnterReadLock();
            foreach (var worker in workers)
            {
                worker.Go();
            }

            removes = workers.Where(work => work.Completed).ToList();
            this.workersLock.ExitReadLock();

            if (removes.Any())
            {
                this.workersLock.EnterWriteLock();
                foreach (var remove in removes)
                {
                    remove.Complete();
                    this.workers.Remove(remove);
                }

                this.workersLock.ExitWriteLock();
            }
        }

        public void UpdateSpeed(int speed)
        {
            this.config.Speed = speed;
        }

        public void Clear()
        {
            lock (this.workersLock)
            {
                foreach (var work in workers)
                {
                    work.State = AniWorkerState.Completed;
                }
            }
        }
    }
}
