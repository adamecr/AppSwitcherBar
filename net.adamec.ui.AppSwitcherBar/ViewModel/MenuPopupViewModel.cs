using net.adamec.ui.AppSwitcherBar.Dto.Search;
using net.adamec.ui.AppSwitcherBar.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Startup;
using Wpf.Ui.Markup;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.WpfExt;
using Microsoft.Extensions.Options;
using net.adamec.ui.AppSwitcherBar.AppBar;
using net.adamec.ui.AppSwitcherBar.Dto.AppList;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell;
using Application = System.Windows.Application;
using Microsoft.Extensions.Logging;

namespace net.adamec.ui.AppSwitcherBar.ViewModel
{
    public class MenuPopupViewModel : INotifyPropertyChanged
    {
        #region Logging
        /// <summary>
        /// Logger used
        /// </summary>
        private readonly ILogger logger;


        //EventIds:
        // 1xx - Windows API "interactions"
        // 2xx - Application Window Button interactions
        // 3xx - Telemetry
        // 9xx - Errors/Exceptions
        // ----
        // 1xxx - JumpList Service (19xx Errors/Exceptions)
        // 2xxx - Startup Service (29xx Errors/Exceptions)
        // 3xxx - AppBarWindow (39xx Errors/Exceptions)
        // 4xxx - BackgroundData Service (49xx Errors/Exceptions)

        /// <summary>
        /// Log definition options
        /// </summary>
        private static readonly LogDefineOptions LogOptions = new() { SkipEnabledCheck = true };
        //----------------------------------------------
        // 901 CantStartApp
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogCantStartApp
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly Action<ILogger, string, Exception?> __LogCantStartAppDefinition =
            LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(901, nameof(LogCantStartApp)),
                "Cant's start application {name}",
                LogOptions);

        /// <summary>
        /// Logs record (Warning) of exception thrown when starting the app from context menu or search result
        /// </summary>
        /// <param name="name">Name of the application</param>
        /// <param name="ex">Exception thrown</param>
        private void LogCantStartApp(string name, Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                __LogCantStartAppDefinition(logger, name, ex);
            }
        }
        #endregion

        /// <summary>
        /// Reference to <see cref="MainWindow"/> view model
        /// </summary>
        private MainViewModel Main { get; }

        /// <summary>
        /// Startup service to be used
        /// </summary>
        private IStartupService StartupService { get; }

        /// <summary>
        /// Language  service to be used
        /// </summary>
        private ILanguageService LanguageService { get; }

        /// <summary>
        /// Background data service to be used
        /// </summary>
        private IBackgroundDataService BackgroundDataService { get; }

        /// <summary>
        /// Flag whether the app uses the dark theme
        /// </summary>
        private bool IsDarkTheme => Main.IsDarkTheme;


        /// <summary>
        /// Application settings
        /// </summary>
        public IAppSettings Settings { get; }

        /// <summary>
        /// Array of the screen edges the app-bar can be docked to
        /// </summary>
        public EdgeInfo[] Edges { get; }

        /// <summary>
        /// Array of the information about all monitors (displays)
        /// </summary>
        public MonitorInfo[] AllMonitors => Main.AllMonitors;

        /// <summary>
        /// Flag whether the option to set Run On Windows Startup is available
        /// </summary>
        public bool RunOnWinStartupAvailable => Settings.AllowRunOnWindowsStartup;

        /// <summary>
        /// Flag whether the AppSwitcherBar is set to run on Windows startup
        /// </summary>
        private bool runOnWinStartupSet;
        /// <summary>
        /// Flag whether the AppSwitcherBar is set to run on Windows startup
        /// </summary>
        public bool RunOnWinStartupSet
        {
            get => runOnWinStartupSet;
            set
            {
                if (runOnWinStartupSet != value)
                {
                    runOnWinStartupSet = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Flag whether the search is in progress 
        /// </summary>
        private bool isInSearch;

        /// <summary>
        /// Flag whether the search is in progress 
        /// </summary>
        public bool IsInSearch
        {
            get => isInSearch;
            set
            {
                if (isInSearch != value)
                {
                    isInSearch = value;
                    if (isInSearch)
                    {
                        //switch panels
                        IsInSettings = false;
                        IsInColors = false;
                        IsInApps = false;

                        //init search
                        InitSearch();
                    }
                    else
                    {
                        EndSearch();
                    }
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Flag whether the search has results
        /// </summary>
        private bool hasSearchResults;

        /// <summary>
        /// Flag whether the search has results 
        /// </summary>
        public bool HasSearchResults
        {
            get => hasSearchResults;
            private set
            {
                if (hasSearchResults != value)
                {
                    hasSearchResults = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Text to be searched for
        /// </summary>
        private string? searchText;

        /// <summary>
        /// Text to be searched for
        /// </summary>
        public string? SearchText
        {
            get => searchText;
            set
            {
                if (searchText != value)
                {
                    searchText = value;
                    DoSearch(searchText);
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Flag whether the settings panel is active 
        /// </summary>
        private bool isInSettings;

        /// <summary>
        /// Flag whether the settings panel is active 
        /// </summary>
        public bool IsInSettings
        {
            get => isInSettings;
            set
            {
                if (isInSettings != value)
                {
                    isInSettings = value;
                    if (isInSettings)
                    {
                        //switch panels
                        EndSearch();
                        IsInColors = false;
                        IsInApps = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Flag whether the colors panel is active 
        /// </summary>
        private bool isInColors;

        /// <summary>
        /// Flag whether the colors panel is active 
        /// </summary>
        public bool IsInColors
        {
            get => isInColors;
            set
            {
                if (isInColors != value)
                {
                    isInColors = value;
                    if (isInColors)
                    {
                        //switch panels
                        EndSearch();
                        IsInSettings = false;
                        IsInApps = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Flag whether the apps panel is active 
        /// </summary>
        private bool isInApps;

        /// <summary>
        /// Flag whether the apps panel is active 
        /// </summary>
        public bool IsInApps
        {
            get => isInApps;
            set
            {
                if (isInApps != value)
                {
                    isInApps = value;
                    if (isInApps)
                    {
                        //switch panels
                        EndSearch();
                        IsInSettings = false;
                        IsInColors = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Flag whether the menu popup is active  
        /// </summary>
        private bool isInMenuPopup;
        /// <summary>
        /// Flag whether the menu popup is active 
        /// </summary>
        public bool IsInMenuPopup
        {
            get => isInMenuPopup;
            set
            {
                if (isInMenuPopup != value)
                {
                    isInMenuPopup = value;
                    if (isInMenuPopup)
                    {
                        IsInSearch = true; //init search on open
                    }
                    else
                    {
                        IsInSearch = false; //ensure to finish search
                        IsInSettings = false; //ensure to "hide" settings
                        IsInColors = false; //ensure to "hide" colors
                        IsInApps = false; //ensure to "hide" apps
                    }

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Flag whether the colors panel is enabled
        /// </summary>
        public bool IsColorsEnabled { get; }

        /// <summary>
        /// Information about brushes in Light and Dark themes
        /// </summary>
        public ObservableCollection<BrushInfo> ThemeBrushes { get; } = new();

        /// <summary>
        /// Set of search results
        /// </summary>
        public ObservableCollection<SearchResultItem> SearchResults { get; } = new();


        /// <summary>
        /// Set of application list items
        /// </summary>
        public ObservableCollection<AppListItem> AppList { get; } = new();

        /// <summary>
        /// Command sending a special key press related to search
        /// </summary>
        public ICommand SearchSpecialKeyCommand { get; }

        /// <summary>
        /// Command requesting to show search 
        /// </summary>
        public ICommand ShowSearchCommand { get; }

        /// <summary>
        /// Command requesting to show settings
        /// </summary>
        public ICommand ShowSettingsCommand { get; }


        /// <summary>
        /// Command requesting to show colors
        /// </summary>
        public ICommand ShowColorsCommand { get; }

        /// <summary>
        /// Command requesting to show installed apps & docs
        /// </summary>
        public ICommand ShowAppsCommand { get; }

        /// <summary>
        /// Command sending a key press related to app list
        /// </summary>
        public ICommand AppListKeyCommand { get; }

        /// <summary>
        /// Command requesting to toggle desktop
        /// </summary>
        public ICommand ToggleDesktopCommand { get; }

        /// <summary>
        /// Command requesting to toggle themes
        /// </summary>
        public ICommand ToggleThemeCommand { get; }

        /// <summary>
        /// Command requesting to hide menu popup
        /// </summary>
        public ICommand HideMenuPopupCommand { get; }

        /// <summary>
        /// Command requesting to toggle Run on Windows startup - set/remove the startup link
        /// </summary>
        public ICommand ToggleRunOnStartupCommand { get; }

        /// <summary>
        /// Command requesting an "ad-hoc" refresh of the list of application windows (no param used)
        /// </summary>
        public ICommand RefreshWindowCollectionCommand { get; }

        /// <summary>
        /// Name of the Feature Flag for enabling the color panel in menu popup
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const string FF_EnableColorsInMenuPopup = "EnableColorsInMenuPopup";

        /// <summary>
        /// Internal CTOR
        /// </summary>
        /// <param name="main">Reference to main view model</param>
        /// <param name="settings">Application setting</param>
        /// <param name="logger">Logger to be used</param>
        /// <param name="startupService">Startup service to be used</param>
        /// <param name="languageService">Language  service to be used</param>
        /// <param name="backgroundDataService">Background Data service to be used</param>
        internal MenuPopupViewModel(MainViewModel main, IAppSettings settings, ILogger logger, IStartupService startupService, ILanguageService languageService, IBackgroundDataService backgroundDataService)
        {
            this.logger = logger;

            Main = main;
            Settings = settings;
            StartupService = startupService;
            LanguageService = languageService;
            BackgroundDataService = backgroundDataService;

            ToggleRunOnStartupCommand = new RelayCommand(ToggleRunOnWinStartup);
            SearchSpecialKeyCommand = new RelayCommand(SearchSpecialKey);
            ShowSettingsCommand = new RelayCommand(ShowSettings);
            ToggleDesktopCommand = new RelayCommand(ToggleDesktop);
            ToggleThemeCommand = new RelayCommand(ToggleTheme);
            ShowSearchCommand = new RelayCommand(ShowSearch);
            ShowColorsCommand = new RelayCommand(ShowColors);
            ShowAppsCommand = new RelayCommand(ShowApps);
            AppListKeyCommand = new RelayCommand(AppListKey);
            HideMenuPopupCommand = new RelayCommand(HideMenuPopup);
            RefreshWindowCollectionCommand = new RelayCommand(Main.RefreshAllWindowsCollection);

            runOnWinStartupSet = startupService.HasAppStartupLink();
            IsColorsEnabled = Settings.FeatureFlag(FF_EnableColorsInMenuPopup, false);
            LanguageService = languageService;

            Edges = new EdgeInfo[] {
                new(AppBarDockMode.Left,LanguageService.Translate(TranslationKeys.EdgeLeft)),
                new(AppBarDockMode.Right,LanguageService.Translate(TranslationKeys.EdgeRight)),
                new(AppBarDockMode.Top,LanguageService.Translate(TranslationKeys.EdgeTop)),
                new(AppBarDockMode.Bottom,LanguageService.Translate(TranslationKeys.EdgeBottom))};

            backgroundDataService.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(BackgroundDataService.BackgroundDataRetrieved) && backgroundDataService.BackgroundDataRetrieved)
                {
                    BuildAppList();
                }
            };
        }

        /// <summary>
        /// CTOR used by DI
        /// </summary>
        /// <param name="main">Reference to main view model</param>
        /// <param name="logger">Logger to be used</param>
        /// <param name="options">Application settings configuration</param>
        /// <param name="startupService">Startup service to be used</param>
        /// <param name="languageService">Language  service to be used</param>
        /// <param name="backgroundDataService">Background Data service to be used</param>
        // ReSharper disable once UnusedMember.Global
        public MenuPopupViewModel(MainViewModel main, ILogger<MainViewModel> logger, IOptions<AppSettings> options, IStartupService startupService, ILanguageService languageService, IBackgroundDataService backgroundDataService)
            : this(main, options.Value,logger, startupService, languageService, backgroundDataService)
        {
            //used from DI - DI populates the parameters and the internal CTOR is called then
        }

        /// <summary>
        /// Fills/Refreshes the <see cref="ThemeBrushes"/> collection from current theme
        /// </summary>
        internal void FillThemeResourcesColl()
        {
            foreach (var name in Enum.GetNames<ThemeResource>())
            {
                if (!name.ToLower().EndsWith("brush")) continue;
                var resource = Application.Current.Resources[name];
                if (resource is not Brush brush) continue;

                var brushInfo = ThemeBrushes.FirstOrDefault(b => b.Name == name);
                if (brushInfo == null)
                {
                    brushInfo = new BrushInfo(name, brush, IsDarkTheme);
                    ThemeBrushes.Add(brushInfo);
                }
                else
                {
                    brushInfo.SetBrush(brush, IsDarkTheme);
                }
            }
        }

        /// <summary>
        /// Toggles Run On Windows startup option.
        /// When it's being set, the AppSwitcherBar link is created in Windows startup folder
        /// When it's being re-set, the AppSwitcherBar link is removed from Windows startup folder
        /// </summary>
        private void ToggleRunOnWinStartup()
        {
            if (StartupService.HasAppStartupLink())
            {
                StartupService.RemoveAppStartupLink();
            }
            else
            {
                StartupService.CreateAppStartupLink("AppSwitcherBar application");
            }

            RunOnWinStartupSet = StartupService.HasAppStartupLink();
        }

        #region Search

        /// <summary>
        /// Initialize search
        /// </summary>
        private void InitSearch()
        {
            IsInSettings = false; //ensure to switch the "tab" in popup
            if (!Settings.AllowSearch) return;

            //clean the search box
            SearchText = string.Empty;

            IsInSearch = true;

        }

        /// <summary>
        /// Execute the search. Searches for the <paramref name="text"/> in the window captions and names of pinned and installed applications.
        /// Use prefix "w:" to search in window captions only or "a:" to search in applications only
        /// </summary>
        /// <param name="text">Text to search for</param>
        private void DoSearch(string? text)
        {
            if (!Settings.AllowSearch) return;

            if (string.IsNullOrEmpty(text))
            {
                SearchResults.Clear();
                HasSearchResults = false;
                return;
            }

            var lastDefaultRef = GetSearchResultDefault()?.ResultReference;
            SearchResults.Clear();
            var isWindowsOnlySearch = false;
            var isAppsOnlySearch = false;

            if (text.ToLowerInvariant().StartsWith("w:"))
            {
                text = (text + " ")[2..];
                isWindowsOnlySearch = true;
            }
            else if (text.ToLowerInvariant().StartsWith("a:"))
            {
                text = (text + " ")[2..];
                isAppsOnlySearch = true;
            }

            text = text.Trim();

            var categoryLimit = Settings.SearchListCategoryLimit;
            var needsSeparator = false;

            //windows
            if (!isAppsOnlySearch)
            {
                var windows = Main.ButtonManager
                    .Where(b => b is WndInfo && b.Title.Contains(text, StringComparison.InvariantCultureIgnoreCase))
                    .Cast<WndInfo>().ToArray();
                if (windows.Length > 0)
                {
                    SearchResults.Add(new SearchResultItemHeader(LanguageService.Translate(TranslationKeys.SearchCategoryWindows) ?? "Windows"));
                    foreach (var window in windows.Take(categoryLimit))
                    {
                        var item = new SearchResultItemWindow(window, w => Main.ToggleApplicationWindow(w.Hwnd, true));
                        if (window == lastDefaultRef)
                        {
                            item.IsDefault = true;
                        }

                        SearchResults.Add(item);
                    }

                    if (windows.Length > categoryLimit) SearchResults.Add(new SearchResultItemMoreItems());

                    needsSeparator = true;
                }
            }

            //pinned and installed apps
            if (!isWindowsOnlySearch)
            {
                var pins = Main.PinnedApplications
                    .Where(b => b.Title.Contains(text, StringComparison.InvariantCultureIgnoreCase))
                    .ToArray();
                if (pins.Length > 0)
                {
                    if (needsSeparator)
                    {
                        SearchResults.Add(new SearchResultItemSeparator());
                    }

                    SearchResults.Add(new SearchResultItemHeader(LanguageService.Translate(TranslationKeys.SearchCategoryPinnedApps) ?? "Pinned applications"));
                    foreach (var pin in pins.Take(categoryLimit))
                    {
                        var item = new SearchResultItemPinnedApp(pin, Main.LaunchPinnedApp);
                        if (pin == lastDefaultRef)
                        {
                            item.IsDefault = true;
                        }

                        SearchResults.Add(item);
                    }

                    if (pins.Length > categoryLimit) SearchResults.Add(new SearchResultItemMoreItems());

                    needsSeparator = true;
                }


                var installs = BackgroundDataService.InstalledApplications.SearchByName(text).ToArray();
                if (installs.Length > 0)
                {
                    if (needsSeparator)
                    {
                        SearchResults.Add(new SearchResultItemSeparator());
                    }

                    SearchResults.Add(new SearchResultItemHeader(LanguageService.Translate(TranslationKeys.SearchCategoryInstalledApps) ?? "Applications"));
                    foreach (var install in installs.Take(categoryLimit))
                    {
                        var item = new SearchResultItemInstalledApp(install, Main.LaunchInstalledApp);
                        if (install == lastDefaultRef)
                        {
                            item.IsDefault = true;
                        }

                        SearchResults.Add(item);
                    }

                    if (installs.Length > categoryLimit) SearchResults.Add(new SearchResultItemMoreItems());

                    needsSeparator = true;
                }
            }

            //installed non-apps (documents)
            if (!isWindowsOnlySearch && !isAppsOnlySearch)
            {
                var docs = BackgroundDataService.InstalledApplications.SearchDocumentsByName(text).ToArray();
                if (docs.Length > 0)
                {
                    if (needsSeparator)
                    {
                        SearchResults.Add(new SearchResultItemSeparator());
                    }

                    SearchResults.Add(new SearchResultItemHeader(LanguageService.Translate(TranslationKeys.SearchCategoryInstalledDocs) ?? "Documents"));
                    foreach (var install in docs.Take(categoryLimit))
                    {
                        var item = new SearchResultItemInstalledDoc(install, Main.LaunchInstalledApp);
                        if (install == lastDefaultRef)
                        {
                            item.IsDefault = true;
                        }

                        SearchResults.Add(item);
                    }

                    if (docs.Length > categoryLimit) SearchResults.Add(new SearchResultItemMoreItems());
                }
            }

            HasSearchResults = SearchResults.Count > 0;
            if (GetSearchResultDefault() == null && HasSearchResults)
            {
                GetSearchResultsWithRef()[0].IsDefault = true;
            }
        }

        //Finishes search - cleanup
        private void EndSearch()
        {
            //clean the search box
            SearchText = string.Empty;
            IsInSearch = false;
        }

        /// <summary>
        /// Process the special key for search 
        /// Parameter <paramref name="param"/> must be <see cref="Key"/> value
        /// </summary>
        /// <param name="param"><see cref="Key"/> pressed </param>
        /// <exception cref="ArgumentException">When the <paramref name="param"/> is not <see cref="Key"/> object or is null, <see cref="ArgumentException"/> is thrown</exception>
        private void SearchSpecialKey(object? param)
        {
            if (!Settings.AllowSearch) return;

            if (param is not KeyCommand.CommandParameters commandParameters)
            {
                throw new ArgumentException($"Command parameter must be {nameof(KeyCommand.CommandParameters)}", nameof(param));
            }

            var key = commandParameters.Key;

            if (key == Key.Escape)
            {
                if (!string.IsNullOrEmpty(SearchText))
                {
                    SearchText = string.Empty;
                }
                else
                {
                    EndSearch();
                    HideMenuPopup();
                }
            }

            if (key == Key.Enter)
            {
                GetSearchResultDefault()?.Launch();
            }

            if (key == Key.Up)
            {
                var results = GetSearchResultsWithRef();
                var current = GetSearchResultDefault();
                if (current != null)
                {
                    var idx = results.IndexOf(current);
                    idx--;
                    if (idx >= 0)
                    {
                        current.IsDefault = false;
                        results[idx].IsDefault = true;
                    }
                }
            }

            if (key == Key.Down)
            {
                var results = GetSearchResultsWithRef();
                var current = GetSearchResultDefault();
                if (current != null)
                {
                    var idx = results.IndexOf(current);
                    idx++;
                    if (idx < results.Count)
                    {
                        current.IsDefault = false;
                        results[idx].IsDefault = true;
                    }
                }
            }

            if (key == Key.PageUp)
            {
                var results = GetSearchResultsWithRef();
                var current = GetSearchResultDefault();

                if (current != null)
                {
                    var idx = results.IndexOf(current);
                    var currentType = current.GetType();

                    do
                    {
                        idx--;
                        if (idx < 0 || results[idx].GetType() == currentType) continue;

                        current.IsDefault = false;
                        results[idx].IsDefault = true;
                        break;
                    } while (idx >= 0);
                }
            }

            if (key == Key.PageDown)
            {
                var results = GetSearchResultsWithRef();
                var current = GetSearchResultDefault();

                if (current != null)
                {
                    var idx = results.IndexOf(current);
                    var currentType = current.GetType();

                    do
                    {
                        idx++;
                        if (idx >= results.Count || results[idx].GetType() == currentType) continue;

                        current.IsDefault = false;
                        results[idx].IsDefault = true;
                        break;
                    } while (idx < results.Count);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="SearchResults"/> that can be launched (windows, applications).
        /// </summary>
        /// <returns>Search results that can be launched (windows, applications)</returns>
        private List<SearchResultItemWithRef> GetSearchResultsWithRef()
        {
            var results = SearchResults.Where(r => r is SearchResultItemWithRef).Cast<SearchResultItemWithRef>().ToList();
            return results;
        }

        /// <summary>
        /// Gets the search result that is marked as <see cref="SearchResultItemWithRef.IsDefault"/>
        /// </summary>
        /// <returns>Default search result or null if no search result is marked as default</returns>
        private SearchResultItemWithRef? GetSearchResultDefault()
        {
            var result = GetSearchResultsWithRef().FirstOrDefault(r => r.IsDefault);
            return result;
        }

        /// <summary>
        /// Switch the panel to Search
        /// </summary>
        private void ShowSearch()
        {
            IsInSearch = true;
        }

        #endregion

        /// <summary>
        /// Switch the panel to Settings
        /// </summary>
        private void ShowSettings()
        {
            IsInSettings = true;
        }

        /// <summary>
        /// Switch the panel to Colors
        /// </summary>
        private void ShowColors()
        {
            IsInColors = true;
        }

        /// <summary>
        /// Switch the panel to Apps
        /// </summary>
        private void ShowApps()
        {
            IsInApps = true;
        }

        /// <summary>
        /// Builds the Apps list
        /// </summary>
        private void BuildAppList()
        {
            AppList.Clear();

            var appListItemsSource = BackgroundDataService.InstalledApplications.GetAllItems().OrderBy(a => a.AppListSortKey).ToArray();
            if (appListItemsSource.Length > 0)
            {

                var lastCategory = "";
                var lastFolderName = "";
                AppListFolder? lastFolder = null;

                foreach (var sourceItem in appListItemsSource)
                {
                    if (lastCategory != sourceItem.AppListCategory)
                    {
                        //new category
                        lastCategory = sourceItem.AppListCategory;
                        var categoryItem = new AppListCategory(lastCategory);
                        AppList.Add(categoryItem);
                    }

                    if (lastFolderName != sourceItem.AppListFolder)
                    {
                        lastFolderName = sourceItem.AppListFolder;
                        lastFolder = lastFolderName != string.Empty ? new AppListFolder(lastFolderName) : null;
                        if (lastFolder != null) AppList.Add(lastFolder);
                    }

                    var item = new AppListInstalledItem(sourceItem, lastFolder != null, a => a.LaunchInstalledApp(e =>
                    {
                        LogCantStartApp(sourceItem.ShellProperties.IsStoreApp ? sourceItem.AppUserModelId ?? "[Null] appID" : sourceItem.Executable ?? "[Null] file", e);
                    })); 

                    lastFolder?.AddItem(item);
                    AppList.Add(item);
                }
            }
        }


        /// <summary>
        /// Process the key for app list
        /// Parameter <paramref name="param"/> must be <see cref="Key"/> value
        /// </summary>
        /// <param name="param"><see cref="Key"/> pressed </param>
        /// <exception cref="ArgumentException">When the <paramref name="param"/> is not <see cref="Key"/> object or is null, <see cref="ArgumentException"/> is thrown</exception>
        private void AppListKey(object? param)
        {
            if (param is not KeyCommand.CommandParameters commandParameters)
            {
                throw new ArgumentException($"Command parameter must be {nameof(KeyCommand.CommandParameters)}", nameof(param));
            }

            var sender = commandParameters.Sender;
            var key = commandParameters.Key;


            if (key is >= Key.A and <= Key.Z)
            {

                var category = key.ToString();
                var appListItem = AppList.FirstOrDefault(a => a is AppListCategory c && c.Title == category);
                if (appListItem == null) return;

                var categoryUiElement = WpfTools.FirstOrDefaultChild<MenuItem>(sender, i => i.DataContext == appListItem);
                categoryUiElement?.BringIntoView();
            }
        }

        /// <summary>
        /// Toggle application theme between Light and Dark
        /// </summary>
        private void ToggleTheme()
        {
            Theme.Apply(IsDarkTheme ? ThemeType.Light : ThemeType.Dark, BackgroundType.None, true, true);
            Main.IsDarkTheme = Theme.GetAppTheme() == ThemeType.Dark;

            //refresh info about brushes (colors)
            FillThemeResourcesColl();

            //refresh icons from applications
            Main.RefreshAllWindowsCollection(true);

#if DEBUG
            Debug.WriteLine($"Theme change to {(IsDarkTheme ? "Dark" : "Light")}");
#endif
        }

        /// <summary>
        /// Toggles Windows Desktop
        /// </summary>
        private void ToggleDesktop()
        {
            Shell.ToggleDesktop();
        }

        /// <summary>
        ///  Close the menu popup
        /// </summary>
        private void HideMenuPopup()
        {
            IsInMenuPopup = false;
            Main.IsInMenuPopup = false;
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
}
