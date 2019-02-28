﻿using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterFarmAnimalVariety.Framework.Commands.FarmAnimal
{
    class AddTypes : Command
    {
        public AddTypes(IModHelper helper, IMonitor monitor, ModConfig config)
            : base("bfav_fa_addtypes", "Add at least one animal type to a category.\nUsage: bfav_fa_addtypes <category> <types>\n- category: the unique animal category.\n- types: a comma separated list in quotes (ex \"White Cow,Brown Cow\").", helper, monitor, config) { }

        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        public override void Callback(string command, string[] args)
        {
            try
            {
                Helpers.Assert.GameNotLoaded();
                Helpers.Assert.ArgumentInRange(args.Length, 2);
                Helpers.Assert.RequiredArgumentOrder(args.Length, 1, "category");

                string category = args[0].Trim();

                Helpers.Assert.FarmAnimalCategoryExists(category);
                Helpers.Assert.RequiredArgumentOrder(args.Length, 2, "type");

                Framework.Config.FarmAnimal animal = this.Config.GetCategory(category);

                List<string> types = new List<string>(animal.Types);
                List<string> newTypes = args[1].Split(',').Select(i => i.Trim()).ToList();

                Helpers.Assert.FarmAnimalTypesExist(newTypes);

                animal.Types = types.Concat(newTypes).Distinct().ToArray();

                this.Helper.WriteConfig(this.Config);

                string output = this.DescribeFarmAnimalCategory(animal);

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