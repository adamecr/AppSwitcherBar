using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using net.adamec.ui.AppSwitcherBar.AppBar;
using net.adamec.ui.AppSwitcherBar.Config;

// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Dto;

/// <summary>
/// Window Buttons Manager serves as the managed data source of window and pinned app buttons
/// </summary>
public class AppButtonManager : IEnumerable<ButtonInfo>, INotifyPropertyChanged, INotifyCollectionChanged
{
    /// <summary>
    /// Application settings
    /// </summary>
    private IAppSettings Settings { get; }


    /// <summary>
    /// Flag whether the <see cref="AppButtonManager"/> is busy with update
    /// </summary>
    public bool IsBusy => updateMode != UpdateModeEnum.Idle;
    /// <summary>
    /// Current update mode of the manager
    /// </summary>
    private UpdateModeEnum updateMode = UpdateModeEnum.Idle;
    /// <summary>
    /// Flag whether the <see cref="allButtons"/> list changed during the update
    /// (including the order change)
    /// </summary>
    private bool isDirty;
    /// <summary>
    /// Flag whether the application window buttons need to be regrouped 
    /// </summary>
    private bool needsRegroup;

    /// <summary>
    /// Internal list of all application windows and pinned apps
    /// </summary>
    private readonly List<ButtonInfo> allButtons = new();

    /// <summary>
    /// Internal list of all application windows 
    /// </summary>
    private WndInfo[] AllWindows => allButtons.Where(b => b is WndInfo).Cast<WndInfo>().ToArray();

    /// <summary>
    /// Internal dictionary allowing to get the application window by HWND
    /// </summary>
    private readonly Dictionary<IntPtr, WndInfo> allWindowsByHwnd = new();

    /// <summary>
    /// Indexer allowing to retrieve the application window or pinned app by its position within the <see cref="allButtons"/> list
    /// </summary>
    /// <param name="index">Index of application window within the <see cref="allButtons"/> list</param>
    /// <returns></returns>
    // ReSharper disable once UnusedMember.Local
    private ButtonInfo? this[int index] => index >= 0 && index < allButtons.Count ? allButtons[index] : null;
    /// <summary>
    /// Indexer allowing to retrieve the application window from the <see cref="allButtons"/> list by its HWND
    /// </summary>
    /// <param name="hwnd">HWND of application window within the <see cref="allButtons"/> list</param>
    /// <returns></returns>
    public WndInfo? this[IntPtr hwnd] => allWindowsByHwnd.TryGetValue(hwnd, out var w) ? w : null;

    /// <summary>
    /// List of all available pinned applications
    /// </summary>
    private PinnedAppInfo[] allAvailablePinnedApps=Array.Empty<PinnedAppInfo>();

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="appSettings">Application settings</param>
    public AppButtonManager(IAppSettings appSettings)
    {
        Settings=appSettings;
    }

    /// <summary>
    /// Starts the update mode when the <see cref="allButtons"/> list can be manipulated
    /// </summary>
    /// <exception cref="InvalidOperationException">When the manager is already in update mode</exception>
    public void BeginUpdate()
    {
        if (updateMode != UpdateModeEnum.Idle) throw new InvalidOperationException($"{nameof(AppButtonManager)} is already updating");
        isDirty = false;
        needsRegroup = false;
        
        updateMode = UpdateModeEnum.Update;
        //Mark existing window records for removal - when the record is updated the mark is changed, otherwise the record will be removed from collection at the end of processing
        foreach (var w in AllWindows)
        {
            w.MarkForRemoval();
        }
    }

    /// <summary>
    /// Starts the hard refresh mode when the <see cref="allButtons"/> list is being (re)created from scratch
    /// </summary>
    /// <param name="pinnedApps">Collection of pinned applications or empty array</param>
    /// <exception cref="InvalidOperationException">When the manager is already in update mode</exception>
    public void BeginHardRefresh(PinnedAppInfo[] pinnedApps)
    {
        if (updateMode != UpdateModeEnum.Idle) throw new InvalidOperationException($"{nameof(AppButtonManager)} is already updating");
        isDirty = false;
        needsRegroup = false;

        updateMode = UpdateModeEnum.HardRefresh;
        allButtons.Clear(); //clear the All windows collection
        allWindowsByHwnd.Clear();

        //add all pinned applications first (might be "hidden" when adding the windows)
        foreach (var pinnedApp in pinnedApps)
        {
            //initialize indicies
            pinnedApp.SetIndicies(pinnedApp.PinnedAppIndex, pinnedApp.PinnedAppIndex);
           
            pinnedApp.PropertyChanged += ButtonPropertyChanged; //listen for Index change for reordering
        }
        allAvailablePinnedApps = pinnedApps;
        allButtons.AddRange(allAvailablePinnedApps);

    }

    /// <summary>
    /// Finishes the update mode when the <see cref="allButtons"/> list have been manipulated
    /// </summary>
    /// <exception cref="InvalidOperationException">When the manager is notupdate mode</exception>
    public void EndUpdate()
    {
        if (updateMode == UpdateModeEnum.Idle) throw new InvalidOperationException($"{nameof(AppButtonManager)} is not updating");


        if (Settings.HideSingleWindowApps)
        {
            //Hide app single windows
            var singles = AllWindows
                .GroupBy(w => w.Group, w => w)
                .Where(g => g.Count() == 1)
                .SelectMany(w => w)
                .Where(w => allAvailablePinnedApps.All(p => p.Group != w.Group))
                .ToArray();
            foreach (var single in singles)
            {
                //silent remove for new - was created as new and instantly removed, so no dirty nor regroup flag
                if (single.ChangeStatus != WndInfo.ChangeStatusEnum.New) isDirty = true;
                allButtons.Remove(single);
                allWindowsByHwnd.Remove(single.Hwnd);
                single.PropertyChanged -= ButtonPropertyChanged;

            }
        }

        var anyNew= AllWindows.Any(w => w.ChangeStatus == WndInfo.ChangeStatusEnum.New);
        needsRegroup |= anyNew;
        isDirty |=anyNew;
        
        //Cleanup the collection - remove the windows that were not present in the last enum windows
        var toRemove = AllWindows.Where(w => w.ChangeStatus == WndInfo.ChangeStatusEnum.ToRemove).ToArray();
        foreach (var w in toRemove)
        {
            
            var group = w.Group;
            var replacedByPin = false;
            if (allButtons.Count(b => b.Group == group)==1)
            {
                //is the last one in group, check if a pin should be inserted
                var pinToUse = allAvailablePinnedApps.FirstOrDefault(p => p.Group == group);
                if (pinToUse != null)
                {
                    ReplaceButton(w,pinToUse);
                    replacedByPin = true;
                }
            }

            if(!replacedByPin) allButtons.Remove(w);
            allWindowsByHwnd.Remove(w.Hwnd);
            w.PropertyChanged -= ButtonPropertyChanged;
            isDirty = true;
        }

        

        //regroup if needed - keep the order how the windows have been added to allWindows as much as possible
        if (needsRegroup)
        {
            Regroup();
            needsRegroup = false;
            isDirty = true;
        }

        if (isDirty) OnCollectionChanged(); //notify UI about the collection change
        updateMode = UpdateModeEnum.Idle;
    }

    /// <summary>
    /// Regroups the <see cref="allButtons"/> collection when a new window is added or <see cref="WndInfo.AppId"/> is updated.
    /// Initialize/update <see cref="WndInfo.GroupIndex"/> and <see cref="WndInfo.WindowIndex"/>
    /// </summary>
    private void Regroup()
    {
        //regroup  - keep the order how the windows have been added to allWindows as much as possible
#if DEBUG
        Debug.WriteLine("Regroup");
#endif
        //get the window buttons groups in the order how the windows have been added to allWindows
        var groups = allButtons
            .GroupBy(
                w => w.Group,
                w => allButtons.IndexOf(w),
                (g, i) => new
                {
                    Group = g,
                    GroupIndex = i.Min()
                })
            .OrderBy(i => i.GroupIndex)
            .ToDictionary(
                i => i.Group,
                i => i.GroupIndex);

        //order the window buttons - get the order of groups and keep the order of buttons within the group 
        //respective keep the order of windows how added to window list
        var orderedWindowInfos = allButtons
            .Select(w => w.SetIndicies(groups[w.Group], allButtons.IndexOf(w)))
            .OrderBy(i => i.Index)
            .ToArray();

        //rebuild the allWindows list
        allButtons.Clear();
        allButtons.AddRange(orderedWindowInfos);
    }

    /// <summary>
    /// Force the reorder when any of windows raise <see cref="WndInfo.PropertyChanged"/> event for <see cref="WndInfo.Index"/> property.
    /// The <see cref="WndInfo.Index"/> property change must be explicitly signaled from code by calling <see cref="ButtonInfo.ForceReorder"/> - 
    /// it's used by <see cref="AppButtonsPanel"/> when reordering the window buttons.
    /// </summary>
    private void Reorder()
    {
        //regroup  - keep the order how the windows have been added to allWindows as much as possible
#if DEBUG
        Debug.WriteLine("Regroup by index");
#endif
        var orderedWindowInfos = allButtons
            .OrderBy(i => i.Index)
            .ToArray();

        //rebuild the allWindows list
        allButtons.Clear();
        allButtons.AddRange(orderedWindowInfos);

        //notify UI about the collection change
        OnCollectionChanged();
    }


    /// <summary>
    /// Adds the <paramref name="windowInfo"/> into the manager
    /// </summary>
    /// <param name="windowInfo">Information about the application window</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="windowInfo"/> is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the manager is not in update mode</exception>
    public void Add(WndInfo windowInfo)
    {
        if (windowInfo == null) throw new ArgumentNullException(nameof(windowInfo));
        if (updateMode == UpdateModeEnum.Idle) throw new InvalidOperationException($"{nameof(AppButtonManager)} is not updating");


        var lastWithinGroup = allButtons.LastOrDefault(w => w.Group == windowInfo.Group); //any window or pin from the same group already there?
        if (lastWithinGroup != null)
        {
            var pinButton = allButtons.FirstOrDefault(b => b is PinnedAppInfo && b.Group == windowInfo.Group); //if there pin for the app? (group)
            if (pinButton != null)
            {
                //replace pin
                ReplaceButton(pinButton,windowInfo);
            }
            else
            {
                //there is already a window for the same process
                var idx = allButtons.IndexOf(lastWithinGroup);
                idx++;
                if (idx < allButtons.Count)
                {
                    //add to existing process sequence
                    allButtons.Insert(idx, windowInfo); 
                }
                else
                {
                    //add to end of collection as the windows from the process are the last within the collection
                    allButtons.Add(windowInfo);
                }
            }
        }
        else
        {
            //new process, add to end
            allButtons.Add(windowInfo);
        }

        allWindowsByHwnd.Add(windowInfo.Hwnd, windowInfo);

        windowInfo.PropertyChanged += ButtonPropertyChanged; //listen for AppId change that might cause regrouping or Index change for reordering

       // needsRegroup = true;
       // isDirty = true;
    }

    /// <summary>
    /// Replace <param name="oldButton"> in collection of buttons with <paramref name="newButton"/> and
    /// copy indicies from <paramref name="oldButton"/> to <paramref name="newButton"/></param>
    /// </summary>
    /// <param name="oldButton">Button to be replaced</param>
    /// <param name="newButton">New button</param>
    private void ReplaceButton(ButtonInfo oldButton, ButtonInfo newButton)
    {
        if(allButtons.Contains(newButton)) allButtons.Remove(newButton); //remove the new button first if already in collection
        allButtons[allButtons.IndexOf(oldButton)]=newButton; //get the position of the old button to be replaced
        newButton.SetIndicies(oldButton.GroupIndex, newButton.WindowIndex); //place the new button instead of the old one
    }

    /// <summary>
    /// Listen for AppId changes that might cause regrouping or Index change for forced reordering
    /// </summary>
    /// <param name="sender">Application window info</param>
    /// <param name="e">Event arguments</param>
    private void ButtonPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (updateMode != UpdateModeEnum.Idle && e.PropertyName == nameof(WndInfo.AppId))
        {
            if (sender is WndInfo wnd)
            {
                //check pin both for old group (may be inserted) and the new 
                var oldGroup = wnd.OldGroup;
                if (allButtons.Count(b => b.Group== oldGroup) ==0)
                {
                    //was the last one in group, check if a pin should be inserted
                    var pinToUse = allAvailablePinnedApps.FirstOrDefault(p => p.Group == oldGroup);
                    if (pinToUse != null)
                    {
                        allButtons.Insert(allButtons.IndexOf(wnd),pinToUse);
                    }
                }
                
                //new group - check if there is a pin
                var pin = allButtons.FirstOrDefault(b => b.Group == wnd.Group && b is PinnedAppInfo);
                if (pin != null)
                {
                    //replace
                    ReplaceButton(pin,wnd);
                }
            }

            needsRegroup = true;
        }

        if (e.PropertyName == nameof(WndInfo.Index))
        {
            Reorder();
        }
    }

    /// <summary>
    /// Occurs when property value changes
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    /// <summary>
    /// Occurs when application windows collection changes
    /// </summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Raise <see cref="PropertyChanged"/> event
    /// </summary>
    /// <param name="propertyName">Name of the property changed</param>
    // ReSharper disable once UnusedMember.Local
    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Raise <see cref="CollectionChanged"/> event
    /// </summary>
    private void OnCollectionChanged()
    {
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// Enumerator that can be used to iterate through the collection of application windows and pinned apps
    /// </summary>
    public IEnumerator<ButtonInfo> GetEnumerator()
    {
        return allButtons.GetEnumerator();
    }

    /// <summary>
    /// Enumerator that can be used to iterate through the collection of application windows and pinned apps
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// <see cref="AppButtonManager"/> update modes
    /// </summary>
    private enum UpdateModeEnum
    {
        /// <summary>
        /// Manager is not in update mode, no application windows collection changes are allowed
        /// </summary>
        Idle,
        /// <summary>
        /// Manager is in update mode, application windows collection changes are allowed
        /// </summary>
        Update,
        /// <summary>
        /// Manager is in update mode with hard refresh, application windows collection is rebuilt from scratch
        /// </summary>
        HardRefresh
    }
}