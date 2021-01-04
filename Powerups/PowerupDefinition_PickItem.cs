using System;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using HamstarHelpers.Classes.DataStructures;
using HamstarHelpers.Helpers.TModLoader;
using HamstarHelpers.Services.EntityGroups;
using HamstarHelpers.Services.EntityGroups.Definitions;


namespace Powerups {
	public partial class PowerupDefinition {
		public static int[] FilterGroup( IList<PowerupDefinition> filters, int[] itemTypes ) {
			var filteredItems = new HashSet<int>( itemTypes );

			foreach( PowerupDefinition undef in filters ) {
				if( !undef.RemoveMeFromPool ) {
					continue;
				}

				int[] exclusions = undef.GetAllItemTypes();
				filteredItems.ExceptWith( exclusions );

				if( filteredItems.Count == 0 ) {
					break;
				}
			}

			return filteredItems.ToArray();
		}



		////////////////

		public Item PickBaseItem() {
			Item baseItem = null;

			if( this.ItemDef != null ) {
				baseItem = new Item();
				baseItem.SetDefaults( this.ItemDef.Type, true );
			} else {
				baseItem = this.PickItemFromGroup( this.GetAllItemTypes() );
			}
			
			if( baseItem == null ) {
				return null;
			}

			baseItem = PowerupsAPI.OnPickItem( this, baseItem );

			return baseItem;
		}


		////////////////

		private Item PickItemFromGroup( int[] group ) {
			Item baseItem = null;

			if( this.Filters != null ) {
				group = PowerupDefinition.FilterGroup( this.Filters, group );
				if( group.Length == 0 ) {
					return null;
				}
			}

			int randIdx = TmlHelpers.SafelyGetRand().Next( 0, group.Length );

			baseItem = new Item();
			baseItem.SetDefaults( group[randIdx], true );

			return baseItem;
		}


		////////////////

		private int[] GetAllItemTypes() {
			if( this.ItemDef != null ) {
				return new[] { this.ItemDef.Type };
			} else if( !string.IsNullOrEmpty( this.ItemEntityGroupName ) ) {
				return this.GetEntityGroupItemTypes();
			} else {
				return this.GetWildcardItemTypes();
			}
		}

		private int[] GetEntityGroupItemTypes() {
			IReadOnlySet<int> group;
			if( !EntityGroups.TryGetItemGroup(this.ItemEntityGroupName, out group) ) {
				return new int[ 0 ];
			}

			return group.ToArray();
		}


		public int[] GetWildcardItemTypes() {
			IReadOnlySet<int> accGrp, potGrp;

			if( !EntityGroups.TryGetItemGroup( ItemGroupIDs.AnyAccessory, out accGrp ) ) {
				return new int[0];
			}
			if( !EntityGroups.TryGetItemGroup( ItemGroupIDs.AnyPotion, out potGrp ) ) {
				return new int[0];
			}

			ISet<int> totalGrp = new HashSet<int>( accGrp );
			totalGrp.UnionWith( potGrp );

			int[] totalGrpArr = totalGrp.ToArray();
			return totalGrpArr;
		}
	}
}
