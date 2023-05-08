using System.IO;
using Blish_HUD;
using Blish_HUD.Content;

namespace Tortle.PlayerMarker.Models
{
	internal class CustomTexture : ITexture
	{
		private readonly string _filePath;
		private AsyncTexture2D _texture;

		public string Id { get; }

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

		public void Dispose()
		{
			_texture?.Dispose();
			_texture = null;
		}
	}
}
