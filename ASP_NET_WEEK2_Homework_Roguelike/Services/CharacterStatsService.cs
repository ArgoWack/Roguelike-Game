﻿using ASP_NET_WEEK2_Homework_Roguelike.Model;
using ASP_NET_WEEK2_Homework_Roguelike.ItemKinds;

namespace ASP_NET_WEEK2_Homework_Roguelike.Services
{
    public class CharacterStatsService
    {
        // Calculate and return attack value based on equipped items
        public float CalculateAttack(PlayerCharacter player)
        {
            return (player.EquippedHelmet?.Attack ?? 0) +
                   (player.EquippedArmor?.Attack ?? 0) +
                   (player.EquippedShield?.Attack ?? 0) +
                   (player.EquippedGloves?.Attack ?? 0) +
                   (player.EquippedTrousers?.Attack ?? 0) +
                   (player.EquippedBoots?.Attack ?? 0) +
                   (player.EquippedAmulet?.Attack ?? 0) +
                   (player.EquippedSwordOneHanded?.Attack ?? 0) +
                   (player.EquippedSwordTwoHanded?.Attack ?? 0);
        }

        // Calculate and return defense value based on equipped items
        public float CalculateDefense(PlayerCharacter player)
        {
            return (player.EquippedHelmet?.Defense ?? 0) +
                   (player.EquippedArmor?.Defense ?? 0) +
                   (player.EquippedShield?.Defense ?? 0) +
                   (player.EquippedGloves?.Defense ?? 0) +
                   (player.EquippedTrousers?.Defense ?? 0) +
                   (player.EquippedBoots?.Defense ?? 0) +
                   (player.EquippedAmulet?.Defense ?? 0) +
                   (player.EquippedSwordOneHanded?.Defense ?? 0) +
                   (player.EquippedSwordTwoHanded?.Defense ?? 0);
        }

        // Calculate and return weight based on inventory and equipped items
        public int CalculateWeight(PlayerCharacter player)
        {
            int totalWeight = 0;
            foreach (var item in player.Inventory)
            {
                totalWeight += item.Weight;
            }
            return totalWeight;
        }
    }
}