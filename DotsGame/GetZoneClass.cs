using System;
using System.Collections.Generic;

namespace DotsGame
{
    class GetZoneClass //Радиальная развертка !
    {
        // 0 - empty, 1 - player1, 2 - player2/bot, 3 - closed
        DotStates owner;
        XY startPoint;
        NewDot[,] by;

        //MAKE THIS ONLY FOR ONE ZONE
        public List<List<XY>> closedZones;
        public List<XY> dotsInside;
        public bool enemyInside = false;

        int finalSide = 0;
        bool zoneFinded = false;

        public GetZoneClass(XY _startCenterPoint, XY _startPriviousPoint, DotStates _owner, DotStates _enemy, NewDot[,] _by)
        {
            startPoint = _startCenterPoint;
            owner = _owner;
            by = _by;

            closedZones = new List<List<XY>>();
            FindClosedZones(_startCenterPoint, _startPriviousPoint, new List<XY>(), 0);
            //MAKE THIS ONLY FOR ONE ZONE
            if (closedZones.Count == 0) return;


            //
            //Найти точку внутри зоны
            //
            XY startInsideDot = new XY(0,0); // ?????

            bool findedStartInside = false;
            for(int i = 0; i < closedZones[0].Count; i++)
            {
                //Часики
                int xCow;
                int yCow;
                if (i+1 < closedZones[0].Count)
                {
                    xCow = closedZones[0][i + 1].x;
                    yCow = closedZones[0][i + 1].y;
                }
                else
                {
                    xCow = closedZones[0][0].x;
                    yCow = closedZones[0][0].y;
                }

                //Проверить 2ю после точку
                int k = i + 2;
                if (k >= closedZones[0].Count)
                {
                    k = k - closedZones[0].Count;
                }

                for (byte g = 0; g < 7; g++)
                {
                    if(finalSide < 0)
                    {
                        if (xCow < closedZones[0][i].x + 1 && yCow < closedZones[0][i].y)
                            xCow++;
                        else if (xCow > closedZones[0][i].x - 1 && yCow > closedZones[0][i].y)
                            xCow--;
                        else if (yCow < closedZones[0][i].y + 1 && xCow > (closedZones[0][i].x))
                            yCow++;
                        else if (yCow > closedZones[0][i].y - 1 && xCow < (closedZones[0][i].x))
                            yCow--;
                    }
                    else
                    {
                        if (xCow < closedZones[0][i].x && yCow < (closedZones[0][i].y + 1))
                            yCow++;
                        else if (xCow > closedZones[0][i].x && yCow > (closedZones[0][i].y - 1))
                            yCow--;
                        else if (yCow < closedZones[0][i].y && xCow > (closedZones[0][i].x - 1))
                            xCow--;
                        else if (yCow > closedZones[0][i].y && xCow < (closedZones[0][i].x + 1))
                            xCow++;
                    }

                    //DEBUG
                    Console.WriteLine($"cowCoord [{xCow},{yCow}]");

                    try
                    {
                        if(by[xCow, yCow].dotState == owner) { break; }
                        if (by[xCow, yCow].dotState != owner && by[xCow, yCow].ZoneIndex == 0)
                        {
                            if (closedZones[0][i].x + 1 == closedZones[0][k].x && closedZones[0][i].y == closedZones[0][k].y ||
                                closedZones[0][i].x - 1 == closedZones[0][k].x && closedZones[0][i].y == closedZones[0][k].y ||
                                closedZones[0][i].x == closedZones[0][k].x && closedZones[0][i].y + 1 == closedZones[0][k].y ||
                                closedZones[0][i].x == closedZones[0][k].x && closedZones[0][i].y - 1 == closedZones[0][k].y)
                            {
                                continue;
                            }
                            //by[xCow, yCow].zone = new NewZone(owner);
                            startInsideDot = new XY((byte)xCow, (byte)yCow);
                            findedStartInside = true;
                            break;
                        }
                    }
                    catch { continue; }
                }

                if (findedStartInside) break;

            }

            if (!findedStartInside) 
                return;


            //
            //Поиск точек внутри зоны вширь ВВЛП Для Поля 5 на 5
            //
            bool[,] visited = new bool[10,10];
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    visited[i,j] = false;

            List<XY> queue = new List<XY>();
            dotsInside = new List<XY>();
            visited[startInsideDot.x, startInsideDot.y] = true;
            queue.Add(startInsideDot);
            dotsInside.Add(startInsideDot);
            Console.WriteLine($"Мы посетили вершину {startInsideDot.x} {startInsideDot.y}");
            //Если этот цикл уже начался - значит была зотя бы одна начальная точка внутри зоны
            while (queue.Count != 0)
            {
                XY checking = queue[0];
                if (by[checking.x, checking.y].dotState == _enemy) enemyInside = true;

                queue.RemoveAt(0);

                //Проверка в 4 стороны
                try
                {
                    if ( (by[checking.x - 1, checking.y].dotState != owner || by[checking.x - 1, checking.y].ZoneIndex != 0) && visited[checking.x - 1, checking.y] == false)
                    {
                        visited[checking.x - 1, checking.y] = true;
                        dotsInside.Add(new XY((byte)(checking.x - 1), checking.y));
                        queue.Add(new XY((byte)(checking.x - 1), checking.y));
                    }
                }
                catch { }
                
                try
                {
                    if ((by[checking.x + 1, checking.y].dotState != owner || by[checking.x + 1, checking.y].ZoneIndex != 0) && visited[checking.x + 1, checking.y] == false)
                    {
                        visited[checking.x + 1, checking.y] = true;
                        dotsInside.Add(new XY((byte)(checking.x + 1), checking.y));
                        queue.Add(new XY((byte)(checking.x + 1), checking.y));
                    }
                }
                catch { }
               
                try
                {
                    if ((by[checking.x, checking.y - 1].dotState != owner || by[checking.x, checking.y - 1].ZoneIndex != 0) && visited[checking.x, checking.y - 1] == false)
                    {
                        visited[checking.x, checking.y - 1] = true;
                        dotsInside.Add(new XY(checking.x, (byte)(checking.y - 1)));
                        queue.Add(new XY(checking.x, (byte)(checking.y - 1)));
                    }
                }
                catch { }
                
                try
                {
                    if ((by[checking.x, checking.y + 1].dotState != owner || by[checking.x, checking.y + 1].ZoneIndex != 0) && visited[checking.x, checking.y + 1] == false)
                    {
                        visited[checking.x, checking.y + 1] = true;
                        dotsInside.Add(new XY(checking.x, (byte)(checking.y + 1)));
                        queue.Add(new XY(checking.x, (byte)(checking.y + 1)));
                    }
                }
                catch { }
                

            }

        }
        
        void FindClosedZones(XY currentPoint, XY priviousPoint, List<XY> priviousDots, int side)
        {

            if (currentPoint.Equals(startPoint) && priviousDots.Count > 3)
            {
                //DEBUG
                Console.WriteLine($"FINDED ZONE");
                zoneFinded = true;
                closedZones.Add(new List<XY>(priviousDots));
                finalSide = side; //????? Should i check it for one more dot?
                return;
            }

            if (priviousDots.Contains(currentPoint))
            {
                //DEBUG
                Console.WriteLine($"BAD ZONE");
                return;
            }


            Dictionary<XY, byte> nextDots = СounterСlockwise(currentPoint, priviousPoint);
            if (nextDots.Count == 0)
                return;

            foreach(KeyValuePair<XY, byte> d in nextDots)
            {
                if (zoneFinded) { return; }
                if(by[d.Key.x, d.Key.y].ZoneIndex != 0) { continue; }
                //DEBUG
                Console.WriteLine($"Finded  owner Dot: x={d.Key.x}, y={d.Key.y}");

                List<XY> newPriviousDots = new List<XY>(priviousDots);
                newPriviousDots.Add(currentPoint);
                if(d.Value < 3)
                {
                    FindClosedZones(d.Key, currentPoint, newPriviousDots, side+1);
                }
                else if(d.Value == 3)
                {
                    FindClosedZones(d.Key, currentPoint, newPriviousDots, side);
                }
                else if(d.Value < 7)
                {
                    FindClosedZones(d.Key, currentPoint, newPriviousDots, side-1);
                }
            }
        }

        Dictionary<XY, byte> СounterСlockwise(XY center, XY FirstPos) // List<Dot>
        {
            Dictionary<XY, byte> findedDots = new Dictionary<XY, byte>();
            int xCow = FirstPos.x;
            int yCow = FirstPos.y;

            //DEBUG
            Console.WriteLine($"Center [{center.x},{center.y}]");
            Console.WriteLine($"cowStartCoord [{xCow},{yCow}]");

            for (byte i = 0; i < 7; i++)
            {
                if (xCow < center.x+1 && yCow < center.y)
                    xCow++;
                else if (xCow > center.x-1 && yCow > center.y)
                    xCow--;
                else if (yCow < center.y+1 && xCow > (center.x))
                    yCow++;
                else if (yCow > center.y-1 && xCow < (center.x))
                    yCow--;

                
                //DEBUG
                Console.WriteLine($"cowCoord [{xCow},{yCow}]");

                try
                {
                    if (by[xCow, yCow].dotState == owner)
                        findedDots.Add(new XY((byte)xCow, (byte)yCow), i);

                } catch { continue; }
               
            }

            return findedDots;
        }

        public static XY[] Сlockwise(XY center, XY FirstPos, DotStates owner, NewDot[,] by) // List<Dot>
        {
            List<XY> findedDots = new List<XY>();
            int xCow = FirstPos.x;
            int yCow = FirstPos.y;

            //DEBUG
            Console.WriteLine($"Center [{center.x},{center.y}]");
            Console.WriteLine($"cowStartCoord [{xCow},{yCow}]");

            for (byte i = 0; i < 7; i++)
            {
                
                if (xCow < center.x && yCow < (center.y + 1))
                    yCow++;
                else if (xCow > center.x && yCow > (center.y - 1))
                    yCow--;
                else if (yCow < center.y && xCow > (center.x - 1))
                    xCow--;
                else if (yCow > center.y && xCow < (center.x + 1))
                    xCow++;
                
                //DEBUG
                Console.WriteLine($"cowCoord [{xCow},{yCow}]");

                try
                {
                    if (by[xCow, yCow].dotState == owner)
                        findedDots.Add(new XY((byte)xCow, (byte)yCow));
                }
                catch { continue; }

            }

            return findedDots.ToArray();
        }
    }
}
