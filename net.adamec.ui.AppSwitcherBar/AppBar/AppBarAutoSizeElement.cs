namespace net.adamec.ui.AppSwitcherBar.AppBar;

/// <summary>
/// Used in attached property <see cref="AppBarWindow.BarAutoSizeProperty"/> to identify the UI elements to be taken into the consideration to auto sizing
/// </summary>
public enum AppBarAutoSizeElement
{
    /// <summary>
    /// Default value - not taken into consideration
    /// </summary>
    None,
    /// <summary>
    /// Add the width or height to target size
    /// </summary>
    Add,
    /// <summary>
    /// Measure this element ("main" element for auto size)
    /// </summary>
    Measure
}