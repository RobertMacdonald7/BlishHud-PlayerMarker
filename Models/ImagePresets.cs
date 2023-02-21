using System;

namespace Tortle.PlayerMarker.Models
{
	internal static class ImagePresets
	{
		private const string Gw2TargetString = "GW2 Target";
		private const string CircleFillString = "Circle Fill";
		private const string CircleThinString = "Circle Thin";
		private const string CircleThickString = "Circle Thick";
		private const string CustomString = "Custom";

		public static string ToDisplayString(this ImagePreset preset)
		{
			return preset switch
			{
				ImagePreset.Gw2Target => Gw2TargetString,
				ImagePreset.CircleFill => CircleFillString,
				ImagePreset.CircleThin => CircleThinString,
				ImagePreset.CircleThick => CircleThickString,
				ImagePreset.Custom => CustomString,
				_ => throw new ArgumentOutOfRangeException(nameof(preset), preset, null),
			};
		}

		public static ImagePreset ToImagePreset(this string presetString)
		{
			return presetString switch
			{
				Gw2TargetString => ImagePreset.Gw2Target,
				CircleFillString => ImagePreset.CircleFill,
				CircleThinString => ImagePreset.CircleThin,
				CircleThickString => ImagePreset.CircleThick,
				CustomString => ImagePreset.Custom,
				_ => throw new ArgumentOutOfRangeException(nameof(presetString), presetString, null),
			};
		}
	}

	public enum ImagePreset
	{
		Gw2Target,
		CircleFill,
		CircleThin,
		CircleThick,
		Custom
	}
}
