using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using net.adamec.ui.AppSwitcherBar.Wpf;

namespace net.adamec.ui.AppSwitcherBar.AppBar;

/// <summary>
/// <see cref="WrapPanel"/> implementation with support of resizable application buttons (auto size) and drag and drop reorder
/// </summary>
public class AppButtonsPanel : WrapPanel
{
    /// <summary>
    /// Flag whether drag and drop operation is active 
    /// </summary>
    private bool isDragging;
    /// <summary>
    /// Click point from what the dragging starts
    /// </summary>
    private Point dragAndDropStartPoint;
    /// <summary>
    /// Drag and drop source button
    /// </summary>
    private AppButton? dragAndDropSource;
    /// <summary>
    /// Drag and drop target button
    /// </summary>
    private AppButton? dragAndDropTarget;
    /// <summary>
    /// Button the mouse is over during drag and drop progress (mouse move)
    /// </summary>
    private AppButton? dragAndDropMouseOverButton;

    /// <summary>
    /// Flag whether the application bar is auto sized
    /// </summary>
    public bool IsAutoSized
    {
        get => (bool)GetValue(IsAutoSizedProperty);
        set => SetValue(IsAutoSizedProperty, value);
    }
    /// <summary>
    /// Flag whether the application bar is auto sized
    /// </summary>
    public static readonly DependencyProperty IsAutoSizedProperty = DependencyProperty.Register(
        nameof(IsAutoSized),
        typeof(bool),
        typeof(AppButtonsPanel),
        new FrameworkPropertyMetadata(true, (o, _) => { if (o is AppButtonsPanel p) p.InvalidateMeasure(); }));

    /// <summary>
    /// Flag whether the application bar allows application windows reordering (drag and drop)
    /// </summary>
    public bool AllowButtonReorder
    {
        get => (bool)GetValue(AllowButtonReorderProperty);
        set => SetValue(AllowButtonReorderProperty, value);
    }
    /// <summary>
    /// Flag whether the application bar allows application windows reordering (drag and drop)
    /// </summary>
    public static readonly DependencyProperty AllowButtonReorderProperty = DependencyProperty.Register(
        nameof(AllowButtonReorder),
        typeof(bool),
        typeof(AppButtonsPanel),
        new FrameworkPropertyMetadata(true));

    /// <summary>
    /// App window max button width
    /// </summary>
    public int ButtonMaxWidth
    {
        get => (int)GetValue(ButtonMaxWidthProperty);
        set => SetValue(ButtonMaxWidthProperty, value);
    }
    /// <summary>
    /// App window max button width
    /// </summary>
    public static readonly DependencyProperty ButtonMaxWidthProperty = DependencyProperty.Register(
        nameof(ButtonMaxWidth),
        typeof(int),
        typeof(AppButtonsPanel),
        new FrameworkPropertyMetadata(150));

    /// <summary>
    /// App window min button width
    /// </summary>
    private int ButtonMinWidth { get; set; }

    /// <summary>
    /// App window min button width ratio
    /// </summary>
    public double ButtonMinWidthRatio
    {
        get => (double)GetValue(ButtonMinWidthRatioProperty);
        set => SetValue(ButtonMinWidthRatioProperty, value);
    }
    /// <summary>
    /// App window min button width ratio
    /// </summary>
    public static readonly DependencyProperty ButtonMinWidthRatioProperty = DependencyProperty.Register(
        nameof(ButtonMinWidthRatio),
        typeof(double),
        typeof(AppButtonsPanel),
        new FrameworkPropertyMetadata(0.6, (o, _) =>
        {
            if (o is AppButtonsPanel p)
                p.ButtonMinWidth = (int)(p.ButtonMaxWidth * p.ButtonMinWidthRatio);
        }));


    /// <summary>
    /// App window button height
    /// </summary>
    public int ButtonHeight
    {
        get => (int)GetValue(ButtonHeightProperty);
        set => SetValue(ButtonHeightProperty, value);
    }
    /// <summary>
    /// App window button height
    /// </summary>
    public static readonly DependencyProperty ButtonHeightProperty = DependencyProperty.Register(
        nameof(ButtonHeight),
        typeof(int),
        typeof(AppButtonsPanel),
        new FrameworkPropertyMetadata(28));

    /// <summary>
    /// App window button margin
    /// </summary>
    public Thickness ButtonMargin
    {
        get => (Thickness)GetValue(ButtonMarginProperty);
        set => SetValue(ButtonMarginProperty, value);
    }
    /// <summary>
    /// App window button margin
    /// </summary>
    public static readonly DependencyProperty ButtonMarginProperty = DependencyProperty.Register(
        nameof(ButtonMargin),
        typeof(Thickness),
        typeof(AppButtonsPanel),
        new FrameworkPropertyMetadata(new Thickness(1)));

    /// <summary>
    /// Get's the window buttons within the panel
    /// </summary>
    private List<AppButton> AppButtons => WpfTools.AllChildren<AppButton>(this);


    #region Autosize
    /// <summary>
    /// Calculate required size of panel with app windows buttons. Resize buttons when allowed and needed
    /// </summary>
    /// <param name="constraint">Available space</param>
    /// <returns>Required space</returns>
    protected override Size MeasureOverride(Size constraint)
    {

        var requiredSize = Orientation == Orientation.Horizontal
            ? new Size(constraint.Width, double.PositiveInfinity)
            : new Size(double.PositiveInfinity, constraint.Height);

        if (IsAutoSized)
        {
            //Resize buttons when needed
            requiredSize = Orientation == Orientation.Horizontal
                ? AutoSizeHorizontal(constraint)
                : AutoSizeVertical(constraint);
        }
        else
        {
            //Use fixed button size
            var newItemWidth = (int)(ButtonMaxWidth + ButtonMargin.Left + ButtonMargin.Right);
            var newItemHeight = (int)(ButtonHeight + ButtonMargin.Top + ButtonMargin.Bottom);

            if (newItemWidth != (int)ItemWidth) ItemWidth = newItemWidth;
            if (newItemHeight != (int)ItemHeight) ItemHeight = newItemHeight;
        }

        //Ensure the buttons are properly sized
        var childSize = new Size(ItemWidth, ItemHeight);
        foreach (UIElement child in Children)
        {
            child.Measure(childSize);
        }

        return requiredSize;
    }

    /// <summary>
    /// Auto size the application window buttons when the bar is horizontal
    /// </summary>
    /// <param name="constraint">Available size</param>
    /// <returns>Required size for the panel containing the buttons</returns>
    private Size AutoSizeHorizontal(Size constraint)
    {
        //Horizontal bar
        // - use constant button height
        // - calculate max available rows
        // - calculate how many columns are needed for buttons
        // - for single column use max button width, when multi-column use min button width

        var containerWidth = (int)constraint.Width;
        if (containerWidth <= 0 || double.IsPositiveInfinity(containerWidth)) return new Size(double.PositiveInfinity, double.PositiveInfinity); //Can't count, request max available size

        var minButtonWidth = ButtonMinWidth;
        var maxButtonWidth = ButtonMaxWidth;
        var minButtonWidthWithMargin = minButtonWidth + ButtonMargin.Left + ButtonMargin.Right;

        var maxColumns = (int)(containerWidth / minButtonWidthWithMargin);
        var cnt = Children.Count;
        if (maxColumns < 1 || cnt < 1) return new Size(double.PositiveInfinity, double.PositiveInfinity); //Can't count, request max available

        var rows = (int)((double)(cnt - 1) / maxColumns) + 1; //round up
        var columns = (int)((double)(cnt - 1) / rows) + 1; //round up

        var newButtonWidthWithMargin = (int)((double)containerWidth / columns); //round down
        var newButtonWidth = newButtonWidthWithMargin - ButtonMargin.Left - ButtonMargin.Right;

        if (newButtonWidth > maxButtonWidth) newButtonWidth = maxButtonWidth;
        if (newButtonWidth < minButtonWidth) newButtonWidth = minButtonWidth; //should not happen

        var newItemWidth = (int)(newButtonWidth + ButtonMargin.Left + ButtonMargin.Right);
        var newItemHeight = (int)(ButtonHeight + ButtonMargin.Top + ButtonMargin.Bottom);

        if (newItemWidth != (int)ItemWidth) ItemWidth = newItemWidth;
        if (newItemHeight != (int)ItemHeight) ItemHeight = newItemHeight;

        var requiredContainerHeight = ItemHeight * rows;

#if DEBUG
        Debug.WriteLine($"Measure(H): container:{containerWidth:####}, cnt:{cnt},mc:{maxColumns}, c:{columns}, r:{rows}, iw:{newItemWidth}, ih:{newItemHeight}");
#endif

        return new Size(containerWidth, requiredContainerHeight);
    }

    /// <summary>
    /// Auto size the application window buttons when the bar is vertical
    /// </summary>
    /// <param name="constraint">Available size</param>
    /// <returns>Required size for the panel containing the buttons</returns>
    private Size AutoSizeVertical(Size constraint)
    {
        //Vertical bar
        // - use constant button height
        // - calculate max available rows
        // - calculate how many columns are needed for buttons
        // - for single column use max button width, when multi-column use min button width

        var containerHeight = (int)constraint.Height;
        if (containerHeight <= 0 || double.IsPositiveInfinity(containerHeight)) return new Size(double.PositiveInfinity, double.PositiveInfinity); //Can't count, request max available

        var minButtonWidth = ButtonMinWidth;
        var maxButtonWidth = ButtonMaxWidth;
        var buttonHeightWithMargin = ButtonHeight + ButtonMargin.Top + ButtonMargin.Bottom;

        var maxRows = (int)(containerHeight / buttonHeightWithMargin);
        var cnt = Children.Count;
        if (maxRows < 1 || cnt < 1) return new Size(double.PositiveInfinity, double.PositiveInfinity); //Can't count, request max available

        var columns = (int)((double)(cnt - 1) / maxRows) + 1; //round up

        //single column - max button width, multi column - min button width
        var newButtonWidth = columns < 2 ? maxButtonWidth : minButtonWidth;

        var newItemWidth = (int)(newButtonWidth + ButtonMargin.Left + ButtonMargin.Right);
        var newItemHeight = (int)(ButtonHeight + ButtonMargin.Top + ButtonMargin.Bottom);

        if (newItemWidth != (int)ItemWidth) ItemWidth = newItemWidth;
        if (newItemHeight != (int)ItemHeight) ItemHeight = newItemHeight;

        var requiredContainerWidth = ItemWidth * columns;

#if DEBUG
        Debug.WriteLine($"Measure(V): container:{containerHeight:####}, cnt:{cnt},mr:{maxRows}, c:{columns}, iw:{newItemWidth}, ih:{newItemHeight}");
#endif

        return new Size(requiredContainerWidth, containerHeight);
    }

    #endregion

    #region Drag And Drop (Reorder)

    /// <summary>
    /// Register the cursor position on left button down
    /// The drag and drop will start when the mouse moves as least "a bit" (managed by <see cref="OnPreviewMouseMove"/>)
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseLeftButtonDown(e);
        if (!AllowButtonReorder) return;

        dragAndDropStartPoint = e.GetPosition(this);
    }

    /// <summary>
    /// Finish drag and drop when the left button is released
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseLeftButtonUp(e);
        if (!AllowButtonReorder) return;

        DragAndDropFinish();
    }

#if DEBUG
    /// <summary>
    /// The last application button detected when debugging the window info on mouse move
    /// </summary>
    private AppButton? debugLastAppWindowButton;
#endif

    /// <summary>
    /// Manage the mouse move during the drag and drop.
    /// Starts the drag and drop when the button is pressed and "a bit of move" is detected
    /// </summary>
    /// <remarks>When in DEBUG, writes the information about the window info attached to button below the mouse cursor</remarks>
    /// <param name="e">Event arguments</param>
    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
        base.OnPreviewMouseMove(e);

#if DEBUG
        //debug the window info on mouse move
        var tSrc = e.OriginalSource;
        if (tSrc is DependencyObject tDependencyObject)
        {

            var tAppButton = WpfTools.FindVisualAncestor<AppButton>(tDependencyObject);
            if (tAppButton != null && tAppButton != debugLastAppWindowButton)
            {
                var tWndInfo = tAppButton.ButtonInfo;
                Debug.WriteLine($"Mouse over {tWndInfo}");
                debugLastAppWindowButton = tAppButton;
            }
        }
        else
        {
            debugLastAppWindowButton = null;
        }
#endif

        if (!AllowButtonReorder) return;

        var mousePos = e.GetPosition(this);
        var diff = dragAndDropStartPoint - mousePos;

        if (e.LeftButton == MouseButtonState.Pressed && !isDragging && (
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
        {
            var src = e.OriginalSource;
            if (src is not DependencyObject dependencyObject) return;

            var appButton = WpfTools.FindVisualAncestor<AppButton>(dependencyObject);
            if (appButton != null)
            {
                DragAndDropBegin(appButton);
                return;
            }

            DragAndDropCancel();
            return;
        }

        if (e.LeftButton == MouseButtonState.Pressed && isDragging)
        {

            dragAndDropMouseOverButton = null;
            VisualTreeHelper.HitTest(this, null, result =>
            {
                var appButton = WpfTools.FindVisualAncestor<AppButton>(result.VisualHit);
                if (appButton == null) return HitTestResultBehavior.Continue;

                dragAndDropMouseOverButton = appButton;
                return HitTestResultBehavior.Stop;
            }, new PointHitTestParameters(mousePos));
            DragAndDropUpdateTarget(dragAndDropMouseOverButton);
        }

    }

    /// <summary>
    /// Begins the drag and drop for app buttons reorder
    /// </summary>
    /// <param name="source">App button to be reordered (drag and drop source)</param>
    private void DragAndDropBegin(AppButton? source)
    {
        if (!AllowButtonReorder) return;

        if (source != null && !isDragging)
        {
            source.HideThumbnail();
            isDragging = true;
            SetDragAndDropSourceInternal(source);

#if DEBUG
            Debug.WriteLine($"Drag and Drop Begin {source.ButtonInfo.Title}");
#endif
        }
    }

    /// <summary>
    /// Cancels the drag and drop for window buttons reorder
    /// </summary>
    private void DragAndDropCancel()
    {
        if (isDragging)
        {
            Debug.WriteLine("Drag and Drop Cancel");
        }

        Cursor = Cursors.Arrow;
        isDragging = false;
        SetDragAndDropSourceInternal(null);
        SetDragAndDropTargetInternal(null);
    }

    /// <summary>
    /// Updates the drag and drop target when reordering the app buttons
    /// </summary>
    /// <remarks>This just updates the information about the target based on current mouse position, it doesn't do the reorder (drag and drop is not finished yet)</remarks>
    /// <param name="target">New target of drag and drop</param>
    private void DragAndDropUpdateTarget(AppButton? target)
    {
        if (target != dragAndDropTarget && isDragging)
        {
            SetDragAndDropTargetInternal(target);
            SetDragAndDropSourceInternal(dragAndDropSource); //adjust the source single/multi based on target

#if DEBUG
            Debug.WriteLine($"Drag and Drop Target {target?.ButtonInfo.Title ?? "N/A"}");
#endif
        }

        //set cursor 
        // when dragging over button, signal the direction where the dragged button (group) will be placed
        // when dragging not-over button, signal the "wrong" target
        // when not dragging, set default Arrow button
        if (target != null && dragAndDropSource != null)
        {
            var targetIdx = target.ButtonInfo.Index;
            var sourceIdx = dragAndDropSource.ButtonInfo.Index;
            var toLeft = targetIdx < sourceIdx;
            Cursor =
                targetIdx == sourceIdx
                    ? Cursors.No
                    : toLeft
                        ? Orientation == Orientation.Horizontal
                            ? Cursors.ScrollW
                            : Cursors.ScrollN
                        : Orientation == Orientation.Horizontal
                            ? Cursors.ScrollE :
                            Cursors.ScrollS;
        }
        else
        {
            Cursor = isDragging ? Cursors.No : Cursors.Arrow;
        }
    }

    /// <summary>
    /// Finish the button reorder drag and drop (it's the "real drop").
    /// When the <see cref="dragAndDropTarget"/> is in same group as <see cref="dragAndDropSource"/>, reorder the buttons within the group
    /// When the <see cref="dragAndDropTarget"/> and <see cref="dragAndDropSource"/> are in different groups, reorder the whole groups
    /// </summary>
    private void DragAndDropFinish()
    {
        if (isDragging && dragAndDropSource != null && dragAndDropTarget != null && dragAndDropTarget != dragAndDropSource)
        {
            Debug.WriteLine($"Drag and Drop Finish {dragAndDropSource.ButtonInfo.Title} -> {dragAndDropTarget.ButtonInfo.Title}");

            var withinGroup = dragAndDropTarget.ButtonInfo.GroupIndex == dragAndDropSource.ButtonInfo.GroupIndex;
            if (withinGroup)
            {
                DragAndDropMoveWindows();
            }
            else
            {
                //move whole group
                DragAndDropMoveGroup();
            }
            Cursor = Cursors.Arrow;
            isDragging = false;
            SetDragAndDropSourceInternal(null);
            SetDragAndDropTargetInternal(null);
        }
        else
        {
            DragAndDropCancel();
        }
    }

    /// <summary>
    /// Reorder (move) the app buttons within the group as a result of button drag and drop
    /// </summary>
    private void DragAndDropMoveWindows()
    {
        if (dragAndDropSource == null || dragAndDropTarget == null) return;

        var targetIdx = dragAndDropTarget.ButtonInfo.WindowIndex;
        var sourceIdx = dragAndDropSource.ButtonInfo.WindowIndex;
        var sourceWndInfo = dragAndDropSource.ButtonInfo;
        var toLeft = targetIdx < sourceIdx;
        if (toLeft)
        {
            var windowsToShift = AppButtons
                .Select(b => b.ButtonInfo)
                .Where(w => w.WindowIndex >= targetIdx && w.WindowIndex < sourceIdx)
                .OrderByDescending(w => w.WindowIndex) //shift from right
                .ToArray();
            foreach (var wndInfo in windowsToShift)
            {
                wndInfo.SetIndicies(wndInfo.GroupIndex, wndInfo.WindowIndex + 1);
            }

            sourceWndInfo.SetIndicies(sourceWndInfo.GroupIndex, targetIdx); //move the source
        }
        else
        {
            var windowsToShift = AppButtons
                .Select(b => b.ButtonInfo)
                .Where(w => w.WindowIndex > sourceIdx && w.WindowIndex <= targetIdx)
                .OrderBy(w => w.WindowIndex) //shift from left
                .ToArray();
            foreach (var wndInfo in windowsToShift)
            {
                wndInfo.SetIndicies(wndInfo.GroupIndex, wndInfo.WindowIndex - 1);
            }

            sourceWndInfo.SetIndicies(sourceWndInfo.GroupIndex, targetIdx); //move the source
        }

        sourceWndInfo.ForceReorder();
    }

    /// <summary>
    /// Reorder (move) the app buttons groups as a result of button drag and drop
    /// </summary>
    private void DragAndDropMoveGroup()
    {
        if (dragAndDropSource == null || dragAndDropTarget == null) return;

        var targetIdx = dragAndDropTarget.ButtonInfo.GroupIndex;
        var sourceIdx = dragAndDropSource.ButtonInfo.GroupIndex;
        var sourceWndInfo = dragAndDropSource.ButtonInfo;
        var toLeft = targetIdx < sourceIdx;
        if (toLeft)
        {
            var windowsToShift = AppButtons
                .Select(b => b.ButtonInfo)
                .Where(w => w.GroupIndex >= targetIdx && w.GroupIndex < sourceIdx)
                .OrderByDescending(w => w.GroupIndex) //shift from right
                .ToArray();

            var windowsToShiftSourceGroup = AppButtons
                .Select(b => b.ButtonInfo)
                .Where(w => w.GroupIndex == sourceIdx) //move the whole source group
                .ToArray();

            foreach (var wndInfo in windowsToShift)
            {
                wndInfo.SetIndicies(wndInfo.GroupIndex + 1, wndInfo.WindowIndex);
            }


            foreach (var wndInfo in windowsToShiftSourceGroup)
            {
                wndInfo.SetIndicies(targetIdx, wndInfo.WindowIndex);
            }
        }
        else
        {
            var windowsToShift = AppButtons
                .Select(b => b.ButtonInfo)
                .Where(w => w.GroupIndex > sourceIdx && w.GroupIndex <= targetIdx)
                .OrderBy(w => w.GroupIndex) //shift from left
                .ToArray();

            var windowsToShiftSourceGroup = AppButtons
                .Select(b => b.ButtonInfo)
                .Where(w => w.GroupIndex == sourceIdx) //move the whole source group
                .ToArray();

            foreach (var wndInfo in windowsToShift)
            {
                wndInfo.SetIndicies(wndInfo.GroupIndex - 1, wndInfo.WindowIndex);
            }

            foreach (var wndInfo in windowsToShiftSourceGroup) //move the source (group)
            {
                wndInfo.SetIndicies(targetIdx, wndInfo.WindowIndex);
            }
        }

        sourceWndInfo.ForceReorder();
    }

    /// <summary>
    /// Updates the <see cref="AppButton.IsDragAndDropSource"/> information for <see cref="AppButtons"/>.
    /// The value is set to false for all app buttons except the <paramref name="source"/> (if not null) that is set to true
    /// </summary>
    /// <param name="source">Drag and drop source button if any</param>
    private void SetDragAndDropSourceInternal(AppButton? source)
    {
        var buttons = AppButtons;

        foreach (var wndInfo in buttons.Where(w => w.IsDragAndDropSource))
        {
            wndInfo.SetIsDragAndDropSource(false);
        }

        if (source != null)
        {

            var singleSelect = dragAndDropTarget == null || dragAndDropTarget.ButtonInfo.GroupIndex == source.ButtonInfo.GroupIndex;
            if (singleSelect)
            {
                source.SetIsDragAndDropSource(true);
            }
            else
            {
                var groupIdx = source.ButtonInfo.GroupIndex;
                foreach (var windowButton in buttons.Where(w => w.ButtonInfo.GroupIndex == groupIdx))
                {
                    windowButton.SetIsDragAndDropSource(true);
                }
            }
        }
        dragAndDropSource = source;
    }

    /// <summary>
    /// Updates the <see cref="AppButton.IsDragAndDropTarget"/> information for <see cref="AppButtons"/>.
    /// The value is set to false for all app buttons except the <paramref name="target"/> (if not null) that is set to true
    /// </summary>
    /// <param name="target">Drag and drop target button if any</param>
    private void SetDragAndDropTargetInternal(AppButton? target)
    {
        if (target == dragAndDropTarget) return;
        var buttons = AppButtons;

        foreach (var windowButton in buttons.Where(w => w.IsDragAndDropTarget).ToArray())
        {
            windowButton.SetIsDragAndDropTarget(false);
        }


        if (target != null && dragAndDropSource != null)
        {
            var sameGroup = target.ButtonInfo.GroupIndex == dragAndDropSource.ButtonInfo.GroupIndex;
            if (sameGroup)
            {
                target.SetIsDragAndDropTarget(true);
            }
            else
            {
                var groupIdx = target.ButtonInfo.GroupIndex;
                foreach (var windowButton in buttons.Where(w => w.ButtonInfo.GroupIndex == groupIdx).ToArray())
                {
                    windowButton.SetIsDragAndDropTarget(true);
                }
            }
        }
        dragAndDropTarget = target;
    }



    #endregion
}