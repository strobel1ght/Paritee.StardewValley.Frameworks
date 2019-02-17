﻿using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace BetterFarmAnimalVariety.Framework.Commands.FarmAnimal
{
    class SetAnimalShopIconCommand : Command
    {
        public SetAnimalShopIconCommand(IModHelper helper, IMonitor monitor, ModConfig config)
            : base("bfav_fa_setshopicon", $"Set the category's animal shop icon.\nUsage: bfav_fa_setshopicon <category> <filename>\n- category: the unique animal category.\n- filename: the name of the file (ex. filename.png).", helper, monitor, config) { }

        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        public override void Callback(string command, string[] args)
        {
            try
            {
                this.AssertGameNotLoaded();
                this.AssertRequiredArgumentOrder(args.Length, 1, "category");

                string category = args[0].Trim();

                this.AssertFarmAnimalCategoryExists(category);
                this.AssertFarmAnimalCanBePurchased(category);
                this.AssertRequiredArgumentOrder(args.Length, 2, "icon");
                this.AssertValidIcon(args[1]);

                this.Config.FarmAnimals[category].AnimalShop.Icon = args[1].Trim();

                this.Helper.WriteConfig(this.Config);

                string output = Helpers.Commands.DescribeFarmAnimalCategory(new KeyValuePair<string, Framework.Config.FarmAnimal>(category, this.Config.FarmAnimals[category]));

                this.Monitor.Log(output, LogLevel.Info);
            }
            catch (Exception e)
            {
                this.Monitor.Log(e.Message, LogLevel.Error);

                return;
            }
        }
    }
}