using ASP_NET_WEEK3_Homework_Roguelike.Model;
using ASP_NET_WEEK3_Homework_Roguelike.Model.Events;

namespace ASP_NET_WEEK3_Homework_Roguelike.Services
{
    public class MapService : IMapService
    {
        private readonly EventService _eventService;
        public MapService(EventService eventService)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }
        public void InitializeStartingRoom(Map map)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));

            var startingRoom = new Room(new Point(0, 0));
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                startingRoom.Exits[direction] = null;
            }
            AddDiscoveredRoom(map, startingRoom);
        }
        public void AddDiscoveredRoom(Map map, Room room)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));
            if (room == null) throw new ArgumentNullException(nameof(room));

            map.DiscoveredRooms[room.Coordinates] = room;
            room.IsExplored = true;

            foreach (var direction in room.Exits.Keys)
            {
                var newCoordinates = room.Coordinates.Move(direction);
                if (!map.DiscoveredRooms.ContainsKey(newCoordinates))
                {
                    var roomToDiscover = new RoomToDiscover(newCoordinates, OppositeDirection(direction));
                    map.RoomsToDiscover.Add(roomToDiscover);
                    roomToDiscover.BlockedDirections.Add(OppositeDirection(direction));
                }
            }
            map.RoomsToDiscover.RemoveAll(r => r.Coordinates.Equals(room.Coordinates));
        }
        public Room GenerateRoom(Map map, Point currentCoordinates, Direction direction)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));
            if (currentCoordinates == null) throw new ArgumentNullException(nameof(currentCoordinates));

            var newCoordinates = currentCoordinates.Move(direction);
            var newRoom = new Room(newCoordinates);

            var randomEvent = EventGenerator.GenerateEvent();
            newRoom.EventStatus = randomEvent != null ? randomEvent.GetType().Name : "none";

            AddDiscoveredRoom(map, newRoom);
            GenerateRandomExits(map, newRoom);

            if (map.DiscoveredRooms.TryGetValue(currentCoordinates, out var currentRoom))
            {
                currentRoom.Exits[direction] = newRoom;
                newRoom.Exits[OppositeDirection(direction)] = currentRoom;
            }
            return newRoom;
        }
        public void MovePlayer(Map map, ref Point playerCoordinates, Direction direction)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));
            if (playerCoordinates == null) throw new ArgumentNullException(nameof(playerCoordinates));

            var newCoordinates = playerCoordinates.Move(direction);
            if (!map.DiscoveredRooms.ContainsKey(newCoordinates))
            {
                GenerateRoom(map, playerCoordinates, direction);
            }

            playerCoordinates = newCoordinates;
        }
        public Room GetDiscoveredRoom(Map map, Point coordinates)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));
            if (coordinates == null) throw new ArgumentNullException(nameof(coordinates));

            return map.DiscoveredRooms.TryGetValue(coordinates, out var room) ? room : null;
        }
        private void GenerateRandomExits(Map map, Room room)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));
            if (room == null) throw new ArgumentNullException(nameof(room));
            var directions = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToList();
            var random = new Random();
            int numberOfExits = random.Next(2, 5);
            var availableDirections = directions.OrderBy(_ => random.Next()).Take(numberOfExits).ToList();
            foreach (var direction in availableDirections)
            {
                if (!room.Exits.ContainsKey(direction))
                {
                    var newCoordinates = room.Coordinates.Move(direction);
                    if (!map.DiscoveredRooms.ContainsKey(newCoordinates))
                    {
                        room.Exits[direction] = null;
                        var roomToDiscover = new RoomToDiscover(newCoordinates, OppositeDirection(direction));
                        map.RoomsToDiscover.Add(roomToDiscover);
                    }
                }
            }
        }
        private Direction OppositeDirection(Direction direction)
        {
            return direction switch
            {
                Direction.North => Direction.South,
                Direction.South => Direction.North,
                Direction.East => Direction.West,
                Direction.West => Direction.East,
                _ => throw new InvalidOperationException($"Invalid direction: {direction}")
            };
        }
    }
}