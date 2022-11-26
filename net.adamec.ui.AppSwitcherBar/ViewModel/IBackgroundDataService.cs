using System.ComponentModel;
using net.adamec.ui.AppSwitcherBar.Dto;

namespace net.adamec.ui.AppSwitcherBar.ViewModel;

/// <summary>
/// Provides the data being retrieved on background - <see cref="InstalledApplications"/>
/// </summary>
public interface IBackgroundDataService : INotifyPropertyChanged
{
    /// <summary>
    /// Flag whether the background data have been retrieved 
    /// </summary>
    bool BackgroundDataRetrieved { get; }
    /// <summary>
    /// Information about the applications installed in system
    /// </summary>
    InstalledApplications InstalledApplications { get; }
    /// <summary>
    /// Refresh (reload) the background data
    /// </summary>
    void Refresh();
}