using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using net.adamec.ui.AppSwitcherBar.WpfExt;

namespace net.adamec.ui.AppSwitcherBar.Dto.AppList;

/// <summary>
/// Application list item base class
/// </summary>
public abstract class AppListItem:INotifyPropertyChanged
{
    /// <summary>
    /// Title of the app list item
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// App list item icon
    /// </summary>
    public BitmapSource? Icon { get; init; }

    /// <summary>
    /// Type of the app list item
    /// </summary>
    public AppListItemTypeEnum ItemType { get; init; }

    /// <summary>
    /// Flag whether the app list item is visible
    /// </summary>
    private bool isVisible = true;

    /// <summary>
    /// Flag whether the app list item is visible
    /// </summary>
    public bool IsVisible
    {
        get => isVisible;
        set
        {
            if (isVisible != value)
            {
                isVisible=value;
                OnPropertyChanged();
            }
        }
    }
    /// <summary>
    /// Type of the app list item
    /// </summary>
    public enum AppListItemTypeEnum 
    {
        /// <summary>
        /// Unknown
        /// </summary>
        None,
        /// <summary>
        /// Category - first char of folder/name
        /// </summary>
        Category,
        /// <summary>
        /// Start menu folder
        /// </summary>
        Folder,
        /// <summary>
        /// Installed application or document ("non-application")
        /// </summary>
        InstalledItem,
    }

    /// <summary>
    /// Flag whether the app list item is executable
    /// </summary>
    public virtual bool IsExecutable  => false;
    
    /// <summary>
    /// Command to be executed when the item is executed from UI
    /// </summary>
    public ICommand? Command { get; }

    //CTOR
    protected AppListItem()
    {
        Command = new RelayCommand(OnCommand, CanCommandExecute);
    }

    /// <summary>
    /// Returns the information whether the app list item can be "executed" from UI (see <see cref="OnCommand"/>)
    /// </summary>
    /// <param name="parameter">Command parameter - not used</param>
    /// <returns>True when the app list item can be executed from UI - when <see cref="IsExecutable"/> and <see cref="IsVisible"/> are true</returns>
    protected virtual bool CanCommandExecute(object? parameter)
    {
        return IsVisible && IsExecutable;
    }

    /// <summary>
    /// Action to be run when the app list item is "executed" from UI
    /// </summary>
    /// <param name="parameter">Command parameter - not used</param>
    protected virtual void OnCommand(object? parameter)
    {

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