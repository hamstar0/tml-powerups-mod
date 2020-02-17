using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.TModLoader;
using HamstarHelpers.Helpers.Misc;
using HamstarHelpers.Helpers.Items.Attributes;
using HamstarHelpers.Helpers.XNA;


namespace Powerups.Buffs {
	class PowerupBuff : ModBuff {
		public static void DrawBuffIconOverlay( Rectangle frame ) {
			f
		}



		////////////////

		public override void SetDefaults() {
			this.DisplayName.SetDefault( "Powerup" );
			this.Description.SetDefault( "Applies an effect as if equipping an item" );
			Main.buffNoTimeDisplay[this.Type] = true;
			Main.buffNoSave[this.Type] = true;
		}

		public override void ModifyBuffTip( ref string tip, ref int rare ) {
			var myplayer = TmlHelpers.SafelyGetModPlayer<PowerupsPlayer>( Main.LocalPlayer );

			tip = "Applies an effect as if equipping these items:";

			foreach( (int tickDuration, Item item) in myplayer.PowerupItems ) {
				if( !item.accessory ) { continue; }

				Color color = ItemRarityAttributeHelpers.RarityColor[ item.rare ];
				string colorHex = XNAColorHelpers.RenderHex( color );

				tip += "\n [c/"+colorHex+":" + item.Name + "]: " + MiscHelpers.RenderTickDuration(tickDuration) + " remaining";
			}
		}


		////////////////

		public override void Update( Player player, ref int buffIndex ) {
			var myplayer = TmlHelpers.SafelyGetModPlayer<PowerupsPlayer>( player );

			foreach( (int tickDuration, Item item) in myplayer.PowerupItems ) {
				if( tickDuration > 0 ) {
					player.buffTime[ buffIndex ] = 2;
					return;
				}
			}

			player.buffTime[ buffIndex ] = 0;
		}
	}
}
