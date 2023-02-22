using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Input;
using Gw2Sharp.WebApi.V2.Models;
using Microsoft.Xna.Framework;
using Tortle.PlayerMarker.Models;
using Color = Gw2Sharp.WebApi.V2.Models.Color;

namespace Tortle.PlayerMarker.Views
{
	public class SettingsView : View
	{
		private Panel _colorPickerPanel;
		private ColorPicker _colorPicker;
		private ColorBox _settingPlayerMarkerColorBox;

		protected override void Build(Container buildPanel)
		{
			var parentPanel = new Panel()
			{
				CanScroll = false,
				Parent = buildPanel,
				Height = buildPanel.Height,
				HeightSizingMode = SizingMode.AutoSize,
				Width = 700, //bug? with buildPanel.Width changing to 40 after loading a different module settings and coming back.,
			};
			parentPanel.LeftMouseButtonPressed += delegate
			{
				if (_colorPickerPanel.Visible && !_colorPickerPanel.MouseOver &&
					!_settingPlayerMarkerColorBox.MouseOver)
				{
					_colorPickerPanel.Visible = false;
				}
			};

			_colorPickerPanel = new Panel()
			{
				Location = new Point(parentPanel.Width - 420 - 10, 10),
				Size = new Point(420, 255),
				Visible = false,
				ZIndex = 10,
				Parent = parentPanel,
				BackgroundTexture = PlayerMarkerModule.ModuleInstance.ContentsManager.GetTexture("155976.png"),
				ShowBorder = false,
			};
			var colorPickerBg = new Panel()
			{
				Location = new Point(15, 15),
				Size = new Point(_colorPickerPanel.Size.X - 35, _colorPickerPanel.Size.Y - 30),
				Parent = _colorPickerPanel,
				BackgroundTexture = PlayerMarkerModule.ModuleInstance.ContentsManager.GetTexture("buttondark.png"),
				ShowBorder = true,
			};
			_colorPicker = new ColorPicker()
			{
				Location = new Point(10, 10),
				CanScroll = false,
				Size = new Point(colorPickerBg.Size.X - 20, colorPickerBg.Size.Y - 20),
				Parent = colorPickerBg,
				ShowTint = false,
				Visible = true
			};
			_colorPicker.SelectedColorChanged += delegate
			{
				_colorPicker.AssociatedColorBox.Color = _colorPicker.SelectedColor;
				try
				{
					PlayerMarkerModule.ModuleInstance.SettingPlayerMarkerColor.Value =
						_colorPicker.SelectedColor.Name;
				}
				catch
				{
					PlayerMarkerModule.ModuleInstance.SettingPlayerMarkerColor.Value = "White0";
				}

				_colorPickerPanel.Visible = false;
			};
			_colorPicker.LeftMouseButtonPressed += delegate { _colorPickerPanel.Visible = false; };
			foreach (var color in ColorPresets.Colors)
			{
				_colorPicker.Colors.Add(ConvertColor(color.Key, color.Value));
			}

			var settingPlayerMarkerEnable_Label = new Label()
			{
				Location = new Point(10, 18),
				Width = 100,
				HorizontalAlignment = HorizontalAlignment.Right,
				WrapText = false,
				Parent = parentPanel,
				Text = "Enable Marker: ",
			};
			var settingPlayerMarkerEnable_Checkbox = new Checkbox()
			{
				Location = new Point(settingPlayerMarkerEnable_Label.Right + 5, settingPlayerMarkerEnable_Label.Top + 2),
				Parent = parentPanel,
				Checked = PlayerMarkerModule.ModuleInstance.SettingPlayerMarkerEnable.Value
			};
			settingPlayerMarkerEnable_Checkbox.CheckedChanged += delegate (object sender, CheckChangedEvent e)
			{
				PlayerMarkerModule.ModuleInstance.SettingPlayerMarkerEnable.Value = e.Checked;
			};

			var settingPlayerMarkerRadius_Label = new Label()
			{
				Location = new Point(settingPlayerMarkerEnable_Label.Left, settingPlayerMarkerEnable_Label.Bottom + 8),
				Width = 100,
				WrapText = false,
				Parent = parentPanel,
				Text = "Radius: ",
				HorizontalAlignment = HorizontalAlignment.Right,
			};
			var settingsPlayerMarkerRadius_Slider = new TrackBar()
			{
				Location =
					new Point(settingPlayerMarkerRadius_Label.Right + 5, settingPlayerMarkerRadius_Label.Top + 2),
				Width = 250,
				MaxValue = 40,
				MinValue = 1,
				Value = PlayerMarkerModule.ModuleInstance.SettingPlayerMarkerRadius.Value,
				Parent = parentPanel,
			};
			settingsPlayerMarkerRadius_Slider.ValueChanged += delegate (object sender, ValueEventArgs<float> args)
			{
				PlayerMarkerModule.ModuleInstance.SettingPlayerMarkerRadius.Value = args.Value;
			};

			var settingPlayerMarkerColor_Label = new Label()
			{
				Location = new Point(settingPlayerMarkerEnable_Label.Left, settingPlayerMarkerRadius_Label.Bottom + 8),
				Width = 100,
				WrapText = false,
				Parent = parentPanel,
				Text = "Color: ",
				HorizontalAlignment = HorizontalAlignment.Right,
			};
			_settingPlayerMarkerColorBox = new ColorBox()
			{
				Location = new Point(settingPlayerMarkerColor_Label.Right + 5, settingPlayerMarkerColor_Label.Top - 5),
				Parent = parentPanel,
				Color = ConvertColor(PlayerMarkerModule.ModuleInstance.SettingPlayerMarkerColor.Value,
					ColorPresets.Colors[PlayerMarkerModule.ModuleInstance.SettingPlayerMarkerColor.Value]),
			};
			_settingPlayerMarkerColorBox.Click += delegate (object sender, MouseEventArgs e)
			{
				_colorPicker.AssociatedColorBox = (ColorBox)sender;
				_colorPickerPanel.Visible = !_colorPickerPanel.Visible;
			};

			var settingPlayerMarkerOpacity_Label = new Label()
			{
				Location =
					new Point(settingPlayerMarkerEnable_Label.Left, _settingPlayerMarkerColorBox.Bottom + 8),
				Width = 100,
				WrapText = false,
				Parent = parentPanel,
				Text = "Opacity: ",
				HorizontalAlignment = HorizontalAlignment.Right,
			};
			var settingPlayerMarkerOpacity_Slider = new TrackBar()
			{
				Location = new Point(settingPlayerMarkerOpacity_Label.Right + 5,
					settingPlayerMarkerOpacity_Label.Top + 2),
				Width = 250,
				MaxValue = 100,
				MinValue = 0,
				Value = PlayerMarkerModule.ModuleInstance.SettingPlayerMarkerOpacity.Value * 100,
				Parent = parentPanel,
			};
			settingPlayerMarkerOpacity_Slider.ValueChanged += delegate
			{
				PlayerMarkerModule.ModuleInstance.SettingPlayerMarkerOpacity.Value =
					settingPlayerMarkerOpacity_Slider.Value / 100;
			};

			var settingPlayerMarkerVerticalOffset_Label = new Label()
			{
				Location = new Point(settingPlayerMarkerEnable_Label.Left, settingPlayerMarkerOpacity_Label.Bottom + 8),
				Width = 100,
				WrapText = false,
				Parent = parentPanel,
				Text = "Vertical Offset: ",
				HorizontalAlignment = HorizontalAlignment.Right,
			};
			var settingPlayerMarkerVerticalOffset_Slider = new TrackBar()
			{
				Location = new Point(settingPlayerMarkerVerticalOffset_Label.Right + 5, settingPlayerMarkerVerticalOffset_Label.Top + 2),
				Width = 250,
				MaxValue = 40,
				MinValue = 0,
				Value = (PlayerMarkerModule.ModuleInstance.SettingPlayerMarkerVerticalOffset.Value * 5) + 10,
				Parent = parentPanel,
			};
			settingPlayerMarkerVerticalOffset_Slider.ValueChanged += delegate
			{
				PlayerMarkerModule.ModuleInstance.SettingPlayerMarkerVerticalOffset.Value =
					(settingPlayerMarkerVerticalOffset_Slider.Value - 10) / 5;
			};

			var settingPlayerMarkerTextureLabel = new Label()
			{
				Location = new Point(settingPlayerMarkerEnable_Label.Left, settingPlayerMarkerVerticalOffset_Label.Bottom + 8),
				Width = 100,
				WrapText = false,
				Parent = parentPanel,
				Text = "Marker Image: ",
				HorizontalAlignment = HorizontalAlignment.Right,
			};
			var settingPlayerMarkerTexture_Select = new Dropdown()
			{
				Location = new Point(settingPlayerMarkerTextureLabel.Right + 5, settingPlayerMarkerTextureLabel.Top),
				Width = 250,
				Parent = parentPanel,
			};
			foreach (ImagePreset preset in Enum.GetValues(typeof(ImagePreset)))
			{
				settingPlayerMarkerTexture_Select.Items.Add(preset.ToDisplayString());
			}
			settingPlayerMarkerTexture_Select.SelectedItem = PlayerMarkerModule.ModuleInstance.SettingPlayerMarkerImage.Value.ToDisplayString();
			settingPlayerMarkerTexture_Select.ValueChanged += delegate {
				PlayerMarkerModule.ModuleInstance.SettingPlayerMarkerImage.Value = settingPlayerMarkerTexture_Select.SelectedItem.ToImagePreset();
			};

			var settingPlayerMarkerCustomImagePath_Label = new Label()
			{
				Location =
					new Point(settingPlayerMarkerEnable_Label.Left,
						settingPlayerMarkerTextureLabel.Bottom + 8),
				Width = 100,
				WrapText = false,
				Parent = parentPanel,
				Text = "Custom Image: ",
				HorizontalAlignment = HorizontalAlignment.Right,
			};
			var settingPlayerMarkerCustomImagePath_TextBox = new TextBox()
			{
				Location = new Point(settingPlayerMarkerCustomImagePath_Label.Right + 5,
					settingPlayerMarkerCustomImagePath_Label.Top),
				Width = 250,
				Text = PlayerMarkerModule.ModuleInstance.SettingPlayerMarkerCustomImagePath.Value,
				Parent = parentPanel,
			};

			settingPlayerMarkerCustomImagePath_TextBox.EnterPressed += delegate
			{
				PlayerMarkerModule.ModuleInstance.SettingPlayerMarkerCustomImagePath.Value =
					settingPlayerMarkerCustomImagePath_TextBox.Text;
			};
		}

		private static Color ConvertColor(string name, Microsoft.Xna.Framework.Color color)
		{
			return new Color()
			{
				Name = name,
				Cloth = new ColorMaterial() { Rgb = new int[] { color.R, color.G, color.B } }
			};
		}
	}
}
