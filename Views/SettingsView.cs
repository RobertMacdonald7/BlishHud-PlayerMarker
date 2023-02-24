using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Input;
using Blish_HUD.Modules.Managers;
using Gw2Sharp.WebApi.V2.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tortle.PlayerMarker.Models;
using Tortle.PlayerMarker.Services;
using Color = Gw2Sharp.WebApi.V2.Models.Color;

namespace Tortle.PlayerMarker.Views
{
	/// <summary>
	/// A custom view containing all configurable module settings.
	/// </summary>
	internal class SettingsView : View, IDisposable
	{
		private static readonly Logger Logger = Logger.GetLogger(typeof(SettingsView));

		private readonly TextureCache _textureCache;
		private readonly ModuleSettings _moduleSettings;
		private readonly ContentsManager _contentsManager;

		private Texture2D _panelBackgroundTexture;
		private Texture2D _buttonDarkTexture;

		#region UI margins

		private readonly Point _topLeft = new Point(10, 18);

		#endregion

		public SettingsView(TextureCache textureCache, ModuleSettings moduleSettings,
			ContentsManager contentsManager)
		{
			_textureCache = textureCache;
			_moduleSettings = moduleSettings;
			_contentsManager = contentsManager;
		}

		protected override void Build(Container buildPanel)
		{
			Logger.Info("Building settings view");
			var parentPanel = new Panel()
			{
				CanScroll = false,
				Parent = buildPanel,
				Height = buildPanel.Height,
				HeightSizingMode = SizingMode.AutoSize,
				Width = buildPanel.Width,
			};

			Logger.Debug("Building 'Enabled' setting controls");
			#region Enabled

			var enableLabel = new Label()
			{
				Location = _topLeft,
				AutoSizeWidth = true,
				WrapText = false,
				Parent = parentPanel,
				Text = _moduleSettings.Enabled.DisplayName,
				BasicTooltipText = _moduleSettings.Enabled.Description,
			};

			var enableCheckbox = new Checkbox()
			{
				Location = new Point(enableLabel.Right + 5, enableLabel.Top + 2),
				Parent = parentPanel,
				Checked = _moduleSettings.Enabled.Value,
				Width = 10,
			};

			enableCheckbox.CheckedChanged += delegate(object sender, CheckChangedEvent e)
			{
				_moduleSettings.Enabled.Value = e.Checked;
			};

			#endregion

			Logger.Debug("Building 'Color' setting controls");
			#region Color

			var colorLabel = new Label()
			{
				Location = new Point(enableCheckbox.Right + 10, _topLeft.Y),
				AutoSizeWidth = true,
				WrapText = false,
				Parent = parentPanel,
				Text = _moduleSettings.Color.DisplayName,
				BasicTooltipText = _moduleSettings.Color.Description,
			};

			var colorBox = new ColorBox()
			{
				Location = new Point(colorLabel.Right + 5, colorLabel.Top - 5),
				Parent = parentPanel,
				Color = ConvertColor(_moduleSettings.Color.Value,
					ColorPresets.Colors[_moduleSettings.Color.Value]),
			};

			var colorPickerPanel = new Panel()
			{
				Location = new Point(colorBox.Right + 5, 0),
				Size = new Point(400, 235),
				Visible = false,
				ZIndex = 10,
				Parent = parentPanel,
				BackgroundTexture = _panelBackgroundTexture,
				ShowBorder = false,
			};

			var colorPickerBg = new Panel()
			{
				Location = new Point(15, 15),
				Size = new Point(colorPickerPanel.Size.X - 35, colorPickerPanel.Size.Y - 35),
				Parent = colorPickerPanel,
				BackgroundTexture = _buttonDarkTexture,
				ShowBorder = true,
			};

			var colorPicker = new ColorPicker()
			{
				Size = colorPickerBg.Size,
				CanScroll = false,
				Parent = colorPickerBg,
				ShowTint = false,
				Visible = true,
			};

			colorPicker.SelectedColorChanged += delegate (object sender, EventArgs args)
			{
				colorPicker.AssociatedColorBox.Color = colorPicker.SelectedColor;
				_moduleSettings.Color.Value = colorPicker.SelectedColor.Name;
			};

			colorPicker.Click += delegate
			{
				colorPickerPanel.Visible = false;
			};

			foreach (var color in ColorPresets.Colors)
			{
				colorPicker.Colors.Add(ConvertColor(color.Key, color.Value));
			}

			colorBox.Click += delegate (object sender, MouseEventArgs e)
			{
				colorPicker.AssociatedColorBox = (ColorBox)sender;
				colorPickerPanel.Visible = !colorPickerPanel.Visible;
			};

			parentPanel.LeftMouseButtonPressed += delegate
			{
				if (colorPickerPanel.Visible && !colorPickerPanel.MouseOver &&
					!colorBox.MouseOver)
				{
					colorPickerPanel.Visible = false;
				}
			};

			#endregion

			Logger.Debug("Building 'Image' setting controls");
			#region Image

			var imageLabel = new Label()
			{
				Location = new Point(_topLeft.X, colorBox.Bottom + 8),
				AutoSizeWidth = true,
				WrapText = false,
				Parent = parentPanel,
				Text = _moduleSettings.ImageName.DisplayName,
				BasicTooltipText = _moduleSettings.ImageName.Description,
			};

			var imageSelect = new Dropdown()
			{
				Location = new Point(imageLabel.Right + 5, imageLabel.Top),
				Width = 250,
				Parent = parentPanel,
			};

			foreach (var name in _textureCache.GetNames())
			{
				imageSelect.Items.Add(name);
			}

			imageSelect.SelectedItem = _moduleSettings.ImageName.Value;

			imageSelect.ValueChanged += delegate
			{
				_moduleSettings.ImageName.Value = imageSelect.SelectedItem;
			};

			#endregion

			Logger.Debug("Building 'Size' setting controls");
			#region Size

			var sizeLabel = new Label()
			{
				Location = new Point(_topLeft.X, imageLabel.Bottom + 8),
				AutoSizeWidth = true,
				WrapText = false,
				Parent = parentPanel,
				Text = _moduleSettings.Size.DisplayName,
				BasicTooltipText = _moduleSettings.Size.Description,
			};

			var sizeSlider = new TrackBar()
			{
				Location =
					new Point(sizeLabel.Right + 5, sizeLabel.Top + 2),
				Width = 250,
				MaxValue = 40,
				MinValue = 1,
				Value = _moduleSettings.Size.Value * 40,
				Parent = parentPanel,
			};

			sizeSlider.ValueChanged += delegate(object sender, ValueEventArgs<float> args)
			{
				_moduleSettings.Size.Value = args.Value / 40;
			};

			#endregion

			Logger.Debug("Building 'Opacity' setting controls");
			#region Opacity

			var opacityLabel = new Label()
			{
				Location =
					new Point(_topLeft.X, sizeLabel.Bottom + 8),
				AutoSizeWidth = true,
				WrapText = false,
				Parent = parentPanel,
				Text = _moduleSettings.Opacity.DisplayName,
				BasicTooltipText = _moduleSettings.Opacity.Description,
			};

			var opacitySlider = new TrackBar()
			{
				Location = new Point(opacityLabel.Right + 5,
					opacityLabel.Top + 2),
				Width = 250,
				MaxValue = 100,
				MinValue = 0,
				Value = _moduleSettings.Opacity.Value * 100,
				Parent = parentPanel,
			};

			opacitySlider.ValueChanged += delegate
			{
				_moduleSettings.Opacity.Value =
					opacitySlider.Value / 100;
			};

			#endregion

			Logger.Debug("Building 'Vertical Offset' setting controls");
			#region Vertical Offset

			var verticalOffsetLabel = new Label()
			{
				Location = new Point(_topLeft.X, opacityLabel.Bottom + 8),
				AutoSizeWidth = true,
				WrapText = false,
				Parent = parentPanel,
				Text = _moduleSettings.VerticalOffset.DisplayName,
				BasicTooltipText = _moduleSettings.VerticalOffset.Description,
			};

			var verticalOffsetSlider = new TrackBar()
			{
				Location = new Point(verticalOffsetLabel.Right + 5, verticalOffsetLabel.Top + 2),
				Width = 250,
				MaxValue = 60,
				MinValue = 0,
				Value = _moduleSettings.VerticalOffset.Value * 10,
				Parent = parentPanel,
			};

			verticalOffsetSlider.ValueChanged += delegate
			{
				_moduleSettings.VerticalOffset.Value = verticalOffsetSlider.Value / 10;
			};

			#endregion

			Logger.Info("Finished building settings view");
		}

		/// <summary>
		/// Converts a <see cref="Microsoft.Xna.Framework.Color"/> name string into a <see cref="Color"/>.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="color"></param>
		/// <returns>A <see cref="Color"/>.</returns>
		private static Color ConvertColor(string name, Microsoft.Xna.Framework.Color color)
		{
			return new Color()
			{
				Name = name, Cloth = new ColorMaterial() { Rgb = new int[] { color.R, color.G, color.B } },
			};
		}

		/// <summary>
		/// Loads all required textures.
		/// </summary>
		public void LoadTextures()
		{
			Logger.Info("Loading textures");
			_panelBackgroundTexture = _contentsManager.GetTexture("panelBackground.png");
			_buttonDarkTexture = _contentsManager.GetTexture("buttonDark.png");
		}

		public void Dispose()
		{
			Logger.Debug("Disposing");
			_panelBackgroundTexture?.Dispose();
			_buttonDarkTexture?.Dispose();
		}
	}
}
