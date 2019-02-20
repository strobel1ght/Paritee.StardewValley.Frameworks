﻿using Harmony;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Events;
using System.Collections.Generic;
using System.Linq;
using PariteeCore = Paritee.StardewValley.Core;

namespace BetterFarmAnimalVariety.Framework.Patches.AnimalHouse
{
    [HarmonyPatch(typeof(StardewValley.AnimalHouse), "addNewHatchedAnimal")]
    class AddNewHatchedAnimal
    {
        public static bool Prefix(ref StardewValley.AnimalHouse __instance, ref string name)
        {
            if (__instance.getBuilding() is StardewValley.Buildings.Coop coop)
            {
                AddNewHatchedAnimal.HandleHatchling(ref __instance, name);
            }
            else if (PariteeCore.Api.Event.IsFarmEventOccurring<QuestionEvent>(out QuestionEvent questionEvent))
            {
                AddNewHatchedAnimal.HandleNewborn(ref __instance, name, ref questionEvent);
            }

            GameLocation currentLocation = PariteeCore.Api.Game.GetCurrentLocation();

            PariteeCore.Api.Event.GoToNextEventCommandInLocation(ref currentLocation);
            PariteeCore.Api.Game.ExitActiveMenu();

            // Everything in this function was handled here
            return false;
        }

        private static void HandleHatchling(ref StardewValley.AnimalHouse animalHouse, string name)
        {
            StardewValley.Object incubator = PariteeCore.Api.AnimalHouse.GetIncubator(animalHouse);

            if (incubator == null)
            {
                // Can't do anything about it
                return;
            }

            Farmer player = PariteeCore.Api.Game.GetPlayer();

            // Grab the types with their associated categories in string form
            Dictionary<string, List<string>> restrictions = Helpers.Mod.ReadConfig<ModConfig>().GroupTypesByCategory()
                .ToDictionary(kvp => kvp.Key, kvp => PariteeCore.Api.FarmAnimal.SanitizeBlueChickens(kvp.Value, player));

            // Return a matched type or user default coop dweller
            string type = PariteeCore.Api.AnimalHouse.GetRandomTypeFromIncubator(incubator, restrictions)
                ?? PariteeCore.Api.FarmAnimal.GetDefaultCoopDwellerType();

            Building building = animalHouse.getBuilding();
            FarmAnimal animal = PariteeCore.Api.FarmAnimal.CreateFarmAnimal(type, PariteeCore.Api.Farmer.GetUniqueId(player), name, building);

            PariteeCore.Api.FarmAnimal.AddToBuilding(ref animal, ref building);
            PariteeCore.Api.AnimalHouse.ResetIncubator(animalHouse, incubator);
        }

        private static void HandleNewborn(ref StardewValley.AnimalHouse animalHouse, string name, ref QuestionEvent questionEvent)
        {
            Farmer player = PariteeCore.Api.Game.GetPlayer();

            // Check the config
            ModConfig config = Helpers.Mod.ReadConfig<ModConfig>();

            // Grab the types with their associated categories in string form
            Dictionary<string, List<string>> restrictions = Helpers.Mod.ReadConfig<ModConfig>().GroupTypesByCategory()
                .ToDictionary(kvp => kvp.Key, kvp => PariteeCore.Api.FarmAnimal.SanitizeBlueChickens(kvp.Value, player));

            // Return a matched type or user default barn dweller
            string type = PariteeCore.Api.FarmAnimal.GetRandomTypeFromParent(questionEvent.animal, restrictions)
                ?? PariteeCore.Api.FarmAnimal.GetDefaultBarnDwellerType();

            Building building = animalHouse.getBuilding();
            FarmAnimal animal = PariteeCore.Api.FarmAnimal.CreateFarmAnimal(type, PariteeCore.Api.Farmer.GetUniqueId(player), name, building);

            PariteeCore.Api.FarmAnimal.AssociateParent(ref animal, questionEvent.animal.myID.Value);
            PariteeCore.Api.FarmAnimal.AddToBuilding(ref animal, ref building);

            PariteeCore.Api.Event.ForceQuestionEventToProceed(ref questionEvent);
        }
    }
}
