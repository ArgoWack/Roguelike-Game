﻿using ASP_NET_WEEK3_Homework_Roguelike.Services;
using ASP_NET_WEEK3_Homework_Roguelike.View;
using ASP_NET_WEEK3_Homework_Roguelike.Shared;
namespace ASP_NET_WEEK3_Homework_Roguelike.Model.Events
{
    public static class EventGenerator
    {
        private static EventService _eventService;
        private static CharacterInteractionService _interactionService;
        private static GameView _gameView;
        private static PlayerCharacterView _playerCharacterView;
        private static readonly Random random = new Random();
        public static void Initialize(EventService eventService, CharacterInteractionService interactionService, GameView gameView, PlayerCharacterView playerCharacterView)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
            _gameView = gameView ?? throw new ArgumentNullException(nameof(gameView));
            _playerCharacterView = playerCharacterView ?? throw new ArgumentNullException(nameof(playerCharacterView));
        }
        public static RandomEvent GenerateEvent()
        {
            int roll = random.Next(100);
            if (roll < Constants.chanceForFindItemEvent)
                return new FindItemEvent(_eventService, _interactionService, _playerCharacterView);
            else if (roll < Constants.chanceForMonsterEvent)
                return new MonsterEvent(_eventService, _interactionService, _playerCharacterView);
            else if (roll < Constants.chanceForDialogEvent)
                return new DialogEvent(_eventService, _interactionService, _playerCharacterView);
            else
                return null; // no event occurs
        }
        public static RandomEvent GenerateEvent(string eventStatus)
        {
            return eventStatus switch
            {
                "FindItemEvent" => new FindItemEvent(_eventService, _interactionService, _playerCharacterView),
                "MonsterEvent" => new MonsterEvent(_eventService, _interactionService, _playerCharacterView),
                "DialogEvent" => new DialogEvent(_eventService, _interactionService, _playerCharacterView),
                _ => null, // no valid event
            };
        }
    }
}