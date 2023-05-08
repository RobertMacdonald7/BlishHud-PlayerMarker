using System.IO;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Modules.Managers;
using Tortle.PlayerMarker.Services;

namespace Tortle.PlayerMarker.Models
{
	internal class RefTexture : ITexture
	{
		private static readonly Logger Logger = Logger.GetLogger(typeof(MarkerTextureManager));

		private readonly ContentsManager _contentsManager;
		private AsyncTexture2D _texture;

		public string Id { get; }

		public RefTexture(ContentsManager contentsManager, string directory, string fileName)
		{
			_contentsManager = contentsManager;
			Id = fileName;
		}

		public AsyncTexture2D Get()
		{
			if (_texture != null)
			{
				Logger.Debug("{filename} returning cached texture", Id);
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
				Logger.Debug("{filename} texture loaded", Id);
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
