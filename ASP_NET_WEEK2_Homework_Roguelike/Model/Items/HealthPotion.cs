﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASP_NET_WEEK2_Homework_Roguelike.Model.Items;

namespace ASP_NET_WEEK2_Homework_Roguelike.Model.Items
{
    [ItemType("HealthPotion")]
    public class HealthPotion : Item
    {
        public int HealingAmount { get; set; }
        public int StackSize { get; set; } // Max potions per stack
    }
}
