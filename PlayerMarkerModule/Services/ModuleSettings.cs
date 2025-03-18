using System;
using System.Linq;
using Blish_HUD;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Tortle.PlayerMarker.Models;
using Tortle.PlayerMarker.Util;

namespace Tortle.PlayerMarker.Services
{
	/// <summary>
	/// Setting definitions and event handlers.
	/// </summary>
	internal sealed class ModuleSettings : IDisposable
	{
		private static readonly Logger Logger = Logger.GetLogger(typeof(ModuleSettings));

		private readonly Entity.PlayerMarker _playerMarker;
		private readonly MarkerTextureManager _markerTextureManager;

		public SettingEntry<bool> Enabled { get; private set; }
		public SettingEntry<string> Color { get; private set; }
		public SettingEntry<float> Opacity { get; private set; }
		public SettingEntry<float> Size { get; private set; }
		public SettingEntry<float> VerticalOffset { get; private set; }
		public SettingEntry<string> ImageName { get; private set; }

		public ModuleSettings(MarkerTextureManager markerTextureManager, Entity.PlayerMarker playerMarker)
		{
			_markerTextureManager = markerTextureManager;
			_playerMarker = playerMarker;
		}

		/// <summary>
		/// Initializes the module's settings.
		/// </summary>
		/// <param name="settingCollection"></param>
		public void Initialize(SettingCollection settingCollection)
		{
			Logger.Info("Initializing");
			Enabled = settingCollection.DefineSetting("PlayerMarkerEnable", true,
				() => Localization.ModuleSettings.PlayerMarkerEnable_Name,
				() => Localization.ModuleSettings.PlayerMarkerEnable_Tooltip);
			Color = settingCollection.DefineSetting("PlayerMarkerColor", ColorPresets.Default,
				() => Localization.ModuleSettings.PlayerMarkerColor_Name,
				() => Localization.ModuleSettings.PlayerMarkerColor_Tooltip);
			Size = settingCollection.DefineSetting("PlayerMarkerSize", 0.25f,
				() => Localization.ModuleSettings.PlayerMarkerSize_Name,
				() => Localization.ModuleSettings.PlayerMarkerSize_Tooltip);
			Opacity = settingCollection.DefineSetting("PlayerMarkerOpacity", 1f,
				() => Localization.ModuleSettings.PlayerMarkerOpacity_Name,
				() => Localization.ModuleSettings.PlayerMarkerOpacity_Tooltip);
			VerticalOffset = settingCollection.DefineSetting("PlayerMarkerVerticalOffset", 2.5f,
				() => Localization.ModuleSettings.PlayerMarkerVerticalOffset_Name,
				() => Localization.ModuleSettings.PlayerMarkerVerticalOffset_Tooltip);
			ImageName = settingCollection.DefineSetting("PlayerMarkerImage",
				_markerTextureManager.DefaultTextures.First().Id,
				() => Localization.ModuleSettings.PlayerMarkerImage_Name,
				() => Localization.ModuleSettings.PlayerMarkerImage_Tooltip);

			Enabled.SettingChanged += UpdateSettings_Enabled;
			Color.SettingChanged += UpdateSettings_Color;
			Size.SettingChanged += UpdateSettings_Size;
			Opacity.SettingChanged += UpdateSettings_Opacity;
			VerticalOffset.SettingChanged += UpdateSettings_VerticalOffset;
			ImageName.SettingChanged += UpdateSettings_Image;
			_playerMarker.OnError += PlayerMarker_OnError;
		}

		public void Dispose()
		{
			Logger.Debug("Disposing");
			Enabled.SettingChanged -= UpdateSettings_Enabled;
			Size.SettingChanged -= UpdateSettings_Size;
			Color.SettingChanged -= UpdateSettings_Color;
			Opacity.SettingChanged -= UpdateSettings_Opacity;
			VerticalOffset.SettingChanged -= UpdateSettings_VerticalOffset;
			ImageName.SettingChanged -= UpdateSettings_Image;
			_playerMarker.OnError -= PlayerMarker_OnError;
		}

		private void UpdateSettings_Enabled(object sender, ValueChangedEventArgs<bool> e)
		{
			_playerMarker.Visible = Enabled.Value;
		}

		private void UpdateSettings_Size(object sender, ValueChangedEventArgs<float> e)
		{
			_playerMarker.Size = new Vector3(e.NewValue, e.NewValue, 0);
			_playerMarker.UpdateMarker();
		}

		private void UpdateSettings_Color(object sender, ValueChangedEventArgs<string> e)
		{
			_playerMarker.MarkerColor = ConversionUtil.ToRgb(e.NewValue);
			_playerMarker.UpdateMarker();
		}

		private void UpdateSettings_Opacity(object sender, ValueChangedEventArgs<float> e)
		{
			_playerMarker.MarkerOpacity = e.NewValue;
		}

		private void UpdateSettings_VerticalOffset(object sender, ValueChangedEventArgs<float> e)
		{
			_playerMarker.VerticalOffset = e.NewValue;
		}

		private void UpdateSettings_Image(object sender, ValueChangedEventArgs<string> e)
		{
			_playerMarker.MarkerTexture = _markerTextureManager.Get(e.NewValue);
		}

		private void PlayerMarker_OnError(object sender, Exception ex)
		{
			Logger.Warn("Disabling marker because entity threw an exception", ex);
			Enabled.Value = false;
		}
	}
}
