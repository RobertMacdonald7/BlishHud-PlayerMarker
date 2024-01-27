using Blish_HUD;
using Blish_HUD.Content;

namespace Tortle.PlayerMarker.Models
{
	internal class CustomTexture : ModuleManagedTextureBase, ITexture
	{
		private readonly string _filePath;

		public CustomTexture(string filePath, string fileName)
		{
			Id = fileName;
			_filePath = filePath;
		}

		public AsyncTexture2D Get()
		{
			if (_texture != null)
			{
				return _texture;
			}

			_texture = new AsyncTexture2D();
			Util.TextureUtil.FromPathPremultipliedAsync(_filePath).ContinueWith(textureResponse =>
			{
				var loadedTexture = ContentService.Textures.Error;

				if (textureResponse.Exception == null)
				{
					loadedTexture = textureResponse.Result;
				}

				_texture.SwapTexture(loadedTexture);
			});

			return _texture;
		}
	}
}
