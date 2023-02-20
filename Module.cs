using Blish_HUD;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tortle.PlayerMarker.Models;
using Tortle.PlayerMarker.Views;

namespace Tortle.PlayerMarker
{
	[Export(typeof(Blish_HUD.Modules.Module))]
	public class PlayerMarkerModule : Blish_HUD.Modules.Module
	{

		private static readonly Logger Logger = Logger.GetLogger<Module>();
		internal static PlayerMarkerModule ModuleInstance;

		#region Service Managers
		internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
		internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
		internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
		internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
		#endregion

		public enum Colors
		{
			White,
			Black,
			Red,
			Blue,
			Green,
			Cyan,
			Magenta,
			Yellow,
		}

		public static SettingEntry<bool> _settingPlayerMarkerEnable;
		public static SettingEntry<string> _settingPlayerMarkerColor;
		public static SettingEntry<float> _settingPlayerMarkerOpacity;
		public static SettingEntry<float> _settingPlayerMarkerRadius;
		public static SettingEntry<float> _settingPlayerMarkerVerticalOffset;

		private Control.PlayerMarker _playerMarker;

		private Texture2D _texturethick;
		private Texture2D _texturethin;
		private Texture2D _texturefill;
		public static List<Gw2Sharp.WebApi.V2.Models.Color> _colors = new List<Gw2Sharp.WebApi.V2.Models.Color>();


		[ImportingConstructor]
		public PlayerMarkerModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { ModuleInstance = this; }

		protected override void DefineSettings(SettingCollection settings)
		{
			_settingPlayerMarkerEnable = settings.DefineSetting("PlayerMarkerEnable", true, "Enabled", "");
			_settingPlayerMarkerColor = settings.DefineSetting("PlayerMarkerColor", "White0", "Color", "Color of the marker.");
			_settingPlayerMarkerOpacity = settings.DefineSetting("PlayerMarkerOpacity", 1f, "1Opacity", "Transparency of the marker.");
			_settingPlayerMarkerRadius = settings.DefineSetting("PlayerMarkerRadius", 10f, "Radius", "Radius of the marker.");

			_settingPlayerMarkerOpacity.SetRange(0f, 1f);

			_settingPlayerMarkerVerticalOffset = settings.DefineSetting("PlayerMarkerVerticalOffset", 2.5f, "Vertical Offset", "How high to offset the marker off the ground.");

			_settingPlayerMarkerEnable.SettingChanged += UpdateSettings_Enabled;
			_settingPlayerMarkerColor.SettingChanged += UpdateSettings_Color;
			_settingPlayerMarkerOpacity.SettingChanged += UpdateSettings_Opacity;
			_settingPlayerMarkerRadius.SettingChanged += UpdateSettings_Radius;
			_settingPlayerMarkerVerticalOffset.SettingChanged += UpdateSettings_VerticalOffset;
		}
		public override IView GetSettingsView() {
			return new SettingsView();
			//return new SettingsView( (this.ModuleParameters.SettingsManager.ModuleSettings);
		}

		private float DistToPx(float f)
		{
			return (float)(f * 2 * .0128);
		}
		protected override void Initialize()
		{
			_colors = new List<Gw2Sharp.WebApi.V2.Models.Color>();
			foreach (KeyValuePair<string, int[]> color in MyColors.Colors) {
				_colors.Add(new Gw2Sharp.WebApi.V2.Models.Color() { Name = color.Key, Cloth = new Gw2Sharp.WebApi.V2.Models.ColorMaterial() { Rgb = color.Value } });
			}

			_texturethin = ContentsManager.GetTexture("circlethin.png");
			_texturethick = ContentsManager.GetTexture("circlethick.png");
			_texturefill = ContentsManager.GetTexture("circlefill.png");

			_playerMarker = new Control.PlayerMarker();
			_playerMarker.MarkerTexture = _texturefill;
			GameService.Graphics.World.AddEntity(_playerMarker);

			UpdateSettings_Enabled();
			UpdateSettings_Radius();
			UpdateSettings_Color();
			UpdateSettings_Opacity();
			UpdateSettings_VerticalOffset();
		}

		private void UpdateSettings_Enabled(object sender = null, ValueChangedEventArgs<bool> e = null)
		{
			_playerMarker.Visible = _settingPlayerMarkerEnable.Value;
		}

		private void UpdateSettings_VerticalOffset(object sender = null, ValueChangedEventArgs<float> e = null)
		{
			_playerMarker.VerticalOffset = _settingPlayerMarkerVerticalOffset.Value;
		}

		private void UpdateSettings_Radius(object sender = null, ValueChangedEventArgs<float> e = null)
		{
			float diam = 0;
			diam = DistToPx(_settingPlayerMarkerRadius.Value);
			_playerMarker.Size = new Vector3(diam, diam, 0);
			_playerMarker.MarkerTexture = _texturefill;

			_playerMarker.UpdateRings();
		}
		private void UpdateSettings_Color(object sender = null, ValueChangedEventArgs<string> e = null)
		{
			_playerMarker.MarkerColor = ToRGB(_colors.Find(x => x.Name.Equals(_settingPlayerMarkerColor.Value)));

			_playerMarker.UpdateRings();
		}
		private void UpdateSettings_Opacity(object sender = null, ValueChangedEventArgs<float> e = null)
		{
			_playerMarker.MarkerOpacity = _settingPlayerMarkerOpacity.Value;
		}

		protected override async Task LoadAsync()
		{
		}

		protected override void OnModuleLoaded(EventArgs e)
		{
			base.OnModuleLoaded(e);
		}

		protected override void Update(GameTime gameTime)
		{
		}

		/// <inheritdoc />
		protected override void Unload()
		{
			_settingPlayerMarkerEnable.SettingChanged -= UpdateSettings_Enabled;
			_settingPlayerMarkerRadius.SettingChanged -= UpdateSettings_Radius;
			_settingPlayerMarkerColor.SettingChanged -= UpdateSettings_Color;
			_settingPlayerMarkerOpacity.SettingChanged -= UpdateSettings_Opacity;
			_settingPlayerMarkerVerticalOffset.SettingChanged -= UpdateSettings_VerticalOffset;

			GameService.Graphics.World.RemoveEntity(_playerMarker);

			ModuleInstance = null;
		}

		private Color ToRGB(Gw2Sharp.WebApi.V2.Models.Color color) {
			if (color == null)
				return new Color(255, 255, 255);
			else
				return new Color(color.Cloth.Rgb[0], color.Cloth.Rgb[1], color.Cloth.Rgb[2]);
		}

	}

}
