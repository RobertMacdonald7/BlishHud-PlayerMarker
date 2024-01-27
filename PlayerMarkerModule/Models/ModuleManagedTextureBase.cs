using Blish_HUD;
using Blish_HUD.Content;

namespace Tortle.PlayerMarker.Models
{
	internal abstract class ModuleManagedTextureBase
	{
		protected AsyncTexture2D _texture;

		public string Id { get; protected set; }


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
