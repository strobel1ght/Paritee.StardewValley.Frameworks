﻿using BetterFarmAnimalVariety.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace BetterFarmAnimalVariety.Framework.Helpers
{
    class Commands
    {
        public static ConfigFarmAnimalAnimalShop GetAnimalShopConfig(string category, string animalShop)
        {
            ConfigFarmAnimalAnimalShop configFarmAnimalAnimalShop = new ConfigFarmAnimalAnimalShop();

            if (animalShop.Equals(false.ToString().ToLower()))
            {
                return configFarmAnimalAnimalShop;
            }

            configFarmAnimalAnimalShop.Category = category;
            configFarmAnimalAnimalShop.Name = category;
            configFarmAnimalAnimalShop.Description = configFarmAnimalAnimalShop.GetDescriptionPlaceholder();
            configFarmAnimalAnimalShop.Price = ConfigFarmAnimalAnimalShop.PRICE_PLACEHOLDER;
            configFarmAnimalAnimalShop.Icon = configFarmAnimalAnimalShop.GetDefaultIconPath();

            string fullPathToIcon = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, configFarmAnimalAnimalShop.Icon);

            if (!File.Exists(fullPathToIcon))
            {
                throw new FileNotFoundException($"{fullPathToIcon} does not exist");
            }

            return configFarmAnimalAnimalShop;
        }

        public static string DescribeFarmAnimalCategory(KeyValuePair<string, ConfigFarmAnimal> entry)
        {
            string output = "";

            output += $"{entry.Key}\n";
            output += $"- Types: {String.Join(",", entry.Value.Types)}\n";
            output += $"- Buildings: {String.Join(",", entry.Value.Buildings)}\n";
            output += $"- AnimalShop:\n";
            output += $"-- Name: {entry.Value.AnimalShop.Name}\n";
            output += $"-- Description: {entry.Value.AnimalShop.Description}\n";
            output += $"-- Price: {entry.Value.AnimalShop.Price}\n";
            output += $"-- Icon: {entry.Value.AnimalShop.Icon}\n";

            return output;
        }
    }
}
