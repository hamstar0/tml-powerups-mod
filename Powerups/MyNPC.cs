using Powerups.Items;
using System;
using Terraria;
using Terraria.ModLoader;


namespace Powerups {
	class PowerupsNPC : GlobalNPC {
		public override void NPCLoot( NPC npc ) {
			foreach( PowerupDefinition powDef in PowerupsConfig.Instance.NPCLootPowerups ) {
				if( powDef.Context?.ToContext().Check() ?? true ) {
					PowerupItem.Create( powDef.PickBaseItem(), npc.position, powDef.TickDuration, powDef.IsTypeHidden );
				}
			}
		}
	}
}
