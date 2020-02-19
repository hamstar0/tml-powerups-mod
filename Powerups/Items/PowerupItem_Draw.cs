using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;


namespace Powerups.Items {
	public partial class PowerupItem : ModItem {
		private Texture2D GetOverlayTexture() {
			if( this.IsTypeHidden ) {
				return Main.confuseTexture;
			}
			if( this.BaseItem != null ) {
				return Main.itemTexture[ this.BaseItem.type ];
			}
			return null;
		}


		////////////////

		/*public override void PostDrawInInventory(
					SpriteBatch spriteBatch,
					Vector2 position,
					Rectangle frame,
					Color drawColor,
					Color itemColor,
					Vector2 origin,
					float scale ) {
			Texture2D containerItemTex = Main.itemTexture[ ModContent.ItemType<PowerupItem>() ];
			Texture2D overlayItemTex = this.GetOverlayTexture();
			if( overlayItemTex == null ) { return; }

			int hiDim = overlayItemTex.Width > overlayItemTex.Height ? overlayItemTex.Width : overlayItemTex.Height;
			float customScale = 32f / (float)hiDim;//0.75f;

			float overlayWid = overlayItemTex.Width * customScale;
			float overlayHei = overlayItemTex.Height * customScale;

			float halfContainerWid = ( (float)containerItemTex.Width * scale ) * 0.5f;
			float halfContainerHei = ( (float)containerItemTex.Height * scale ) * 0.5f;
			float halfOverlayWid = ( (float)overlayWid * scale ) * 0.5f;
			float halfOverlayHei = ( (float)overlayHei * scale ) * 0.5f;
			float posX = ( position.X + halfContainerWid ) - halfOverlayWid;
			float posY = ( position.Y + halfContainerHei ) - halfOverlayHei;
			//posY += 6f * scale;

			var srcRect = new Rectangle( 0, 0, overlayItemTex.Width, overlayItemTex.Height );
			float newScale = scale * customScale;

			Main.spriteBatch.Draw(
				texture: overlayItemTex,
				position: new Vector2(posX, posY),
				sourceRectangle: srcRect,
				color: Color.White,
				rotation: 0f,
				origin: default(Vector2),
				scale: newScale,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);
		}*/


		////

		private bool Glow = false;

		public override Color? GetAlpha( Color lightColor ) {
			this.Glow = !this.Glow;
			return this.Glow
				? Color.White * 0.8f
				: Color.White * 0.1f;
		}

		public override void PostDrawInWorld(
					SpriteBatch sb,
					Color lightColor,
					Color alphaColor,
					float rotation,
					float scale,
					int whoAmI ) {
			Texture2D containerItemTex = Main.itemTexture[ ModContent.ItemType<PowerupItem>() ];
			Texture2D overlayItemTex = this.GetOverlayTexture();
			if( overlayItemTex == null ) { return; }

			Vector2 center = this.item.Center - Main.screenPosition;

			int containerDim = containerItemTex.Width;
			int overlayDim = overlayItemTex.Width > overlayItemTex.Height
				? overlayItemTex.Width
				: overlayItemTex.Height;

			float overlayFitScale = ((float)containerDim * 0.65f) / (float)overlayDim;
			float overlayScale = overlayFitScale * scale;

			float halfOverlayWid = (float)overlayItemTex.Width * overlayScale * 0.5f;
			float halfOverlayHei = (float)overlayItemTex.Height * overlayScale * 0.5f;

			float posX = center.X - halfOverlayWid;
			float posY = center.Y - halfOverlayHei;

			sb.Draw(
				texture: overlayItemTex,
				position: new Vector2( posX, posY ),
				sourceRectangle: null,
				color: this.Glow
					? Color.White * 0.8f
					: Color.White * 0.1f,
				rotation: rotation,
				origin: default(Vector2),
				scale: overlayScale,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);
		}
	}
}