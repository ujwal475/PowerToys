using ColorPicker.Helpers;
using ColorPicker.Settings;
using Microsoft.Toolkit.Wpf.UI.XamlHost;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Windows.UI;

namespace ColorPicker.Controls
{
    /// <summary>
    /// Interaction logic for ColorPickerEditorWindow.xaml
    /// </summary>
    public partial class ColorPickerEditorWindow : Window
    {
        private readonly IUserSettings _userSettings;
        private readonly AppStateHandler _appStateHandler;
        private WindowsXamlHost _windowsXamlHost;
        private ColorPickerEditorUI.ColorPickerEditor _editorControl;
        private Dispatcher _dispatcher;
        private bool _initializing = false;

        public ColorPickerEditorWindow(IUserSettings userSettings, AppStateHandler appStateHandler)
        {
            _userSettings = userSettings;
            _appStateHandler = appStateHandler;
            _dispatcher = Application.Current.MainWindow.Dispatcher;
            InitializeComponent();
            IsVisibleChanged += ColorPickerEditorWindow_IsVisibleChanged;
        }

        private void ColorPickerEditorWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                if(_editorControl != null)
                {
                    _editorControl.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                }
            }
        }

        private void colorPickerEditor_ChildChanged(object sender, EventArgs e)
        {
            if (_windowsXamlHost == null)
            {
                _windowsXamlHost =
                sender as WindowsXamlHost;
            }

            _editorControl = _windowsXamlHost.GetUwpInternalObject() as ColorPickerEditorUI.ColorPickerEditor;

            if (_editorControl != null)
            {
                _editorControl.PickedColors.CollectionChanged += PickedColors_CollectionChanged;
                _editorControl.OpenColorPickerPopupClicked += UserControl_OpenColorPickerPopupClicked;
                _editorControl.ColorCopied += EditorControl_ColorCopied;
                _editorControl.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            }
        }

        public void Initialize()
        {
            InitializeColors(_editorControl, _userSettings);
        }

        private void EditorControl_ColorCopied(object sender, string copiedColor)
        {
            if (!string.IsNullOrEmpty(copiedColor))
            {
                ClipboardHelper.CopyToClipboard(copiedColor);
            }
        }

        private void PickedColors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(!_initializing)
            {
                _userSettings.ColorHistory.Clear();
                foreach (var item in _editorControl.PickedColors)
                {
                    _userSettings.ColorHistory.Add(item.A + "|" + item.R + "|" + item.G + "|" + item.B);
                }
            }
        }
        private void InitializeColors(ColorPickerEditorUI.ColorPickerEditor editor, IUserSettings userSettings)
        {
            _initializing = true;
            editor.PickedColors.Clear();
            {
                foreach (var item in userSettings.ColorHistory)
                {
                    var parts = item.Split('|');
                    editor.PickedColors.Add(new Color()
                    {
                        A = byte.Parse(parts[0]),
                        R = byte.Parse(parts[1]),
                        G = byte.Parse(parts[2]),
                        B = byte.Parse(parts[3])
                    });
                }
                editor.SelectFirstColor();
            }

            _initializing = false;
        }

        private void UserControl_OpenColorPickerPopupClicked(object sender, Color initialColor)
        {
            Task.Run(async () =>
            {
                await Task.Delay(10);
                _dispatcher.Invoke(() =>
                {
                    var window = new ColorPickerEditorPopup(initialColor);
                    window.Owner = this;
                    if (window.ShowDialog() == true)
                    {
                        _editorControl.AddColor(window.SelectedColor);
                    }
                });
            });
        }

        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void colorPickerButton_Click(object sender, RoutedEventArgs e)
        {
            _appStateHandler.ShowColorPicker();
            Hide();
        }
    }
}
