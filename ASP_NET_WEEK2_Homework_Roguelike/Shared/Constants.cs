namespace ASP_NET_WEEK3_Homework_Roguelike.Shared
{
    // Item constants for generation are in ItemStats
    public static class Constants
    {
        // For EventGenerator
        public const int chanceForFindItemEvent = 25;  // up to 25%
        public const int chanceForMonsterEvent = 75; //from 25 to 75%
        public const int chanceForDialogEvent = 85;  //from 75% to 85%
                                                     // 85% to 100% no event
        // For FindItemEvents
        public const int chanceForReceiveHealthPotion = 34;  // up to 34%
        public const int lowestAmountOfHealthPotionsObtainable = 1;
        public const int highestAmountOfHealthPotionsObtainable = 5;

        // For MonsterEvents
        public const int moneyDropPerMonestLevelMultiplier = 10;

        //For MonesterEvent and PlayerCharacter
        public const int experiencePerLevelMultiplier = 100;

        //For PlayerCharacter
        public const int baseHealth = 100;
        public const int healthPerLevelMultiplier = 10;
        public const float minimumSpeedPenalty = 0.5f;
        public const float maximumSpeedPenalty = 1.0f;
        public const float weightPenalityDivider = 100.0f;
        public const float speedEffectDivider = 10.0f;
        public const int speedPerLevelMultiplier = 10;
        public const int healthPotionValue = 40;

        //For ItemFactoryService
        public const float variationPercentageforItemStats = 0.25f; // from -25% to +25%
    }
}