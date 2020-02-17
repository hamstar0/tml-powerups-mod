using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;
using HamstarHelpers.Helpers.Buffs;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Items;


namespace Powerups.Items {
	public class PowerupItem : ModItem {
		public static Item Create( Item baseItem, Vector2 position, int tickDuration ) {
			int powerupType = ModContent.ItemType<PowerupItem>();
			int powerupItemWho = ItemHelpers.CreateItem( position, powerupType, 1, 16, 16 );
			Item powerupItem = Main.item[powerupItemWho];

			var myitem = (PowerupItem)powerupItem.modItem;
			myitem.BaseItem = baseItem;
			myitem.TickDuration = tickDuration;

			if( baseItem.potion ) {
				myitem.BaseBuffType = baseItem.buffType;
			} else if( !baseItem.accessory ) {
				LogHelpers.Alert( "Invalid powerup base item "+baseItem.Name );
				return null;
			}

			return powerupItem;
		}



		////////////////

		private Item BaseItem = null;
		private int BaseBuffType = -1;
		private int TickDuration = 0;



		////////////////

		public override bool CloneNewInstances => false;



		////////////////

		public override void SetStaticDefaults() {
			this.DisplayName.SetDefault("Powerup");
			this.Tooltip.SetDefault( "Can you feel the power?" );
		}

		public override void SetDefaults() {
			this.item.width = 16;
			this.item.height = 16;
			this.item.rare = 2;
		}


		////////////////

		public override void Load( TagCompound tag ) {
			this.BaseItem = null;

			string itemName = tag.GetString( "item" );
			this.BaseBuffType = tag.GetInt( "buff" );

			if( string.IsNullOrEmpty(itemName) ) {
				var itemDef = new ItemDefinition( itemName );
				this.BaseItem = new Item();
				this.BaseItem.SetDefaults( itemDef.Type );
			}
		}

		public override TagCompound Save() {
			string itemKey = this.BaseItem != null
				? ItemID.GetUniqueKey( this.BaseItem.type )
				: "";
			var tag = new TagCompound {
				{ "item", itemKey },
				{ "buff", this.BaseBuffType }
			};

			return tag;
		}


		////////////////

		public override bool OnPickup( Player player ) {
			if( this.BaseBuffType != -1 ) {
				player.AddBuff( this.BaseBuffType, this.TickDuration );

				string msg = BuffAttributesHelpers.GetBuffDisplayName( this.BaseBuffType )+" Powerup!";
				CombatText.NewText( player.getRect(), Color.Lime, msg );
			}

			if( this.BaseItem != null ) {
				var myplayer = player.GetModPlayer<PowerupsPlayer>();
				myplayer.PowerupItems.Add( (this.TickDuration, this.BaseItem) );

				string msg = this.BaseItem.Name + " Powerup!";
				CombatText.NewText( player.getRect(), Color.Lime, msg );
			}

			return false;
		}
	}
}