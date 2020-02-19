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
				PowerupItem.Create( powDef.PickBaseItem(), npc.position, powDef.TickDuration, powDef.IsTypeHidden );
			}
		}
	}
}
