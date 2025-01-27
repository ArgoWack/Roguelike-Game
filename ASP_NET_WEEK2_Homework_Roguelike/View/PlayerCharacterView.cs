﻿using ASP_NET_WEEK3_Homework_Roguelike.Model;
using ASP_NET_WEEK3_Homework_Roguelike.Model.Items;
using static System.Console;

namespace ASP_NET_WEEK3_Homework_Roguelike.View
{
    public class PlayerCharacterView: IPlayerCharacterView
    {
        public void ShowMap(Map map, PlayerCharacter player)
        {
            var minX = map.DiscoveredRooms.Keys.Min(point => point.X);
            var maxX = map.DiscoveredRooms.Keys.Max(point => point.X);
            var minY = map.DiscoveredRooms.Keys.Min(point => point.Y);
            var maxY = map.DiscoveredRooms.Keys.Max(point => point.Y);
            WriteLine();

            for (int y = maxY; y >= minY; y--)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    var currentPoint = new Point(x, y);

                    if (map.DiscoveredRooms.TryGetValue(currentPoint, out Room room))
                    {
                        if (player.Coordinates != null && player.Coordinates.Equals(currentPoint))
                        {
                            ConsoleHelper.PrintColored("P", ConsoleColor.Yellow, false); // Player's current position
                        }
                        else
                        {
                            ConsoleHelper.PrintColored("+", ConsoleColor.DarkGreen, false); // Discovered room
                        }
                    }
                    else
                    {
                        Write(" "); // Undiscovered room
                    }

                    // Draw horizontal connections
                    if (x < maxX && map.DiscoveredRooms.TryGetValue(currentPoint, out Room currentRoom) && currentRoom.Exits.ContainsKey(Direction.East))
                    {
                        ConsoleHelper.PrintColored("-", ConsoleColor.Green, false);
                    }
                    else
                    {
                        Write(" ");
                    }
                }
                WriteLine();

                // Draw vertical connections
                if (y > minY)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        var currentPoint = new Point(x, y);
                        if (map.DiscoveredRooms.TryGetValue(currentPoint, out Room room) && room.Exits.ContainsKey(Direction.South))
                        {
                            ConsoleHelper.PrintColored("|", ConsoleColor.Green, false);
                        }
                        else
                        {
                            Write(" ");
                        }

                        // Space between columns
                        if (x < maxX)
                        {
                            Write(" ");
                        }
                    }
                    WriteLine();
                }
            }
        }
        public void ShowCharacterStats(PlayerCharacter player)
        {
            if (player == null)
            {
                ConsoleHelper.PrintColored("Character data is unavailable.", ConsoleColor.Red);
                return;
            }

            // Display Player Stats
            var stats = new Dictionary<string, (object Value, ConsoleColor Color)>
            {
                { "Character name", (player.Name ?? "N/A", ConsoleColor.Yellow) },
                { "Attack", (Math.Round(player.Attack), ConsoleColor.Green) },
                { "Defense", (Math.Round(player.Defense), ConsoleColor.Blue) },
                { "Speed", (Math.Round(player.Speed), ConsoleColor.Magenta) },
                { "Weight", (player.Weight, ConsoleColor.Gray) },
                { "Money", (player.Money, ConsoleColor.DarkYellow) },
                { "Health", ($"{player.Health}/{player.HealthLimit}", ConsoleColor.Red) },
                { "Level", (player.Level, ConsoleColor.White) },
                { "Experience", (player.Experience, ConsoleColor.DarkCyan) }
            };
            foreach (var stat in stats)
            {
                ConsoleHelper.PrintColored($"{stat.Key}: ", ConsoleColor.Cyan, false);
                ConsoleHelper.PrintColored(stat.Value.Value.ToString(), stat.Value.Color);
            }

            // Ensure EquippedItems is initialized
            if (player.EquippedItems == null || !player.EquippedItems.Any())
            {
                ConsoleHelper.PrintColored("\nNo equipped items.", ConsoleColor.DarkGray);
                return;
            }

            // Equipped Items Table Header
            ConsoleHelper.PrintColored("\nEquipped Items:", ConsoleColor.Cyan);
            ConsoleHelper.PrintColored($"{"Item",-22} {"ID",-8} {"Defense",-10} {"Attack",-10} {"Weight",-10} {"Value",-10} {"Description",-20}", ConsoleColor.White);
            ConsoleHelper.PrintColored(new string('-', 92), ConsoleColor.DarkGray);

            // Display each equipped item
            foreach (var equippedItem in player.EquippedItems)
            {
                var item = equippedItem.Value;
                if (item != null) // Safeguard against null items
                {
                    ConsoleHelper.PrintColored($"{item.Name,-23}", ConsoleColor.Cyan, false); // Use full name
                    ConsoleHelper.PrintColored($"{item.ID,-13}", ConsoleColor.Yellow, false);
                    ConsoleHelper.PrintColored($"{item.Defense,-10}", ConsoleColor.Blue, false);
                    ConsoleHelper.PrintColored($"{item.Attack,-11}", ConsoleColor.Green, false);
                    ConsoleHelper.PrintColored($"{item.Weight,-11}", ConsoleColor.Gray, false);
                    ConsoleHelper.PrintColored($"{item.MoneyWorth,-12}", ConsoleColor.DarkYellow, false);
                    ConsoleHelper.PrintColored($"{item.Description ?? "N/A",-20}", ConsoleColor.Magenta);
                }
                else
                {
                    ConsoleHelper.PrintColored($"{equippedItem.Key,-23}", ConsoleColor.Cyan, false);
                    ConsoleHelper.PrintColored($"{"None",-13}", ConsoleColor.Red, false);
                    ConsoleHelper.PrintColored($"{"-",-10}", ConsoleColor.DarkGray, false);
                    ConsoleHelper.PrintColored($"{"-",-11}", ConsoleColor.DarkGray, false);
                    ConsoleHelper.PrintColored($"{"-",-11}", ConsoleColor.DarkGray, false);
                    ConsoleHelper.PrintColored($"{"-",-12}", ConsoleColor.DarkGray, false);
                    ConsoleHelper.PrintColored($"{"N/A",-20}", ConsoleColor.DarkGray);
                }
            }
        }
        public void DisplayInventory(PlayerCharacter player)
        {
            WriteLine("\nHere is your inventory:");

            if (!player.Inventory.Any())
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Your inventory is empty.");
                ResetColor();
                return;
            }

            // Define column headers and their widths
            ForegroundColor = ConsoleColor.Cyan;
            string header = $"{"Item",-30} {"ID",-5} {"Defense",-10} {"Attack",-10} {"Weight",-10} {"Value",-10}";
            WriteLine(header);
            WriteLine(new string('-', header.Length)); // Separator line
            ResetColor();
            foreach (var item in player.Inventory)
            {
                if (item is HealthPotion potion)
                {
                    ForegroundColor = ConsoleColor.Green; // Special items (potions)
                    Write($"{potion.Name,-30} ");
                    ResetColor();

                    Write($"{potion.ID,-8} ");
                    Write($"{"-",-9} {"-",-10} {potion.Weight * potion.Quantity,-10} {potion.MoneyWorth,-10} ");

                    ForegroundColor = ConsoleColor.DarkYellow; // Highlight potion attributes
                    WriteLine($"Quantity: {potion.Quantity}/{potion.MaxStackSize} Healing: {potion.HealingAmount}");
                    ResetColor();
                }
                else
                {
                    ForegroundColor = ConsoleColor.Yellow; // Normal items
                    Write($"{item.Name,-30} ");
                    ResetColor();

                    Write($"{item.ID,-8} ");
                    Write($"{item.Defense,-9} {item.Attack,-10} {item.Weight,-10} {item.MoneyWorth,-10}");
                    WriteLine();
                }
            }
        }
        public void ShowEquipItemSuccess(string itemName)
        {
            ConsoleHelper.PrintColored($"You have equipped {itemName}.", ConsoleColor.Yellow, true);
        }
        public void ShowDiscardItemSuccess(string itemName)
        {
            ConsoleHelper.PrintColored($"You have discarded {itemName}.", ConsoleColor.Yellow, true);
        }
        public void ShowPlayerMovement(string direction, int currentX, int currentY)
        {
            ConsoleHelper.PrintColored($"\nMoved {direction}. Current position: ({currentX}, {currentY})\n", ConsoleColor.Yellow, true);
        }
        public void ShowEventEncounter(string eventType)
        {
            ConsoleHelper.PrintColored($"You encounter a {eventType}!", ConsoleColor.Yellow, true);
        }
        public void ShowError(string message)
        {
            ConsoleHelper.PrintColored($"Error: {message}", ConsoleColor.Red, true);
        }
        public void RelayMessage(string message)
        {
            ConsoleHelper.PrintColored($"{message}", ConsoleColor.Yellow, true);
        }
        public void ShowItemGenerated(string message)
        {
            ConsoleHelper.PrintColored($"Item Generated: {message}", ConsoleColor.Yellow, true);
        }
    }
}