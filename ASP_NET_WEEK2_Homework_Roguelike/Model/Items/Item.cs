﻿namespace ASP_NET_WEEK3_Homework_Roguelike.Model.Items
{
    public class Item
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public int Defense { get; set; }
        public int Attack { get; set; }
        public int MoneyWorth { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }
        public int Quantity { get; set; } = 1; // Default for stackable items
    }
}