using System;
using Blish_HUD.Content;

namespace Tortle.PlayerMarker.Models
{
	internal interface ITexture : IDisposable
	{
		string Id { get; }

		AsyncTexture2D Get();
	}
}
