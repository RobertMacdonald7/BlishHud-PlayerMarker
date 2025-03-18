using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Modules;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Tortle.PlayerMarker.Models;
using Tortle.PlayerMarker.Services;
using Tortle.PlayerMarker.Util;
using Tortle.PlayerMarker.Views;

namespace Tortle.PlayerMarker
{
	/// <summary>
	/// A module that displays a configurable image above the player's head.
	/// </summary>
	[Export(typeof(Module))]
	public class PlayerMarkerModule : Module
	{
		private static readonly Logger Logger = Logger.GetLogger<PlayerMarkerModule>();

		private readonly Entity.PlayerMarker _playerMarker;
		private readonly MarkerTextureManager _markerTextureManager;
		private readonly ModuleSettings _moduleSettings;
		private readonly SettingsView _settingsView;

		[ImportingConstructor]
		public PlayerMarkerModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(
			moduleParameters)
		{
			var contentsManager = ModuleParameters.ContentsManager;
			var directoriesManager = ModuleParameters.DirectoriesManager;

			_playerMarker = new Entity.PlayerMarker();
			_markerTextureManager = new MarkerTextureManager(directoriesManager, contentsManager);
			_moduleSettings = new ModuleSettings(_markerTextureManager, _playerMarker);
			_settingsView = new SettingsView(_markerTextureManager, _moduleSettings, contentsManager);
		}

		protected override void DefineSettings(SettingCollection settings)
		{
			Logger.Info("Initializing settings");
			_moduleSettings.Initialize(settings);
		}

		public override IView GetSettingsView()
		{
			Logger.Info("Returning settings view");
			return _settingsView;
		}

		protected override Task LoadAsync()
		{
			Logger.Info("Loading module");

			_markerTextureManager.Load();

			NormalizeSettings();

			_settingsView.Initialize();

			GameService.Graphics.World.AddEntity(_playerMarker);
			return Task.CompletedTask;
		}

		protected override void OnModuleLoaded(EventArgs e)
		{
			var diameterPx = _moduleSettings.Size.Value;
			_playerMarker.Visible = _moduleSettings.Enabled.Value;
			_playerMarker.MarkerColor = ConversionUtil.ToRgb(_moduleSettings.Color.Value);
			_playerMarker.MarkerOpacity = _moduleSettings.Opacity.Value;
			_playerMarker.MarkerTexture = _markerTextureManager.Get(_moduleSettings.ImageName.Value);
			_playerMarker.Size = new Vector3(diameterPx, diameterPx, 0);
			_playerMarker.VerticalOffset = _moduleSettings.VerticalOffset.Value;
			Logger.Info("Marker properties set");

			_playerMarker.UpdateMarker();

			Logger.Info("Finished loading module");
			base.OnModuleLoaded(e);
		}

		protected override void Unload()
		{
			_moduleSettings.Dispose();
			_markerTextureManager.Dispose();
			_settingsView.Dispose();

			GameService.Graphics.World.RemoveEntity(_playerMarker);
			_playerMarker.Dispose();
		}

		private void NormalizeSettings()
		{
			// If the file got removed, reset the setting to default
			if (!_markerTextureManager.ContainsKey(_moduleSettings.ImageName.Value))
			{
				Logger.Warn("Resetting {setting} setting back to default", nameof(_moduleSettings.ImageName));
				_moduleSettings.ImageName.Value = _markerTextureManager.DefaultTextures.First().Id;
			}

			// If the color isn't supported, reset it to default
			if (!ColorPresets.Colors.ContainsKey(_moduleSettings.Color.Value))
			{
				Logger.Warn("Resetting {setting} setting back to default", nameof(_moduleSettings.Color));
				_moduleSettings.Color.Value = ColorPresets.Default;
			}
		}
	}
}
