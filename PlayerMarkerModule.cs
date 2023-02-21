using Blish_HUD;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel.Composition;
using Tortle.PlayerMarker.Models;
using Tortle.PlayerMarker.Views;

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

		private Tortle.PlayerMarker.Controls.PlayerMarker _playerMarker;

		private Texture2D _textureCircleFill;

		[ImportingConstructor]
		public PlayerMarkerModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(
			moduleParameters)
		{
			ModuleInstance = this;
		}

		protected override void DefineSettings(SettingCollection settings)
		{
			SettingPlayerMarkerEnable = settings.DefineSetting("PlayerMarkerEnable", true, () => "Enabled", () => "");
			SettingPlayerMarkerColor = settings.DefineSetting("PlayerMarkerColor", "White0", () => "Color", () => "Color of the marker.");
			SettingPlayerMarkerRadius = settings.DefineSetting("PlayerMarkerRadius", 10f, () => "Radius", () => "Radius of the marker.");

			SettingPlayerMarkerOpacity = settings.DefineSetting("PlayerMarkerOpacity", 1f, () => "Opacity", () => "Transparency of the marker.");
			SettingPlayerMarkerOpacity.SetRange(0f, 1f);

			SettingPlayerMarkerVerticalOffset = settings.DefineSetting("PlayerMarkerVerticalOffset", 2.5f, () => "Vertical Offset", () => "How high to offset the marker off the ground.");

			SettingPlayerMarkerEnable.SettingChanged += UpdateSettings_Enabled;
			SettingPlayerMarkerColor.SettingChanged += UpdateSettings_Color;
			SettingPlayerMarkerRadius.SettingChanged += UpdateSettings_Radius;
			SettingPlayerMarkerOpacity.SettingChanged += UpdateSettings_Opacity;
			SettingPlayerMarkerVerticalOffset.SettingChanged += UpdateSettings_VerticalOffset;
		}

		public override IView GetSettingsView()
		{
			return new SettingsView();
		}

		protected override void Initialize()
		{
			_textureCircleFill = ContentsManager.GetTexture("circlefill.png");
			var diameterPx = DistToPx(SettingPlayerMarkerRadius.Value);

			_playerMarker = new Controls.PlayerMarker
			{
				Visible = SettingPlayerMarkerEnable.Value,
				MarkerColor = ToRgb(SettingPlayerMarkerColor.Value),
				MarkerOpacity = SettingPlayerMarkerOpacity.Value,
				MarkerTexture = _textureCircleFill,
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
			_playerMarker.MarkerTexture = _textureCircleFill;

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

		#endregion

		#region Helpers

		private static Color ToRgb(string colorName)
		{
			var success = MyColors.Colors.TryGetValue(colorName, out var color);
			return success ? color : MyColors.Colors["White0"];
		}

		private static float DistToPx(float f)
		{
			return (float)(f * 2 * .0128);
		}

		#endregion
	}
}
