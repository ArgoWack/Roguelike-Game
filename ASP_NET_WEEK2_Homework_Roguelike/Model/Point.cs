namespace ASP_NET_WEEK3_Homework_Roguelike.Model
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Point Move(Direction direction)
        {
            return direction switch
            {
                Direction.North => new Point(X, Y + 1),
                Direction.South => new Point(X, Y - 1),
                Direction.East => new Point(X + 1, Y),
                Direction.West => new Point(X - 1, Y),
                _ => throw new InvalidOperationException($"Invalid direction: {direction}")
            };
        }
        public override bool Equals(object? obj)
        {
            if (obj is Point other)
                return X == other.X && Y == other.Y;
            return false;
        }
        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}