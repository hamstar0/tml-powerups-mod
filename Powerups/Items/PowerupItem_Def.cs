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

			if( baseItem.buffType > 0 && !baseItem.accessory ) {
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
				{ "item", itemKey }
			};

			return tag;
		}


		////////////////

		public override bool OnPickup( Player player ) {
			if( this.BaseItem.buffType > 0 ) {
				player.AddBuff( this.BaseItem.buffType, this.TickDuration );

				string msg = BuffAttributesHelpers.GetBuffDisplayName(this.BaseItem.buffType) + " Powerup!";
				CombatText.NewText( player.getRect(), Color.Lime, msg );
			} else if( this.BaseItem.potion ) {
				if( this.BaseItem.healLife > 0 ) {
					player.statLife += this.BaseItem.healLife;

					string msg = "+"+ this.BaseItem.healLife + " HP Healed!";
					CombatText.NewText( player.getRect(), Color.Lime, msg );
				}
				if( this.BaseItem.healMana > 0 ) {
					player.statMana += this.BaseItem.healMana;

					string msg = "+" + this.BaseItem.healMana + " Mana Healed!";
					CombatText.NewText( player.getRect(), Color.Blue, msg );
				}
			} else {
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