using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader.Config;
using Terraria.Utilities;
using HamstarHelpers.Classes.Context;
using HamstarHelpers.Classes.DataStructures;
using HamstarHelpers.Helpers.TModLoader;
using HamstarHelpers.Services.EntityGroups;
using HamstarHelpers.Services.EntityGroups.Definitions;


namespace Powerups {
	public partial class PowerupDefinition {
		public static PowerupDefinition TryPickDefinition( Vector2 position ) {
			UnifiedRandom rand = TmlHelpers.SafelyGetRand();
			var powDefs = new List<PowerupDefinition>();
			var unpowDefs = new List<PowerupDefinition>();
			int tileX = (int)position.X / 16;
			int tileY = (int)position.Y / 16;

			foreach( PowerupDefinition powDef in PowerupsConfig.Instance.NPCLootPowerups ) {
				if( powDef.Context?.ToContext().Check(tileX, tileY) ?? true ) {
					if( !powDef.RemoveMeFromPool ) {
						powDefs.Add( powDef );
					}
					else {
						unpowDefs.Add( powDef );
					}
				}
			}

			PowerupDefinition pick = null;
			float totalChance = powDefs.Sum( pd => pd.PercentDropChance );
			float chance = rand.NextFloat() * totalChance;

			float countedChance = 0f;
			for( int i=0; i<powDefs.Count; i++ ) {
				countedChance += powDefs[i].PercentDropChance;

				if( chance < countedChance ) {
					pick = powDefs[i];
					pick.Filters = unpowDefs;

					break;
				}
			}

			return pick;
		}



		////////////////

		[Range( 0f, 1f )]
		[DefaultValue( 1f )]
		public float PercentDropChance { get; set; } = 1f;

		[Label( "When and where this powerup appears" )]
		public ContextConfig Context { get; set; } = null;

		[Label( "Does not reveal the powerup type" )]
		public bool IsTypeHidden { get; set; } = false;

		[Label( "Item to use as a powerup" )]
		public ItemDefinition ItemDef { get; set; }

		[Label( "Set of items to pick from (by group name) when no single item given" )]
		public string ItemEntityGroupName { get; set; } = null;

		[Label( "Duration of powerup" )]
		[Range( 2, 60 * 60 * 60 )]
		[DefaultValue( 60 * 90 )]
		public int TickDuration { get; set; } = 60 * 90;

		[Label( "Instead remove the given item(s) from the pool" )]
		public bool RemoveMeFromPool { get; set; } = false;


		////////////////

		private IList<PowerupDefinition> Filters = new List<PowerupDefinition>();



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
						totalGrp.UnionWith( potGrp );
						int[] totalGrpArr = totalGrp.ToArray();
						if( totalGrpArr.Length == 0 ) {
							return null;
						}

						int randIdx = TmlHelpers.SafelyGetRand().Next( 0, totalGrpArr.Length );

						baseItem = new Item();
						baseItem.SetDefaults( totalGrpArr[randIdx], true );
					}
				}
			}
			
			baseItem = PowerupsAPI.OnPickItem( this, baseItem );

			return baseItem;
		}
	}
}
