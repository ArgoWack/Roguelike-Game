﻿using ASP_NET_WEEK3_Homework_Roguelike.Model;
using ASP_NET_WEEK3_Homework_Roguelike.Model.Events;

namespace ASP_NET_WEEK3_Homework_Roguelike.Services
{
    public class MapService: IMapService
    {
        private readonly EventService _eventService;
        public MapService(EventService eventService)
        {
            _eventService = eventService;
        }
        public void InitializeStartingRoom(Map map)
        {
            Room startingRoom = new Room(0, 0);

            startingRoom.Exits["north"] = null;
            startingRoom.Exits["south"] = null;
            startingRoom.Exits["east"] = null;
            startingRoom.Exits["west"] = null;

            AddDiscoveredRoom(map, startingRoom);
        }
        public void AddDiscoveredRoom(Map map, Room room)
        {
            map.DiscoveredRooms[(room.X, room.Y)] = room;
            room.IsExplored = true;

            // Logic for exits and room to discover
            foreach (var direction in room.Exits.Keys)
            {
                (int newX, int newY) = GetCoordinatesInDirection(room.X, room.Y, direction);
                if (!map.DiscoveredRooms.ContainsKey((newX, newY)))
                {
                    RoomToDiscover rtd = new RoomToDiscover(newX, newY, OppositeDirection(direction));
                    map.RoomsToDiscover.Add(rtd);
                    rtd.BlockedDirections.Add(OppositeDirection(direction));
                }
            }
            map.RoomsToDiscover.RemoveAll(r => r.X == room.X && r.Y == room.Y);
        }
        public Room GenerateRoom(Map map, int currentX, int currentY, string direction)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map), "Map cannot be null.");

            if (string.IsNullOrWhiteSpace(direction))
                throw new ArgumentException("Direction cannot be null or empty.", nameof(direction));

            try
            {
                (int newX, int newY) = GetCoordinatesInDirection(currentX, currentY, direction);
                Room newRoom = new Room(newX, newY);

                // Handles event generation here
                RandomEvent randomEvent = EventGenerator.GenerateEvent();
                newRoom.EventStatus = randomEvent != null ? randomEvent.GetType().Name : "none";
                AddDiscoveredRoom(map, newRoom);
                GenerateRandomExits(map, newRoom);
                Room currentRoom = GetDiscoveredRoom(map, currentX, currentY);
                if (currentRoom != null)
                {
                    currentRoom.Exits[direction] = newRoom;
                    newRoom.Exits[OppositeDirection(direction)] = currentRoom;
                }
                return newRoom;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to generate room.", ex);
            }
        }
        public void MovePlayer(Map map, ref int playerX, ref int playerY, string direction)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map), "Map cannot be null.");

            if (string.IsNullOrWhiteSpace(direction))
                throw new ArgumentException("Direction cannot be null or empty.", nameof(direction));

            var validDirections = new[] { "north", "south", "east", "west" };
            if (!validDirections.Contains(direction.ToLower()))
            {
                throw new ArgumentException($"Invalid direction: {direction}. Allowed values are: {string.Join(", ", validDirections)}.");
            }

            try
            {
                (int newX, int newY) = GetCoordinatesInDirection(playerX, playerY, direction);
                Room targetRoom = GetDiscoveredRoom(map, newX, newY);
                if (targetRoom == null)
                {
                    targetRoom = GenerateRoom(map, playerX, playerY, direction);
                }
                playerX = newX;
                playerY = newY;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to move player.", ex);
            }
        }
        public Room GetDiscoveredRoom(Map map, int x, int y)
        {
            return map.DiscoveredRooms.TryGetValue((x, y), out Room room) ? room : null;
        }
        private void GenerateRandomExits(Map map, Room room)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map), "Map cannot be null.");
            if (room == null)
                throw new ArgumentNullException(nameof(room), "Room cannot be null.");

            try
            {
                var directions = new[] { "north", "south", "east", "west" };
                var random = new Random();
                int numberOfExits = random.Next(2, 5);
                var availableDirections = directions.OrderBy(_ => random.Next()).Take(numberOfExits).ToList();
                foreach (var direction in availableDirections)
                {
                    if (!room.Exits.ContainsKey(direction))
                    {
                        (int newX, int newY) = GetCoordinatesInDirection(room.X, room.Y, direction);
                        if (!map.DiscoveredRooms.ContainsKey((newX, newY)))
                        {
                            room.Exits[direction] = null;
                            RoomToDiscover rtd = new RoomToDiscover(newX, newY, OppositeDirection(direction));
                            map.RoomsToDiscover.Add(rtd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to generate random exits for room.", ex);
            }
        }
        private (int, int) GetCoordinatesInDirection(int x, int y, string direction)
        {
            return direction switch
            {
                "north" => (x, y + 1),
                "south" => (x, y - 1),
                "east" => (x + 1, y),
                "west" => (x - 1, y),
                _ => (x, y)
            };
        }
        private string OppositeDirection(string direction)
        {
            return direction switch
            {
                "north" => "south",
                "south" => "north",
                "east" => "west",
                "west" => "east",
                _ => ""
            };
        }
    }
}