using net.adamec.ui.AppSwitcherBar.Dto.Search;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace net.adamec.ui.AppSwitcherBar.Dto;

/// <summary>
/// Information about the button representing the window (task bar application) or pinned application
/// </summary>
public abstract class ButtonInfo : INotifyPropertyChanged, ISearchable
{

    /// <summary>
    /// Returns the name of ButtonInfo concrete type (can be used in XAML instead of "is")
    /// </summary>
    public Type Type => GetType();

    /// <summary>
    /// Returns itself (can be used in XAML for casting)
    /// </summary>
    public ButtonInfo Self => this;

    /// <summary>
    /// Button title
    /// </summary>
    private string title;
    /// <summary>
    /// Window title
    /// </summary>
    public string Title
    {
        get => title;
        set
        {
            if (title != value)
            {
                title = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Icon bitmap source (if available)
    /// </summary>
    private BitmapSource? bitmapSource;
    /// <summary>
    /// Icon bitmap source (if available)
    /// </summary>
    public BitmapSource? BitmapSource
    {
        get => bitmapSource;
        set
        {
            if (bitmapSource != value)
            {
                bitmapSource = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Application executable
    /// </summary>
    public string? Executable { get; }

    /// <summary>
    /// ApplicationUserModelId
    /// </summary>
    private string? appId;
    /// <summary>
    /// ApplicationUserModelId
    /// </summary>
    public string? AppId
    {
        get => appId;
        set
        {
            if (appId != value)
            {
                appId = value;
                UpdateGroup();
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Some of the shell properties
    /// </summary>
    private ShellPropertiesSubset shellProperties = new();

    /// <summary>
    /// Some of the shell properties
    /// </summary>
    public ShellPropertiesSubset ShellProperties
    {
        get => shellProperties;
        set
        {
            if (shellProperties != value)
            {
                shellProperties = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Search sort key
    /// </summary>
    public virtual string SearchSortKey
    {
        get
        {
            var modificationTime = DateTime.MinValue;

            if (!string.IsNullOrEmpty(Executable) && File.Exists(Executable))
            {
                modificationTime = File.GetLastWriteTime(Executable);
            }
            if (ShellProperties.IsStoreApp && !string.IsNullOrEmpty(ShellProperties.PackageInstallPath) && Directory.Exists(ShellProperties.PackageInstallPath))
            {
                modificationTime = Directory.GetLastWriteTime(ShellProperties.PackageInstallPath);
            }

            return RunStats.BuildStandardSearchSortKey(modificationTime);
        }
    }

    /// <summary>
    /// Application run statistics
    /// </summary>
    public RunStats RunStats { get; } = new();

    /// <summary>
    /// Identifier of the <see cref="Group"/> before the change by <see cref="UpdateGroup"/> after <see cref="AppId"/> change
    /// </summary>
    public string OldGroup { get; private set; } = string.Empty;

    /// <summary>
    /// Identifier of the "group" the window belongs to.
    /// It's used for grouping the window buttons of the same application together
    /// Priority: <see cref="AppId"/>, <see cref="Executable"/>, <see cref="GroupFallback"/>
    /// </summary>
    public string Group { get; private set; } = string.Empty;


    /// <summary>
    /// <see cref="Group"/> identifier fallback when neither AppId not Executable is known or not set yet
    /// </summary>
    protected abstract string GroupFallback { get; }

    /// <summary>
    /// Updates the <see cref="Group"/> identified
    /// Priority: <see cref="AppId"/>, <see cref="Executable"/>, <see cref="GroupFallback"/>
    /// </summary>
    private void UpdateGroup()
    {
        OldGroup = Group;
        var group =
            !string.IsNullOrEmpty(AppId) ? AppId : //includes the "Executable option" as the Executable is the fallback source for AppId whenever the AppId is set
            !string.IsNullOrEmpty(Executable) ? Executable : //when AppId is not set yet and executable is set
            GroupFallback; //a fallback when neither AppId not Executable is known or not set yet
        Group = group.ToLowerInvariant();
    }

    /// <summary>
    /// Index (order) of the button group the window belongs to
    /// </summary>
    public int GroupIndex { get; private set; }
    /// <summary>
    /// Index (order) of the window button within the group
    /// </summary>
    /// <remarks>Actually, the value is the index (order) within the whole window's collection</remarks>
    public int WindowIndex { get; private set; }
    /// <summary>
    /// Global index (order) of the window button
    /// </summary>
    public int Index => GroupIndex * 1000 + WindowIndex;

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="title">Button title</param>
    /// <param name="executable">Application executable</param>
    /// <param name="appId">ApplicationUserModelId</param>
    protected ButtonInfo(string title, string? executable, string? appId)
    {
        this.title = title;
        this.appId = appId;
        Executable = executable;
        UpdateGroup();
    }

    /// <summary>
    /// Sets <see cref="GroupIndex"/> and <see cref="WindowIndex"/>
    /// </summary>
    /// <param name="groupIndex">New <see cref="GroupIndex"/></param>
    /// <param name="windowIndex">New <see cref="WindowIndex"/></param>
    /// <returns>This object</returns>
    // ReSharper disable once IdentifierTypo
    public virtual ButtonInfo SetIndicies(int groupIndex, int windowIndex)
    {
        GroupIndex = groupIndex;
        WindowIndex = windowIndex;
        return this;
    }

    /// <summary>
    /// Forces reorder by using property changed on <see cref="Index"/>
    /// <see cref="AppButtonManager"/> listens for such change and does the regroup
    /// </summary>
    public void ForceReorder()
    {
        OnPropertyChanged(nameof(Index));
    }

    /// <summary>
    /// Occurs when a property changes
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raise <see cref="PropertyChanged"/> event
    /// </summary>
    /// <param name="propertyName">Name of the property changed</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Returns the string representation of the object
    /// </summary>
    /// <returns>String representation of the object</returns>
    public override string ToString()
    {
        return $"{Title} ({GetType().Name}) Idx:{GroupIndex}/{WindowIndex}, A:{AppId}, E:{Executable}";
    }
}