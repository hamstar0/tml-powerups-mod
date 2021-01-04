using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Utilities;
using HamstarHelpers.Helpers.TModLoader;


namespace Powerups {
	public partial class PowerupDefinition {
		public static PowerupDefinition TryPickDefinition( Vector2 position ) {
			UnifiedRandom rand = TmlHelpers.SafelyGetRand();
			var powDefs = new List<PowerupDefinition>();
			var unpowDefs = new List<PowerupDefinition>();
			int tileX = (int)position.X / 16;
			int tileY = (int)position.Y / 16;

			foreach( PowerupDefinition powDef in PowerupsConfig.Instance.NPCLootPowerups ) {
				if( powDef.Context?.ToContext().Check(tileX, tileY) ?? true ) {
					if( !powDef.RemoveMeFromPool ) {
						powDefs.Add( powDef );
					}
					else {
						unpowDefs.Add( powDef );
					}
				}
			}

			PowerupDefinition pick = null;
			float totalChance = powDefs.Sum( pd => pd.PercentDropChance );
			float chance = rand.NextFloat() * totalChance;

			float countedChance = 0f;
			for( int i=0; i<powDefs.Count; i++ ) {
				countedChance += powDefs[i].PercentDropChance;

				if( chance < countedChance ) {
					pick = powDefs[i];
					pick.Filters = unpowDefs;

					break;
				}
			}

			return pick;
		}



		////////////////

		private IList<PowerupDefinition> Filters = new List<PowerupDefinition>();
	}
}
