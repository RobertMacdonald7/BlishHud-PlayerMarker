using System;
using Blish_HUD.Content;

namespace Tortle.PlayerMarker.Models
{
	internal class Gw2DatTexture : ITexture
	{
		private readonly int _assetId;
		private AsyncTexture2D _texture;

		public string Id { get; }

		public Gw2DatTexture(int assetId, string id)
		{
			_assetId = assetId;
			Id = id;
		}

		public AsyncTexture2D Get()
		{
			if (_texture != null)
			{
				return _texture;
			}

			return _texture = AsyncTexture2D.FromAssetId(_assetId);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing) return;

			// Blish handles disposal of these.
			_texture = null;
		}
	}
}
