using System;
using System.ComponentModel;
using Terraria;
using Terraria.ModLoader.Config;
using HamstarHelpers.Classes.Context;


namespace Powerups {
	public partial class PowerupDefinition {
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
	}
}
