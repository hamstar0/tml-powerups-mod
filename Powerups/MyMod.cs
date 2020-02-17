using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Services.EntityGroups;
using Powerups.Items;
using System.Collections.Generic;
using Terraria.UI;
using Powerups.Buffs;
using HamstarHelpers.Helpers.Buffs;


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

		////

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


		////////////////

		public override void ModifyInterfaceLayers( List<GameInterfaceLayer> layers ) {
			int idx = layers.FindIndex( layer => layer.Name.Equals( "Vanilla: Info Accessories Bar" ) );
			if( idx == -1 ) {
				idx = layers.FindIndex( layer => layer.Name.Equals( "Vanilla: Inventory" ) );
				if( idx == -1 ) { return; }

				idx += 1;
			}

			GameInterfaceDrawMethod buffIconOverlays = () => {
				int idx = Main.LocalPlayer.FindBuffIndex( ModContent.BuffType<PowerupBuff>() );
				if( idx == -1 ) {
					return true;
				}

				IDictionary<int, Rectangle> buffFrames = BuffHUDHelpers.GetVanillaBuffIconRectanglesByPosition( false );

				if( buffFrames.ContainsKey(idx) ) {
					PowerupBuff.DrawBuffIconOverlay( buffFrames[idx] );
				}
				return true;
			};

			var tradeLayer = new LegacyGameInterfaceLayer( "Powerups: Buff Icon Overlays", buffIconOverlays, InterfaceScaleType.UI );
			layers.Insert( idx, tradeLayer );
		}
	}
}