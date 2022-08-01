using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Dto;

/// <summary>
/// Window Buttons Manager serves as the managed data source of application window buttons
/// </summary>
public class WndButtonManager : IEnumerable<WndInfo>, INotifyPropertyChanged, INotifyCollectionChanged
{
    /// <summary>
    /// Current update mode of the manager
    /// </summary>
    private UpdateModeEnum updateMode = UpdateModeEnum.Idle;
    /// <summary>
    /// Flag whether the <see cref="allWindows"/> list changed during the update
    /// (including the order change)
    /// </summary>
    private bool isDirty;
    /// <summary>
    /// Flag whether the application window buttons need to be regrouped 
    /// </summary>
    private bool needsRegroup;

    /// <summary>
    /// Internal list of all application windows
    /// </summary>
    private readonly List<WndInfo> allWindows = new();
    /// <summary>
    /// Internal dictionary allowing to get the application window by HWND
    /// </summary>
    private readonly Dictionary<IntPtr, WndInfo> allWindowsByHwnd = new();

    /// <summary>
    /// Indexer allowing to retrieve the application window by its position within the <see cref="allWindows"/> list
    /// </summary>
    /// <param name="index">Index of application window within the <see cref="allWindows"/> list</param>
    /// <returns></returns>
    // ReSharper disable once UnusedMember.Local
    private WndInfo? this[int index] => index >= 0 && index < allWindows.Count ? allWindows[index] : null;
    /// <summary>
    /// Indexer allowing to retrieve the application window from the <see cref="allWindows"/> list by its HWND
    /// </summary>
    /// <param name="hwnd">HWND of application window within the <see cref="allWindows"/> list</param>
    /// <returns></returns>
    public WndInfo? this[IntPtr hwnd] => allWindowsByHwnd.TryGetValue(hwnd, out var w) ? w : null;


    /// <summary>
    /// Starts the update mode when the <see cref="allWindows"/> list can be manipulated
    /// </summary>
    /// <param name="isHardRefresh">Flag whether the hard refresh has been requested</param>
    /// <exception cref="InvalidOperationException">When the manager is already in update mode</exception>
    public void BeginUpdate(bool isHardRefresh)
    {
        if (updateMode != UpdateModeEnum.Idle) throw new InvalidOperationException($"{nameof(WndButtonManager)} is already updating");
        isDirty = false;
        needsRegroup = false;

        if (isHardRefresh)
        {
            updateMode = UpdateModeEnum.HardRefresh;
            allWindows.Clear(); //clear the All windows collection
            allWindowsByHwnd.Clear();
        }
        else
        {
            updateMode = UpdateModeEnum.Update;
            //Mark existing records for removal - when the record is updated the mark is changed, otherwise the record will be removed from collection at the end of processing
            foreach (var w in allWindows)
            {
                w.MarkForRemoval();
            }
        }
    }

    /// <summary>
    /// Finishes the update mode when the <see cref="allWindows"/> list have been manipulated
    /// </summary>
    /// <exception cref="InvalidOperationException">When the manager is notupdate mode</exception>
    public void EndUpdate()
    {
        if (updateMode == UpdateModeEnum.Idle) throw new InvalidOperationException($"{nameof(WndButtonManager)} is not updating");

        //Cleanup the collection - remove the windows that were not present in the last enum windows
        var toRemove = allWindows.Where(w => w.ChangeStatus == WndInfo.ChangeStatusEnum.ToRemove).ToArray();
        foreach (var w in toRemove)
        {
            allWindows.Remove(w);
            allWindowsByHwnd.Remove(w.Hwnd);
            w.PropertyChanged -= WndPropertyChanged;
            isDirty = true;
        }

        //regroup if needed - keep the order how the windows have been added to allWindows as much as possible
        if (needsRegroup)
        {
            //get the window buttons groups in the order how the windows have been added to allWindows
            var groups = allWindows
                .GroupBy(
                    w => w.Group,
                    w => allWindows.IndexOf(w),
                    (g, i) => new
                    {
                        Group = g,
                        Index = i.Min()

                    })
                .OrderBy(i => i.Index)
                .ToDictionary(
                    i => i.Group,
                    i => i.Index);

            //order the window buttons - get the order of groups and keep the order of buttons within the group 
            //respective keep the order of windows how added to window list
            var orderedWindowInfos = allWindows
                .Select(w => new
                {
                    Index = groups[w.Group] * 1000 + allWindows.IndexOf(w),
                    Wnd = w
                })
                .OrderBy(i => i.Index)
                .ToArray();

            //rebuild the allWindows list
            allWindows.Clear();
            allWindows.AddRange(orderedWindowInfos.Select(i => i.Wnd));

            needsRegroup = false;
            isDirty = true;
        }

        if (isDirty) OnCollectionChanged(); //notify UI about the collection change
        updateMode = UpdateModeEnum.Idle;
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
        if (updateMode == UpdateModeEnum.Idle) throw new InvalidOperationException($"{nameof(WndButtonManager)} is not updating");

        var lastWithinGroup = allWindows.LastOrDefault(w => w.Group == windowInfo.Group); //any window from the same process already exist in collection?
        if (lastWithinGroup != null)
        {
            //there is already a window for the same process
            var idx = allWindows.IndexOf(lastWithinGroup);
            idx++;
            if (idx < allWindows.Count)
            {
                allWindows.Insert(idx, windowInfo); //add to existing process sequence
            }
            else
            {
                allWindows.Add(windowInfo); //add to end of collection as the windows from the process are the last within the collection
            }
        }
        else
        {
            allWindows.Add(windowInfo); //new process, add to end
        }

        allWindowsByHwnd.Add(windowInfo.Hwnd, windowInfo);

        windowInfo.PropertyChanged += WndPropertyChanged; //listen for AppId change that might cause regrouping

        isDirty = true;
    }

    /// <summary>
    /// Listen for AppId changes that might cause regrouping
    /// </summary>
    /// <param name="sender">Application window info</param>
    /// <param name="e">Event arguments</param>
    private void WndPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (updateMode != UpdateModeEnum.Idle && e.PropertyName == nameof(WndInfo.AppId))
        {
            needsRegroup = true;
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
    /// Enumerator that can be used to iterate through the collection of application windows
    /// </summary>
    public IEnumerator<WndInfo> GetEnumerator()
    {
        return allWindows.GetEnumerator();
    }

    /// <summary>
    /// Enumerator that can be used to iterate through the collection of application windows
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// <see cref="WndButtonManager"/> update modes
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