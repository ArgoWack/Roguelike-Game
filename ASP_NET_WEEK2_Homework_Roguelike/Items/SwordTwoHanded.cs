﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASP_NET_WEEK2_Homework_Roguelike.Items;

namespace ASP_NET_WEEK2_Homework_Roguelike.ItemKinds
{
    [ItemType("SwordTwoHanded")]
    public class SwordTwoHanded : Item
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public int Defense { get; set; }
        public int Attack { get; set; }
        public int MoneyWorth { get; set; }
        public string Description { get; set; }
    }
}
