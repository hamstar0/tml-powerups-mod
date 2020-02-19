using System;
using Terraria;


namespace Powerups {
	public class PowerupsAPI {
		public static void PrePickBaseItem( Func<PowerupDefinition, Item, Item> func ) {
			PowerupsMod.Instance.PrePickBaseItemFuncs.Add( func );
		}


		////////////////

		internal static Item OnPickItem( PowerupDefinition powerupDef, Item pickedItem ) {
			Item item = pickedItem;

			foreach( Func<PowerupDefinition, Item, Item> func in PowerupsMod.Instance.PrePickBaseItemFuncs ) {
				item = func( powerupDef, item );
			}

			return item;
		}
	}
}
