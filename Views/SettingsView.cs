using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Input;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework.Graphics;

namespace Tortle.PlayerMarker.Views
{
    public class SettingsView : View
    {
        Panel colorPickerPanel;
        ColorPicker colorPicker;
        private ColorBox settingPlayerMarkerColor_Box;
        SettingEntry<string> colorBoxSelected = new SettingEntry<string>();

        protected override void Build(Container buildPanel) {
            var parentPanel = new Panel() {
                CanScroll = false,
                Parent = buildPanel,
                Height = buildPanel.Height,
                HeightSizingMode = SizingMode.AutoSize,
                Width = 700,  //bug? with buildPanel.Width changing to 40 after loading a different module settings and coming back.,
            };
            parentPanel.LeftMouseButtonPressed += delegate {
                if (colorPickerPanel.Visible && !colorPickerPanel.MouseOver && !settingPlayerMarkerColor_Box.MouseOver)
                    colorPickerPanel.Visible = false;
            };

            colorPickerPanel = new Panel() {
                Location = new Point(parentPanel.Width - 420 - 10, 10),
                Size = new Point(420, 255),
                Visible = false,
                ZIndex = 10,
                Parent = parentPanel,
                BackgroundTexture = PlayerMarkerModule.ModuleInstance.ContentsManager.GetTexture("155976.png"),
                ShowBorder = false,
            };
            var colorPickerBG = new Panel() {
                Location = new Point(15, 15),
                Size = new Point(colorPickerPanel.Size.X - 35, colorPickerPanel.Size.Y - 30),
                Parent = colorPickerPanel,
                BackgroundTexture = PlayerMarkerModule.ModuleInstance.ContentsManager.GetTexture("buttondark.png"),
                ShowBorder = true,
            };
            colorPicker = new ColorPicker() {
                Location = new Point(10, 10),
                CanScroll = false,
                Size = new Point(colorPickerBG.Size.X - 20, colorPickerBG.Size.Y - 20),
                Parent = colorPickerBG,
                ShowTint = false,
                Visible = true
            };
            colorPicker.SelectedColorChanged += delegate {
                colorPicker.AssociatedColorBox.Color = colorPicker.SelectedColor;
                try {
                    colorBoxSelected.Value = colorPicker.SelectedColor.Name;
                } catch {
                    colorBoxSelected.Value = "White0";
                }
                colorPickerPanel.Visible = false;
            };
            colorPicker.LeftMouseButtonPressed += delegate {
                colorPickerPanel.Visible = false;
            };
            foreach (var color in PlayerMarkerModule._colors) {
                colorPicker.Colors.Add(color);
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
            var settingPlayerMarkerEnable_Checkbox = new Checkbox() {
                Location = new Point(settingPlayerMarkerEnable_Label.Right + 5, settingPlayerMarkerEnable_Label.Top + 2),
                Parent = parentPanel,
                Checked = PlayerMarkerModule._settingPlayerMarkerEnable.Value
            };
            settingPlayerMarkerEnable_Checkbox.CheckedChanged += delegate {
                PlayerMarkerModule._settingPlayerMarkerEnable.Value = settingPlayerMarkerEnable_Checkbox.Checked;
            };

            var settingPlayerMarkerRadius_Label = new Label() {
                Location = new Point(settingPlayerMarkerEnable_Label.Left, settingPlayerMarkerEnable_Label.Bottom + 5),
                Width = 100,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Radius: ",
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            var settingsPlayerMarkerRadius_Slider = new TrackBar()
            {
	            Location = new Point(settingPlayerMarkerRadius_Label.Right + 5, settingPlayerMarkerRadius_Label.Top + 2),
	            Width = 250,
	            MaxValue = 40,
	            MinValue = 1,
	            Value = PlayerMarkerModule._settingPlayerMarkerRadius.Value,
	            Parent = parentPanel,
            };
            settingsPlayerMarkerRadius_Slider.ValueChanged += delegate
            {
	            PlayerMarkerModule._settingPlayerMarkerRadius.Value = settingsPlayerMarkerRadius_Slider.Value;
            };

            var settingPlayerMarkerColor_Label = new Label()
            {
	            Location = new Point(settingPlayerMarkerEnable_Label.Left, settingPlayerMarkerRadius_Label.Bottom + 5),
	            Width = 100,
	            AutoSizeHeight = false,
	            WrapText = false,
	            Parent = parentPanel,
	            Text = "Color: ",
	            HorizontalAlignment = HorizontalAlignment.Right,
            };
            settingPlayerMarkerColor_Box = new ColorBox() {
                Location = new Point(settingPlayerMarkerColor_Label.Right + 5, settingPlayerMarkerColor_Label.Top - 5),
                Parent = parentPanel,
                Color = PlayerMarkerModule._colors.Find(x => x.Name.Equals(PlayerMarkerModule._settingPlayerMarkerColor.Value)),
            };
            settingPlayerMarkerColor_Box.Click += delegate (object sender, MouseEventArgs e) {
                SetColorSetting(ref PlayerMarkerModule._settingPlayerMarkerColor);
                colorPicker.AssociatedColorBox = (ColorBox)sender;
                colorPickerPanel.Visible = !colorPickerPanel.Visible;
            };

            var settingPlayerMarkerOpacity_Label = new Label() {
                Location = new Point(settingPlayerMarkerEnable_Label.Left, settingPlayerMarkerColor_Box.Bottom + 5),
                Width = 100,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Opacity: ",
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            var settingPlayerMarkerOpacity_Slider = new TrackBar() {
                Location = new Point(settingPlayerMarkerOpacity_Label.Right + 5, settingPlayerMarkerOpacity_Label.Top + 2),
                Width = 250,
                MaxValue = 100,
                MinValue = 0,
                Value = PlayerMarkerModule._settingPlayerMarkerOpacity.Value * 100,
                Parent = parentPanel,
            };
            settingPlayerMarkerOpacity_Slider.ValueChanged += delegate { PlayerMarkerModule._settingPlayerMarkerOpacity.Value = settingPlayerMarkerOpacity_Slider.Value / 100; };

            var settingPlayerMarkerVerticalOffset_Label = new Label() {
                Location = new Point(settingPlayerMarkerEnable_Label.Left, settingPlayerMarkerOpacity_Label.Bottom + 5),
                Width = 100,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Vertical Offset: ",
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            var settingPlayerMarkerVerticalOffset_Slider = new TrackBar() {
                Location = new Point(settingPlayerMarkerVerticalOffset_Label.Right + 5, settingPlayerMarkerVerticalOffset_Label.Top + 2),
                Width = 250,
                MaxValue = 40,
                MinValue = 0,
                Value = (PlayerMarkerModule._settingPlayerMarkerVerticalOffset.Value * 5) + 10,
                Parent = parentPanel,
            };
            settingPlayerMarkerVerticalOffset_Slider.ValueChanged += delegate { PlayerMarkerModule._settingPlayerMarkerVerticalOffset.Value = (settingPlayerMarkerVerticalOffset_Slider.Value - 10) / 5; };
        }

        private void SetColorSetting(ref SettingEntry<string> setting) {
            colorBoxSelected = setting;
        }

    }
}
