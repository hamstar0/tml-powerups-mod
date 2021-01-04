using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using HamstarHelpers.Classes.UI.ModConfig;


namespace Powerups {
	class MyFloatInputElement : FloatInputElement { }




	class PowerupsConfig : ModConfig {
		public static PowerupsConfig Instance => ModContent.GetInstance<PowerupsConfig>();



		////////////////

		public override ConfigScope Mode => ConfigScope.ServerSide;


		////////////////

		public List<PowerupDefinition> NPCLootPowerups = new List<PowerupDefinition> {
			new PowerupDefinition {
				PercentDropChance = 1f,
				Context = null,
				ItemDef = null
			}
		};

		public List<PowerupDefinition> PotLootPowerups = new List<PowerupDefinition> {
			new PowerupDefinition {
				PercentDropChance = 0.25f,
				Context = null,
				ItemDef = null
			}
		};
	}
}
