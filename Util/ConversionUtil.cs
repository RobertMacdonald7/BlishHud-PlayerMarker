using Microsoft.Xna.Framework;
using Tortle.PlayerMarker.Models;

namespace Tortle.PlayerMarker.Util
{
	internal static class ConversionUtil
	{
		/// <summary>
		/// Converts <paramref name="colorName"/> into a <see cref="Color"/>.
		/// </summary>
		/// <param name="colorName"></param>
		/// <returns>A <see cref="Color"/>; if unsuccessful, white is returned.</returns>
		public static Color ToRgb(string colorName)
		{
			var success = ColorPresets.Colors.TryGetValue(colorName, out var color);
			return success ? color : ColorPresets.Colors[ColorPresets.Default];
		}
	}
}
