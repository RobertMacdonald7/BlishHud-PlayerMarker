using System;
using System.IO;
using System.Threading.Tasks;
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
		public async static Task<Texture2D> FromPathPremultipliedAsync(string filePath)
		{
			return await Task.Run(() =>
			{
				Texture2D texture;
				try
				{
					using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
					texture = Blish_HUD.TextureUtil.FromStreamPremultiplied(fs);
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
			});
		}
	}
}
