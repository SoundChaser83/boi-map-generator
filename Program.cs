using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binding_of_Isaac_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Map map = new Map();
            
            while (true)
            {
                Console.ReadLine();
                map = Map.GenerateNewMap(50);

                foreach (Room r in map.Rooms)
                {
                    Console.Write($"{r.Coordinates[0]}, {r.Coordinates[1]}");

                    if (r.HasBoss)
                    {
                        Console.Write("    Boss room (B)");
                    }
                    else if (r.IsItemRoom)
                    {
                        Console.Write("    Item room (I)");
                    }
                    else if (r.IsSecret)
                    {
                        Console.Write("    Secret room (S)");
                    }
                    else if (r.IsSuperSecret)
                    {
                        Console.Write("    Super secret room (U)");
                    }

                    Console.WriteLine();
                }

                Console.WriteLine();
                Map.ShowMap(map);
            }
        }
    }
}
