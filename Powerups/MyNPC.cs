using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;
using Powerups.Items;


namespace Powerups {
	class PowerupsNPC : GlobalNPC {
		public override void NPCLoot( NPC npc ) {
			if( Main.netMode != NetmodeID.MultiplayerClient ) {
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
}
