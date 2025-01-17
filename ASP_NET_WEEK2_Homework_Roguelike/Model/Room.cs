namespace ASP_NET_WEEK3_Homework_Roguelike.Model
{
    public class Room
    {
        public Point Coordinates { get; set; }
        public string EventStatus { get; set; }
        public Dictionary<Direction, Room> Exits { get; set; }
        public bool IsExplored { get; set; }

        public Room(int x, int y): this(new Point(x, y))
        {
        }
        public Room(Point coordinates)
        {
            Coordinates = coordinates ?? throw new ArgumentNullException(nameof(coordinates));
            Exits = new Dictionary<Direction, Room>();
            IsExplored = false;
            EventStatus = "none";
        }
        // Default constructor for serialization
        public Room()
        {
            Coordinates = new Point(0, 0); // default to (0,0)
            Exits = new Dictionary<Direction, Room>();
            IsExplored = false;
            EventStatus = "none";
        }
    }
}