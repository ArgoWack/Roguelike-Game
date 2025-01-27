﻿using ASP_NET_WEEK3_Homework_Roguelike.Controller;
using ASP_NET_WEEK3_Homework_Roguelike.Model;
using ASP_NET_WEEK3_Homework_Roguelike.View;
using static System.Console;
using ASP_NET_WEEK3_Homework_Roguelike.Model.Events;
using System.Text.Json;
using ASP_NET_WEEK3_Homework_Roguelike.Model.Items;

namespace ASP_NET_WEEK3_Homework_Roguelike.Services
{
    public class GameService
    {
        private PlayerCharacter _playerCharacter;
        private Map _map;
        private PlayerCharacterController _playerController;
        private EventService _eventService;
        private CharacterInteractionService _characterInteractionService;
        private EventController _eventController;
        private MapService _mapService;
        private bool _inGame;
        private MenuActionService _menuActionService;
        private GameView _gameView;
        public GameService()
        {
            _menuActionService = new MenuActionService();
            InitializeMenuActions(_menuActionService);
            _gameView = new GameView();
        }
        public void StartGame()
        {
            try
            {
                // Step 1: Initializes essential services
                var statsService = new CharacterStatsService();
                var inventoryService = new InventoryService();
                var playerCharacterView = new PlayerCharacterView();
                _gameView = new GameView();

                // Step 2: Initializes EventService (requires PlayerCharacterController, so deferred)
                _eventService = new EventService(null, _gameView); // Temporarily pass null for the controller

                // Step 3: Initializes PlayerCharacter with required dependencies
                _playerCharacter = new PlayerCharacter(statsService, inventoryService, _eventService, playerCharacterView);

                // Step 4: Initializes map and MapService
                _map = new Map();
                _mapService = new MapService(_eventService);

                // Step 5: Initializes PlayerCharacterController 
                _playerController = new PlayerCharacterController(_playerCharacter, _map, _mapService);

                // Step 6: Links EventService to PlayerCharacterController
                _eventService.SetPlayerController(_playerController);

                // Step 7: Initializes additional services and controllers
                _characterInteractionService = new CharacterInteractionService(_eventService);
                _eventController = new EventController(_playerController, _eventService);

                // Step 8: Initialize EventGenerator
                EventGenerator.Initialize(_eventService, _characterInteractionService, _gameView, playerCharacterView);

                // Step 9: Starts the game by showing the welcome message and main menu
                _gameView.ShowWelcomeMessage();
                ShowMainMenu();
            }
            catch (Exception ex)
            {
                _gameView.ShowError($"An error occurred during game initialization: {ex.Message}");
            }
        }
        private void ShowMainMenu()
        {
            ConsoleKeyInfo operation;
            do
            {
                operation = _gameView.DisplayMenuAndGetChoice<int>("Main", "What would you like to do?", _menuActionService);

                switch (operation.KeyChar)
                {
                    case '1':
                        StartNewGame();
                        break;
                    case '2':
                        ContinueGame();
                        break;
                    case '3':
                        _gameView.ShowDescription();
                        break;
                    case '4':
                        QuitGame();
                        break;
                    default:
                        _gameView.ShowError("Wrong input");
                        break;
                }
                if (_inGame)
                {
                    RunInGameLoop();
                }
            } while (operation.KeyChar != '4');
        }
        public void RunInGameLoop()
        {
            ConsoleKeyInfo operation;
            do
            {
                operation = _gameView.DisplayMenuAndGetChoice<char>("InGameMenu", "\nWhat would you like to do: a/w/s/d/e/q/m/p? \n", _menuActionService);
                switch (operation.KeyChar)
                {
                    case 'a':
                        MovePlayer("west");
                        break;
                    case 'w':
                        MovePlayer("north");
                        break;
                    case 's':
                        MovePlayer("south");
                        break;
                    case 'd':
                        MovePlayer("east");
                        break;
                    case 'm':
                        _playerController.ShowMap();
                        break;
                    case 'p':
                        _playerController.ShowCharacterStats();
                        break;
                    case 'e':
                        HandleInventory();
                        break;
                    case 'q':
                        SaveGame();
                        _inGame = false;
                        break;
                    default:
                        _gameView.ShowError("Wrong input");
                        break;
                }
            } while (_inGame);
        }
        private void StartNewGame()
        {
            _playerCharacter.Name = _gameView.PromptForCharacterName();
            ReadKey();
            _mapService.InitializeStartingRoom(_map);
            _playerCharacter.Coordinates = new Point(0, 0);
            SaveGame();
            _inGame = true;
        }
        public void ContinueGame()
        {
            try
            {
                var saveFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*_savefile.json");
                if (saveFiles.Length == 0)
                {
                    _gameView.ShowError("No saved games found.");
                    return;
                }
                int selectedIndex = _gameView.PromptForSaveFileSelection(saveFiles);
                if (selectedIndex > 0 && selectedIndex <= saveFiles.Length)
                {
                    var selectedFile = saveFiles[selectedIndex - 1];
                    var characterName = Path.GetFileNameWithoutExtension(selectedFile).Replace("_savefile", "");
                    try
                    {
                        var gameState = LoadGame(characterName, _gameView);
                        if (gameState == null)
                        {
                            _gameView.ShowError("Failed to load game state.");
                            return;
                        }
                        _playerCharacter = gameState.PlayerCharacter;
                        _map = gameState.Map;

                        // Initializes services for the loaded PlayerCharacter
                        _playerCharacter.InitializeServices(
                            new CharacterStatsService(),
                            new InventoryService(),
                            _eventService,
                            new PlayerCharacterView()
                        );

                        // Recalculate stats after loading
                        _playerCharacter.UpdateStats();

                        _playerController = new PlayerCharacterController(_playerCharacter, _map, _mapService);
                        _inGame = true;
                    }
                    catch (Exception ex)
                    {
                        _gameView.ShowError($"Error loading save: {ex.Message}");
                    }
                }
                else
                {
                    _gameView.ShowError("Invalid selection. Returning to main menu.");
                }
            }
            catch (Exception ex)
            {
                _gameView.ShowError($"An error occurred during loading of game: {ex.Message}");
            }
        }
        private void QuitGame()
        {
            _gameView.ShowEndGameMessage();
            Environment.Exit(0);
        }
        private void MovePlayer(string direction)
        {
            try
            {
                _playerController.MovePlayer(direction);
                var currentRoom = _mapService.GetDiscoveredRoom(_map, _playerCharacter.Coordinates);
                if (currentRoom != null)
                {
                    // skips the "no new events" message if the event status was just handled
                    if (currentRoom.EventStatus == "handled")
                    {
                        return;
                    }

                    if (currentRoom.EventStatus != "handled")
                    {
                        _eventController.ExecuteEvent(currentRoom);
                    }
                    else
                    {
                        _gameView.ShowMessage("This room has no new events.");
                    }
                }
                else
                {
                    _gameView.ShowMessage("Cannot move in that direction. No valid room exists.");
                }
            }
            catch (Exception ex)
            {
                _gameView.ShowError($"An error occurred during attempt of movement: {ex.Message}");
            }
        }
        private void HandleInventory()
        {
            try
            {
                char choice;
            do
            {
                _playerController.ShowInventory();
                choice = _gameView.PromptForInventoryChoice();

                if (choice == 'e')
                {
                    var id = _gameView.PromptForItemId("use");
                    if (id.HasValue)
                    {
                        var item = _playerController.PlayerCharacter.Inventory.FirstOrDefault(i => i.ID == id.Value);
                        if (item is HealthPotion)
                        {
                            _playerController.PlayerCharacter.HealByPotion();
                        }
                        else
                        {
                            _playerController.EquipItem(id.Value);
                        }
                    }
                }
                else if (choice == 'd')
                {
                    var id = _gameView.PromptForItemId("discard");
                    if (id.HasValue)
                    {
                        _playerController.DiscardItem(id.Value);
                    }
                }
            } 
                while (choice != 'l');
            }
            catch (Exception ex)
            {
                _gameView.ShowError($"Unexpected error handling inventory: {ex.Message}");
            }
        }
        private static void InitializeMenuActions(MenuActionService actionService)
        {
            actionService.AddNewAction(1, "New game", "Main");
            actionService.AddNewAction(2, "Continue", "Main");
            actionService.AddNewAction(3, "Game description", "Main");
            actionService.AddNewAction(4, "Quit game", "Main");

            actionService.AddNewAction('a', "Move left", "InGameMenu");
            actionService.AddNewAction('w', "Move forward", "InGameMenu");
            actionService.AddNewAction('s', "Move backward", "InGameMenu");
            actionService.AddNewAction('d', "Move right", "InGameMenu");
            actionService.AddNewAction('e', "Open inventory", "InGameMenu");
            actionService.AddNewAction('q', "Save and quit to main menu", "InGameMenu");
            actionService.AddNewAction('m', "Show map", "InGameMenu");
            actionService.AddNewAction('p', "Show player statistics", "InGameMenu");

            actionService.AddNewAction('e', "Use an item", "InventoryMenu");
            actionService.AddNewAction('d', "Discard an item", "InventoryMenu");
            actionService.AddNewAction('l', "Leave inventory", "InventoryMenu");
        }
        // Save/Load
        public void SaveGame()
        {
            string sanitizedFileName = $"{_playerCharacter.Name}_savefile.json".Replace(" ", "_").Replace(":", "_").Replace("/", "_");

            var gameState = new GameState
            {
                PlayerCharacter = _playerCharacter,
                Map = _map,
                LastGeneratedItemId = ItemFactoryService.LastGeneratedItemId
            };
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new MapConverter(),
                    new ItemConverter(),
                    new PlayerCharacterConverter()
                }
            };
            try
            {
                string jsonString = JsonSerializer.Serialize(gameState, options);
                File.WriteAllText(sanitizedFileName, jsonString);
                _gameView.ShowMessage($"Game saved as {sanitizedFileName}");
            }
            catch (Exception ex)
            {
                _gameView.ShowError($"Failed to save the game: {ex.Message}");
            }
        }
        public static GameState? LoadGame(string characterName, GameView gameView)
        {
            string sanitizedFileName = $"{characterName}_savefile.json".Replace(" ", "_").Replace(":", "_").Replace("/", "_");

            if (File.Exists(sanitizedFileName))
            {
                try
                {
                    string jsonString = File.ReadAllText(sanitizedFileName);
                    var options = new JsonSerializerOptions
                    {
                        Converters =
                        {
                            new MapConverter(),
                            new ItemConverter(),
                            new PlayerCharacterConverter()
                        }
                    };
                    var gameState = JsonSerializer.Deserialize<GameState>(jsonString, options);
                    if (gameState != null)
                    {
                        ItemFactoryService.LastGeneratedItemId = gameState.LastGeneratedItemId;
                        return gameState;
                    }
                }
                catch (Exception ex)
                {
                    gameView.ShowError($"Failed to load the game: {ex.Message}");
                }
            }
            else
            {
                gameView.ShowError($"Save file for character '{characterName}' not found.");
            }
            return null;
        }
    }
}