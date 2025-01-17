using ASP_NET_WEEK3_Homework_Roguelike.View;
using ASP_NET_WEEK3_Homework_Roguelike.Model;
using ASP_NET_WEEK3_Homework_Roguelike.Model.Events;
using ASP_NET_WEEK3_Homework_Roguelike.Services;

namespace ASP_NET_WEEK3_Homework_Roguelike.Controller
{
    public class PlayerCharacterController : IPlayerCharacterController
    {
        private readonly PlayerCharacter _playerCharacter;
        private readonly PlayerCharacterView _view;
        private readonly MapService _mapService;
        private readonly Map _map;
        public PlayerCharacterView View => _view;
        public PlayerCharacter PlayerCharacter => _playerCharacter;
        public PlayerCharacterController(PlayerCharacter playerCharacter, Map map, MapService mapService)
        {
            _playerCharacter = playerCharacter ?? throw new ArgumentNullException(nameof(playerCharacter));
            _map = map ?? throw new ArgumentNullException(nameof(map));
            _mapService = mapService ?? throw new ArgumentNullException(nameof(mapService));
            _view = new PlayerCharacterView();
            _playerCharacter.CurrentMap = map;
        }
        public void ShowCharacterStats()
        {
            _playerCharacter.UpdateStats();
            _view.ShowCharacterStats(_playerCharacter);
        }
        public void ShowMap()
        {
            _view.ShowMap(_map, _playerCharacter);
        }
        public void MovePlayer(string directionInput)
        {
            if (string.IsNullOrWhiteSpace(directionInput))
            {
                _view.ShowError("Direction cannot be empty.");
                return;
            }
            if (!Enum.TryParse<Direction>(directionInput, true, out Direction direction))
            {
                _view.ShowError("Invalid direction input.");
                return;
            }
            try
            {
                var currentCoordinates = _playerCharacter.Coordinates;
                var currentRoom = _mapService.GetDiscoveredRoom(_map, currentCoordinates);

                if (currentRoom != null && currentRoom.Exits.ContainsKey(direction))
                {
                    _mapService.MovePlayer(_map, ref currentCoordinates, direction);
                    _playerCharacter.Coordinates = currentCoordinates;
                    _view.ShowPlayerMovement(direction.ToString(), currentCoordinates.X, currentCoordinates.Y);

                    var newRoom = _mapService.GetDiscoveredRoom(_map, currentCoordinates);
                    if (newRoom?.EventStatus != "none")
                    {
                        var roomEvent = EventGenerator.GenerateEvent(newRoom.EventStatus);
                        roomEvent?.Execute(_playerCharacter, newRoom, this);
                    }
                }
                else
                {
                    _view.RelayMessage("Cannot move in that direction. No valid room exists.");
                }
            }
            catch (Exception ex)
            {
                _view.ShowError(ex.Message);
            }
        }
        public void EquipItem(int itemId)
        {
            try
            {
                _playerCharacter.EquipItem(itemId);
                var item = _playerCharacter.Inventory.FirstOrDefault(i => i.ID == itemId);
                if (item != null)
                {
                    _view.ShowEquipItemSuccess(item.Name);
                }
            }
            catch (Exception ex)
            {
                _view.ShowError(ex.Message);
            }
        }
        public void DiscardItem(int itemId)
        {
            try
            {
                var item = _playerCharacter.Inventory.FirstOrDefault(i => i.ID == itemId);
                if (item != null)
                {
                    _playerCharacter.DiscardItem(itemId);
                    _view.ShowDiscardItemSuccess(item.Name);
                }
            }
            catch (Exception ex)
            {
                _view.ShowError(ex.Message);
            }
        }
        public void ShowInventory()
        {
            _view.DisplayInventory(_playerCharacter);
        }
    }
}