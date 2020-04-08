using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binding_of_Isaac_console
{
    class Map
    {
        public List<Room> Rooms { get; private set; }
        public int YMax { get; private set; }
        public int XMax { get; private set; }

        public Map() { }
        public Map(List<Room> rooms)
        {
            Rooms = rooms;
        }

        /// <summary>
        /// Returns a randomized new map with numRooms amount of rooms
        /// (numRooms must be greater than or equal to 15)
        /// </summary>
        /// <param name="numRooms"></param>
        /// <returns></returns>
        public static Map GenerateNewMap(int numRooms)
        {
            if (numRooms < 10)
            {
                throw new Exception("Please generate a map with at least 10 rooms.");
            }
            else if (numRooms > 50)
            {
                throw new Exception("Please generate a map with no more than 50 rooms.");
            }
            else
            {                
                List<Room> rooms;
                Map newMap;
                int[] tempLocation = new int[2];

                do
                {
                    rooms = new List<Room>();
                    rooms.Add(new BossRoom(NewLocation(rooms)));

                    for (int i = 0; i < numRooms - 4; i++)
                    {
                        do
                        {
                            tempLocation = NewLocation(rooms);
                        } while (MakesABox(rooms, tempLocation));
                        
                        rooms.Add(new NormRoom(NewLocation(rooms)));
                    }

                    rooms.Add(new ItemRoom(NewLocation(rooms)));                    
                    rooms.Add(new SuperSecretRoom(NewLocation(rooms)));
                    rooms.Add(new SecretRoom(NewLocation(rooms)));

                    int minY = GetMinY(rooms), minX = GetMinX(rooms);

                    foreach (Room r in rooms)
                    {
                        r.Coordinates[0] -= minY;
                        r.Coordinates[1] -= minX;
                    }

                    newMap = new Map(rooms);
                } while (!IsGoodMap(newMap));
                
                newMap.YMax = GetMaxY(rooms);
                newMap.XMax = GetMaxX(rooms);
                return newMap;
            }            
        }

        /// <summary>
        /// Prints an ASCII representation of map to console (N = normal room, I = item room, 
        /// B = boss room, S = secret room, U = super secret room)
        /// </summary>
        /// <param name="map"></param>
        public static void ShowMap(Map map)
        {
            for (int i = 0; i <= map.YMax; i++)
            {
                for (int j = 0; j <= map.XMax; j++)
                {
                    if (!map.Rooms.Exists(e => e.Coordinates[0] == i && e.Coordinates[1] == j))
                    {
                        Console.Write(" ");
                    }
                    else if (map.Rooms.Exists(e => e.Coordinates[0] == i && e.Coordinates[1] == j && e.HasBoss))
                    {
                        Console.Write("B");
                    }
                    else if (map.Rooms.Exists(e => e.Coordinates[0] == i && e.Coordinates[1] == j && e.IsItemRoom))
                    {
                        Console.Write("I");
                    }
                    else if (map.Rooms.Exists(e => e.Coordinates[0] == i && e.Coordinates[1] == j && e.IsSecret))
                    {
                        Console.Write("S");
                    }
                    else if (map.Rooms.Exists(e => e.Coordinates[0] == i && e.Coordinates[1] == j && e.IsSuperSecret))
                    {
                        Console.Write("U");
                    }
                    else
                    {
                        Console.Write("N");
                    }
                }

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Returns a valid set of coordinates adjacent to a random existing room
        /// (returns coordinates { 0, 0 } when there are no existing rooms)
        /// </summary>
        /// <param name="existingRooms"></param>
        /// <returns></returns>
        private static int[] NewLocation(List<Room> existingRooms)
        {
            Random rng = new Random();
            int directionalRng, roomRng;
            int[] newCoord = new int[2] { 0, 0 };

            if (existingRooms.Count > 0)
            {
                do
                {
                    directionalRng = rng.Next(4);
                    roomRng = rng.Next(existingRooms.Count);
                    newCoord[0] = existingRooms[roomRng].Coordinates[0];
                    newCoord[1] = existingRooms[roomRng].Coordinates[1];

                    if (directionalRng == 0)
                    {
                        newCoord[0] += 1;
                    }
                    else if (directionalRng == 1)
                    {
                        newCoord[0] -= 1;
                    }
                    else if (directionalRng == 2)
                    {
                        newCoord[1] += 1;
                    }
                    else
                    {
                        newCoord[1] -= 1;
                    }
                } while (!IsValid(existingRooms, newCoord));
            }

            return newCoord;
        }

        /// <summary>
        /// Returns a boolean indicating whether testCoord is a 
        /// valid location for a new room to be spawned
        /// </summary>
        /// <param name="rooms"></param>
        /// <param name="testCoord"></param>
        /// <returns></returns>
        private static bool IsValid(List<Room> rooms, int[] testCoord)
        {
            foreach (Room r in rooms)
            {
                //Tests to make sure testCoord doesn't have the same coordinates as an existing room
                if (testCoord[0] == r.Coordinates[0] && testCoord[1] == r.Coordinates[1])
                {
                    return false;
                }
                //Tests to make sure testCoord isn't adjacent to a boss room that 
                //already has at least one other room adjacent to it
                else if (IsAdjacentTo(r.Coordinates, testCoord) && r.HasBoss 
                    && AdjacentTo(rooms, r.Coordinates).Count >= 1)
                {
                    return false;
                }
                //Tests to make sure testCoord isn't adjacent to a super secret room that 
                //already has at least one other room adjacent to it
                else if (IsAdjacentTo(r.Coordinates, testCoord) && r.IsSuperSecret 
                    && AdjacentTo(rooms, r.Coordinates).Count >= 1)
                {
                    return false;
                }
                //Tests to make sure testCoord isn't adjacent to an item room when
                //the item room already has at least two rooms, including a secret room,
                //adjacent to it.
                else if (IsAdjacentTo(r.Coordinates, testCoord) && r.IsItemRoom 
                    && AdjacentTo(rooms, r.Coordinates).Exists(e => e.IsSecret == true) 
                    && AdjacentTo(rooms, r.Coordinates).Count >= 2)
                {
                    return false;
                }
                //Tests to make sure there are no 2x2 orientations of normal rooms
                /*else if (MakesABox(rooms, testCoord))
                {
                    return false;
                }*/
            }

            return true;
        }

        private static bool IsGoodMap(Map map)
        {
            //Tests to make sure the super secret room only spawns adjacent to at most one room
            if (map.Rooms.Exists(e => e.IsSuperSecret && 
                AdjacentTo(map.Rooms, e.Coordinates).Count > 1))
            {
                return false;
            }
            //Tests to make sure the super secret room cannot be adjacent to the 
            //item room or the regular secret room
            else if (map.Rooms.Exists(e => e.IsSuperSecret && 
                AdjacentTo(map.Rooms, e.Coordinates).Exists(f => f.IsItemRoom || f.IsSecret)))
            {
                return false;
            }
            //Tests to make sure the item room cannot spawn adjacent to more than one normal room
            else if (map.Rooms.Exists(e => e.IsItemRoom && 
                AdjacentTo(map.Rooms, e.Coordinates).Where(f => f.IsNormRoom).Count() > 1))
            {
                return false;
            }
            //Tests to make sure the secret room must spawn adjacent to at least two rooms
            else if (map.Rooms.Exists(e => e.IsSecret && AdjacentTo(map.Rooms, e.Coordinates).Count() <= 1))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a boolean indicating whether coord1 is adjacent to coord2
        /// </summary>
        /// <param name="coord1"></param>
        /// <param name="coord2"></param>
        /// <returns></returns>
        private static bool IsAdjacentTo(int[] coord1, int[] coord2)
        {
            if (coord1[0] == coord2[0] && coord1[1] == coord2[1] + 1)
            {
                return true;
            }
            else if (coord1[0] == coord2[0] && coord1[1] == coord2[1] - 1)
            {
                return true;
            }
            else if (coord1[0] == coord2[0] + 1 && coord1[1] == coord2[1])
            {
                return true;
            }
            else if (coord1[0] == coord2[0] - 1 && coord1[1] == coord2[1])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a list of all rooms adjacent to coord
        /// </summary>
        /// <param name="rooms"></param>
        /// <param name="coord"></param>
        /// <returns></returns>
        private static List<Room> AdjacentTo(List<Room> rooms, int[] coord)
        {
            List<Room> adjRooms = new List<Room>();

            foreach (Room r in rooms)
            {
                if (IsAdjacentTo(r.Coordinates, coord))
                {
                    adjRooms.Add(r);
                }
            }

            return adjRooms;
        }

        /// <summary>
        /// Returns the minimum y-value of all coordinate pairs in rooms
        /// </summary>
        /// <param name="rooms"></param>
        /// <returns></returns>
        private static int GetMinY(List<Room> rooms)
        {
            List<int> yValues = new List<int>();

            foreach (Room r in rooms)
            {
                yValues.Add(r.Coordinates[0]);
            }

            return yValues.Min();
        }

        /// <summary>
        /// Returns the minimum x-value of all coordinate pairs in rooms
        /// </summary>
        /// <param name="rooms"></param>
        /// <returns></returns>
        private static int GetMinX(List<Room> rooms)
        {
            List<int> xValues = new List<int>();

            foreach (Room r in rooms)
            {
                xValues.Add(r.Coordinates[1]);
            }

            return xValues.Min();
        }

        /// <summary>
        /// Returns the maximum y-value of all coordinate pairs in rooms
        /// </summary>
        /// <param name="rooms"></param>
        /// <returns></returns>
        private static int GetMaxY(List<Room> rooms)
        {
            List<int> yValues = new List<int>();

            foreach (Room r in rooms)
            {
                yValues.Add(r.Coordinates[0]);
            }

            return yValues.Max();
        }

        /// <summary>
        /// Returns the maximum x-value of all coordinate pairs in rooms
        /// </summary>
        /// <param name="rooms"></param>
        /// <returns></returns>
        private static int GetMaxX(List<Room> rooms)
        {
            List<int> xValues = new List<int>();

            foreach (Room r in rooms)
            {
                xValues.Add(r.Coordinates[1]);
            }

            return xValues.Max();
        }

        /// <summary>
        /// Returns a boolean indicating whether a room placed at coords will 
        /// form a 2x2 box with other normal rooms
        /// </summary>
        /// <param name="allRooms"></param>
        /// <param name="coords"></param>
        /// <returns></returns>
        private static bool MakesABox(List<Room> allRooms, int[] coords)
        {
            int[] roomS = new int[2] { coords[0] + 1, coords[1] };
            int[] roomE = new int[2] { coords[0], coords[1] + 1};
            int[] roomSE = new int[2] { coords[0] + 1, coords[1] + 1 };
            int[] roomN = new int[2] { coords[0] - 1, coords[1] };
            int[] roomW = new int[2] { coords[0], coords[1] - 1 };
            int[] roomNW = new int[2] { coords[0] - 1, coords[1] - 1 };
            int[] roomNE = new int[2] { coords[0] - 1, coords[1] + 1 };
            int[] roomSW = new int[2] { coords[0] + 1, coords[1] - 1 };

            if (allRooms.Exists(e => e.Coordinates[0] == roomS[0] && e.Coordinates[1] == roomS[1] && e.IsNormRoom)
                && allRooms.Exists(e => e.Coordinates[0] == roomE[0] && e.Coordinates[1] == roomE[1] && e.IsNormRoom)
                && allRooms.Exists(e => e.Coordinates[0] == roomSE[0] && e.Coordinates[1] == roomSE[1] && e.IsNormRoom))
            {
                return true;
            }
            else if (allRooms.Exists(e => e.Coordinates[0] == roomS[0] && e.Coordinates[1] == roomS[1] && e.IsNormRoom)
                && allRooms.Exists(e => e.Coordinates[0] == roomW[0] && e.Coordinates[1] == roomW[1] && e.IsNormRoom)
                && allRooms.Exists(e => e.Coordinates[0] == roomSW[0] && e.Coordinates[1] == roomSW[1] && e.IsNormRoom))
            {
                return true;
            }
            else if (allRooms.Exists(e => e.Coordinates[0] == roomN[0] && e.Coordinates[1] == roomN[1] && e.IsNormRoom)
                && allRooms.Exists(e => e.Coordinates[0] == roomW[0] && e.Coordinates[1] == roomW[1] && e.IsNormRoom)
                && allRooms.Exists(e => e.Coordinates[0] == roomNW[0] && e.Coordinates[1] == roomNW[1] && e.IsNormRoom))
            {
                return true;
            }
            else if (allRooms.Exists(e => e.Coordinates[0] == roomN[0] && e.Coordinates[1] == roomN[1] && e.IsNormRoom)
                && allRooms.Exists(e => e.Coordinates[0] == roomE[0] && e.Coordinates[1] == roomE[1] && e.IsNormRoom)
                && allRooms.Exists(e => e.Coordinates[0] == roomNE[0] && e.Coordinates[1] == roomNE[1] && e.IsNormRoom))
            {
                return true;
            }

            return false;
        }
    }
}