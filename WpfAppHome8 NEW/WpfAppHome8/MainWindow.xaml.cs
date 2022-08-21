using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace WpfAppHome8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool left, right;
        Polyline polyline;
        BrushConverter brushConverter;
        Point position;
        UIElement saveElement;

        public MainWindow()
        {
            InitializeComponent();
            StartupSetting();
        }

        private void StartupSetting()
        {
            // Button
            buttonBrush.BorderBrush = Brushes.Green;

            // Brush
            sliderBrushSize.Value = 10;
            sliderBrushOpacity.Value = 1;

            // Eraser
            sliderEraserSize.Value = 10;
            sliderEraserOpacity.Value = 1;

            // Colors
            brushConverter = new BrushConverter();
            colorpickerOne.SelectedColor = Colors.Black;
            colorpickerTwo.SelectedColor = Colors.White;

            position = new Point();
            saveElement = new UIElement();
        }

        // Buttons
        private void ButtonBorderBrush(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                BrushConverter brushConverter = new BrushConverter();
                foreach (Button item in uniformgridInstruments.Children) // Check buttons
                {
                    if (item == button)
                        button.BorderBrush = Brushes.Green;
                    else
                        item.BorderBrush = (Brush)brushConverter.ConvertFromString("#FF707070"); // Get original color BorderBrush
                }

                switch (((Button)sender).Name)
                {
                    case "buttonBrush": popupBrush.IsOpen = true; break;
                    case "buttonEraser": popupEraser.IsOpen = true; break;
                    case "buttonInstruments": popupInstruments.IsOpen = true; break;
                }
            }
        }

        // Popup Open
        private void buttonBrush_MouseEnter(object sender, MouseEventArgs e)
        {
            popupBrush.IsOpen = false;
            if (buttonBrush.BorderBrush == Brushes.Green)
                popupBrush.IsOpen = true;
        }

        private void buttonEraser_MouseEnter(object sender, MouseEventArgs e)
        {
            popupEraser.IsOpen = false;
            if (buttonEraser.BorderBrush == Brushes.Green)
                popupEraser.IsOpen = true;
        }

        private void buttonInstruments_MouseEnter(object sender, MouseEventArgs e)
        {
            popupInstruments.IsOpen = false;
            if (buttonInstruments.BorderBrush == Brushes.Green)
                popupInstruments.IsOpen = true;
        }

        // Popup Close
        private void borderpopupBrush_MouseLeave(object sender, MouseEventArgs e)
        {
            popupBrush.IsOpen = false;
        }

        private void borderpopupEraser_MouseLeave(object sender, MouseEventArgs e)
        {
            popupEraser.IsOpen = false;
        }

        private void borderpopupInstruments_MouseLeave(object sender, MouseEventArgs e)
        {
            popupInstruments.IsOpen = false;
        }

        // Brush Size
        private void sliderBrushSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string[] array = sliderBrushSize.Value.ToString().Split('.');
            labelBrushSize.Content = $"Size: {array[0]}";
        }

        // Brush Opacity
        private void sliderBrushOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            labelBrushOpacityInPopup.Content = $"Opacity: ";
            int count = 0;
            foreach (char item in sliderBrushOpacity.Value.ToString())
            {
                if (count < 3)
                    labelBrushOpacityInPopup.Content += item.ToString();
                else
                    break;
                count++;
            }
        }

        private bool CanDraw(double x, double y)
        {
            return x > 0 + sliderBrushSize.Value / 3 && x < canvasMain.ActualWidth - sliderBrushSize.Value / 3 &&
                   y > 0 + sliderBrushSize.Value / 3 && y < canvasMain.ActualHeight - sliderBrushSize.Value / 3 ? true : false;
        }

        private void AddPolylineOnCanvas()
        {
            polyline = new Polyline();
            polyline.StrokeLineJoin = PenLineJoin.Round;
            polyline.StrokeStartLineCap = PenLineCap.Round;
            polyline.StrokeEndLineCap = PenLineCap.Round;

            canvasMain.Children.Add(polyline);
        }

        // Pressed and Released on Border (Canvas)
        // Left
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            left = true;
            AddPolylineOnCanvas();
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            left = false;
        }

        // Right
        private void Border_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            right = true;
            AddPolylineOnCanvas();
        }

        private void Border_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            right = false;
        }

        // Colors
        private Brush GetColors()
        {
            if (left)
            {
                return (Brush)brushConverter.ConvertFromString(colorpickerOne.SelectedColor.Value.ToString());
            }
            else if (right)
            {
                return (Brush)brushConverter.ConvertFromString(colorpickerTwo.SelectedColor.Value.ToString());
            }
            return Brushes.Transparent;
        }

        private void figure_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle)
            {
                if (buttonInstruments.BorderBrush == Brushes.Green)
                {
                    Rectangle rectangle = new Rectangle()
                    {
                        Stroke = GetColors(),
                        StrokeThickness = 2,
                        Width = 100,
                        Height = 100,
                        Fill = Brushes.Black
                    };
                    rectangle.MouseDown += Rectangle_MouseDown;
                    canvasMain.Children.Add(rectangle);
                }
            }
            if (sender is Ellipse)
            {
                if (buttonInstruments.BorderBrush == Brushes.Green)
                {
                    Ellipse rectangle = new Ellipse()
                    {
                        Stroke = GetColors(),
                        StrokeThickness = 2,
                        Width = 100,
                        Height = 100,
                        Fill = Brushes.Black
                    };
                    rectangle.MouseDown += Rectangle_MouseDown;
                    canvasMain.Children.Add(rectangle);
                }
            }
        }

        // Canvas
        private void canvasMain_MouseMove(object sender, MouseEventArgs e)
        {
            position = e.GetPosition(canvasMain);
            if (CanDraw(position.X, position.Y))
            {
                if (left || right)
                {
                    if (buttonBrush.BorderBrush == Brushes.Green)
                    {
                        polyline.Points.Add(position);
                        polyline.StrokeThickness = sliderBrushSize.Value;
                        polyline.Opacity = sliderBrushOpacity.Value;
                        polyline.Stroke = GetColors();
                    }
                    if (buttonEraser.BorderBrush == Brushes.Green)
                    {
                        polyline.Points.Add(position);
                        polyline.StrokeThickness = sliderEraserSize.Value;
                        polyline.Opacity = sliderEraserOpacity.Value;
                        polyline.Stroke = (Brush)brushConverter.ConvertFromString(Colors.White.ToString());
                    }
                    if (buttonInstruments.BorderBrush == Brushes.Green)
                    {
                        if (left || right)
                        {
                            Canvas.SetLeft(saveElement, position.X);
                            Canvas.SetTop(saveElement, position.Y);
                        }
                    }
                }
            }
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            saveElement = (UIElement)sender;
        }

        private void buttonpopupText_Click(object sender, RoutedEventArgs e)
        {
            FontFamilyConverter fontFamilyConverter = new FontFamilyConverter();
            TextBox textBox = new TextBox()
            {
                FontSize = 15,
                FontFamily = (FontFamily)fontFamilyConverter.ConvertFromString("Consolas"),
                Height = 20,
                Width = 200,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                BorderBrush = Brushes.Transparent,
                Background = Brushes.Transparent
            };
            textBox.TextChanged += TextBox_TextChanged;
            canvasMain.Children.Add(textBox);
            saveElement = textBox;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            saveElement = (UIElement)sender;

            var tempTextBox = ((TextBox)saveElement).Text.Split('\n');
            if (tempTextBox.Length > 1)
                ((TextBox)saveElement).Height = ((TextBox)saveElement).ExtentHeight;
        }

        // Eraser Size
        private void sliderEraserSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string[] array = sliderEraserSize.Value.ToString().Split('.');
            labelEraserSize.Content = $"Size: {array[0]}";
        }

        // Eraser Opacity
        private void sliderEraserOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            labelEraserOpacity.Content = $"Opacity: ";
            int count = 0;
            foreach (char item in sliderEraserOpacity.Value.ToString())
            {
                if (count < 3)
                    labelEraserOpacity.Content += item.ToString();
                else
                    break;
                count++;
            }
        }

        private void GetSelectedColor(Label label, object sender)
        {
            label.Background = (Brush)brushConverter.ConvertFromString(((ColorPicker)sender).SelectedColor.Value.ToString());
        }

        // Label Preview Colors
        private void colorpickerOne_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            GetSelectedColor(labelCurrentColor, sender);
        }

        private void colorpickerTwo_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            GetSelectedColor(labelCurrentColorTwo, sender);
        }

        // Fill
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (buttonFill.BorderBrush == Brushes.Green)
            {
                canvasMain.Children.Clear();
                canvasMain.Background = GetColors();
            }
            if (buttonInstruments.BorderBrush == Brushes.Green)
            {
                foreach (var item in canvasMain.Children)
                {
                    if (item == saveElement)
                    {
                        if (item is Rectangle rectangle)
                        {
                            rectangle.Fill = GetColors();
                        }
                        if (item is Ellipse ellipse)
                        {
                            ellipse.Fill = GetColors();
                        }
                    }
                }
                if (saveElement is TextBox)
                {
                    Canvas.SetLeft(saveElement, position.X);
                    Canvas.SetTop(saveElement, position.Y);
                }
            }
        }

    }
}