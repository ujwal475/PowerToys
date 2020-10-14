using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ColorPicker.Controls
{
    /// <summary>
    /// Interaction logic for ColorPickerEditorPopup.xaml
    /// </summary>
    public partial class ColorPickerEditorPopup : Window
    {
        private Windows.UI.Xaml.Controls.ColorPicker _colorPickerControl;

        public ColorPickerEditorPopup(Windows.UI.Color initialColor)
        {
            InitializeComponent();
            SelectedColor = initialColor;
        }

        public Windows.UI.Color SelectedColor;

        private void colorPicker_ChildChanged(object sender, EventArgs e)
        {
            global::Microsoft.Toolkit.Wpf.UI.XamlHost.WindowsXamlHost windowsXamlHost =
               sender as global::Microsoft.Toolkit.Wpf.UI.XamlHost.WindowsXamlHost;

            _colorPickerControl = windowsXamlHost.GetUwpInternalObject() as global::Windows.UI.Xaml.Controls.ColorPicker;

            if (_colorPickerControl != null)
            {
                _colorPickerControl.Color = SelectedColor;
                _colorPickerControl.PointerCaptureLost += (s, e1) => {
                    SelectedColor = _colorPickerControl.Color;
                };
                this.LostFocus += ColorPickerPopupWindow_LostFocus;
            }
            else
            {
                this.LostFocus -= ColorPickerPopupWindow_LostFocus;
            }
        }

        private void ColorPickerPopupWindow_LostFocus(object sender, RoutedEventArgs e)
        {
         //   this.DialogResult = true;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedColor = _colorPickerControl.Color;
            this.DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
