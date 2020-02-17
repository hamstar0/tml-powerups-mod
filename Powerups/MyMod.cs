using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Services.EntityGroups;
using Powerups.Items;


namespace Powerups {
	public class PowerupsMod : Mod {
		public static PowerupsMod Instance { get; private set; }


		////////////////

		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-powerups-mod";



		////////////////

		public PowerupsMod() {
			PowerupsMod.Instance = this;
		}

		public override void Load() {
			EntityGroups.Enable();
		}

		public override void Unload() {
			PowerupsMod.Instance = null;
		}


		public override void PostSetupContent() {
			Mod potluckMod = ModLoader.GetMod( "PotLuck" );

			if( potluckMod != null ) {
				Func<(int, int), Item[]> onTileBreak = ( (int x, int y) tile ) => {
					var pos = new Vector2( tile.x << 4, tile.y << 4 );

					foreach( PowerupDefinition powDef in PowerupsConfig.Instance.PotLootPowerups ) {
						if( powDef.Context?.ToContext().Check() ?? true ) {
							PowerupItem.Create( powDef.PickBaseItem(), pos, powDef.TickDuration );
						}
					}
					return null;
				};

				potluckMod.Call( "AddPotBreakAction", onTileBreak );
			}
		}
	}
}