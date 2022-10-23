using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using net.adamec.ui.AppSwitcherBar.Wpf;

namespace net.adamec.ui.AppSwitcherBar.Dto.Search;

/// <summary>
/// Search result item base class
/// </summary>
public abstract class SearchResultItem:INotifyPropertyChanged
{
    /// <summary>
    /// Title of the result item
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Result item icon
    /// </summary>
    public BitmapSource? Icon { get; init; }

    /// <summary>
    /// Type of the search result item
    /// </summary>
    public SearchResultItemTypeEnum ItemType { get; init; }

    /// <summary>
    /// Type of the search result item
    /// </summary>
    public enum SearchResultItemTypeEnum
    {
        /// <summary>
        /// Unknown
        /// </summary>
        None,
        /// <summary>
        /// Header - just text
        /// </summary>
        Header,
        /// <summary>
        /// Application window
        /// </summary>
        Window,
        /// <summary>
        /// Pinned application
        /// </summary>
        PinnedApplication,
        /// <summary>
        /// Installed application
        /// </summary>
        InstalledApplication,
        /// <summary>
        /// Separator
        /// </summary>
        Separator,
        /// <summary>
        /// More items flag
        /// </summary>
        MoreItems
    }

    /// <summary>
    /// Flag whether the search result item behaves as separator in UI (menu showing the results)
    /// </summary>
    public virtual bool IsSeparator  => false;
    
    /// <summary>
    /// Command to be executed when the results is executed from UI
    /// </summary>
    public ICommand? Command { get; }

    //CTOR
    protected SearchResultItem()
    {
        Command = new RelayCommand(OnCommand, CanCommandExecute);
    }

    /// <summary>
    /// Returns the information whether the search result can be "executed" from UI (see <see cref="OnCommand"/>)
    /// </summary>
    /// <param name="parameter">Command parameter - not used</param>
    /// <returns>True when the search result can be executed from UI - when <see cref="IsSeparator"/> is false</returns>
    protected virtual bool CanCommandExecute(object? parameter)
    {
        return !IsSeparator;
    }

    /// <summary>
    /// Action to be run when the search result is "executed" from UI
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