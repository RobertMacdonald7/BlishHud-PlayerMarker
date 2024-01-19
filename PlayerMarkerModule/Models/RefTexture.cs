using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Modules.Managers;

namespace Tortle.PlayerMarker.Models
{
	internal class RefTexture : ITexture
	{
		private readonly ContentsManager _contentsManager;
		private AsyncTexture2D _texture;

		public string Id { get; }

		public RefTexture(ContentsManager contentsManager, string fileName)
		{
			_contentsManager = contentsManager;
			Id = fileName;
		}

		public AsyncTexture2D Get()
		{
			if (_texture != null)
			{
				return _texture;
			}

			_texture = new AsyncTexture2D();
			Task.Run(() => _contentsManager.GetTexture(Id)).ContinueWith(textureResponse =>
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
			// Don't dispose BlishHUD's error texture!
			if (_texture?.Texture != ContentService.Textures.Error)
			{
				_texture?.Dispose();
			}

			_texture = null;
		}
	}
}
