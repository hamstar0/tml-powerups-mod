using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using HamstarHelpers.Helpers.Buffs;
using HamstarHelpers.Helpers.TModLoader.Mods;
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

		internal IList<Func<PowerupDefinition, Item, Item>> PrePickBaseItemFuncs = new List<Func<PowerupDefinition, Item, Item>>();



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

					PowerupDefinition powDef = PowerupDefinition.TryPickDefinition( pos );

					if( powDef != null ) {
						Item baseItem = powDef.PickBaseItem();

						if( baseItem != null ) {
							PowerupItem.Create( baseItem, pos, powDef.TickDuration, powDef.IsTypeHidden );
						}
					}
					return null;
				};

				potluckMod.Call( "AddPotBreakAction", onTileBreak );
			}
		}


		////////////////

		public override object Call( params object[] args ) {
			return ModBoilerplateHelpers.HandleModCall( typeof(PowerupsAPI), args );
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
					Rectangle frame = kv.Value;
					buffFrames[kv.Key] = new Rectangle( frame.X+2, frame.Y+2, frame.Width-4, frame.Height-4 );
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