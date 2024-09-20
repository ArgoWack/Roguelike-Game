﻿using ASP_NET_WEEK2_Homework_Roguelike.Services;
using ASP_NET_WEEK2_Homework_Roguelike.Model;
using ASP_NET_WEEK2_Homework_Roguelike.Controller;
using ASP_NET_WEEK2_Homework_Roguelike.Model.Events;

namespace ASP_NET_WEEK2_Homework_Roguelike.Model.Events
{
    public class DialogEvent : RandomEvent
    {
        private readonly EventService _eventService;
        private readonly CharacterInteractionService _interactionService;

        public DialogEvent(EventService eventService, CharacterInteractionService interactionService)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
        }

        public override void Execute(PlayerCharacter player, Room room, PlayerCharacterController controller)
        {
            if (player == null || room == null)
                throw new ArgumentNullException("Player or Room cannot be null.");

            string eventType = GetRandomEventType();
            _eventService.HandleEventOutcome($"You encounter a {eventType}");

            switch (eventType)
            {
                case "WiseTraveler":
                    HandleWiseTraveler(player);
                    break;
                case "Monk":
                    _eventService.HealPlayer(player, 1000);
                    break;
                case "Witch":
                    HandleWitchCurse(player);
                    break;
                case "Merchant":
                    ExecuteMerchantEvent(player);
                    break;
                default:
                    _eventService.HandleEventOutcome("You encounter a mysterious stranger who says nothing and disappears.");
                    break;
            }

            room.EventStatus = "none";
        }

        private void HandleWiseTraveler(PlayerCharacter player)
        {
            int buffType = new Random().Next(4);
            switch (buffType)
            {
                case 0:
                    _eventService.ModifyPlayerStats(player, "experience", 200);
                    break;
                case 1:
                    _eventService.ModifyPlayerStats(player, "speed", 5);
                    break;
                case 2:
                    _eventService.ModifyPlayerStats(player, "attack", 5);
                    break;
                case 3:
                    _eventService.ModifyPlayerStats(player, "defense", 5);
                    break;
            }
        }

        private void HandleWitchCurse(PlayerCharacter player)
        {
            player.Health /= 2;
            _eventService.ModifyPlayerStats(player, "speed", -2);
            _eventService.ModifyPlayerStats(player, "attack", -2);
            _eventService.ModifyPlayerStats(player, "defense", -2);
            _eventService.HandleEventOutcome("You got cursed by the witch. Your health is halved, and your stats are reduced.");
        }

        private void ExecuteMerchantEvent(PlayerCharacter player)
        {
            string choice;
            do
            {
                _eventService.HandleEventOutcome("Merchant encountered. What would you like to do?");
                choice = _eventService.GetMerchantOptions();

                switch (choice)
                {
                    case "b":
                        _eventService.BuyHealthPotion(player);
                        break;
                    case "s":
                        int? itemId = _eventService.PromptForItemIdToSell();
                        if (itemId.HasValue)
                        {
                            _eventService.SellItem(player, itemId.Value);
                        }
                        else
                        {
                            _eventService.HandleEventOutcome("Invalid item ID.");
                        }
                        break;
                    case "l":
                        _eventService.HandleEventOutcome("The merchant nods and moves on.");
                        break;
                    default:
                        _eventService.HandleEventOutcome("Invalid choice.");
                        break;
                }

            } while (choice != "l");
        }

        private string GetRandomEventType()
        {
            var eventTypes = new[] { "WiseTraveler", "Monk", "Witch", "Merchant" };
            return eventTypes[new Random().Next(eventTypes.Length)];
        }
    }
}