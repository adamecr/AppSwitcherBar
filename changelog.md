# Changelog #
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

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

[1.2.0]: https://github.com/adamecr/AppSwitcherBar/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/adamecr/AppSwitcherBar/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/adamecr/AppSwitcherBar/releases/tag/v1.0.0