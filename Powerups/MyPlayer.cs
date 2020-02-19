using Powerups.Buffs;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace Powerups {
	class PowerupsPlayer : ModPlayer {
		internal IList<(int Duration, Item Item)> PowerupItems = new List<(int, Item)>();



		////////////////

		public override bool CloneNewInstances => false;



		////////////////

		public override void clientClone( ModPlayer clientClone ) {
			base.clientClone( clientClone );
		}


		////////////////

		public override void PreUpdate() {
			if( !this.player.HasBuff(ModContent.BuffType<PowerupBuff>()) ) {
				if( this.PowerupItems.Count > 0 ) {
					this.PowerupItems.Clear();
				}
			}
		}

		public override void UpdateEquips( ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff ) {
			for( int i = 0; i < this.PowerupItems.Count; i++ ) {
				int duration = this.PowerupItems[i].Duration;
				Item powerupItem = this.PowerupItems[i].Item;

				bool _ = false;
				this.player.VanillaUpdateEquip( powerupItem );
				this.player.VanillaUpdateAccessory( this.player.whoAmI, powerupItem, false, ref _, ref _, ref _ );

				this.PowerupItems[i] = (duration - 1, powerupItem);
				if( this.PowerupItems[i].Duration <= 0 ) {
					this.PowerupItems.RemoveAt( i );
					i--;
				}
			}
		}
	}
}