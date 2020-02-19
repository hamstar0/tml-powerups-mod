using HamstarHelpers.Helpers.Debug;
using Powerups.Items;
using System;
using Terraria;
using Terraria.ModLoader;


namespace Powerups {
	class PowerupsNPC : GlobalNPC {
		public override void NPCLoot( NPC npc ) {
			PowerupDefinition powDef = PowerupDefinition.TryPickDefinition( npc.position );
			if( powDef != null ) {
				Item baseItem = powDef.PickBaseItem();
				if( baseItem != null ) {
					PowerupItem.Create( baseItem, npc.position, powDef.TickDuration, powDef.IsTypeHidden );
				}
			}
		}
	}
}
