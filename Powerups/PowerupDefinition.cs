using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.ModLoader.Config;
using HamstarHelpers.Classes.Context;
using HamstarHelpers.Classes.DataStructures;
using HamstarHelpers.Helpers.TModLoader;
using HamstarHelpers.Services.EntityGroups;
using HamstarHelpers.Services.EntityGroups.Definitions;


namespace Powerups {
	public class PowerupDefinition {
		[Range( 0f, 1f )]
		[DefaultValue( 1f )]
		public float PercentDropChance { get; set; } = 1f;

		public ContextConfig Context { get; set; } = null;

		public ItemDefinition ItemDef { get; set; }

		public string ItemEntityGroupName { get; set; } = null;

		[Range( 2, 60 * 60 * 60 )]
		[DefaultValue( 60 * 90 )]
		public int TickDuration { get; set; } = 60 * 90;



		////////////////

		public Item PickBaseItem() {
			Item baseItem = null;

			if( this.ItemDef != null ) {
				baseItem = new Item();
				baseItem.SetDefaults( this.ItemDef.Type, true );
			} else if( !string.IsNullOrEmpty(this.ItemEntityGroupName) ) {
				IReadOnlySet<int> group;
				if( EntityGroups.TryGetItemGroup(this.ItemEntityGroupName, out group) ) {
					var groupArr = group.ToArray();
					int randIdx = TmlHelpers.SafelyGetRand().Next( 0, groupArr.Length );

					baseItem = new Item();
					baseItem.SetDefaults( groupArr[randIdx], true );
				}
			} else {
				IReadOnlySet<int> accGrp, potGrp;
				if( EntityGroups.TryGetItemGroup(ItemGroupIDs.AnyAccessory, out accGrp) ) {
					if( EntityGroups.TryGetItemGroup(ItemGroupIDs.AnyPotion, out potGrp) ) {
						ISet<int> totalGrp = new HashSet<int>( accGrp );
						int[] totalGrpArr = totalGrp.Union(potGrp).ToArray();

						int randIdx = TmlHelpers.SafelyGetRand().Next( 0, totalGrpArr.Length );

						baseItem = new Item();
						baseItem.SetDefaults( totalGrpArr[randIdx], true );
					}
				}
			}

			return baseItem;
		}
	}
}
