using System;
using System.Collections.Generic;
using System.Text;

namespace DotsGame
{
    enum DotStates
    {
        EMPTY = 0,
        PLAYER,
        PLAYER2ORBOT,
        CLOSED
    }

    class Program
    {
        
        static List<bool> zoneOwner = new List<bool>(); // true = player, false = player2/bot
        static bool player = true;
        static bool bot = false;
        static bool end = false;
        static int move = 0;
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Battleyard battle = new Battleyard(new List<XY[]>(), new List<XY[]>(), new NewDot[10,10],9,9,0,0);
            //StartGame
            while (!end)
            {
                move++;
                Console.WriteLine("====================");
                battle.ShowBattleyard();
                Console.WriteLine($"Игрок 1 окружил {battle.player1DotsInside} точек | Игрок 2 окружил {battle.player2OrBotDotsInside} точек");
                Console.WriteLine("====================");

                //PLAYER 1
                if (player)
                {
                    Console.WriteLine("--- Игрок 1:");
                    // 0, 2 DEBUG
                    XY checkZonesFrom = battle.PutDot(DotStates.PLAYER);
                    if (checkZonesFrom.x > battle.ToX) battle.ToX = checkZonesFrom.x+1;
                    if (checkZonesFrom.y > battle.ToY) battle.ToY = checkZonesFrom.y+1;
                    if (checkZonesFrom.x < battle.FromX) battle.FromX = checkZonesFrom.x-1;
                    if (checkZonesFrom.y < battle.FromY) battle.FromY = checkZonesFrom.y-1;

                    if(battle.ToX > 9) { battle.ToX = 9; }
                    if (battle.ToY > 9) { battle.ToY = 9; }
                    if (battle.FromX < 0) { battle.FromX = 0; }
                    if (battle.FromY < 0) { battle.FromY = 0; }



                    //Проверка на то, что точка внутри чужой старой зоны
                    if (battle.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex != 0)
                    {
                        if (!zoneOwner[battle.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                        {
                            battle.player2OrBotDotsInside++;
                            foreach (XY d in battle.dotsInside[battle.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                            {
                                if (battle.Yard[d.x, d.y].dotState == DotStates.PLAYER && (battle.Yard[d.x, d.y].dotState == DotStates.PLAYER2ORBOT)) continue;
                                battle.Yard[d.x, d.y].dotState = DotStates.CLOSED;
                            }
                            continue;
                        }
                        else if (zoneOwner[battle.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                        {
                            continue;
                        }

                    }

                    //Переделать на нечасики, а просто проверку окрудения и +1
                    if (!(GetZoneClass.Сlockwise(checkZonesFrom, new XY((byte)(checkZonesFrom.x + 1), checkZonesFrom.y), DotStates.PLAYER, battle.Yard).Length >= 2))
                        goto end;


                    GetZoneClass moore = new GetZoneClass(checkZonesFrom, new XY((byte)(checkZonesFrom.x + 1), checkZonesFrom.y), DotStates.PLAYER, DotStates.PLAYER2ORBOT, battle.Yard);
                    if (moore.dotsInside == null || moore.dotsInside.Count == 0)
                        goto end;

                    //Записываем Зону
                    battle.LastZoneIndex++;
                    battle.closedZones.Add(moore.closedZones[0].ToArray()); // ???
                    battle.dotsInside.Add(moore.dotsInside.ToArray());
                    zoneOwner.Add(true);

                    //Обрабатываем зону
                    if (moore.enemyInside)
                    {
                        foreach(XY d in moore.dotsInside)
                        {
                            battle.Yard[d.x, d.y].ZoneIndex = battle.LastZoneIndex;
                            if(battle.Yard[d.x, d.y].dotState == DotStates.PLAYER2ORBOT)
                                battle.player1DotsInside++;
                            else if(battle.Yard[d.x, d.y].dotState == DotStates.PLAYER && battle.Yard[d.x, d.y].ZoneIndex != 0)
                                battle.player2OrBotDotsInside--;
                            else if(battle.Yard[d.x, d.y].dotState == DotStates.EMPTY)
                                battle.Yard[d.x, d.y].dotState = DotStates.CLOSED;
                            else
                                Console.WriteLine("IDK what to do with this dot");
                        }
                    }
                    else
                        foreach (XY d in moore.dotsInside)
                            battle.Yard[d.x, d.y].ZoneIndex = battle.LastZoneIndex;

                    //END TURN
                    end:
                    player = false;
                }
                // PLAYER 2
                else if (!bot)
                {
                    Console.WriteLine("--- Игрок 2:");
                    XY checkZonesFrom = battle.PutDot(DotStates.PLAYER2ORBOT);
                    if (checkZonesFrom.x > battle.ToX) battle.ToX = checkZonesFrom.x + 1;
                    if (checkZonesFrom.y > battle.ToY) battle.ToY = checkZonesFrom.y + 1;
                    if (checkZonesFrom.x < battle.FromX) battle.FromX = checkZonesFrom.x -1;
                    if (checkZonesFrom.y < battle.FromY) battle.FromY = checkZonesFrom.y -1;

                    if (battle.ToX > 9) { battle.ToX = 9; }
                    if (battle.ToY > 9) { battle.ToY = 9; }
                    if (battle.FromX < 0) { battle.FromX = 0; }
                    if (battle.FromY < 0) { battle.FromY = 0; }

                    //Проверка на то, что точка внутри чужой старой зоны
                    if (battle.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex != 0)
                    {
                        if (zoneOwner[battle.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                        {
                            battle.player1DotsInside++;
                            foreach (XY d in battle.dotsInside[battle.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                            {
                                if (battle.Yard[d.x, d.y].dotState == DotStates.PLAYER && (battle.Yard[d.x, d.y].dotState == DotStates.PLAYER2ORBOT)) continue;
                                battle.Yard[d.x, d.y].dotState = DotStates.CLOSED;
                            }
                            return;
                        }
                        else if (!zoneOwner[battle.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                        {
                            return;
                        }
                    }

                    //Переделать на нечасики, а просто проверку окрудения и +1
                    if (!(GetZoneClass.Сlockwise(checkZonesFrom, new XY((byte)(checkZonesFrom.x + 1), checkZonesFrom.y), DotStates.PLAYER2ORBOT, battle.Yard).Length >= 2))
                        goto end;


                    GetZoneClass moore = new GetZoneClass(checkZonesFrom, new XY((byte)(checkZonesFrom.x + 1), checkZonesFrom.y), DotStates.PLAYER2ORBOT, DotStates.PLAYER, battle.Yard);
                    if (moore.dotsInside == null || moore.dotsInside.Count == 0)
                        goto end;

                    //Записываем Зону
                    battle.LastZoneIndex++;
                    battle.closedZones.Add(moore.closedZones[0].ToArray()); // ???
                    battle.dotsInside.Add(moore.dotsInside.ToArray());
                    zoneOwner.Add(false);

                    //Обрабатываем зону
                    if (moore.enemyInside)
                    {
                        foreach (XY d in moore.dotsInside)
                        {
                            battle.Yard[d.x, d.y].ZoneIndex = battle.LastZoneIndex;
                            if (battle.Yard[d.x, d.y].dotState == DotStates.PLAYER)
                                battle.player2OrBotDotsInside++;
                            else if (battle.Yard[d.x, d.y].dotState == DotStates.PLAYER2ORBOT && battle.Yard[d.x, d.y].ZoneIndex != 0)
                                battle.player1DotsInside--;
                            else if (battle.Yard[d.x, d.y].dotState == DotStates.EMPTY)
                                battle.Yard[d.x, d.y].dotState = DotStates.CLOSED;
                            else
                                Console.WriteLine("IDK what to do with this dot");
                        }
                    }
                    else
                        foreach (XY d in moore.dotsInside)
                            battle.Yard[d.x, d.y].ZoneIndex = battle.LastZoneIndex;

                    //END TURN
                    end:
                    player = true;
                }
                else
                //BOT
                {
                    // Оценочная функция
                    
                    XY bestMove = new XY((byte)(battle.FromX+1), (byte)(battle.FromY));
                    if (move == 2)
                    {
                        bestMove.x = 2;
                        bestMove.y = 2;
                        goto MMend;
                    }
                    int lastBestScore = int.MinValue;

                    for(int i = battle.FromX; i <= battle.ToX; i++)
                    {
                        for (int k = battle.FromY; k <= battle.ToY; k++)
                        {
                            if (battle.Yard[i, k].dotState != DotStates.EMPTY) continue;
                            var tupleMINMAX = MinMax(new XY((byte)i, (byte)k), new Battleyard(battle.closedZones, battle.dotsInside, battle.Yard, battle.FromX, battle.FromY, battle.ToX, battle.ToY), 1);
                            if(tupleMINMAX.myDots - tupleMINMAX.enemyDots > lastBestScore)
                            {
                                lastBestScore = tupleMINMAX.myDots - tupleMINMAX.enemyDots;
                                bestMove.x = (byte)i; bestMove.y = (byte)k;
                            }
                        }
                    }

                    (int myDots, int enemyDots) MinMax(XY checkZonesFrom, Battleyard battleyard, int deep)
                    {
                        // mydots - bot dots, enemy - player
                        int myDots = 0, enemyDots = 0;

                        //Ищем кол-во новых точек
                        //Проверка на точку внутри зоны и ее доступность!!!!!
                        battleyard.Yard[checkZonesFrom.x, checkZonesFrom.y].dotState = DotStates.PLAYER2ORBOT;


                        if (checkZonesFrom.x > battleyard.ToX) battleyard.ToX = checkZonesFrom.x + 1;
                        if (checkZonesFrom.y > battleyard.ToY) battleyard.ToY = checkZonesFrom.y + 1;
                        if (checkZonesFrom.x < battleyard.FromX) battleyard.FromX = checkZonesFrom.x - 1;
                        if (checkZonesFrom.y < battleyard.FromY) battleyard.FromY = checkZonesFrom.y - 1;

                        if (battleyard.ToX > 9) { battleyard.ToX = 9; }
                        if (battleyard.ToY > 9) { battleyard.ToY = 9; }
                        if (battleyard.FromX < 0) { battleyard.FromX = 0; }
                        if (battleyard.FromY < 0) { battleyard.FromY = 0; }

                        //Проверка на то, что точка внутри чужой старой зоны
                        if (battleyard.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex != 0)
                        {
                            if (zoneOwner[battleyard.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                            {
                                enemyDots++;
                                foreach (XY d in battleyard.dotsInside[battleyard.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                                {
                                    if (battleyard.Yard[d.x, d.y].dotState == DotStates.PLAYER && (battleyard.Yard[d.x, d.y].dotState == DotStates.PLAYER2ORBOT)) continue;
                                    battleyard.Yard[d.x, d.y].dotState = DotStates.CLOSED;
                                }
                                goto endBot;
                            }
                            else if (!zoneOwner[battleyard.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                            {
                                goto endBot;
                            }
                        }

                        //Переделать на нечасики, а просто проверку окрудения и +1
                        if (!(GetZoneClass.Сlockwise(checkZonesFrom, new XY((byte)(checkZonesFrom.x + 1), checkZonesFrom.y), DotStates.PLAYER2ORBOT, battleyard.Yard).Length >= 2))
                            goto endBot;


                        GetZoneClass moore = new GetZoneClass(checkZonesFrom, new XY((byte)(checkZonesFrom.x + 1), checkZonesFrom.y), DotStates.PLAYER2ORBOT, DotStates.PLAYER, battleyard.Yard);
                        if (moore.dotsInside == null || moore.dotsInside.Count == 0)
                            goto endBot;

                        //Записываем Зону
                        battleyard.LastZoneIndex++;
                        battleyard.closedZones.Add(moore.closedZones[0].ToArray()); // ???
                        battleyard.dotsInside.Add(moore.dotsInside.ToArray());
                        zoneOwner.Add(false);

                        //Обрабатываем зону
                        if (moore.enemyInside)
                        {
                            foreach (XY d in moore.dotsInside)
                            {
                                battleyard.Yard[d.x, d.y].ZoneIndex = battleyard.LastZoneIndex;
                                if (battleyard.Yard[d.x, d.y].dotState == DotStates.PLAYER)
                                    myDots++;
                                else if (battleyard.Yard[d.x, d.y].dotState == DotStates.PLAYER2ORBOT && battleyard.Yard[d.x, d.y].ZoneIndex != 0)
                                    enemyDots--;
                                else if (battleyard.Yard[d.x, d.y].dotState == DotStates.EMPTY)
                                    battleyard.Yard[d.x, d.y].dotState = DotStates.CLOSED;
                                else
                                    Console.WriteLine("IDK what to do with this dot");
                            }
                        }
                        else
                            foreach (XY d in moore.dotsInside)
                                battleyard.Yard[d.x, d.y].ZoneIndex = battleyard.LastZoneIndex;

                        //Заканчиваем проверку точек Бота
                        endBot:

                        //Проверяем 1 раз глубину
                        if(deep == 0)
                        {

                            //
                            //Теперь перебираем возможные варианты хода игрока, НЕ вызивая Просчет хода вглубину
                            int myNewDots = 0, enemyNewDots = 0, result = int.MinValue;


                            for (int i = battleyard.FromX; i <= battleyard.ToX; i++)
                            {
                                for (int k = battleyard.FromY; k <= battleyard.ToY; k++)
                                {
                                    Battleyard battleyard2 = new Battleyard(battleyard.closedZones, battleyard.dotsInside, battleyard.Yard, battleyard.FromX, battleyard.FromY, battleyard.ToX, battle.ToY);

                                    if (battleyard2.Yard[i, k].dotState != DotStates.EMPTY) continue;
                                    checkZonesFrom.x = (byte)i;
                                    checkZonesFrom.y = (byte)k;

                                    int myNewLocalDots = 0, enemyNewLocalDots = 0;
                                    

                                    //Ищем кол-во новых точек
                                    //Проверка на точку внутри зоны и ее доступность!!!!!
                                    battleyard2.Yard[checkZonesFrom.x, checkZonesFrom.y].dotState = DotStates.PLAYER2ORBOT;

                                    if (checkZonesFrom.x > battleyard2.ToX) battleyard2.ToX = checkZonesFrom.x + 1;
                                    if (checkZonesFrom.y > battleyard2.ToY) battleyard2.ToY = checkZonesFrom.y + 1;
                                    if (checkZonesFrom.x < battleyard2.FromX) battleyard2.FromX = checkZonesFrom.x - 1;
                                    if (checkZonesFrom.y < battleyard2.FromY) battleyard2.FromY = checkZonesFrom.y - 1;

                                    if (battleyard2.ToX > 9) { battleyard2.ToX = 9; }
                                    if (battleyard2.ToY > 9) { battleyard2.ToY = 9; }
                                    if (battleyard2.FromX < 0) { battleyard2.FromX = 0; }
                                    if (battleyard2.FromY < 0) { battleyard2.FromY = 0; }



                                    //Проверка на то, что точка внутри чужой старой зоны
                                    if (battleyard2.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex != 0)
                                    {
                                        if (!zoneOwner[battleyard2.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                                        {
                                            myNewLocalDots++;
                                            foreach (XY d in battleyard2.dotsInside[battleyard2.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                                            {
                                                if (battleyard2.Yard[d.x, d.y].dotState == DotStates.PLAYER && (battleyard2.Yard[d.x, d.y].dotState == DotStates.PLAYER2ORBOT)) continue;
                                                battleyard2.Yard[d.x, d.y].dotState = DotStates.CLOSED;
                                            }
                                            continue;
                                        }
                                        else if (zoneOwner[battleyard2.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                                        {
                                            continue;
                                        }

                                    }

                                    //Переделать на нечасики, а просто проверку окрудения и +1
                                    if (!(GetZoneClass.Сlockwise(checkZonesFrom, new XY((byte)(checkZonesFrom.x + 1), checkZonesFrom.y), DotStates.PLAYER, battleyard2.Yard).Length >= 2))
                                        goto endPlayer;


                                    GetZoneClass moore2 = new GetZoneClass(checkZonesFrom, new XY((byte)(checkZonesFrom.x + 1), checkZonesFrom.y), DotStates.PLAYER, DotStates.PLAYER2ORBOT, battleyard2.Yard);
                                    if (moore2.dotsInside == null || moore2.dotsInside.Count == 0)
                                        goto endPlayer;

                                    //Записываем Зону
                                    battleyard2.LastZoneIndex++;
                                    battleyard2.closedZones.Add(moore2.closedZones[0].ToArray()); // ???
                                    battleyard2.dotsInside.Add(moore2.dotsInside.ToArray());
                                    zoneOwner.Add(true);

                                    //Обрабатываем зону
                                    if (moore2.enemyInside)
                                    {
                                        foreach (XY d in moore2.dotsInside)
                                        {
                                            battleyard2.Yard[d.x, d.y].ZoneIndex = battleyard2.LastZoneIndex;
                                            if (battleyard2.Yard[d.x, d.y].dotState == DotStates.PLAYER2ORBOT)
                                                enemyNewLocalDots++;
                                            else if (battleyard2.Yard[d.x, d.y].dotState == DotStates.PLAYER && battleyard2.Yard[d.x, d.y].ZoneIndex != 0)
                                                myNewLocalDots--;
                                            else if (battleyard2.Yard[d.x, d.y].dotState == DotStates.EMPTY)
                                                battleyard2.Yard[d.x, d.y].dotState = DotStates.CLOSED;
                                            else
                                                Console.WriteLine("IDK what to do with this dot");
                                        }
                                    }
                                    else
                                        foreach (XY d in moore2.dotsInside)
                                            battleyard2.Yard[d.x, d.y].ZoneIndex = battleyard2.LastZoneIndex;

                                        //END TURN
                                        endPlayer:

                                    if (enemyNewLocalDots - myNewLocalDots > result)
                                    {
                                        result = enemyNewLocalDots - myNewLocalDots;
                                        enemyNewDots = enemyNewLocalDots; myNewDots = myNewLocalDots;
                                    }
                                    
                                }
                            }

                            myDots += myNewDots;
                            enemyDots += enemyNewDots;
                        }
                        else
                        {
                            //Теперь перебираем возможные варианты хода игрока, ВЫЗЫВАЯ Просчет хода вглубину
                            int myNewDots = 0, enemyNewDots = 0, result = int.MinValue;


                            for (int i = battleyard.FromX; i <= battleyard.ToX; i++)
                            {
                                for (int k = battleyard.FromY; k <= battleyard.ToY; k++)
                                {
                                    Battleyard battleyard2 = new Battleyard(battleyard.closedZones, battleyard.dotsInside, battleyard.Yard, battleyard.FromX, battleyard.FromY, battleyard.ToX, battleyard.ToY);
                                    if (battleyard2.Yard[i, k].dotState != DotStates.EMPTY) continue;
                                    checkZonesFrom.x = (byte)i;
                                    checkZonesFrom.y = (byte)k;

                                    int myNewLocalDots = 0, enemyNewLocalDots = 0;

                                    //Ищем кол-во новых точек
                                    //Проверка на точку внутри зоны и ее доступность!!!!!
                                    battleyard2.Yard[checkZonesFrom.x, checkZonesFrom.y].dotState = DotStates.PLAYER2ORBOT;

                                    if (checkZonesFrom.x > battleyard2.ToX) battleyard2.ToX = checkZonesFrom.x + 2;
                                    if (checkZonesFrom.y > battleyard2.ToY) battleyard2.ToY = checkZonesFrom.y + 2;
                                    if (checkZonesFrom.x < battleyard2.FromX) battleyard2.FromX = checkZonesFrom.x - 2;
                                    if (checkZonesFrom.y < battleyard2.FromY) battleyard2.FromY = checkZonesFrom.y - 2;

                                    if (battleyard2.ToX > 9) { battleyard2.ToX = 9; }
                                    if (battleyard2.ToY > 9) { battleyard2.ToY = 9; }
                                    if (battleyard2.FromX < 0) { battleyard2.FromX = 0; }
                                    if (battleyard2.FromY < 0) { battleyard2.FromY = 0; }



                                    //Проверка на то, что точка внутри чужой старой зоны
                                    if (battleyard2.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex != 0)
                                    {
                                        if (!zoneOwner[battleyard2.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                                        {
                                            myNewLocalDots++;
                                            foreach (XY d in battleyard2.dotsInside[battleyard2.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                                            {
                                                if (battleyard2.Yard[d.x, d.y].dotState == DotStates.PLAYER && (battleyard2.Yard[d.x, d.y].dotState == DotStates.PLAYER2ORBOT)) continue;
                                                battleyard2.Yard[d.x, d.y].dotState = DotStates.CLOSED;
                                            }
                                            continue;
                                        }
                                        else if (zoneOwner[battleyard2.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                                        {
                                            continue;
                                        }

                                    }

                                    //Переделать на нечасики, а просто проверку окрудения и +1
                                    if (!(GetZoneClass.Сlockwise(checkZonesFrom, new XY((byte)(checkZonesFrom.x + 1), checkZonesFrom.y), DotStates.PLAYER, battleyard2.Yard).Length >= 2))
                                        goto endPlayer;


                                    GetZoneClass moore2 = new GetZoneClass(checkZonesFrom, new XY((byte)(checkZonesFrom.x + 1), checkZonesFrom.y), DotStates.PLAYER, DotStates.PLAYER2ORBOT, battleyard2.Yard);
                                    if (moore2.dotsInside == null || moore2.dotsInside.Count == 0)
                                        goto endPlayer;

                                    //Записываем Зону
                                    battleyard2.LastZoneIndex++;
                                    battleyard2.closedZones.Add(moore2.closedZones[0].ToArray()); // ???
                                    battleyard2.dotsInside.Add(moore2.dotsInside.ToArray());
                                    zoneOwner.Add(true);

                                    //Обрабатываем зону
                                    if (moore2.enemyInside)
                                    {
                                        foreach (XY d in moore2.dotsInside)
                                        {
                                            battleyard2.Yard[d.x, d.y].ZoneIndex = battleyard2.LastZoneIndex;
                                            if (battleyard2.Yard[d.x, d.y].dotState == DotStates.PLAYER2ORBOT)
                                                enemyNewLocalDots++;
                                            else if (battleyard2.Yard[d.x, d.y].dotState == DotStates.PLAYER && battleyard2.Yard[d.x, d.y].ZoneIndex != 0)
                                                myNewLocalDots--;
                                            else if (battleyard2.Yard[d.x, d.y].dotState == DotStates.EMPTY)
                                                battleyard2.Yard[d.x, d.y].dotState = DotStates.CLOSED;
                                            else
                                                Console.WriteLine("IDK what to do with this dot");
                                        }
                                    }
                                    else
                                        foreach (XY d in moore2.dotsInside)
                                            battleyard2.Yard[d.x, d.y].ZoneIndex = battleyard2.LastZoneIndex;

                                        //END TURN
                                    endPlayer:

                                   

                                    for (int a = battleyard2.FromX; a <= battleyard2.ToX; a++)
                                    {
                                        for (int b = battleyard2.FromY; b <= battleyard2.ToY; b++)
                                        {
                                            if (battleyard2.Yard[a, b].dotState != DotStates.EMPTY) continue;
                                            var tupleMINMAX = MinMax(new XY((byte)a,(byte)b), new Battleyard(battleyard2.closedZones, battleyard2.dotsInside, battleyard2.Yard, battleyard2.FromX, battleyard2.FromY, battleyard2.ToX, battleyard2.ToY), deep-1);
                                            if (enemyNewLocalDots + tupleMINMAX.enemyDots - myNewLocalDots - tupleMINMAX.myDots > result)
                                            {
                                                result = enemyNewLocalDots + tupleMINMAX.enemyDots - myNewLocalDots - tupleMINMAX.myDots;
                                                enemyDots = enemyNewLocalDots + tupleMINMAX.enemyDots; myNewDots = myNewLocalDots + tupleMINMAX.myDots;
                                            }
                                        }
                                    }
                                }
                            }

                            myDots += myNewDots;
                            enemyDots += enemyDots;
                        }

                        return (myDots, enemyDots);
                    }

                    // Конец
                        MMend:
                    battle.Yard[bestMove.x, bestMove.y].dotState = DotStates.PLAYER2ORBOT;
                    XY checkZonesFrom = new XY();
                    checkZonesFrom.x = bestMove.x;
                    checkZonesFrom.y = bestMove.y;

                    if (checkZonesFrom.x > battle.ToX) battle.ToX = checkZonesFrom.x + 1;
                    if (checkZonesFrom.y > battle.ToY) battle.ToY = checkZonesFrom.y + 1;
                    if (checkZonesFrom.x < battle.FromX) battle.FromX = checkZonesFrom.x - 1;
                    if (checkZonesFrom.y < battle.FromY) battle.FromY = checkZonesFrom.y - 1;

                    if (battle.ToX > 9) { battle.ToX = 9; }
                    if (battle.ToY > 9) { battle.ToY = 9; }
                    if (battle.FromX < 0) { battle.FromX = 0; }
                    if (battle.FromY < 0) { battle.FromY = 0; }

                    //Проверка на то, что точка внутри чужой старой зоны
                    if (battle.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex != 0)
                    {
                        if (zoneOwner[battle.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                        {
                            battle.player1DotsInside++;
                            foreach (XY d in battle.dotsInside[battle.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                            {
                                if (battle.Yard[d.x, d.y].dotState == DotStates.PLAYER && (battle.Yard[d.x, d.y].dotState == DotStates.PLAYER2ORBOT)) continue;
                                battle.Yard[d.x, d.y].dotState = DotStates.CLOSED;
                            }
                            return;
                        }
                        else if (!zoneOwner[battle.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                        {
                            return;
                        }
                    }

                    //Переделать на нечасики, а просто проверку окрудения и +1
                    if (!(GetZoneClass.Сlockwise(checkZonesFrom, new XY((byte)(checkZonesFrom.x + 1), checkZonesFrom.y), DotStates.PLAYER2ORBOT, battle.Yard).Length >= 2))
                        goto end;


                    GetZoneClass moore = new GetZoneClass(checkZonesFrom, new XY((byte)(checkZonesFrom.x + 1), checkZonesFrom.y), DotStates.PLAYER2ORBOT, DotStates.PLAYER, battle.Yard);
                    if (moore.dotsInside == null || moore.dotsInside.Count == 0)
                        goto end;

                    //Записываем Зону
                    battle.LastZoneIndex++;
                    battle.closedZones.Add(moore.closedZones[0].ToArray()); // ???
                    battle.dotsInside.Add(moore.dotsInside.ToArray());
                    zoneOwner.Add(false);

                    //Обрабатываем зону
                    if (moore.enemyInside)
                    {
                        foreach (XY d in moore.dotsInside)
                        {
                            battle.Yard[d.x, d.y].ZoneIndex = battle.LastZoneIndex;
                            if (battle.Yard[d.x, d.y].dotState == DotStates.PLAYER)
                                battle.player2OrBotDotsInside++;
                            else if (battle.Yard[d.x, d.y].dotState == DotStates.PLAYER2ORBOT && battle.Yard[d.x, d.y].ZoneIndex != 0)
                                battle.player1DotsInside--;
                            else if (battle.Yard[d.x, d.y].dotState == DotStates.EMPTY)
                                battle.Yard[d.x, d.y].dotState = DotStates.CLOSED;
                            else
                                Console.WriteLine("IDK what to do with this dot");
                        }
                    }
                    else
                        foreach (XY d in moore.dotsInside)
                            battle.Yard[d.x, d.y].ZoneIndex = battle.LastZoneIndex;

                        //END TURN
                        end:
                    player = true;
                }
            }
            //AlgorythmMoore hui = new AlgorythmMoore(new XY(1,0), new XY(0, 0), battle.Yard[1, 0].dotState, battle.Yard);
            
            Console.ReadKey();
        }

        /*
        static void CheckDots(Battleyard battle)
        {
            Console.WriteLine("--- Игрок 2:");
            XY checkZonesFrom = battle.PutDot(DotStates.PLAYER2ORBOT);
            if (battle.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex != 0)
            {
                if (zoneOwner[battle.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                {
                    player1DotsInside++;
                    foreach (XY d in dotsInside[battle.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                    {
                        if (battle.Yard[d.x, d.y].dotState == DotStates.PLAYER && (battle.Yard[d.x, d.y].dotState == DotStates.PLAYER2ORBOT)) continue;
                        battle.Yard[d.x, d.y].dotState = DotStates.CLOSED;
                    }
                    return;
                }
                else if (!zoneOwner[battle.Yard[checkZonesFrom.x, checkZonesFrom.y].ZoneIndex - 1])
                {
                    return;
                }
            }

            //Переделать на нечасики, а просто проверку окрудения и +1
            if (!(GetZoneClass.Сlockwise(checkZonesFrom, new XY((byte)(checkZonesFrom.x + 1), checkZonesFrom.y), DotStates.PLAYER2ORBOT, battle.Yard).Length >= 2))
                goto end;


            GetZoneClass moore = new GetZoneClass(checkZonesFrom, new XY((byte)(checkZonesFrom.x + 1), checkZonesFrom.y), DotStates.PLAYER2ORBOT, DotStates.PLAYER, battle.Yard);
            if (moore.dotsInside == null || moore.dotsInside.Count == 0)
                goto end;

            //Записываем Зону
            LastZoneIndex++;
            closedZones.Add(moore.closedZones[0].ToArray()); // ???
            dotsInside.Add(moore.dotsInside.ToArray());
            zoneOwner.Add(false);

            //Обрабатываем зону
            if (moore.enemyInside)
            {
                foreach (XY d in moore.dotsInside)
                {
                    battle.Yard[d.x, d.y].ZoneIndex = LastZoneIndex;
                    if (battle.Yard[d.x, d.y].dotState == DotStates.PLAYER)
                        player2OrBotDotsInside++;
                    else if (battle.Yard[d.x, d.y].dotState == DotStates.PLAYER2ORBOT && battle.Yard[d.x, d.y].ZoneIndex != 0)
                        player1DotsInside--;
                    else if (battle.Yard[d.x, d.y].dotState == DotStates.EMPTY)
                        battle.Yard[d.x, d.y].dotState = DotStates.CLOSED;
                    else
                        Console.WriteLine("IDK what to do with this dot");
                }
            }
            else
                foreach (XY d in moore.dotsInside)
                    battle.Yard[d.x, d.y].ZoneIndex = LastZoneIndex;

                //END TURN
                end:
            player = true;
        }
        */
    }

    

    
}
