namespace ASP_NET_WEEK3_Homework_Roguelike.Model
{
    public class RoomToDiscover
    {
        public Point Coordinates { get; set; }
        public Direction EnteringDirection { get; set; }
        public HashSet<Direction> BlockedDirections { get; set; }
        public RoomToDiscover(Point coordinates, Direction enteringDirection)
        {
            Coordinates = coordinates;
            EnteringDirection = enteringDirection;
            BlockedDirections = new HashSet<Direction>();
        }
        // For serialization
        public RoomToDiscover() { }
    }
}