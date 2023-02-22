using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
		public SettingEntry<string> SettingPlayerMarkerImage { get; private set; }

		private Entity.PlayerMarker _playerMarker;

		public IReadOnlyDictionary<string, Texture2D> MarkerTextures => _markerTextures;

		private Dictionary<string, Texture2D> _markerTextures;

		private readonly string[] _defaultMarkerFileNames = { "gw2target.png", "circlethin.png", "circlethick.png", "circlefill.png" };
		private readonly string[] _supportedFileFormats = { ".bmp", ".gif", ".jpg", ".png", ".tif", ".dds" };

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
			SettingPlayerMarkerImage = settings.DefineSetting("PlayerMarkerImage", _defaultMarkerFileNames[0], () => "Marker image", () => "Displayed marker image.");

			SettingPlayerMarkerEnable.SettingChanged += UpdateSettings_Enabled;
			SettingPlayerMarkerColor.SettingChanged += UpdateSettings_Color;
			SettingPlayerMarkerRadius.SettingChanged += UpdateSettings_Radius;
			SettingPlayerMarkerOpacity.SettingChanged += UpdateSettings_Opacity;
			SettingPlayerMarkerVerticalOffset.SettingChanged += UpdateSettings_VerticalOffset;
			SettingPlayerMarkerImage.SettingChanged += UpdateSettings_Image;
		}

		public override IView GetSettingsView()
		{
			return new SettingsView();
		}

		protected override void Initialize()
		{
			_playerMarker = new Entity.PlayerMarker();
			_markerTextures = new Dictionary<string, Texture2D>();
		}

		protected override async Task LoadAsync()
		{
			await LoadPresets();

			GameService.Graphics.World.AddEntity(_playerMarker);
		}

		protected override void OnModuleLoaded(EventArgs e)
		{
			var diameterPx = DistToPx(SettingPlayerMarkerRadius.Value);

			// if the file got removed, reset the setting to default
			SettingPlayerMarkerImage.Value = MarkerTextures.ContainsKey(SettingPlayerMarkerImage.Value)
				? SettingPlayerMarkerImage.Value
				: _defaultMarkerFileNames[0];

			_playerMarker.Visible = SettingPlayerMarkerEnable.Value;
			_playerMarker.MarkerColor = ToRgb(SettingPlayerMarkerColor.Value);
			_playerMarker.MarkerOpacity = SettingPlayerMarkerOpacity.Value;
			_playerMarker.MarkerTexture = MarkerTextures[SettingPlayerMarkerImage.Value];
			_playerMarker.Size = new Vector3(diameterPx, diameterPx, 0);
			_playerMarker.VerticalOffset = SettingPlayerMarkerVerticalOffset.Value;

			_playerMarker.UpdateMarker();

			base.OnModuleLoaded(e);
		}

		protected override void Unload()
		{
			SettingPlayerMarkerEnable.SettingChanged -= UpdateSettings_Enabled;
			SettingPlayerMarkerRadius.SettingChanged -= UpdateSettings_Radius;
			SettingPlayerMarkerColor.SettingChanged -= UpdateSettings_Color;
			SettingPlayerMarkerOpacity.SettingChanged -= UpdateSettings_Opacity;
			SettingPlayerMarkerVerticalOffset.SettingChanged -= UpdateSettings_VerticalOffset;
			SettingPlayerMarkerImage.SettingChanged -= UpdateSettings_Image;

			foreach (var texturePreset in MarkerTextures)
			{
				texturePreset.Value.Dispose();
			}

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

		private void UpdateSettings_Image(object sender, ValueChangedEventArgs<string> e)
		{
			_playerMarker.MarkerTexture = MarkerTextures[e.NewValue];
		}

		#endregion

		#region Helpers

		private async Task LoadPresets()
		{
			var markersPath = DirectoriesManager.GetFullDirectoryPath("tortle/playermarkers");

			foreach (var fileName in _defaultMarkerFileNames)
			{
				using var fs = ContentsManager.GetFileStream(fileName);
				using var wfs = File.Create(Path.Combine(markersPath, fileName));
				await fs.CopyToAsync(wfs);
			}

			foreach (var file in Directory.GetFiles(markersPath))
			{
				if (!_supportedFileFormats.Contains(Path.GetExtension(file)))
				{
					continue;
				}

				_markerTextures.Add(Path.GetFileName(file), TextureUtil.FromPathPremultiplied(file));
			}
		}

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
