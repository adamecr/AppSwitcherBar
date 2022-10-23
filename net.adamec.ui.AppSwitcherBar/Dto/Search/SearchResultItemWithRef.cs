using System;

namespace net.adamec.ui.AppSwitcherBar.Dto.Search;

/// <summary>
/// Search result item base class with result reference
/// </summary>
public abstract class SearchResultItemWithRef : SearchResultItem
{
    private bool isDefault;

    /// <summary>
    /// Flag whether the items is the default one.
    /// Only one item in a time should be set as default
    /// </summary>
    public bool IsDefault
    {
        get => isDefault;
        set
        {
            if (value != isDefault)
            {
                isDefault = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Reference to item found (window, app, ...)
    /// </summary>
    public abstract object? ResultReference { get; }

    

    /// <summary>
    /// Launch the search result
    /// </summary>
    public abstract void Launch();
}

/// <summary>
/// Search result item base class with result reference
/// </summary>
public abstract class SearchResultItemWithRef<TReference> : SearchResultItemWithRef where TReference:class
{
    /// <summary>
    /// Reference to item found (window, app, ...)
    /// </summary>
    public override TReference ResultReference { get; }

    /// <summary>
    /// Action to be run when the search result is launched
    /// </summary>
    protected Action<TReference>? LaunchAction { get; set; }

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="resultReference">Reference to item found (window, app, ...)</param>
    /// <param name="launchAction">Action to be run when the search result is launched</param>
    protected SearchResultItemWithRef(TReference resultReference, Action<TReference>? launchAction)
    {
        ResultReference = resultReference;
        LaunchAction = launchAction;
    }

    /// <summary>
    /// Action to be run when the search result is "executed" from UI
    /// </summary>
    /// <param name="parameter">Command parameter - not used</param>
    protected override void OnCommand(object? parameter)
    {
        Launch();
    }

    /// <summary>
    /// Launch the search result
    /// </summary>
    public override void Launch()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (ResultReference != null) LaunchAction?.Invoke(ResultReference);
    }
}