using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace net.adamec.ui.AppSwitcherBar.Dto
{
    /// <summary>
    /// Provides the information about a brush and color in Light and Dark theme
    /// </summary>
    public class BrushInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// Name of the resource
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Brush in Light theme
        /// </summary>
        private Brush lightBrush = Brushes.Transparent;

        /// <summary>
        /// Brush in Light theme
        /// </summary>
        public Brush LightBrush
        {
            get => lightBrush;
            set
            {
                if (Equals(value, lightBrush)) return;
                lightBrush = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Type of the <see cref="LightBrush"/>
        /// </summary>
        private string lightBrushType = "not set";
        /// <summary>
        /// Type of the <see cref="LightBrush"/>
        /// </summary>
        public string LightBrushType
        {
            get => lightBrushType;
            set
            {
                if (value == lightBrushType) return;
                lightBrushType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Color of the <see cref="LightBrush"/> (if is <see cref="SolidColorBrush"/>)
        /// </summary>
        private string? lightColor;
        /// <summary>
        /// Color of the <see cref="LightBrush"/> (if is <see cref="SolidColorBrush"/>)
        /// </summary>
        public string? LightColor
        {
            get => lightColor;
            set
            {
                if (value == lightColor) return;
                lightColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Brush in Dark theme
        /// </summary>
        private Brush darkBrush = Brushes.Transparent;
        /// <summary>
        /// Brush in Dark theme
        /// </summary>
        public Brush DarkBrush
        {
            get => darkBrush;
            set
            {
                if (Equals(value, darkBrush)) return;
                darkBrush = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Type of the <see cref="DarkBrush"/>
        /// </summary>
        private string darkBrushType = "not set";
        /// <summary>
        /// Type of the <see cref="DarkBrush"/>
        /// </summary>
        public string DarkBrushType
        {
            get => darkBrushType;
            set
            {
                if (value == darkBrushType) return;
                darkBrushType = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Color of the <see cref="DarkBrush"/> (if is <see cref="SolidColorBrush"/>)
        /// </summary>
        private string? darkColor;

        /// <summary>
        /// Color of the <see cref="DarkBrush"/> (if is <see cref="SolidColorBrush"/>)
        /// </summary>
        public string? DarkColor
        {
            get => darkColor;
            set
            {
                if (value == darkColor) return;
                darkColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="name">Name of the resource</param>
        /// <param name="brush">Brush from theme</param>
        /// <param name="isDarkTheme">Flag whether the theme is Dark</param>
        public BrushInfo(string name, Brush brush, bool isDarkTheme)
        {
            Name = name;
            SetBrush(brush, isDarkTheme);
        }

        /// <summary>
        /// Extract the <paramref name="brush"/> information
        /// </summary>
        /// <param name="brush">Brush</param>
        /// <param name="isDarkTheme">Flag whether the theme is Dark</param>
        public void SetBrush(Brush brush, bool isDarkTheme)
        {
            if (!isDarkTheme)
            {
                LightBrush = brush;
                LightBrushType = brush switch
                {
                    SolidColorBrush => "Solid",
                    GradientBrush => "Gradient",
                    _ => brush.GetType().Name
                };

                if (brush is not SolidColorBrush solidColorBrush) return;

                var color = solidColorBrush.Color;
                LightColor = $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
                return;
            }

            DarkBrush = brush;
            DarkBrushType = brush switch
            {
                SolidColorBrush => "Solid",
                GradientBrush => "Gradient",
                _ => brush.GetType().Name
            };

            if (brush is not SolidColorBrush darkSolidColorBrush) return;

            var dColor = darkSolidColorBrush.Color;
            DarkColor = $"#{dColor.A:X2}{dColor.R:X2}{dColor.G:X2}{dColor.B:X2}";
        }

        /// <summary>
        /// Gets string representation of current instance
        /// </summary>
        /// <returns>String representation of current instance</returns>
        public override string ToString()
        {
            return $"{Name}: {LightColor}/{LightBrushType}, {DarkColor}/{DarkBrushType}";
        }

        /// <summary>
        /// Occurs when a property value changes
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raise <see cref="PropertyChanged"/> event for given <paramref name="propertyName"/>
        /// </summary>
        /// <param name="propertyName">Name of the property changed</param>
        // ReSharper disable once UnusedMember.Global
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
