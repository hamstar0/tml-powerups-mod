using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;
using HamstarHelpers.Helpers.Buffs;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Items;
using Powerups.Buffs;


namespace Powerups.Items {
	public partial class PowerupItem : ModItem {
		public static Item Create( Item baseItem, Vector2 position, int tickDuration, bool isTypeHidden ) {
			int powerupType = ModContent.ItemType<PowerupItem>();
			int powerupItemWho = ItemHelpers.CreateItem( position, powerupType, 1, 16, 16 );

			Item powerupItem = Main.item[ powerupItemWho ];
			if( !powerupItem.active ) {
				return null;
			}

			var myitem = (PowerupItem)powerupItem.modItem;
			myitem.BaseItem = baseItem;
			myitem.TickDuration = tickDuration;
			myitem.IsTypeHidden = isTypeHidden;

			if( baseItem.potion ) {
				myitem.BaseBuffType = baseItem.buffType;
			} else if( !baseItem.accessory ) {
				LogHelpers.Alert( "Invalid powerup base item " + baseItem.Name );
				return null;
			}

			if( !isTypeHidden ) {
				powerupItem.SetNameOverride( baseItem.Name + " Powerup" );
			} else {
				powerupItem.SetNameOverride( "Mystery Powerup" );
			}
			
			return powerupItem;
		}



		////////////////

		private Item BaseItem = null;
		private int BaseBuffType = -1;
		private int TickDuration = 0;
		private bool IsTypeHidden = false;



		////////////////

		public override bool CloneNewInstances => false;



		////////////////

		public override void SetStaticDefaults() {
			this.DisplayName.SetDefault( "Powerup" );
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

			if( string.IsNullOrEmpty( itemName ) ) {
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

				string msg = BuffAttributesHelpers.GetBuffDisplayName( this.BaseBuffType ) + " Powerup!";
				CombatText.NewText( player.getRect(), Color.Lime, msg );
			}

			if( this.BaseItem != null ) {
				var myplayer = player.GetModPlayer<PowerupsPlayer>();
				myplayer.PowerupItems.Add( (this.TickDuration, this.BaseItem) );

				string msg = this.BaseItem.Name + " Powerup!";
				CombatText.NewText( player.getRect(), Color.Lime, msg );

				player.AddBuff( ModContent.BuffType<PowerupBuff>(), 2 );
			}

			Main.PlaySound( SoundID.Item30, this.item.Center );

			return false;
		}
	}
}