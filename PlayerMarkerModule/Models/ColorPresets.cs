using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Tortle.PlayerMarker.Models
{
	internal static class ColorPresets
	{
		public static Dictionary<string, Color> Colors { get; } = new Dictionary<string, Color>();
		public const string Default = "White";

		static ColorPresets()
		{
			Colors.Add("White", new Color(255, 255, 255));
			Colors.Add("Red0", new Color(255, 191, 191));
			Colors.Add("Orange0", new Color(255, 223, 191));
			Colors.Add("Yellow0", new Color(255, 255, 191));
			Colors.Add("YellowGreen0", new Color(223, 255, 191));
			Colors.Add("Green0", new Color(191, 255, 191));
			Colors.Add("CyanGreen0", new Color(191, 255, 223));
			Colors.Add("Cyan0", new Color(191, 255, 255));
			Colors.Add("CyanBlue0", new Color(191, 223, 255));
			Colors.Add("Blue0", new Color(191, 191, 255));
			Colors.Add("MagentaBlue0", new Color(223, 191, 255));
			Colors.Add("Magenta0", new Color(255, 191, 255));
			Colors.Add("MagentaRed0", new Color(255, 191, 223));

			Colors.Add("Gray1", new Color(212, 212, 212));
			Colors.Add("Red1", new Color(255, 127, 127));
			Colors.Add("Orange1", new Color(255, 191, 127));
			Colors.Add("Yellow1", new Color(255, 255, 127));
			Colors.Add("YellowGreen1", new Color(191, 255, 127));
			Colors.Add("Green1", new Color(127, 255, 127));
			Colors.Add("CyanGreen1", new Color(127, 255, 191));
			Colors.Add("Cyan1", new Color(127, 255, 255));
			Colors.Add("CyanBlue1", new Color(127, 191, 255));
			Colors.Add("Blue1", new Color(127, 127, 255));
			Colors.Add("MagentaBlue1", new Color(191, 127, 255));
			Colors.Add("Magenta1", new Color(255, 127, 255));
			Colors.Add("MagentaRed1", new Color(255, 127, 191));

			Colors.Add("Gray2", new Color(169, 169, 169));
			Colors.Add("Red2", new Color(255, 63, 63));
			Colors.Add("Orange2", new Color(255, 159, 63));
			Colors.Add("Yellow2", new Color(255, 255, 63));
			Colors.Add("YellowGreen2", new Color(159, 255, 63));
			Colors.Add("Green2", new Color(63, 255, 63));
			Colors.Add("CyanGreen2", new Color(63, 255, 159));
			Colors.Add("Cyan2", new Color(63, 255, 255));
			Colors.Add("CyanBlue2", new Color(63, 159, 255));
			Colors.Add("Blue2", new Color(63, 63, 255));
			Colors.Add("MagentaBlue2", new Color(159, 63, 255));
			Colors.Add("Magenta2", new Color(255, 63, 255));
			Colors.Add("MagentaRed2", new Color(255, 63, 159));

			Colors.Add("Gray3", new Color(127, 127, 127));
			Colors.Add("Red3", new Color(255, 0, 0));
			Colors.Add("Orange3", new Color(255, 127, 0));
			Colors.Add("Yellow3", new Color(255, 255, 0));
			Colors.Add("YellowGreen3", new Color(127, 255, 0));
			Colors.Add("Green3", new Color(0, 255, 0));
			Colors.Add("CyanGreen3", new Color(0, 255, 127));
			Colors.Add("Cyan3", new Color(0, 255, 255));
			Colors.Add("CyanBlue3", new Color(0, 127, 255));
			Colors.Add("Blue3", new Color(0, 0, 255));
			Colors.Add("MagentaBlue3", new Color(127, 0, 255));
			Colors.Add("Magenta3", new Color(255, 0, 255));
			Colors.Add("MagentaRed3", new Color(255, 0, 127));

			Colors.Add("Gray4", new Color(85, 85, 85));
			Colors.Add("Red4", new Color(191, 0, 0));
			Colors.Add("Orange4", new Color(191, 95, 0));
			Colors.Add("Yellow4", new Color(191, 191, 0));
			Colors.Add("YellowGreen4", new Color(95, 191, 0));
			Colors.Add("Green4", new Color(0, 191, 0));
			Colors.Add("CyanGreen4", new Color(0, 191, 95));
			Colors.Add("Cyan4", new Color(0, 191, 191));
			Colors.Add("CyanBlue4", new Color(0, 95, 191));
			Colors.Add("Blue4", new Color(0, 0, 191));
			Colors.Add("MagentaBlue4", new Color(95, 0, 191));
			Colors.Add("Magenta4", new Color(191, 0, 191));
			Colors.Add("MagentaRed4", new Color(191, 0, 95));

			Colors.Add("Gray5", new Color(42, 42, 42));
			Colors.Add("Red5", new Color(127, 0, 0));
			Colors.Add("Orange5", new Color(127, 63, 0));
			Colors.Add("Yellow5", new Color(127, 127, 0));
			Colors.Add("YellowGreen5", new Color(63, 127, 0));
			Colors.Add("Green5", new Color(0, 127, 0));
			Colors.Add("CyanGreen5", new Color(0, 127, 63));
			Colors.Add("Cyan5", new Color(0, 127, 127));
			Colors.Add("CyanBlue5", new Color(0, 63, 127));
			Colors.Add("Blue5", new Color(0, 0, 127));
			Colors.Add("MagentaBlue5", new Color(63, 0, 127));
			Colors.Add("Magenta5", new Color(127, 0, 127));
			Colors.Add("MagentaRed5", new Color(127, 0, 63));

			Colors.Add("Black", new Color(0, 0, 0));
			Colors.Add("Red6", new Color(63, 0, 0));
			Colors.Add("Orange6", new Color(63, 31, 0));
			Colors.Add("Yellow6", new Color(63, 63, 0));
			Colors.Add("YellowGreen6", new Color(31, 63, 0));
			Colors.Add("Green6", new Color(0, 63, 0));
			Colors.Add("CyanGreen6", new Color(0, 63, 31));
			Colors.Add("Cyan6", new Color(0, 63, 63));
			Colors.Add("CyanBlue6", new Color(0, 31, 63));
			Colors.Add("Blue6", new Color(0, 0, 63));
			Colors.Add("MagentaBlue6", new Color(31, 0, 63));
			Colors.Add("Magenta6", new Color(63, 0, 63));
			Colors.Add("MagentaRed6", new Color(63, 0, 31));
		}
	}
}
