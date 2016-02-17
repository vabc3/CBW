using System.Collections;

namespace Cbw.Show.WF
{
    internal class RowHelper
    {
        private BitArray busyRow = new BitArray(1024);

        public int Get()
        {
            int y = -1;
            lock (busyRow)
            {
                while (busyRow.Get(++y)) ;
                busyRow.Set(y, true);
            }

            Logger.Log("Get row: {0}", y);
            return y;
        }

        public void Release(int row)
        {
            lock (busyRow)
            {
                busyRow.Set(row, false);
            }
        }
    }
}
