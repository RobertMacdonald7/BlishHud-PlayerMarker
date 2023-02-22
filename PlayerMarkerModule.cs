using System.Collections.Generic;
using System.ComponentModel.Composition;
using Blish_HUD;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tortle.PlayerMarker.Models;
using Tortle.PlayerMarker.Views;
using TextureUtil = Tortle.PlayerMarker.Util.TextureUtil;

namespace Tortle.PlayerMarker
{
	[Export(typeof(Module))]
	public class PlayerMarkerModule : Module
	{
		private static readonly Logger Logger = Logger.GetLogger<PlayerMarkerModule>();
		internal static PlayerMarkerModule ModuleInstance;

		#region Service Managers

		internal SettingsManager SettingsManager => ModuleParameters.SettingsManager;
		internal ContentsManager ContentsManager => ModuleParameters.ContentsManager;
		internal DirectoriesManager DirectoriesManager => ModuleParameters.DirectoriesManager;
		internal Gw2ApiManager Gw2ApiManager => ModuleParameters.Gw2ApiManager;

		#endregion

		public SettingEntry<bool> SettingPlayerMarkerEnable { get; private set; }
		public SettingEntry<string> SettingPlayerMarkerColor { get; private set; }
		public SettingEntry<float> SettingPlayerMarkerOpacity { get; private set; }
		public SettingEntry<float> SettingPlayerMarkerRadius { get; private set; }
		public SettingEntry<float> SettingPlayerMarkerVerticalOffset { get; private set; }
		public SettingEntry<ImagePreset> SettingPlayerMarkerImage { get; private set; }
		public SettingEntry<string> SettingPlayerMarkerCustomImagePath { get; private set; }

		private Entity.PlayerMarker _playerMarker;

		private readonly Dictionary<ImagePreset, Texture2D> _texturePresets = new Dictionary<ImagePreset, Texture2D>();

		[ImportingConstructor]
		public PlayerMarkerModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(
			moduleParameters)
		{
			ModuleInstance = this;
		}

		protected override void DefineSettings(SettingCollection settings)
		{
			SettingPlayerMarkerEnable = settings.DefineSetting("PlayerMarkerEnable", true, () => "Enabled", () => "");
			SettingPlayerMarkerColor = settings.DefineSetting("PlayerMarkerColor", "White0", () => "Color",
				() => "Color of the marker.");
			SettingPlayerMarkerRadius = settings.DefineSetting("PlayerMarkerRadius", 10f, () => "Radius",
				() => "Radius of the marker.");

			SettingPlayerMarkerOpacity = settings.DefineSetting("PlayerMarkerOpacity", 1f, () => "Opacity",
				() => "Transparency of the marker.");
			SettingPlayerMarkerOpacity.SetRange(0f, 1f);

			SettingPlayerMarkerVerticalOffset = settings.DefineSetting("PlayerMarkerVerticalOffset", 2.5f,
				() => "Vertical Offset", () => "How high to offset the marker off the ground.");
			SettingPlayerMarkerCustomImagePath = settings.DefineSetting("PlayerMarkerCustomImagePath", "",
				() => "Custom marker file path",
				() => "The path to a custom image. Supported formats include: .bmp, .gif, .jpg, .png, .tif. and .dds");

			SettingPlayerMarkerImage = settings.DefineSetting("PlayerMarkerImage", ImagePreset.Gw2Target, () => "Marker image", () => "Displayed marker image.");

			SettingPlayerMarkerEnable.SettingChanged += UpdateSettings_Enabled;
			SettingPlayerMarkerColor.SettingChanged += UpdateSettings_Color;
			SettingPlayerMarkerRadius.SettingChanged += UpdateSettings_Radius;
			SettingPlayerMarkerOpacity.SettingChanged += UpdateSettings_Opacity;
			SettingPlayerMarkerVerticalOffset.SettingChanged += UpdateSettings_VerticalOffset;
			SettingPlayerMarkerImage.SettingChanged += UpdateSettings_Texture;
			SettingPlayerMarkerCustomImagePath.SettingChanged += UpdateSettings_CustomImagePath;
		}

		public override IView GetSettingsView()
		{
			return new SettingsView();
		}

		protected override void Initialize()
		{
			_texturePresets.Add(ImagePreset.Gw2Target, ContentsManager.GetTexture("gw2target.png"));
			_texturePresets.Add(ImagePreset.CircleThin, ContentsManager.GetTexture("circlethin.png"));
			_texturePresets.Add(ImagePreset.CircleThick, ContentsManager.GetTexture("circlethick.png"));
			_texturePresets.Add(ImagePreset.CircleFill, ContentsManager.GetTexture("circlefill.png"));
			_texturePresets.Add(ImagePreset.Custom, TextureUtil.FromPathPremultiplied(SettingPlayerMarkerCustomImagePath.Value, _texturePresets[ImagePreset.Gw2Target]));

			var diameterPx = DistToPx(SettingPlayerMarkerRadius.Value);

			_playerMarker = new Entity.PlayerMarker
			{
				Visible = SettingPlayerMarkerEnable.Value,
				MarkerColor = ToRgb(SettingPlayerMarkerColor.Value),
				MarkerOpacity = SettingPlayerMarkerOpacity.Value,
				MarkerTexture = _texturePresets[SettingPlayerMarkerImage.Value],
				Size = new Vector3(diameterPx, diameterPx, 0),
				VerticalOffset = SettingPlayerMarkerVerticalOffset.Value,
			};

			_playerMarker.UpdateMarker();

			GameService.Graphics.World.AddEntity(_playerMarker);
		}

		protected override void Unload()
		{
			SettingPlayerMarkerEnable.SettingChanged -= UpdateSettings_Enabled;
			SettingPlayerMarkerRadius.SettingChanged -= UpdateSettings_Radius;
			SettingPlayerMarkerColor.SettingChanged -= UpdateSettings_Color;
			SettingPlayerMarkerOpacity.SettingChanged -= UpdateSettings_Opacity;
			SettingPlayerMarkerVerticalOffset.SettingChanged -= UpdateSettings_VerticalOffset;
			SettingPlayerMarkerImage.SettingChanged -= UpdateSettings_Texture;

			GameService.Graphics.World.RemoveEntity(_playerMarker);

			ModuleInstance = null;
		}

		#region SettingChanged

		private void UpdateSettings_Enabled(object sender, ValueChangedEventArgs<bool> e)
		{
			_playerMarker.Visible = SettingPlayerMarkerEnable.Value;
		}

		private void UpdateSettings_VerticalOffset(object sender, ValueChangedEventArgs<float> e)
		{
			_playerMarker.VerticalOffset = e.NewValue;
		}

		private void UpdateSettings_Radius(object sender, ValueChangedEventArgs<float> e)
		{
			var diameterPx = DistToPx(e.NewValue);
			_playerMarker.Size = new Vector3(diameterPx, diameterPx, 0);

			_playerMarker.UpdateMarker();
		}

		private void UpdateSettings_Color(object sender, ValueChangedEventArgs<string> e)
		{
			_playerMarker.MarkerColor = ToRgb(e.NewValue);
			_playerMarker.UpdateMarker();
		}

		private void UpdateSettings_Opacity(object sender, ValueChangedEventArgs<float> e)
		{
			_playerMarker.MarkerOpacity = e.NewValue;
		}

		private void UpdateSettings_Texture(object sender, ValueChangedEventArgs<ImagePreset> e)
		{
			_playerMarker.MarkerTexture = _texturePresets[e.NewValue];
			_playerMarker.UpdateMarker();
		}


		private void UpdateSettings_CustomImagePath(object sender, ValueChangedEventArgs<string> e)
		{
			_texturePresets[ImagePreset.Custom] = TextureUtil.FromPathPremultiplied(e.NewValue, _texturePresets[ImagePreset.Gw2Target]);

			if (SettingPlayerMarkerImage.Value == ImagePreset.Custom)
			{
				_playerMarker.MarkerTexture = _texturePresets[ImagePreset.Custom];
			}
		}

		#endregion

		#region Helpers

		private static Color ToRgb(string colorName)
		{
			var success = ColorPresets.Colors.TryGetValue(colorName, out var color);
			return success ? color : ColorPresets.Colors["White0"];
		}

		private static float DistToPx(float f)
		{
			return (float)(f * 2 * .0128);
		}

		#endregion
	}
}
