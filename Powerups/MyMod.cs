using System;
using System.Collections.Generic;
using System.Linq;
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
			int layerIdx = layers.FindIndex( layer => layer.Name.Equals( "Vanilla: Inventory" ) );
			if( layerIdx == -1 ) { return; }

			GameInterfaceDrawMethod buffIconOverlays = () => {
				if( Main.playerInventory ) {
					return true;
				}

				int buffIdx = Main.LocalPlayer.FindBuffIndex( ModContent.BuffType<PowerupBuff>() );
				if( buffIdx == -1 ) {
					return true;
				}

				IDictionary<int, Rectangle> buffFrames = BuffHUDHelpers.GetVanillaBuffIconRectanglesByPosition( false );
				foreach( KeyValuePair<int, Rectangle> kv in buffFrames.ToArray() ) {
					buffFrames[kv.Key] = new Rectangle( kv.Value.X + 2, kv.Value.Y + 2, kv.Value.Width - 4, kv.Value.Height - 4 );
				}

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