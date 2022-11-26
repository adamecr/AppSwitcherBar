# Changelog #
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [2.0.0] - 2022-11-26 ##
### Added ###
- **Support for Dark and Light UI theme.** 
- **Support for UI translation for other languages**
- When a file is dragged over the application window button, the application windows is activated, so the file can easily be droppped there
- Application setting `InvertBlackIcons`, `StartupTheme`, `MenuPopupWidth`, `MenuPopupMaxHeight`.
- Special application setting `Language`

### Changed ###
- **Major UI rework**
- Removed application setting `SearchResultPanelWidth`

### Fixed ###
- Fixed the inconsistency in AppUI for some pinned applications and "their" windows causing that the window button didn't "replace" the button of pinned app. Now the win32 app resolver is used also when retrieving the pinned apps if needed
- Fixed the handling of "shell targets" - installed applications having executable in form `::{guid}`
- Application button autosize - `ButtonMinWidthRatio` of `AppButtonsPanel` was not bound to `Settings.AppBarButtonMinWidthRatio` but had a constant value 0.7, so the settings was ignored

## [1.3.1] - 2022-10-28 ##
### Fixed ###
- [Thumbnail popup in vertical mode](https://github.com/adamecr/AppSwitcherBar/issues/3) - thumbnail popup placement is now set based on the appbar position (orientation)

### Added ###
- Thumnbnail popup is shown with configurable delay as proposed in [Thumbnail popup in vertical mode](https://github.com/adamecr/AppSwitcherBar/issues/3)
- Application setting `AppBarThumbnailDelayMs`

## [1.3.0] - 2022-10-23 ##
### Added ###
- **Search support** - allows to search by window caption, pinned application name and installed application name. The search results are presented in the list and allow the user to launch them.
- Version 2 of JumpList service (`JumpListService2`)
  - uses IID_IAutomaticDestinationList2 API to read the automatic destinations
  - "merging" of the automatic and custom destinations (categories and items) updated to mirror Windows behavior
  - Version (1 or 2) can be set by feature flag `JumpListSvcVersion`. Version 2 is used by default
- Application settings: `AllowSearch`,`SearchListCategoryLimit`, `SearchResultPanelWidth`

## [1.2.1] - 2022-10-15 ##
### Added ###
- Logging of unhandled exceptions, so it can be possible to check in EventLog the details when the application crash
- Experimental using the undocumented win32 api to get the AppId (under featture flag `UseApplicationResolver`)

### Fixed ###
- Corrections in the "visible windows" filter
- The periodical check and update of windows (buttons) is skipped when the refresh is still in progress (caused the application crash)
- Corrections to JumpList parsing - "early" recognize the issues with the structure and soft-break.
- Corrections to JumpList parsing - use known AppIds from settings when can't get the appId from link. Added Windows Terminal app to settings know app ids.
- Corrections to JumpList parsing - the "known category" type of record in custom destinations is just a "flag" not a list of links
- Some corrections in the documentation
- Exception handling in `WndAndApp.GetWindowIcon` when calling `Imaging.CreateBitmapSourceFromHIcon`


### Fixed ###
- Exclude non runnable links from installed applications (these are usually document/web shortcuts in Start menu)

## [1.2.0] - 2022-08-20 ##
### Added ###
- **Application bar and application window buttons auto-size support**
- **Application window buttons reorder support**
- **Support for taskbar pinned applications**
- Option to hide the applications having single window (if not pinned)
- Persist and use the user settings - dock mode, autosize on/off, application bar dock size
- Application settings: `AppBarAutoSize`,`AppBarButtonHeight`, `AppBarButtonMinWidthRatio`, `AppBarButtonMargin`, `JumpListUseTempFiles`, `AllowAppBarButtonReorder`, `ShowPinnedApps`, `HideSingleWindowApps`

### Changed ###
- Application settings defaults adjusted
- JumpList links are by default processes in-memory (new functionality), however it's still possible to use processing using the temporary files when setting `JumpListUseTempFiles` is set to `true`.

### Fixed ###
- Properly handles the full screen applications

## [1.1.0] - 2022-08-02 ##
### Added ###
- **Context menu allowing to close the application window, start a new instance of application and present the application JumpList**  (Feature flag `JumpList` for JumpLists)
- Background worker collecting the information about installed applications and known folders on AppSwitcherBar start (or hard refresh).
- Information about installed applications is used to "fill the gaps" when needed - for example to properly handle the Store/UWP applications (packages) or to get the application icon if not retrieved from window
- As AppSwitcherBar UI uses the default light theme, it can now invert all-white icons to black when presenting them at UI (this happens when the system runs using the dark theme) 
- Allow the user to run AppSwitcherBar at Windows Startup (Feature flag `RunOnWindowsStartup`)
- Application settings: `CheckForIconChange`, `CheckForAppIdChange`, `AppIds`, `InvertWhiteIcons`, `JumpListCategoryLimit`, `AllowRunOnWindowsStartup`; Feature flags: `JumpList`,`RunOnWindowsStartup`

### Changed ###
- AppSwitcherBar executable name is without namespace now
- Window buttons ordering and grouping has been moved to `WndButtonManager` class and improved for better results of grouping
- Native code is "hidden" behind services or "proxy" static classes as much as possible to provided simple and clear "interface" to front end logic 

### Fixed ###
- Enumeration of application windows can properly get the process ID for the applications launched by application host
- Window visibility condition has been updated to exclude some additional windows that should not have the application button (don't represent the application window)

## [1.0.0] - 2022-07-03 ##
### Added ###
- Initial application release

[2.0.0]: https://github.com/adamecr/AppSwitcherBar/compare/v1.3.1...v2.0.0
[1.3.1]: https://github.com/adamecr/AppSwitcherBar/compare/v1.3.0...v1.3.1
[1.3.0]: https://github.com/adamecr/AppSwitcherBar/compare/v1.2.1...v1.3.0
[1.2.1]: https://github.com/adamecr/AppSwitcherBar/compare/v1.2.0...v1.2.1
[1.2.0]: https://github.com/adamecr/AppSwitcherBar/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/adamecr/AppSwitcherBar/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/adamecr/AppSwitcherBar/releases/tag/v1.0.0