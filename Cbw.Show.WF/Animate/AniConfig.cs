using System.Drawing;

namespace Cbw.Show.WF
{
    internal class AniConfig
    {
        private const int BasicSpeedStack = 2;
        private const int DeltaMoveBase = 3;
        private const int FadeTimeBase = 4000;
        private const int RowCount = 8;

        private int boardHeight;
        private int speed;
        private int deltaMove;

        public AniConfig()
        {
            // can get config from file
            this.Direction = true;
            this.Speed = 2;
            this.Font = new Font("Kaiti", 36);
            this.BoardWidth = 0;
            this.boardHeight = 0;
        }

        public bool Direction { get; set; }

        public int Speed
        {
            get
            {
                return this.speed;
            }
            set
            {
                this.speed = value;
                this.deltaMove = DeltaMoveBase * Speed / BasicSpeedStack;
                this.FadeTime = FadeTimeBase * BasicSpeedStack / Speed;
            }
        }

        public Font Font { get; set; }
        public int BoardWidth { get; set; }
        public int BoardHeight
        {
            get
            {
                return boardHeight;
            }
            set
            {
                this.boardHeight = value;
                this.RowHeight = this.boardHeight / RowCount;
                if (this.RowHeight > 10)
                {
                    this.Font = new Font("Kaiti", this.RowHeight / 2);
                }
            }
        }

        public int RowHeight { get; private set; }

        public int DeltaMove { get { return Direction ? deltaMove : -deltaMove; } }

        public int FadeTime { get; private set; }
    }
}
