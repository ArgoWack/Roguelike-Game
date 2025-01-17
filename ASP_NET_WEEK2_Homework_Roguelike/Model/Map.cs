namespace ASP_NET_WEEK3_Homework_Roguelike.Model
{
    public class Map
    {
        // Stores discovered rooms with their coordinates as keys
        public Dictionary<Point, Room> DiscoveredRooms { get; set; }

        // Stores rooms that are yet to be discovered
        public List<RoomToDiscover> RoomsToDiscover { get; set; }
        public Map()
        {
            DiscoveredRooms = new Dictionary<Point, Room>();
            RoomsToDiscover = new List<RoomToDiscover>();
        }
    }
}