using ASP_NET_WEEK3_Homework_Roguelike.Model;

namespace ASP_NET_WEEK3_Homework_Roguelike.Services
{
    public interface IMapService
    {
        void InitializeStartingRoom(Map map);
        void AddDiscoveredRoom(Map map, Room room);
        Room GenerateRoom(Map map, Point currentCoordinates, Direction direction);
        void MovePlayer(Map map, ref Point playerCoordinates, Direction direction);
        Room GetDiscoveredRoom(Map map, Point coordinates);
    }
}