using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;
using HamstarHelpers.Helpers.Buffs;
using HamstarHelpers.Helpers.Debug;
using Powerups.Buffs;


namespace Powerups.Items {
	public partial class PowerupItem : ModItem {
		public static Item Create( Item powItem, Vector2 position, int tickDuration, bool isTypeHidden ) {
			if( powItem.buffType <= 0 && !powItem.accessory && powItem.headSlot < 0 && powItem.bodySlot < 0 && powItem.legSlot < 0 ) {
				LogHelpers.Log( "Invalid powerup base item " + powItem.Name );
				return null;
			}

			int powerupType = ModContent.ItemType<PowerupItem>();
			int powerupItemWho = Item.NewItem( position: position, Type: powerupType );
			if( powerupItemWho == -1 ) {
				return null;
			}

			Item powerupItem = Main.item[ powerupItemWho ];
			if( !powerupItem.active ) {
				return null;
			}
			
			var myitem = (PowerupItem)powerupItem.modItem;
			myitem.BaseItem = powItem;
			myitem.TickDuration = tickDuration;
			myitem.IsTypeHidden = isTypeHidden;

			if( !isTypeHidden ) {
				powerupItem.SetNameOverride( powItem.Name+" Powerup" );
			} else {
				powerupItem.SetNameOverride( "Mystery Powerup" );
			}

			if( Main.netMode == NetmodeID.Server ) {
				NetMessage.SendData( MessageID.SyncItem, -1, -1, null, powerupItemWho );
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

		public override void NetSend( BinaryWriter writer ) {
			try {
				writer.Write( (int)this.BaseItem.type );
				writer.Write( (int)this.TickDuration );
				writer.Write( (bool)this.IsTypeHidden );
			} catch { }
		}

		public override void NetRecieve( BinaryReader reader ) {
			try {
				int itemType = reader.ReadInt32();
				this.TickDuration = reader.ReadInt32();
				this.IsTypeHidden = reader.ReadBoolean();

				this.BaseItem = new Item();
				this.BaseItem.SetDefaults( itemType, false );
			} catch { }
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