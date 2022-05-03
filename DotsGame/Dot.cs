
namespace DotsGame
{
    struct XY
    {
        public byte x;
        public byte y;

        public XY(byte _x, byte _y)
        {
            x = _x;
            y = _y;
        }
    }

    class NewDot
    {
        public DotStates dotState;
        public int ZoneIndex = 0;

        public NewDot(DotStates _dotState)
        {
            dotState = _dotState;
        }

        public NewDot Clone()
        {
            NewDot r = new NewDot(dotState);
            r.ZoneIndex = ZoneIndex;
            return r;
        }
    }

    /*
    class Dot
    {
        //public int x;
        //public int y;
        // 0 - empty, 1 - player1, 2 - player2/bot, 3 - closed
        public DotStates dotState;

        public bool usedInClosedZone;

        public Dot(int x, int y, DotStates dotState)
        {
            usedInClosedZone = false;
            this.x = x;
            this.y = y;
            this.dotState = dotState;
        }
    }
    */
}
