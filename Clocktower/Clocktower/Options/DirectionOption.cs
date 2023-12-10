using Clocktower.Game;

namespace Clocktower.Options
{
    internal class DirectionOption : IOption
    {
        public string Name => Direction == Direction.Clockwise ? "Clockwise" : "Counter-clockwise";

        public Direction Direction { get; private set; }

        public DirectionOption(Direction direction)
        {
            Direction = direction;
        }
    }
}
