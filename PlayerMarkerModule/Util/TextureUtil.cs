using System;
using System.IO;
using Blish_HUD;
using Microsoft.Xna.Framework.Graphics;

namespace Tortle.PlayerMarker.Util
{
	public static class TextureUtil
	{
		private static readonly Logger Logger = Logger.GetLogger(typeof(TextureUtil));

		/// <summary>
		/// Creates a Texture2D from a file path.
		/// <returns>A <see cref="Texture2D"/>; if unsuccessful, an error texture is returned.</returns>
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
			catch (UnauthorizedAccessException e)
			{
				Logger.Error(e, "Unable to access {file}", filePath);
				Blish_HUD.Debug.Contingency.NotifyFileSaveAccessDenied(filePath, "Unable to access file");
				texture = ContentService.Textures.Error;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Unable to create texture from {file}", filePath);
				texture = ContentService.Textures.Error;
			}

			return texture;
		}
	}
}
