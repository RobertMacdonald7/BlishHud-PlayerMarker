using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Tortle.PlayerMarker.Models
{
	internal static class ColorPresets
	{
		public static Dictionary<string, Color> Colors { get; } = new Dictionary<string, Color>();

		static ColorPresets()
		{
			Colors.Add("White0", new Color(255, 255, 255));
			Colors.Add("Red3", new Color(255, 191, 191));
			Colors.Add("Orange3", new Color(255, 223, 191));
			Colors.Add("Yellow3", new Color(255, 255, 191));
			Colors.Add("YellowGreen3", new Color(223, 255, 191));
			Colors.Add("Green3", new Color(191, 255, 191));
			Colors.Add("CyanGreen3", new Color(191, 255, 223));
			Colors.Add("Cyan3", new Color(191, 255, 255));
			Colors.Add("CyanBlue3", new Color(191, 223, 255));
			Colors.Add("Blue3", new Color(191, 191, 255));
			Colors.Add("MagentaBlue3", new Color(223, 191, 255));
			Colors.Add("Magenta3", new Color(255, 191, 255));
			Colors.Add("MagentaRed3", new Color(255, 191, 223));

			Colors.Add("White-1", new Color(212, 212, 212));
			Colors.Add("Red2", new Color(255, 127, 127));
			Colors.Add("Orange2", new Color(255, 191, 127));
			Colors.Add("Yellow2", new Color(255, 255, 127));
			Colors.Add("YellowGreen2", new Color(191, 255, 127));
			Colors.Add("Green2", new Color(127, 255, 127));
			Colors.Add("CyanGreen2", new Color(127, 255, 191));
			Colors.Add("Cyan2", new Color(127, 255, 255));
			Colors.Add("CyanBlue2", new Color(127, 191, 255));
			Colors.Add("Blue2", new Color(127, 127, 255));
			Colors.Add("MagentaBlue2", new Color(191, 127, 255));
			Colors.Add("Magenta2", new Color(255, 127, 255));
			Colors.Add("MagentaRed2", new Color(255, 127, 191));

			Colors.Add("White-2", new Color(169, 169, 169));
			Colors.Add("Red1", new Color(255, 63, 63));
			Colors.Add("Orange1", new Color(255, 159, 63));
			Colors.Add("Yellow1", new Color(255, 255, 63));
			Colors.Add("YellowGreen1", new Color(159, 255, 63));
			Colors.Add("Green1", new Color(63, 255, 63));
			Colors.Add("CyanGreen1", new Color(63, 255, 159));
			Colors.Add("Cyan1", new Color(63, 255, 255));
			Colors.Add("CyanBlue1", new Color(63, 159, 255));
			Colors.Add("Blue1", new Color(63, 63, 255));
			Colors.Add("MagentaBlue1", new Color(159, 63, 255));
			Colors.Add("Magenta1", new Color(255, 63, 255));
			Colors.Add("MagentaRed1", new Color(255, 63, 159));

			Colors.Add("White-3", new Color(127, 127, 127));
			Colors.Add("Red0", new Color(255, 0, 0));
			Colors.Add("Orange0", new Color(255, 127, 0));
			Colors.Add("Yellow0", new Color(255, 255, 0));
			Colors.Add("YellowGreen0", new Color(127, 255, 0));
			Colors.Add("Green0", new Color(0, 255, 0));
			Colors.Add("CyanGreen0", new Color(0, 255, 127));
			Colors.Add("Cyan0", new Color(0, 255, 255));
			Colors.Add("CyanBlue0", new Color(0, 127, 255));
			Colors.Add("Blue0", new Color(0, 0, 255));
			Colors.Add("MagentaBlue0", new Color(127, 0, 255));
			Colors.Add("Magenta0", new Color(255, 0, 255));
			Colors.Add("MagentaRed0", new Color(255, 0, 127));

			Colors.Add("White-4", new Color(85, 85, 85));
			Colors.Add("Red-1", new Color(191, 0, 0));
			Colors.Add("Orange-1", new Color(191, 95, 0));
			Colors.Add("Yellow-1", new Color(191, 191, 0));
			Colors.Add("YellowGreen-1", new Color(95, 191, 0));
			Colors.Add("Green-1", new Color(0, 191, 0));
			Colors.Add("CyanGreen-1", new Color(0, 191, 95));
			Colors.Add("Cyan-1", new Color(0, 191, 191));
			Colors.Add("CyanBlue-1", new Color(0, 95, 191));
			Colors.Add("Blue-1", new Color(0, 0, 191));
			Colors.Add("MagentaBlue-1", new Color(95, 0, 191));
			Colors.Add("Magenta-1", new Color(191, 0, 191));
			Colors.Add("MagentaRed-1", new Color(191, 0, 95));

			Colors.Add("White-5", new Color(42, 42, 42));
			Colors.Add("Red-2", new Color(127, 0, 0));
			Colors.Add("Orange-2", new Color(127, 63, 0));
			Colors.Add("Yellow-2", new Color(127, 127, 0));
			Colors.Add("YellowGreen-2", new Color(63, 127, 0));
			Colors.Add("Green-2", new Color(0, 127, 0));
			Colors.Add("CyanGreen-2", new Color(0, 127, 63));
			Colors.Add("Cyan-2", new Color(0, 127, 127));
			Colors.Add("CyanBlue-2", new Color(0, 63, 127));
			Colors.Add("Blue-2", new Color(0, 0, 127));
			Colors.Add("MagentaBlue-2", new Color(63, 0, 127));
			Colors.Add("Magenta-2", new Color(127, 0, 127));
			Colors.Add("MagentaRed-2", new Color(127, 0, 63));

			Colors.Add("White-6", new Color(0, 0, 0));
			Colors.Add("Red-3", new Color(63, 0, 0));
			Colors.Add("Orange-3", new Color(63, 31, 0));
			Colors.Add("Yellow-3", new Color(63, 63, 0));
			Colors.Add("YellowGreen-3", new Color(31, 63, 0));
			Colors.Add("Green-3", new Color(0, 63, 0));
			Colors.Add("CyanGreen-3", new Color(0, 63, 31));
			Colors.Add("Cyan-3", new Color(0, 63, 63));
			Colors.Add("CyanBlue-3", new Color(0, 31, 63));
			Colors.Add("Blue-3", new Color(0, 0, 63));
			Colors.Add("MagentaBlue-3", new Color(31, 0, 63));
			Colors.Add("Magenta-3", new Color(63, 0, 63));
			Colors.Add("MagentaRed-3", new Color(63, 0, 31));
		}
	}
}
