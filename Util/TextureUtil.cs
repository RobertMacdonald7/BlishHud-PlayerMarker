using System.IO;
using System.Linq;
using Blish_HUD;
using Microsoft.Xna.Framework.Graphics;

namespace Tortle.PlayerMarker.Util
{
	public static class TextureUtil
	{
		/// <summary>
		/// Creates a Texture2D from a file path.
		/// </summary>
		public static Texture2D FromPathPremultiplied(string filePath)
		{
			Texture2D texture;
			try
			{
				using var fs = File.OpenRead(filePath);
				using var ctx = GameService.Graphics.LendGraphicsDeviceContext();
				texture = Blish_HUD.TextureUtil.FromStreamPremultiplied(ctx.GraphicsDevice, fs);
			}
			catch
			{
				texture = ContentService.Textures.Error;
			}

			return texture;
		}
	}
}
