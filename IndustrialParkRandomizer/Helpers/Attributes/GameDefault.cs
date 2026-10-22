using System;

namespace IndustrialPark.Randomizer
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal class GameAttribute : Attribute
    {
        public int Game { get; }
        public object Value { get; }

        internal GameAttribute(int game, object value = null)
        {
            Game = game;
            Value = value;
        }
    }

    internal class AllAttribute : GameAttribute
    {
        internal AllAttribute(object value = null) : base(-1, value)
        {
        }
    }

    internal class BfbbAttribute : GameAttribute
    {
        internal BfbbAttribute(object value = null) : base(0, value)
        {
        }
    }

    internal class ScoobyAttribute : GameAttribute
    {
        internal ScoobyAttribute(object value = null) : base(1, value)
        {
        }
    }

    internal class MovieAttribute : GameAttribute
    {
        internal MovieAttribute(object value = null) : base(2, value)
        {
        }
    }
}