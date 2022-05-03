using System.Collections.Generic;

namespace DotsGame
{
    class NewZone
    {
        XY[] insideDots;
        DotStates zoneOwner;

        public NewZone(DotStates _owner, XY[] _insideDots)
        {
            zoneOwner = _owner;
            insideDots = _insideDots;
        }
    }

    /*
    class Zone
    {
        
        public bool inside;

        List<Dot> circuit;
        List<Dot> insideDots;
        List<Zone> zonesInside; //only enemyZones, TODO
        DotStates owner; // true - player1, false - plares2 / bot

        public Zone(DotStates _owner, List<Dot> _circuit)
        {
            owner = _owner;
            inside = false;
            circuit = _circuit;
            insideDots = new List<Dot>();
            zonesInside = new List<Zone>();
        }

        public List<Dot> GetInsideDots()
        {
            return insideDots;
        }

        public List<Dot> GetCircuit()
        {
            return insideDots;
        }
    }
    */
}
