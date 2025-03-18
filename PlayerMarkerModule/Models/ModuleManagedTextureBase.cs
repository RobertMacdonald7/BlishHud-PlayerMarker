using System;
using Blish_HUD;
using Blish_HUD.Content;

namespace Tortle.PlayerMarker.Models
{
	internal abstract class ModuleManagedTextureBase : IDisposable
	{
		protected AsyncTexture2D _texture;

		public string Id { get; protected set; }


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing) return;

			// Don't dispose BlishHUD's error texture!
			if (_texture?.Texture != ContentService.Textures.Error)
			{
				_texture?.Dispose();
			}

			_texture = null;
		}
	}
}
