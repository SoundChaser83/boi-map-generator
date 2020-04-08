using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binding_of_Isaac_console
{
    abstract class Room
    {
        public int[] Coordinates { get; set; }
        public bool HasBoss { get; set; } = false;
        public bool IsSecret { get; set; } = false;
        public bool IsSuperSecret { get; set; } = false;
        public bool IsItemRoom { get; set; } = false;
        public bool IsNormRoom { get; set; } = false;
    }

    class NormRoom : Room
    {
        public NormRoom(int[] coordinates)
        {
            Coordinates = coordinates;
            IsNormRoom = true;
        }
    }

    class BossRoom : Room
    {
        public BossRoom(int[] coordinates)
        {
            Coordinates = coordinates;
            HasBoss = true;
        }
    }

    class SecretRoom : Room
    {
        public SecretRoom(int[] coordinates)
        {
            Coordinates = coordinates;
            IsSecret = true;
        }
    }

    class SuperSecretRoom : Room
    {
        public SuperSecretRoom(int[] coordinates)
        {
            Coordinates = coordinates;
            IsSuperSecret = true;
        }
    }

    class ItemRoom : Room
    {
        public ItemRoom(int[] coordinates)
        {
            Coordinates = coordinates;
            IsItemRoom = true;
        }
    }
}