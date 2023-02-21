using System.IO;
using System.Linq;
using Blish_HUD;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Tortle.PlayerMarker.Util
{
	public static class TextureUtil
	{
		/// <summary>
		/// Creates a Texture2D from a file path.
		/// If the path is not valid, or the image failed to be loaded, <paramref name="fallbackTexture"/> is returned
		/// </summary>
		public static Texture2D FromPathPremultiplied(string filePath, Texture2D fallbackTexture)
		{
			if (!IsValidTextureFile(filePath))
			{
				return fallbackTexture;
			}

			Texture2D texture = null;
			try
			{
				using var fs = File.OpenRead(filePath);
				using var ctx = GameService.Graphics.LendGraphicsDeviceContext();
				texture = Blish_HUD.TextureUtil.FromStreamPremultiplied(ctx.GraphicsDevice, fs);
			}
			catch
			{
				// ignored
			}

			return texture == ContentService.Textures.Error ? fallbackTexture : texture;
		}

		private static bool IsValidTextureFile(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				return false;
			}

			if (!File.Exists(filePath))
			{
				return false;
			}

			var invalidChars = Path.GetInvalidPathChars();
			var containsInvalidChars = filePath.IndexOfAny(invalidChars) >= 0;

			if (containsInvalidChars)
			{
				return false;
			}

			var extension = Path.GetExtension(filePath);
			if (string.IsNullOrEmpty(extension))
			{
				return false;
			}

			var supportedExtensions = new[] {".bmp", ".gif", ".jpg", ".png", ".tif", ".dds"};
			var extensionSupported = supportedExtensions.Contains(extension);

			return extensionSupported;
		}
	}
}
