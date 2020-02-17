using System;
using System.Collections.Generic;
using Terraria.ModLoader.Config;
using HamstarHelpers.Classes.UI.ModConfig;
using HamstarHelpers.Services.Configs;


namespace Powerups {
	class MyFloatInputElement : FloatInputElement { }




	class PowerupsConfig : StackableModConfig {
		public static PowerupsConfig Instance => ModConfigStack.GetMergedConfigs<PowerupsConfig>();



		////////////////

		public override ConfigScope Mode => ConfigScope.ServerSide;


		////////////////

		public List<PowerupDefinition> NPCLootPowerups = new List<PowerupDefinition> {
			new PowerupDefinition {
				PercentDropChance = 1f,
				DropsFromPotsOnly = false,
				Context = null,
				ItemDef = null
			}
		};

		public List<PowerupDefinition> PotLootPowerups = new List<PowerupDefinition> {
			new PowerupDefinition {
				PercentDropChance = 0.25f,
				DropsFromPotsOnly = false,
				Context = null,
				ItemDef = null
			}
		};
	}
}
