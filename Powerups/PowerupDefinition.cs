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
using Powerups.Items;


namespace Powerups {
	public class PowerupDefinition {
		public static PowerupDefinition TryPickDefinition( Vector2 position ) {
			UnifiedRandom rand = TmlHelpers.SafelyGetRand();
			var powDefs = new List<PowerupDefinition>();
			int tileX = (int)position.X / 16;
			int tileY = (int)position.Y / 16;

			foreach( PowerupDefinition powDef in PowerupsConfig.Instance.NPCLootPowerups ) {
				if( powDef.Context?.ToContext().Check(tileX, tileY) ?? true ) {
					powDefs.Add( powDef );
					PowerupItem.Create( powDef.PickBaseItem(), position, powDef.TickDuration, powDef.IsTypeHidden );
				}
			}

			float totalChance = powDefs.Sum( pd => pd.PercentDropChance );
			float chance = rand.NextFloat() * totalChance;

			float countedChance = 0f;
			for( int i=0; i<powDefs.Count; i++ ) {
				countedChance += powDefs[i].PercentDropChance;

				if( chance < countedChance ) {
					return powDefs[i];
				}
			}

			return null;
		}



		////////////////

		[Range( 0f, 1f )]
		[DefaultValue( 1f )]
		public float PercentDropChance { get; set; } = 1f;

		public ContextConfig Context { get; set; } = null;

		public bool IsTypeHidden { get; set; } = false;

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
			
			baseItem = PowerupsAPI.OnPickItem( this, baseItem );

			return baseItem;
		}
	}
}
