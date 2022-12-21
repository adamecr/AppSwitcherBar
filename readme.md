# AppSwitcherBar #

`AppSwitcherBar` provides the application bar complementary to Windows 11 Taskbar with ungrouped application windows buttons for easy one-click application switching.

I did not use any invasive techniques that replace the Win11 task bar as I use Win11 for my day-job and didn't want to potentially jeopardize the system stability. `AppSwitcherBar` is not a substitute for Win11 task bar, but it's a complement that can "sit on top of the taskbar" and provide the ungrouped buttons with the possibility to quickly switch between the application by single mouse click. 

*Note: It's built as NET6 WPF application, but as it heavily uses the Win32 API, it's bound to Windows OS by design (tested on Windows 11 and Windows 10).*

## Quick Start ##
To start `AppSwitcherBar`, just run `AppSwitcherBar.exe`. 

![Main AppSwitcherBar window](doc/img/AppSwitcherBar.png)

Main features
 - Top level **application windows** are presented as **buttons within the bar**
 - **Applications pinned to task bar** without any window open are presented as transparent background buttons with title in italics. **Click** on the button to launch the pinned application.
 - The buttons for windows of the same applications are displayed together but there is always a button for each top level window (this is called grouping here)
 - Optionally, it's possible to hide the applications (buttons) having only single window (use `HideSingleWindowApps` setting). When the pinned applications support is on and the application has a pin, it will be not be hiden even having a single window.
 - **Click** on the application window button **to switch to application** or to minimize the window of foreground application
 - When a **file is dragged over** the application window button, the application windows is activated, so the file can easily be droppped there
 - **Hover the mouse** over the application window button **to show the live thumbnail** of the window
   - **Click** on the thumbnail **to switch to application** or to minimize the window of foreground application
   - Use the **close icon** at the thumbnail to **close the window**.
 - **Drag and drop** (using mouse and left button) the application window buttons **to reorder** them. You can reorder the same application windows ("within group") or the whole applications ("groups")
 - **Right click** on the application window button for the context menu with possibility to **close the window**, **start a new application instance** or **launch** the (recent) **item or task from JumpList**.
 - Click on the **hamburger** icon to show the menu (the search panel is open by default)
 
 ### Menu ###

 ![Menu popup](doc/img/Menu.png)

 - Click on the **desktop** icon to toggle the desktop
 - Click on the **search** icon and search for the text in window caption and name of pinned or installed application.
   - Click on the item in the search result to switch to window or launch the app. 
   - Press `ESC` key to clear the search text. When the search text is empty, it will close the search
   - Press `Enter` key to "execute" the search result shown in bold. `Up`, `Down`, `PgUp`, `PgDn` keys will move the selection
   - Use `w:` prefix to search in windows only and `a:` prefix to search the applications only
 - Click on the **pin** icon to show the list of applications pinned to Windows Start and launch them
 - Click on the **applications** icon to show the list of applications (and documents) available in the Windows Start menu
   - Click on the item in the list to lauch the application or document
   - Click on the folder item to expand/collapse the folder
   - Use the letter key (A-Z) on the keyboard to quickly move within the list if any item starting with such letter exists
 - Click on the **theme** icon to toggle between light and dark theme
 - Click on the **gear** icon to show the application settings.
   - **Set the docking edge** (top, bottom, left, right)
   - **Choose the monitor** where to display the `AppSwitcherBar`
   - Enable/disable bar and application window button **auto-size**
   - Enable/disable **starting** the `AppSwitcherBar` **on Windows start**
   - **Refresh** the list of installed applications and applications pinned to Windows Taskbar and Start
 - Click on the **power** icon to close the `AppSwitcherBar`.
 - Use the icon at the top to show/hide the button labels
 - To **close menu**
   - Click again on the hamburger icon at the main window or 
   - Use the icon with arrow down in the menu or
   - Press `Ecs` key when in search and there is no text to search for (or press `Ecs` twice - first to clear the search text)

 ### Additional panels ###

![Additional panels](doc/img/AdditionalControls.png)
- Hover over the clock to see the time in addtional **time zones** if defined in settings
- The mike and speaker icons indicates whether the default audio device is muted or not, the small accented icon also indicates that the **audio device is in use** by any application (for example - some application is capturing the mike)
- Click on the mike or speaker icon to **toggle mute** of the device
- Use the mouse wheel over mike or speaker icon to **adjust volume** of the device
- Right click on the mike or speaker icon to show the information about all **audio devices** (capture on mike icon and render on speaker icon).
  - Click on the device panel to set the **default audio device**.
  - Use the mouse wheel to **adjust volume** of the device
  - Use the click on mike or speaker icon to toggle mute of the device 

 ## Features ##
`AppSwitcherBar` will create the application bar (window) and dock it to the edge of monitor as configured in application settings (bottom edge is the default). When there is a Windows Taskbar at the same edge, the application bar is placed "besides" towards the screen content. The application bar behaves similar way like the Taskbar - it will reduce the area for the other applications so they will not overlap even when maximized.

![Main AppSwitcherBar window](doc/img/AppSwitcherBar.png)

As mentioned above, the idea is not to use any invasive techniques. It's possible to get a good result when combining the `AppSwitcherBar` with auto-hide functionality of standard Windows Taskbar. In such case the `AppSwitcherBar` seems to be "main bar", although it's possible to simply get the Windows Taskbar by moving the mouse "over the outer edge".

`AppSwitcherBar` periodically enumerates the available application windows and presents them as buttons in the bar. The current active (foreground) application window has the highlighted button. The button contains icon (if it can be retrieved) and the window title. The title can be cropped to fit the button size, the full title is available in tool-tip when hovering the mouse over the button.

Click to window button to make it active (foreground). Clicking to the button of the active application window makes such window minimized. When a file is dragged over the application window button, the application windows is also activated, allowing the easy drop of the file to the window.

The windows of the "same" applications don't group into single button (well, that's why I built the app), however the `AppSwitchBar` puts the application windows belonging to the same process together (that's what "grouping" is in context of `AppSwitchBar`).

Hovering over the button pops up the live thumbnail of the application window. Also the full window title is available in tool-tip when hovering the mouse over the button (the title within the button can be cropped to fit the button size). Click to thumbnail to switch to application window or minimize the window if it's foreground one. Use the close icon overlay to close the application window. When the feature flag `EnableContextMenuOnThumbnail` is set, the right click on the thumbnail shows the system context menu of the window the thumbnail belongs to.

![Application window thumbnail](doc/img/Thumbnail.png)


`AppSwitcherBar` shows also the buttons of the applications pinned to the task bar as transparent buttons with italics caption ("pins") when there is no window open for the application. The pin button doesn't have the thumbnail and click to the button launches the application.
*Note: The functionality uses undocumented Windows interface `IPinnedList3`, so it might not be stable. Use the setting `ShowPinnedApps` to switch it off in case of issues.*

![Application pin button](doc/img/Pin.png)

Application window or pin buttons can be reordered using mouse drag and drop. Drag the source button and drop it to the target button. When dragging over the buttons, the cursor will signal where the source will be placed if droped here. When dragging to the left/top, the source will be placed before the target. When dragging to right/bottom, the source will be placed after the target.
When the source and target window buttons belongs to the same application, only the single window button will be moved to the new position ("within the group"). The source button is in light blue color, the target in dark gray.

![Application pin button](doc/img/Reorder1.png)

When the source and target window buttons belongs to different applications, all (source) application window buttons will be moved to the new position. The source group is in light blue color, the target one in dark gray.

![Application pin button](doc/img/Reorder2.png)



Right mouse button click on the button representing a window, opens the context menu allowing to start a new instance of application or close the window. `Cancel` menu item just closes the menu without any action or you can simply click anywhere outside the menu.

![Right click menu](doc/img/RightClick.png)

When the `JumpList` feature flag is set (true by default) and the application provides JumpList, its items are also added to the context menu and can be used to launch the documents or tasks as defined by the application.

![JumpList menu](doc/img/JumpList.png)

### Menu ###
Click on the hamburger icon in the main application bar to show the menu popup. The search panel is open by default.

![Menu popup](doc/img/Menu.png)

To close the menu, click again on the hamburger icon at the main window or use the `Hide popup` item in the menu or press `Ecs` key when in search and there is no text to search for (or press `Ecs` twice - first to clear the search text). 

Use the icon at the top to show/hide the button labels. The desktop icon toggles the desktop and the theme icon toggles between light and dark theme.Click on the power icon to close the `AppSwitcherBar` application and restore the Windows content area.

#### Search ####
Click oo the search icon to open the search box. Type the text to search for within the window captions and within the names of pinned and installed applications. The search results are presented as you type. Click on the result item to switch to window or launch the application. The search is case insensitive and it's possible to limit the search to windows only using the `w:` prefix in the search text. Prefix `a:` limits the search to applications only. Use `ESC` key to clean the search text. When the search text is empty, `ESC` will close the menu popup.

![Search panel](doc/img/Search.png)

The first search result is by default selected and marked with bold caption. Press `Enter` to execute the selected (bold) result the same way as the click will do. The selection can be moved `Up` and `Down` using the arrow keys. When using `PgUp` or `PgDn`, the selection will move to the previous/next category.

The search functionality can be disabled using the `AllowSearch` setting. The settings also allow to set the `SearchResultPanelWidth` and number of items per category (`SearchListCategoryLimit`).

The search results are sorted by a rank created from
 - timestamp, when the window/application was last on foreground. This is updated each time the foreground window change is identified during periodical evaluation of application windows
 - count of runs and the last run date. This is updated when a application is launched from `AppSwitcherBar`.
 - last modified date of the application executable or application directory for Store apps.

``` csharp
var sortKey = $"{LastForegroundDateTime:yyyyMMddHHmmss}{runs:###}{LastRunDateTime:yyyyMMddHHmm}{lastModifiedDateTime:yyyyMMddHH}"
```

The run statistics used for building the search key are not persisted between `AppSwitcherBar` runs.

It's possible to use feature flag `EnableRunInfoFromWindowsPrefetch` to load the information from Windows Prefetch when retrieving the information about the installed/pinned applications. It tries to go through all Prefetch files "belonging" to the application executable (meaning that the hash code part of the Prefetch file is ignored) and extract the last run timestamp and run counter. In case there are multiple prefetch files, the most recent run timestamp is used and the sum of all run counters. In case the Prefetch file doesn't exist or data can't be retrieved, the default statistics with zero runs and `DateTime.MinValue` as timestamp are used. 

*Note: Make sure the user running the `AppSwitcherBar` has the read access to `Windows\Prefetch` directory when using the Prefetch data for search ranking (users don't have the access by default)!*

#### Windows Start pinned applications ####

When enabled by feature flag `EnableStartMenuPins`, the list of applications pinned to Windows Start is retrieved and listed in menu popup pins panel. Click to the application to launch it. On Windows 10, the folders present the tile groups, click on the folder item to expand/collapse the folder. Windows 11 Start pinned application folders are not supported (can't retrieve the information about folders).

![Start pinned applications](doc/img/StartPins.png)

 The shortcuts at the bottom of application list represents - Documents, Pictures, Downloads, Run, Console/Terminal, Devices, Windows Settings. Use the icon right to the shortcuts to switch between the list and icons view.

#### Application list ####
Click on the applications icon to show the list of applications and documents available in the Windows Start menu. The items are grouped by the first letter (A-Z) plus `#` representing digits (0-9) and `~` for other characters, the list is sorted alphabetically. Click on the folder item to expand/collapse the folder, click on the item in the list to lauch the application or document. Use the letter key (A-Z) on the keyboard to quickly move within the list if any item starting with such letter exists.

![Applications list menu](doc/img/AppList.png)

 The shortcuts at the bottom of application list represents - Documents, Pictures, Downloads, Run, Console/Terminal, Devices, Windows Settings.


#### Settings ####
The UI provides a few "on screen" (runtime) settings. Click on gear icon to open the settings panel.

![Settings panel](doc/img/Settings.png)

The top dropdown allows to change the position of the application bar (dock mode).

![Settings panel - dock position](doc/img/Edge.png)

The second dropdown allows to move the application bar to different monitor when running in multiple displays environment. The primary display is used at startup.

![Settings panel - display](doc/img/Monitor.png)

The collection of application windows (buttons) is periodically updated and the changes are applied incrementally to keep the order and position within the bar. Besides the data that can be directly retrieved during the window enumeration, `AppSwitcherBar` also use some "global" helper data about applications installed and available for current user within the system. Installed applications data are used for filling some gaps when the proper information is not retrieved directly from application window, for example icon for Windows Store apps. Installed applications data are also important to get the right Application User Model ID (used for example for grouping or JumpLists) as only a few windows or processes provide it directly. Retrieving installed application data is an operation that typically takes a few seconds, so these helper data are being retrieved on background at startup. The main `AppSwitcherBar` functionality is available during the background data retrieval, user is just notified by "Arrow with CD" cursor when hovering over the buttons, that the retrieval is still in progress.

The `Refresh` button in configuration panel forces "hard" refresh of data. The collection of application windows and pinned applications (buttons) are cleaned and rebuilt from scratch and the background data are reloaded.

It's possible to add `AppSwitcherBar` to Windows Startup folder using the checkbox `Run on Win start`. This functionality can be disabled by settings (see the next chapter).

When the auto-size is off, the buttons use the fixed size as defined in settings (`AppBarButtonWidth`, `AppBarButtonHeight`) and bar width/height is to be adjusted manually using the drag and drop thumb at the outer edge.
When the auto-size switch is on, the `AppSwitcherBar` auto-size itself as well as the application window buttons. Buttons use the fixed height as defined in settings (`AppBarButtonHeight`), the width can vary - buttons can be make smaller up to ratio defined in settings (`AppBarButtonMinWidthRatio`) - default value is 50%.
 - For vertical bar with single column, the full button size is used, when the bar has multiple columns, the minimal button width is used.
 - Horizontal bar tries to accommodate as much buttons as possible in each row adjusting the size of buttons between the minimal and full width to fill the row.

User settings are persisted to `appsettings.user.json` file in the application folder. The file contains the setting for dock position (edge), docked width and height and auto-size on/off switch. These values are used on the application start to keep the last used configuration.  

Note: To reset the user settings, simply delete the file (make sure you delete the *appsettings.**user**.json* settings file, not the *appsettings.json* containing the full application settings).


 ### Additional panels ###

![Additional panels](doc/img/AdditionalControls.png)

The clock panel can be enabled/disabled using the settings `ShowClock`. It shows the current date and time in format defines in settings `ClockDateFormat` and `ClockTimeFormat`. When a format is set to empty string, the date and/or time will not be shown. 

Hover over the clock to see the time in addtional time zones if defined in settings. Time zones are defined as key-value pairs in `ClockTimeZones`. The key is (your) name of the time zone and value is ID of the time zones as recognized by Windows. Time zones are ordered by offset. Formats for the popup items are also configurable in settings - `ClockLongFormat` for local time zone "header", `ClockTimeZoneDateFormat` for date and `ClockTimeZoneTimeFormat` for time.

![Clock popup](doc/img/ClockPopup.png)

Enable or disable the audio panel in settings using `ShowAudioControls`. When enabled, audio panel shows mike and speaker icons representing default audio capture and audio render devices. The icon indicates whether the device is muted or not - click on the icon to toggle mute of the device. The bar under the icon indicates the volume level (gray when the device is muted). Use the mouse wheel to adjust volume of the device.

![Audio in use](doc/img/AudioInUse.png)

When a device is in use (for example any application is capturing the mike), it's signaled by a small accent color icon overlay. 

Right click on the mike or speaker icon to show the information about all audio devices (capture on mike icon and render on speaker icon). The icon indicates the mute state, icon overlay indicates the device in use and the bar represents the volume level.

![Audio popup](doc/img/AudioPopup.png)

Click on the device panel to switch the default audio device. Use the mouse wheel to adjust volume and click to mike or speaker icon to toggle mute of the device. 

*Note: The default communication device can't be set using the `AppSwitcherBar` as it's not implemented. In general, the change of default device is utilizing undocumented API `IPolicyConfig`.*



## Application Settings ##
Besides the runtime configuration described above, it's possible to adjust the application settings stored in `appsettings.json` file in the application folder.

```json
{
  "AppSettings": {
    "ShowInTaskbar": true,
    "AllowRunOnWindowsStartup": true,

    "AppBarDock": "Bottom",
    "AppBarAutoSize": true,
    "AppBarDockedWidth": 160,
    "AppBarDockedHeight": 50,
    "AppBarMinWidth": 160,
    "AppBarMinHeight": 50,

    "AppBarResizeRedrawOnDrag": true,

    "AppBarButtonWidth": 150,
    "AppBarButtonMinWidthRatio": 0.5,
    "AppBarButtonHeight": 26,
    "AppBarButtonMargin": 2,

    "AllowAppBarButtonReorder": true,
    "ShowPinnedApps": true,
    "HideSingleWindowApps": false,

    "AppBarThumbnailWidth": 200,
    "AppBarThumbnailHeight": 120,
    "AppBarThumbnailDelayMs": 400,

    "RefreshWindowInfosIntervalMs": 200,
    "CheckForIconChange": true,
    "CheckForAppIdChange": false,

    "InvertWhiteIcons": true,
    "InvertBlackIcons": true,

    "JumpListCategoryLimit": 10,
    "JumpListUseTempFiles": false,

    "AllowSearch": true,
    "SearchListCategoryLimit": 5,

    "MenuPopupWidth": 400,
    "MenuPopupMaxHeight": 600,

    "StartupTheme":"System",

    "ShowAudioControls": true,
    "AudioControlsPopupWidth": 300,

    "ShowClock": true,
    "ClockWidth": 80,
    "ClockPopupWidth": 200,
    "RefreshClockIntervalMs": 250,
    "ClockDateFormat": "d.M.yyyy",
    "ClockTimeFormat": "H:mm",
    "ClockLongFormat": "dddd d. MMMM yyyy",
    "ClockTimeZoneDateFormat": "ddd d.M.",
    "ClockTimeZoneTimeFormat": "H:mm",

    "ClockTimeZones": {
      "SEA": "Pacific Standard Time",
      "LON": "GMT Standard Time",
      "PRG": "Central Europe Standard Time",
      "MNL": "Singapore Standard Time"
    },

    "FeatureFlags": {
      "JumpList": true,
      "RunOnWindowsStartup": true,
      "AnonymizeWindows": true,
      "UseApplicationResolver": true,
      "EnableColorsInMenuPopup": false,
      "KeepMenuPopupOpen": false,
      "EnableRunInfoFromWindowsPrefetch":true,
      "EnableContextMenuOnThumbnail":false
    },

    "AppIds": {
      "%windir%/explorer.exe": "Microsoft.Windows.Explorer",
      "%LOCALAPPDATA%\\Microsoft\\Teams\\current\\Teams.exe": "com.squirrel.Teams.Teams",
      "%LOCALAPPDATA%\\Microsoft\\WindowsApps\\Microsoft.WindowsTerminal_8wekyb3d8bbwe\\wt.exe": "Microsoft.WindowsTerminal_8wekyb3d8bbwe!App"
    } 
  },

  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    },
    "Debug": {
      "LogLevel": {
        "Default": "Debug"
      }
    }
  }
}
```

- `ShowInTaskbar` - Flag whether to display the `AppSwitcherBar` in the Taskbar (it automatically excludes itself from the list of windows in the bar regardless of this setting)
- `AllowRunOnWindowsStartup` - Flag whether to give use the configuration option to set `AppSwitcherBar` to run on Windows startup
- `AppBarDock` - Startup position of the application bar (Top, Left, Right, Bottom)
- `AppBarAutoSize` - Allow or block the auto-sizing the application bar and window buttons 
- `AppBarDockedWidth` - The width of the application bar when positioned vertically
- `AppBarDockedHeight` - The height of the application bar when positioned horizontally
- `AppBarMinWidth` -  The minimal width of the application bar when positioned vertically
- `AppBarMinHeight` - The minimal height of the application bar when positioned horizontally
- `AppBarResizeRedrawOnDrag` - Flag whether to redraw (resize) the application bar when dragging the thumb. If set to `false`, the bar is only resized when the thumb is dropped
- `AppBarButtonWidth` - The full width of the button representing the application window
- `AppBarButtonMinWidthRatio` - The ratio specifying the minimal button width when autosized. For example `0.6` means that button can be made smaller up to 60% of full width (`AppBarButtonWidth`)
- `AppBarButtonHeight` - The height of the button representing the application window
- `AppBarButtonMargin` - The margin to be added to window buttons as a spacer between them
- `AllowAppBarButtonReorder` - Flag whether to allow button drag and drop reorder
- `ShowPinnedApps` - Flag whether to show buttons ("pins") for applications pinned to taskbar when there is no window for pinned application
- `HideSingleWindowApps` - Flag whether to hide the applications (buttons) having only single window. When the pinned applications support is on and the application has a pin, it will be not be hiden even having a single window.
- `AppBarThumbnailWidth` - The width of application window thumbnail
- `AppBarThumbnailHeight` - The height of application window thumbnail
- `AppBarThumbnailDelayMs` - Delay (in milliseconds) before the window thumbnail is shown
- `RefreshWindowInfosIntervalMs` - The interval in milliseconds for refreshing the collection and status of the application windows 
- `CheckForIconChange` - Flag whether `AppSwitcherBar` periodically checks windows for an icon change or whether the icon retrieved for the first time is used. Switching it of can give some performance gain, however some applications change their icon during their runtime to signal user some state information.
- `CheckForAppIdChange` - Flag whether `AppSwitcherBar` periodically checks windows for change of their Application ID that has an impact to grouping the buttons together. It's off by default as the runtime changes of Application ID are quite rare.
- `InvertWhiteIcons` - When `AppSwitcherBar` uses light WPF and the host Windows are set to dark theme, some icons provided by applications are in shadows of white (different opacity to be exact) that might be hard to see on the light background. When this option is on, `AppSwitcherBar` tries to identify "all white" icons and invert the to black for better visual experience.
- `InvertBlackIcons` - Similar like `InvertWhiteIcons`, but applicable for black icons when using the dark scheme.
- `JumpListCategoryLimit` - The maximum number of items in individual JumpList categories except `Tasks`.
- `JumpListUseTempFiles` - Flag whether to extract the links (.lnk) from JumpLists to temporary files and read properties from them. Otherwise the in-memory processing is used.
- `AllowSearch` - Flag whether to allow the search functionality.
- `SearchListCategoryLimit` - Maximum number of items in single category when presenting the search results
- `MenuPopupWidth` - Width of the menu popup
- `MenuPopupMaxHeight` - Maximal height of the menu popup
- `StartupTheme` - `System`, `Light` or `Dark` theme to be used on application start.
- `ShowAudioControls` - Flag whether to show the audio device info and settings in the app bar
- `AudioControlsPopupWidth` - Width of audio controls popup
- `ShowClock` - Flag whether to show the clock in the app bar
- `ClockWidth` - Width of clock control in the app bar
- `ClockPopupWidth` - Width of clock popup
- `RefreshClockIntervalMs` - Refresh interval (in milliseconds) to update clock
- `ClockDateFormat` - Clock control date format. Keep empty to hide the date
- `ClockTimeFormat` - Clock control time format. Keep empty to hide the time
- `ClockLongFormat` - Long date and time format for time zones popup header
- `ClockTimeZones` - Optional clock time zones - dictionary name-time zone ID. Sample provided in json above.
- `ClockTimeZoneDateFormat` - Date format to be used in time zones popup
- `ClockTimeZoneTimeFormat` - Time format to be used in time zones popup
- `FeatureFlags` - Section used to configure (and allow/block) the experimental features or work in progress.
  - `JumpList` - Enable/Disable JumpList functionality.
  - `JumpListSvcVersion` - `JumpListService` version selector. `JumpListService2` (`"JumpListSvcVersion":2`) is used by default. Use `"JumpListSvcVersion":1` to switch to legacy `JumpListService`.
  - `RunOnWindowsStartup` - Enable/Disable functionality manipulating the Windows startup link for `AppSwitcherBar`. Use `AllowRunOnWindowsStartup` setting to hide the configuration option, use the feature flag to use the dummy implementation. This is probably gonna be used during the development only
  - `AnonymizeWindows` - Enable/Disable anonymization of window captions in buttons. This used when making the app screen shots 
  - `UseApplicationResolver` - Enable/Disable using the undocumented win32 api to get the AppId
  - `EnableColorsInMenuPopup` - Enable/Disable the menu popup panel showing the theme colors 
  - `KeepMenuPopupOpen` - When set, the menu popup is kept open even when another application is active 
  - `EnableRunInfoFromWindowsPrefetch` - When set, the Windows Prefetch files will be used to retrieve initial run statistics for search ranking
  - `EnableStartMenuPins` - When set, the Start pinned applications will be retrieved and presented in menu popup
  - `EnableContextMenuOnThumbnail` - When set, the right click on the thumbnail will show the system context menu of the window the thumbnail belongs to
- `AppIds` - AppIds or Application IDs are used for grouping the buttons together, but they are also used to identify the application within the system and use the information from list of installed applications to get some additional data (for example application icon) or to decide how to launch the application (desktop and Store applications are launched different way). Unfortunately, there is no simple and straight way how to obtain the AppId, so some "try and see" logic is used. For some cases it's just easier to define the AppIds for particular application directly in the configuration. `Explorer` should always be there as defined above.

The application uses the standard [.NET Configuration system](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration), so it's also possible to set the configuration values using the command line parameters or environment values. For example the theme configuration from `appsettings.json` can be overriden using the command line parameter `AppSettings:StartupTheme=Light`. There are two exceptions:
 - To set the language from command line, don't use the prefix. The parameter should be for example `Language=cs"
 - The user settings persisted in `appsettings.user.json` (see info above) takes the priority even over the command line parameters

*Note: The application uses the standard [.NET Logging API](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging) that can be configured in the settings file as well. The default logging providers are used (Console, Debug, EventSource,EventLog)* 

## Themes and Localization ##
[WPF UI](https://github.com/lepoco/wpfui) is used for modern look and feel. It brings an ability to switch between `Light` and `Dark` themes. The startup theme is managed by setting `StartupTheme` and it can be `System`, `Light` or `Dark`. When `System` is set (default), the theme is set to `Light` or `Dark` based on the current system theme. An user can toggle between `Light` and `Dark` theme using the button in the popup menu.

The UI can be translated into different languages by configuration. When the application is starting, it pre-loads the settings from `appsettings.json` command line (use `Language=code` without prefix!)`. If no setting is provided, the default system culture is used(`CultureInfo.CurrentCulture.Name`).

The language code value syntax can be:
 - two-letter language code. For example `en` or `cs`
 - combination of a two-letter language code and a two-letter country or region code. For example `en-US` or `en-GB`
 - extended even more with subtags split by hyphen (`-`)

The application will try to load the translations from `language.{code}.json`. When the code is multi part, it will try to load from "all parts" from less to more specific. For example for `en-US`, it will try   `language.en.json` and `language.en-us.json`. Language file provides the translations (values) for known keys. The language file can contain only some translations - these are the overrides of default (english) of less specific language file.

The language file has following structure:

```json
{
  "Language": {
    "Translations": {
      "MenuPopupHidePopup": "Hide popup",
      "MenuPopupSearch": "Search",
      "MenuPopupSettings": "Settings",
      "MenuPopupColors": "Colors",
      "MenuPopupApps": "Applications",
      "MenuPopupPins": "Pinned apps"
      "MenuPopupToggleDesktop": "Toggle desktop",
      "MenuPopupToggleTheme": "Toggle theme",
      "MenuPopupExit": "Exit app",
      "MenuPopupSettingsRefresh": "Refresh",
      "MenuPopupSettingsRunOnStartup": "Run on Win start",
      "MenuPopupSettingsAutoSize": "Auto size",

      "EdgeLeft": "Left",
      "EdgeRight": "Right",
      "EdgeTop": "Top",
      "EdgeBottom": "Bottom",

      "SearchCategoryWindows": "Windows",
      "SearchCategoryPinnedApps": "Pinned applications",
      "SearchCategoryInstalledApps": "Applications",
      "SearchCategoryInstalledDocs": "Documents",

      "JumpListMenuCloseWindow": "Close window",
      "JumpListMenuCancel": "Cancel",
      "JumpListCategoryTasks": "Tasks",
      "JumpListCategoryPinned": "Pinned",
      "JumpListCategoryRecent": "Recent",
      "JumpListCategoryFrequent": "Frequent",

      "CustomMostVisited": "Most visited",
      "CustomRecentlyClosed": "Recently closeed",
            
      "AudioIsDefault": "Default device",
      "AudioSetDefault": "Set as default device",
      "AudioIsCommDefault": "Default comm device"
    }
  }
} 
```

- `MenuPopup` keys are labels of buttons in menu popup (left bar) and the settings panel
- `Edge` keys are for edge names
- `SearchCategory` keys are for the names for categories in search results
- `JumplListMenu` keys are labels for close window and close jumplist commands
- `JumplListCategory` keys are for the names of known (standard) categories in jumplists
- `Audio` keys are for audio device popup
- `Custom` keys are used for custom JumpList category names that can be provided by individual applications. To translate these, create custom keys named `Custom` followed by name of the category as provided by application without spaces. The matching is case insensitive. For example `CustomMostVisited` can be used to translate the category `Most visited`.


## Behind the Scenes ##
This chapter provides some additional information about the implementation details, that might be useful for the developers.
 
### Application Desktop Toolbar (appbar) ###
Application desktop toolbar (appbar) is an application window that behaves similar way like the Windows Task bar. It is docked to an edge of the screen, and Windows restricts other applications from using the screen area occupied by the appbar. 

The Windows manages the appbars (yes, there can be multiple appbars present) and provides function `SHAppBarMessage` allowing an application implementing the appbar to control the appbar. Windows sends the `ABM` notifications as a payload of application defined message ID back to window. The appbar related logic is implemented in `AppBarWindow` class that extends the WFP `Window` and is used as a base for the main (WPF) application window.

The appbar is created from regular window by sending the message `ABM_NEW` providing the HWND of window and custom message ID (application specific) that is used for sending the notification messages from Windows to `WNDPROC` of appbar application. As WPF by default handles all low-level communication between application window and Windows, it requires bit more work to be done. `AppBarWindow.OnSourceInitialized` override gets the HWND of application window (to-be-appbar) utilizing the `HwndSource` helper and calls `AppBarWindow.AppBarInitialize` method to register appbar. WPF doesn't provide the direct access (override) for `WNDPROC`, but it's again possible to use `HwndSource` to hook `AppBarWindow.WndProc` into the message flow and intercept the messages toward the appbar window. 

Managing the size and position is a bit complex task. In general, Windows is responsible for proper sizing and positioning (incl. z-order) of the appbar so there has to be a kind of collaboration between the application and Windows.

To set the size and position, the application sends `ABM_QUERYPOS` message with `APPBARDATA` containing the information about the required dock mode (edge) and proposed bounding box. Windows checks whether the appbar can be placed to the proposed rectangle (it's not already occupied by Taskbar or another appbar) and returns (adjusted) rectangle back to application as the output of the `SHAppBarMessage`. This call is a kind of query "where can I place the appbar?".

The application use the data from position query to make a "real" size and position request sending the `ABM_SETPOS` message. Again, the system may adjust the rectangle before returning it to the application and the application finally uses the returned bounding box to set its size and position. 

The size and position flow is implemented in `AppBarWindow.AppBarUpdate` method as well as in `AppBarWindow.WndProc`. The `WNDPROC` is a WPF hook to Windows messaging towards the window. It blocks the window position or size change that is not part of "appbar flow" by tweaking `WM_WINDOWPOSCHANGING` messages. It also informs the Windows when the appbar receives `WM_ACTIVATE` and `WM_WINDOWPOSCHANGED` messages by sending `ABM_ACTIVATE` and `ABM_WINDOWPOSCHANGED` messages. 

The above mentioned messages pass through `AppBarWindow.WndProc` towards the low-level window managed by WPF. `WndProc` also listens for the message with application defined message ID (the one provided to Windows in `ABM_NEW`) as it's an endpoint for notification from appbar system. The notification type is sent in `wParam` and the `AppBarWindow.WndProc` handles the `ABN_POSCHANGED` notification causing the repeated size-and-position-flow (`AppBarWindow.AppBarUpdate`).
     
When is the appbar application closing, it must send the `ABM_REMOVE` message to unregister the appbar from system and free the desktop space for normal use. 

### Per-monitor DPI ###
The WPF applications are by default DPI-aware, however it's necessary to opt-in for Per-Monitor DPI awareness by adding/modifying the `app.manifest`as described [here](https://github.com/microsoft/WPF-Samples/blob/main/PerMonitorDPI/readme.md).

```xml
<application xmlns="urn:schemas-microsoft-com:asm.v3">
  <windowsSettings>
    <dpiAwareness xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">PerMonitorV2,PerMonitor</dpiAwareness>
    <dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true</dpiAware>
  </windowsSettings>
</application>
```
The application also overrides the `OnDpiChanged` method in `AppBarWindow` to ensure the proper size and position update when placing the appbar to monitor with different DPI setting as well to use the current DPI when working with size and position of appbar and thumbnails.

### Window buttons ###
`MainViewModel` periodically query Windows for the collection of application windows. The WPF `DispatcherTimer` is used to ensure proper thread safety and `MainViewModel.RefreshAllWindowsCollection` is called when the timer ticks.

It calls `EnumWindows` Windows API and process the result (callback). As the enum windows returns also the windows that are not to be displayed in the bar, some filtering is implemented 

```csharp
//Filter windows - only the top-level application windows except "itself" 
var isCloaked = (DwmApi.DwmGetWindowAttribute(hwnd, DWMWA_CLOAKED, out var isCloakedAttribute, sizeof(bool)) == S_OK) && isCloakedAttribute;

var wndStyle = (ulong)User32.GetWindowLongPtr(hwnd, GWL_STYLE).ToInt64();
var wndStyleEx = (ulong)User32.GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();

var isVisible = applicationHwnd != hwnd &&
     User32.IsWindowVisible(hwnd) &&
     (wndStyleEx & WS_EX_TOOLWINDOW) == 0 &&
     (wndStyle & WS_CHILD) == 0 &&
     (wndStyle & WS_CAPTION) == WS_CAPTION &&
     !isCloaked &&
     (
        (wndStyle & WS_EX_APPWINDOW) == WS_EX_APPWINDOW || User32.GetWindow(hwnd, GW_OWNER) == IntPtr.Zero
     );
  
```

The window title is retrieved using `GetWindowText` API and the windows with empty title are ignored. Next, the window thread and process handles are retrieved using `GetWindowThreadProcessId`. Process handle is currently used to group the "similar" windows in the application bar.

The icon retrieval is also a bit complex as there are multiple possible sources of the icon. First, `WM_GETICON` message is sent to source window. When the window doesn't provide icon via `WM_GETICON` (it's a valid case), the window class is queried for `GCLP_HICONSM` icon pointer. If not set, it tries to get `GCLP_HICON` pointer. When none of the methods provides the icon handle/pointer, no icon is shown in button but the window is kept in the collection.

`GetForegroundWindow` function is used to retrieve the current foreground window, so it can be highlighted in the appbar. There is a simple logic excluding the appbar window as it technically becomes the foreground window when switching the applications. 

### Application Windows Thumbnails ###
Windows Taskbar provides the application window live thumbnails on mouse hover. This functionality is provided by Desktop Window Manage (DWM) and `AppSwitcherBar` uses its API for the same functionality.

The window buttons content is defined in `MainWindow.xaml` and it also "attach" the customized `ThumbnailPopup` that extends the standard WFP `Popup`. Popup is "special" within WPF as it's rendered in own window besides the WPF Application window to allowing to render the popup content outside the application window area. The tricky part is that the popup itself is just a "blank" within the visual tree of button and the content of popup has a dedicated visual tree even with with own HWND although they share the same logical tree. Such behavior needs to be taken into the consideration when accessing the popup content, but it's very useful for required thumbnail functionality (implemented in `ThumbnailPopup` and in `MainViewModel`).

When a popup is open, it sends a command to view model with the information about popup HWND and available bounding box for the thumbnail. `MainViewModel.ShowThumbnail` calls Desktop Window Manager's function `DwmRegisterThumbnail` to establish the relationship between the source window and the target window (thumbnail). Next, it asks DWM for the size of the source window (thumbnail) via `DwmQueryThumbnailSourceSize`. The thumbnail is scaled and centered to fit the popup while keeping the aspect ratio and `DwmUpdateThumbnailProperties` function of DWM is called to start rendering the thumbnail into target window and given bounding box.

When the popup is closed, `DwmUnregisterThumbnail` function is called to stop rendering the thumbnail. 

### Application User Model ID ###
Application User Model ID (aka AUMID, AppId or Application ID) is an identifier used by Windows for grouping the buttons in taskbar. It's also key identifier of the modern applications (UWP/Windows Store apps). `AppSwitcherBar` also use AppId for grouping as well as for other functionality like launching the applications from context menu, or retrieving the JumpLists.
Modern applications use the AppIds like `Microsoft.WindowsCalculator_8wekyb3d8bbwe!App` and it serves as the identifier or application (package). Classic desktop application sometimes provide explicit custom AppId like `Microsoft.Office.EXCEL.EXE.15`, `Chrome` or `com.squirrel.Postman.Postman` however the path to the executable is used as AppId for the most of them. To keep it a bit more complicated, the path part corresponding to known system folder (for example Program Files) can be represented by [GUID of known folder](https://docs.microsoft.com/en-us/windows/win32/shell/knownfolderid) like `{6D809377-6AF0-444B-8957-A3773F02200E}\Altova\UModel2021\UModel.exe`. For some applications, the auto generated AppId is used, although it seems its value is somehow stable across Windows installations - for example `Microsoft.AutoGenerated.{BB044BFD-25B7-2FAA-22A8-6371A93E0456}` for EventViewer. Microsoft provides some general [guidance how to find the AppId](https://docs.microsoft.com/en-us/windows/configuration/find-the-application-user-model-id-of-an-installed-app).

When the feature flag `UseApplicationResolver` is set, `AppSwitcherBar` will try to use the the Windows API to provide the AppId using the method `GetAppIDForWindow' of COM class `CApplicationResolver` implementing `IApplicationResolver` interface. This interface is not documented, I found some hints at [https://a-whiter.livejournal.com/1266.html](https://a-whiter.livejournal.com/1266.html). The interface might change or not work as expected, so in case of problems, it can be switched off using the feature flag. When the API is not used or doesn't get the AppId, the following logic applies.

A few applications publish the AppId at the window level. It can be retrieved from window Property Store as a property with `PKEY_AppUserModel_ID` key. For some modern applications, the AppId can be retrieved at process level using kernel's API `GetApplicationUserModelId`, but for the most applications, `AppSwitcherBar` gets the AppId using the help of installed applications list. It enumerates the shell items in `AppsFolder` on startup (at background) and tries to get the AppId from shell item Property Store (`PKEY_AppUserModel_ID`). The information is stored in `InstalledApplications` collection together with the application executable path. When `AppSwitcherBar` needs to get AppId for application window and it's not provided explicitly for window or process, it get's the application executable (using `Kernel32.QueryFullProcessImageName`) and tries to map is to installed application information. The application executable itself is used as fallback value for AppId. When needed or for some special cases, the AppIds can be provided in `AppSwitcherBar` configuration.

### Context Menus and JumpLists ###
AppId described above is a key information needed for proper functionality of context menu application launch and JumpLists.

The classic desktop application can be simply launched from code using ` Process.Start(wnd.Executable);`, however it's more complicated to launch UWP/Store applications. I use the native/COM `ApplicationActivationManager : IApplicationActivationManager` to launch the modern application using its AppId.

JumpLists in general represent a list of recent/frequent destinations and common tasks associated with an application, and are available in the context menu of application taskbar button. The content of JumpLists is stored in `AutomaticDestinations` and/or `CustomDestinations` subfolders of user's `Recent` folder. Files are named like `ff99ba2fb2e34b73.customDestinations-ms` or `5d696d521de238c3.automaticDestinations-ms`. The name of file is `Crc64` hash of AppId (that can be path to executable in some cases or application defined string as described above). The CRC hash function has a few specifics and is implemented in class `AppIdCrc64`. This is the way, how to get the JumpList files for an application:

```csharp
var b = Encoding.Unicode.GetBytes(appId.ToUpper());
var hash = AppIdCrc64.Compute(b);
var hashStr = hash.ToString("X").ToLowerInvariant();
var recentDir = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
var customDestinationsFile = Path.Combine(recentDir, "CustomDestinations", $"{hashStr}.customDestinations-ms");
var automaticDestinationsFile = Path.Combine(recentDir, "AutomaticDestinations", $"{hashStr}.automaticDestinations-ms");
var customDestinationsFileExists = File.Exists(customDestinationsFile);
var automaticDestinationsFileExists = File.Exists(automaticDestinationsFile);
```

`JumpListService2:IJumpListService` uses undocumented Windows API `IAutomaticDestinationList2` to read the automatic destinations in proper order as well as information about pinned items. It's inspired by [Open-Shell-Menu project](https://github.com/Open-Shell/Open-Shell-Menu). Also merging of the automatic and custom destinations (categories and items) is updated to mirror Windows behavior. 

Legacy `JumpListService:IJumplistService` reads the automatic destinations from file. The automatic destinations are stored in Compound File implementation of [Structured Storage](https://docs.microsoft.com/en-us/windows/win32/stg/structured-storage-start-page) that is widely used in COM/OLE programming. .NET provides some support of Compound Files in `System.IO.Packaging.StorageInfo`, although the access to the root storage (let's say the "entry point to file") is "hidden" in `System.IO.Packaging.StorageRoot` .NET internal class, but can still be used with a dirty reflection magic. The storage contains set of streams. One, called `DestList`, contains details about the items and so far is not that much interesting unless we want to access the information whether an item is pinned. I do ignore the stream as I didn't get reliable understanding of the format that is not documented.
The other streams with numbered names represent individual items that are simply serialized shortcuts (link, `.lnk`) files.

The custom destinations are stored in files containing the information about categories (recent/frequent, tasks, custom,...) and containing items. Although the file format is not documented, it was quite easy to understand (see `JumpListService.ParseCustomDestinations` method for details) and it will also provide the serialized link files.

As the custom destinations contains links serialized as OLE Streams, it can be simply read either using `Ole32.OleLoadFromStream` into the COM object implementing `IShellLinkW`. Automatic destinations serialize the links just as binaries into the Compound file, so I did a small trick to be able to reuse the functionality - a binary buffer is created containing the CLSID of `IShellLinkW` and then the content of link file itself. That's the way how OLE streams are often stored, so I can again use `OleLoadFromStream`.

Once the `IShellLink` object is available, the basic information about the link can be retrieved using the methods of the interface.
When the setting `JumpListUseTempFiles` is set, `IPersistFile` interface of link is used to temporary store the link to file and read it as `IShellItem2` to get the access to shell properties providing the additional information about link that are used by `AppSwitcherBar`.
When the setting `JumpListUseTempFiles` is not set, `IPropertyStore` interface of link is used to process the link in-memory.
*Note: Although both methods should be identical, there are slight differences - for example in-memory processing provides thumbnails of images in icons as the icons are retrieved from link target. And it seems also a bit faster.*

### Taskbar Pinned Applications ###
Getting the information about applications pinned to taskbar seems to be a bit tricky. The simplest way is to get the links (`.lnk`) from `AppData\Roaming\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar`. Trying this method, I struggled with getting the order of pins. The list of pinned applications is also stored in registry value `HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Taskband\Favorites` - a binary value without documented format. As the information in registry value is apparently in the right order, I tried to find the name of the link file and then sorted the links (retrieved from the directory mentioned above) by the position of name within the binary registry value. 
Unfortunatelly, this method doesn't work well for Store/UWP applications. In such case, there is no `.lnk` file created in quick launch folder, but the registry value I used to get the order of links, contains the information about the UWP applications. I tried to somehow extract the AppIds of the pinned Store apps together with the information about order of pins, but it I didn't find a reliable way but to do very slow one by one check against the list of installed applications. So I gave up using this method.

I found in interesting [article](https://geelaw.blog/entries/msedge-pins/) by Gee Law elaborating the native interface `IPinnedList3` and related COM class. As the interface is not documented and there are almost no other sources of information at the internet, I had to play&fail a bit, but finally made it work as needed.

Simplified algorithm follows, the real implementation is in `JumpListService.GetPinnedApplications`.

```csharp
//get the COM class type for {90aa3a4e-1cba-4233-b8bb-535773d48449}
var objType = Type.GetTypeFromCLSID(new Guid(Win32Consts.CLSID_TaskbanPin), false);
//create an instance of COM class
var obj = Activator.CreateInstance(objType);
//get the IPinnedList3 interface
var pinnedList=obj as IPinnedList3;
//get the enumerator
var hrs = pinnedList.EnumObjects(out var iel);
do
{
   hrs = iel.Next(1, out var pidl, out _);
   if (!hrs.IsS_OK) break;

   hrs = Shell32.SHCreateItemFromIDList(pidl, iShellItem2Guid, out var shellItem);
   if (!hrs.IsS_OK || shellItem == null) break;

   //get the information from shell item representing either 
   //link to desktop app or directly Store/UWP app
   ...

   Marshal.FreeCoTaskMem(pidl);
} while (hrs.IsS_OK);
```

As mentioned above, the interface is not public and documented (and it's versioned as well), so it can change with some Windows update. Use the setting `ShowPinnedApps` to switch the functionality off in case of issues.

### Start Pinned Applications ###
The only documented way of getting the information about applications pinned to Windows Start is through the Power Shell commandlet `Export-StartLayout`. It exports the start layout into XML (Windows 10) or JSON (Windows 11) with structure corresponding to OS version. The XML for Windows 10 is documented and supports folders, JSON for Windows 11 doesn't support folders and I didn't find any documentation.
As I didn't find any Windows API that will retrieve the list of applications pinned to Start, I checked the exports of the DLL backing the Power Shell module implementing `Export-StartLayout`. I found that it references and uses COM `IStartLayoutCmdlet` and corresponding class `CStartLayoutCmdlet`, so I embedded these into `AppBarSwitcher` to get the same functionality without a need to host the Power Shell. The start menu layout is exported to temporary file using `IStartLayoutCmdlet`, the XML or JSON is then parsed and used as a source of information about pinned applications.
Retrieval of Start pinned application takes some time, so it's retrieved by backgound data service (after retrieval of installed applications)

Neither the interfaces not the way how I used it is documented, so it can change without any notice and stop working. In case of issues, disabling the feature flag `EnableStartMenuPins` should help.

## Additional (future) Feature Ideas ##
  - JumpLists for items in the application list
  - Optional auto hide functionality of appbar


## Credits & Sources Used ##
 - [Taskbar Extensions documentation](https://docs.microsoft.com/en-us/windows/win32/shell/taskbar-extensions)
 - [Application Desktop Toolbars (appbars) documentation](https://docs.microsoft.com/en-us/windows/win32/shell/application-desktop-toolbars)
 - [Desktop Window Manager (DWM) thumbnail documentation](https://docs.microsoft.com/en-us/windows/win32/dwm/thumbnail-ovw)
 - A lot of Windows API is being used, so [Windows API documentation](https://docs.microsoft.com/en-us/windows/win32/apiindex/windows-api-list) comes handy as well as
   - [PInvoke.net](https://www.pinvoke.net/) 
   - [James Forshaw's OleViewDotNet](https://github.com/tyranid/oleviewdotnet)
   - [Geoff Chappell's Win API analysis](https://www.geoffchappell.com/index.htm)
 - Windows API Code Pack 1.1 is long time depreciated but still a good source of "how to". The original code is not available anymore, but a "mirror" can be found for example [here](https://github.com/aybe/Windows-API-Code-Pack-1.1) 
 - [MahApps.Metro IconPacks](https://github.com/MahApps/MahApps.Metro.IconPacks) are good source of icons for WPF applications (was used in version 1 of the `AppSwitcherBar`)
 - [WPF UI](https://github.com/lepoco/wpfui) is used since version 2 of `AppSwitcherBar` for modern look and feel as well for suport of themes
 - WPF implementation of AppBar is based on work of [Mitch Gaffigan](https://github.com/mgaffigan/WpfAppBar)
