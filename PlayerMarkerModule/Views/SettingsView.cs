﻿using System;
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
		private const int SliderWidth = 175;
		private static readonly Logger Logger = Logger.GetLogger(typeof(SettingsView));

		private readonly MarkerTextureManager _markerTextureManager;
		private readonly ModuleSettings _moduleSettings;
		private readonly ContentsManager _contentsManager;

		private Texture2D _panelBackgroundTexture;
		private Texture2D _buttonDarkTexture;

		#region UI margins

		private readonly Point _topLeft = new Point(10, 10);

		#endregion

		public SettingsView(MarkerTextureManager markerTextureManager, ModuleSettings moduleSettings,
			ContentsManager contentsManager)
		{
			_markerTextureManager = markerTextureManager;
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

			var topMiddleRight = new Point(parentPanel.Width / 2 - 10, 10);

			Logger.Debug("Building 'Enabled' setting controls");

			#region Enabled

			var enableCheckbox = new Checkbox()
			{
				Location = new Point(_topLeft.X, _topLeft.Y + 2),
				Parent = parentPanel,
				Checked = _moduleSettings.Enabled.Value,
				Width = 10,
			};

			var enableLabel = new Label()
			{
				Location = new Point(enableCheckbox.Right + 2, _topLeft.Y),
				AutoSizeWidth = true,
				WrapText = false,
				Parent = parentPanel,
				Text = _moduleSettings.Enabled.DisplayName,
				BasicTooltipText = _moduleSettings.Enabled.Description,
			};

			enableCheckbox.CheckedChanged += delegate (object sender, CheckChangedEvent e)
			{
				_moduleSettings.Enabled.Value = e.Checked;
			};

			#endregion

			Logger.Debug("Building 'Image' setting controls");

			#region Image

			var imageDescription = new Label()
			{
				Location = new Point(_topLeft.X, enableCheckbox.Bottom + 8),
				WrapText = true,
				Width = parentPanel.Width / 2 - _topLeft.X * 2,
				AutoSizeHeight = true,
				Parent = parentPanel,
				Text = Localization.ModuleSettings.PlayerMarkerImage_Description,
			};

			var imageLabel = new Label()
			{
				Location = new Point(_topLeft.X, imageDescription.Bottom + 8),
				AutoSizeWidth = true,
				WrapText = false,
				Parent = parentPanel,
				Text = _moduleSettings.ImageName.DisplayName,
				BasicTooltipText = _moduleSettings.ImageName.Description,
			};

			var imageSelect = new Dropdown()
			{
				Location = new Point(topMiddleRight.X - SliderWidth, imageLabel.Top),
				Width = SliderWidth,
				Parent = parentPanel,
			};

			foreach (var name in _markerTextureManager.GetNames())
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
				Location = new Point(topMiddleRight.X - SliderWidth, sizeLabel.Top + 2),
				Width = SliderWidth,
				MaxValue = 40,
				MinValue = 1,
				Value = _moduleSettings.Size.Value * 40,
				Parent = parentPanel,
			};

			sizeSlider.ValueChanged += delegate (object sender, ValueEventArgs<float> args)
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
				Location = new Point(topMiddleRight.X - SliderWidth, opacityLabel.Top + 2),
				Width = SliderWidth,
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
				Location = new Point(topMiddleRight.X - SliderWidth, verticalOffsetLabel.Top + 2),
				Width = SliderWidth,
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

			Logger.Debug("Building 'Color' setting controls");

			#region Color

			var colorBox = new ColorBox()
			{
				Location = new Point(_topLeft.X, verticalOffsetLabel.Bottom + 8)
				,
				Parent = parentPanel,
				Color = ConvertColor(_moduleSettings.Color.Value, ColorPresets.Colors[_moduleSettings.Color.Value]),
				Height = 20,
				Width = 20,
			};

			var colorLabel = new Label()
			{
				Location = new Point(colorBox.Right + 2, colorBox.Top),
				AutoSizeWidth = true,
				WrapText = false,
				Parent = parentPanel,
				Text = _moduleSettings.Color.DisplayName,
				BasicTooltipText = _moduleSettings.Color.Description,
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

			#region Duplicates

			var cleanUpDuplicatesButton = new StandardButton()
			{
				Location = new Point(_topLeft.X, colorBox.Bottom + 8),
				Width = 200,
				Parent = parentPanel,
				Text = Localization.ModuleSettings.CleanupDuplicates_Name,
				BasicTooltipText = string.Format(Localization.ModuleSettings.CleanupDuplicates_Tooltip, _markerTextureManager.GetDuplicateCount())
			};

			cleanUpDuplicatesButton.Click += delegate
			{
				_markerTextureManager.CleanupDuplicates();
				cleanUpDuplicatesButton.BasicTooltipText = string.Format(Localization.ModuleSettings.CleanupDuplicates_Tooltip, _markerTextureManager.GetDuplicateCount());
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
				Name = name,
				Cloth = new ColorMaterial() { Rgb = new int[] { color.R, color.G, color.B } },
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
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing) return;

			Logger.Debug("Disposing");
			_panelBackgroundTexture?.Dispose();
			_buttonDarkTexture?.Dispose();
		}
	}
}
