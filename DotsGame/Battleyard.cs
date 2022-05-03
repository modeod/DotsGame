using System;
using System.Collections.Generic;

namespace DotsGame
{
    class Battleyard
    {
        public int player1DotsInside = 0;
        public int player2OrBotDotsInside = 0;
        public int LastZoneIndex = 0;

        // Zone index = index in this Lists
        public int FromX;
        public int FromY;

        public int ToX;
        public int ToY;

        public List<XY[]> closedZones;
        public List<XY[]> dotsInside;

        // 0 - empty, 1 - player1, 2 - player2/bot, 3 - closed
        public NewDot[,] Yard = new NewDot[10, 10];

        Dictionary<DotStates, string> dtStToStr = new Dictionary<DotStates, string>() {
            {DotStates.EMPTY, " " },
            {DotStates.PLAYER, "•" },
            {DotStates.PLAYER2ORBOT, "○" },
            {DotStates.CLOSED, "#" }
        };

        

        public void ClearBY()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Yard[i, j] = new NewDot(DotStates.EMPTY);
                }
            }

        }
        public Battleyard(List<XY[]> _closedZones, List<XY[]> _dotsInside, NewDot[,] _Yard, int _FromX, int _FromY, int _ToX, int _ToY)
        {
              FromX = _FromX;
            FromY = _FromY;

            ToX = _ToX;
            ToY = _ToY;
             closedZones = new List<XY[]>(_closedZones);
            dotsInside = new List<XY[]>(_dotsInside);
            if(_Yard[0,0] == null)
            {
                ClearBY();

            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        Yard[i, j] = _Yard[i, j].Clone();
                    }
                }
            }
            
        }

        public void ShowBattleyard()
        {
            Console.Write("  ");
            for (int i = 0; i < 10; i++)
                Console.Write(i + " ");

            Console.WriteLine();
            for (int i = 0; i < 10; i++)
            {
                Console.Write(i + " ");
                for (int j = 0; j < 10; j++)
                {
                    NewDot d = Yard[i, j];
                    Console.Write(dtStToStr[d.dotState] + " ");
                }
                Console.WriteLine();
            }
        }

        public XY PutDot(DotStates st)
        {
            while (true)
            {
                Console.WriteLine($"Введите  координаты x y: ");
                string s = Console.ReadLine();
                //проверяем существуют ли заданные вершины
                try
                {
                    string[] ss = s.Split(' ');
                    if (Yard[Int32.Parse(ss[0]), Int32.Parse(ss[1])].dotState != DotStates.EMPTY)
                    {
                        Console.WriteLine("Точка занята"); continue;
                    }
                     
                    Yard[Int32.Parse(ss[0]), Int32.Parse(ss[1])].dotState = st;
                    return new XY((byte) Int32.Parse(ss[0]), (byte) Int32.Parse(ss[1]));
                }
                catch { Console.WriteLine("Нету такой точки"); continue; }
            }
        }
    }
}
