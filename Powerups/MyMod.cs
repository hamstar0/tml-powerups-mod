using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using HamstarHelpers.Helpers.Buffs;
using HamstarHelpers.Services.EntityGroups;
using Powerups.Items;
using Powerups.Buffs;


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
			int layerIdx = layers.FindIndex( layer => layer.Name.Equals( "Vanilla: Info Accessories Bar" ) );
			if( layerIdx == -1 ) {
				layerIdx = layers.FindIndex( layer => layer.Name.Equals( "Vanilla: Inventory" ) );
				if( layerIdx == -1 ) { return; }

				layerIdx += 1;
			}

			GameInterfaceDrawMethod buffIconOverlays = () => {
				int buffIdx = Main.LocalPlayer.FindBuffIndex( ModContent.BuffType<PowerupBuff>() );
				if( buffIdx == -1 ) {
					return true;
				}

				IDictionary<int, Rectangle> buffFrames = BuffHUDHelpers.GetVanillaBuffIconRectanglesByPosition( false );

				if( buffFrames.ContainsKey(buffIdx) ) {
					PowerupBuff.DrawBuffIconOverlay( buffFrames[buffIdx] );
				}
				return true;
			};

			var tradeLayer = new LegacyGameInterfaceLayer( "Powerups: Buff Icon Overlays", buffIconOverlays, InterfaceScaleType.UI );
			layers.Insert( layerIdx, tradeLayer );
		}
	}
}