using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.ViewModel;
using net.adamec.ui.AppSwitcherBar.Win32.NativeClasses;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces.Extensions;
using net.adamec.ui.AppSwitcherBar.Win32.NativeMethods;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;

// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.JumpLists
{
    /// <summary>
    /// Encapsulates the JumpList functionality
    /// V2 of service
    ///  - uses IAutomaticDestinationList2 API to read the automatic destinations
    ///  - "merging" of the automatic and custom destinations (categories and items) updated to mirror Windows behavior
    ///  - inspired by https://github.com/Open-Shell/Open-Shell-Menu
    /// </summary>
    public class JumpListService2 : IJumpListService
    {
        #region Logging
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// Logger used
        /// </summary>
        private readonly ILogger logger;
        // 1xxx - JumpList Service (19xx Errors/Exceptions)
        // 12xx -   Taskbar pinned apps
        /// <summary>
        /// Log definition options
        /// </summary>
        private static readonly LogDefineOptions LogOptions = new() { SkipEnabledCheck = true };


        //----------------------------------------------
        // 1001 Processing JumpList file start
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogJumpListProcessingStart
        /// </summary>
        private static readonly Action<ILogger, string, Exception?> __LogJumpListProcessingStartDefinition =

            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(1002, nameof(LogJumpListProcessingStart)),
                "JumpList processing starts: {fileName}",
                LogOptions);

        /// <summary>
        /// Logs record (Information) when a jump list file processing starts
        /// </summary>
        /// <param name="fileName">Source file of the JumpList item (full path)</param>
        private void LogJumpListProcessingStart(string fileName)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                __LogJumpListProcessingStartDefinition(logger, fileName, null);
            }
        }

        //----------------------------------------------
        // 1002 Processing JumpList file end
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogJumpListProcessingEnd
        /// </summary>
        private static readonly Action<ILogger, string, int, Exception?> __LogJumpListProcessingEndDefinition =

            LoggerMessage.Define<string, int>(
                LogLevel.Information,
                new EventId(1002, nameof(LogJumpListProcessingEnd)),
                "JumpList processing ends: {fileName}, {itemsCount} retrieved",
                LogOptions);

        /// <summary>
        /// Logs record (Information) when a jump list file processing ends
        /// </summary>
        /// <param name="fileName">Source file of the JumpList item (full path)</param>
        /// <param name="itemsCount">Number of items retrieved</param>
        private void LogJumpListProcessingEnd(string fileName, int itemsCount)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                __LogJumpListProcessingEndDefinition(logger, fileName, itemsCount, null);
            }
        }

        //----------------------------------------------
        // 1003 Got JumpList item
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogGotJumpListItem
        /// </summary>
        private static readonly Action<ILogger, string, string, string?, bool, Exception?> __LogGotJumpListItemDefinition =

            LoggerMessage.Define<string, string, string?, bool>(
                LogLevel.Debug,
                new EventId(1003, nameof(LogGotJumpListItem)),
                "Retrieved JumpList item from {source}: {name}: {executable}; has icon: {hasIcon}",
                LogOptions);

        /// <summary>
        /// Logs record (Debug) when a jump list item is retrieved
        /// </summary>
        /// <param name="source">Source file of the JumpList item</param>
        /// <param name="name">Name of JumpList item (category/title)</param>
        /// <param name="executable">Executable with optional arguments of JumpList item</param>
        /// <param name="hasIcon">Flag whether the JumpList item has an icon</param>
        private void LogGotJumpListItem(string source, string name, string? executable, bool hasIcon)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                __LogGotJumpListItemDefinition(logger, source, name, executable, hasIcon, null);
            }
        }

        //----------------------------------------------
        // 1201 Got Pinned application
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogGotPinnedApplication
        /// </summary>
        private static readonly Action<ILogger, string, Exception?> __LogGotPinnedApplicationDefinition =

            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(1201, nameof(LogGotPinnedApplication)),
                "Retrieved pinned application info: {pinnedAppInfo}",
                LogOptions);

        /// <summary>
        /// Logs record (Debug) when a pinned application information is retrieved
        /// </summary>
        /// <param name="pinnedAppInfo">Information about pinned application</param>
        private void LogGotPinnedApplication(PinnedAppInfo pinnedAppInfo)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                __LogGotPinnedApplicationDefinition(logger, pinnedAppInfo.ToString(), null);
            }
        }

        //----------------------------------------------
        // 1901 JumpList Exception
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogJumpListException
        /// </summary>
        private static readonly Action<ILogger, string, Exception?> __LogJumpListExceptionDefinition =
            LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(1901, nameof(LogJumpListException)),
                "Exception while processing the JumpList {source}",
                LogOptions);

        /// <summary>
        /// Logs record (Warning) of exception thrown when processing the JumpList
        /// </summary>
        /// <param name="source">Name of the JumpList source file</param>
        /// <param name="ex">Exception thrown</param>
        private void LogJumpListException(string source, Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                __LogJumpListExceptionDefinition(logger, source, ex);
            }
        }

        //----------------------------------------------
        // 1902 JumpList source parsing error
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogJumpListSourceParsingError
        /// </summary>
        private static readonly Action<ILogger, string, Exception?> __LogJumpListSourceParsingErrorDefinition =
            LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(1902, nameof(LogJumpListSourceParsingError)),
                "Error while parsing the JumpList source - {source}",
                LogOptions);

        /// <summary>
        /// Logs record (Warning) when there is a problem when reading the JumpList source while
        /// </summary>
        /// <param name="source">Name of the JumpList source file</param>
        private void LogJumpListSourceParsingError(string source)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                __LogJumpListSourceParsingErrorDefinition(logger, source, null);
            }
        }

        // ReSharper restore InconsistentNaming
        #endregion

        /// <summary>
        /// Application settings
        /// </summary>
        private readonly IAppSettings settings;

        /// <summary>
        /// Dictionary of known AppIds from configuration containing pairs executable-appId (the key is in lower case)
        /// When built from configuration, the record (key) is created for full path from config and another one without a path (file name only) if applicable
        /// </summary>
        private readonly Dictionary<string, string> knownAppIds;

        /// <summary>
        /// Internal CTOR
        /// Directly used by <see cref="ViewModelLocator"/> when creating a design time instance.
        /// Internally called by public "DI bound" CTOR
        /// </summary>
        /// <param name="settings">Application setting</param>
        /// <param name="logger">Logger to be used</param>
        internal JumpListService2(IAppSettings settings, ILogger logger)
        {
            this.logger = logger;
            this.settings = settings;
            knownAppIds = settings.GetKnowAppIds();
        }

        /// <summary>
        /// CTOR used by DI
        /// </summary>
        /// <param name="options">Application settings configuration</param>
        /// <param name="logger">Logger used</param>
        // ReSharper disable once UnusedMember.Global
        public JumpListService2(IOptions<AppSettings> options, ILogger<JumpListService> logger) : this(options.Value, logger)
        {
            //used from DI - DI populates the parameters and the internal CTOR is called then
        }


        /// <summary>
        /// Checks whether there is automatic and/or custom destinations file for given <paramref name="appId"/> and
        /// if yes, parses the destination lists and provides the retrieved JumpList items
        /// </summary>
        /// <param name="appId">Application ID - either explicit AppId or full path to executable</param>
        /// <param name="installedApplications">Information about installed applications</param>
        /// <returns>Array of JumpList items retrieved for given <paramref name="appId"/></returns>
        public LinkInfo[] GetJumpListItems(string appId, InstalledApplications installedApplications)
        {
            var jumplistItems = new List<LinkInfo>();
            var b = Encoding.Unicode.GetBytes(appId.ToUpper());
            var hash = AppIdCrc64.Compute(b);
            var hashStr = hash.ToString("X").ToLowerInvariant();
            var recentDir = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
            var customDestinationsFile = Path.Combine(recentDir, "CustomDestinations", $"{hashStr}.customDestinations-ms");
            var automaticDestinationsFile = Path.Combine(recentDir, "AutomaticDestinations", $"{hashStr}.automaticDestinations-ms");
            var customDestinationsFileExists = File.Exists(customDestinationsFile);
            var automaticDestinationsFileExists = File.Exists(automaticDestinationsFile);


            // ReSharper disable once InvertIf
            if (customDestinationsFileExists || automaticDestinationsFileExists)
            {
                //get the jumplist items
                var automaticDestinations = ParseAutomaticDestinations(appId, installedApplications);

                var customDestinations = JumpListCategories.Empty;
                if (customDestinationsFileExists)
                {
                    customDestinations = ParseCustomDestinations(customDestinationsFile, installedApplications);
                }

                //build the jumplist

                //pinned
                if (automaticDestinations.HasCategory(KnownJumpListCategory.Pinned, out var categoryPinned))
                {
                    jumplistItems.AddRange(categoryPinned!.Items);
                }

                //no pin, no task
                foreach (var category in customDestinations.Categories)
                {
                    // ReSharper disable once InvertIf
                    if (category.KnownCategory != KnownJumpListCategory.Pinned && category.KnownCategory != KnownJumpListCategory.Tasks)
                    {
                        if (category.KnownCategory == KnownJumpListCategory.None)
                        {
                            var items = category.Items.Where(i => jumplistItems.All(e => e.Hash != i.Hash)).ToArray();
                            jumplistItems.AddRange(items);
                        }
                        else
                        {
                            // ReSharper disable once InvertIf
                            if (automaticDestinations.HasCategory(category.KnownCategory, out var knownCategory))
                            {
                                var items = knownCategory!.Items.Where(i => jumplistItems.All(e => e.Hash != i.Hash)).ToArray();
                                jumplistItems.AddRange(items);
                            }
                        }
                    }
                }

                if (customDestinations.Categories.Length == 0 && automaticDestinations.HasCategory(KnownJumpListCategory.Recent, out var autoRecentCategory))
                {
                    //add recent from automatic destinations if exists and there are no custom destinations
                    var items = autoRecentCategory!.Items.Where(i => jumplistItems.All(e => e.Hash != i.Hash)).ToArray();
                    jumplistItems.AddRange(items);
                }

                //tasks
                if (customDestinations.HasCategory(KnownJumpListCategory.Tasks, out var categoryTasks))
                {
                    jumplistItems.AddRange(categoryTasks!.Items);
                }
            }

            return jumplistItems.ToArray();
        }

        /// <summary>
        /// Parses the automatic destinations file (.automaticDestinations-ms) and provides the JumpList items
        /// </summary>
        /// <param name="appId">Application ID - either explicit AppId or full path to executable</param>
        /// <param name="installedApplications">Information about installed applications</param>
        /// <returns>Array of JumpList items retrieved from automatic destination file</returns>
        private JumpListCategories ParseAutomaticDestinations(string appId, InstalledApplications installedApplications)
        {
            LogJumpListProcessingStart(appId);
            var categories = new List<JumpListCategory>();

            try
            {
                var cAutomaticDestinationList = new CAutomaticDestinationList();
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (cAutomaticDestinationList is not IAutomaticDestinationList2 automaticDestinationList)
                {
                    LogJumpListException(appId, new InvalidOperationException("Can't instantiate IAutomaticDestinationList2"));
                    return JumpListCategories.Empty;
                }

                var hr = automaticDestinationList.Initialize(appId, null, null);
                if (hr.IsSuccess)
                {
                    hr = automaticDestinationList.HasList(out var hasList);
                    if (hr.IsSuccess && hasList)
                    {
                        ProcessCategory(KnownJumpListCategory.Pinned, "Pinned", settings.JumpListCategoryLimit, out var pinnedLinks);
                        if (pinnedLinks != null) categories.Add(pinnedLinks);

                        ProcessCategory(KnownJumpListCategory.Recent, "Recent", settings.JumpListCategoryLimit, out var recentLinks);
                        if (recentLinks != null) categories.Add(recentLinks);

                        ProcessCategory(KnownJumpListCategory.Frequent, "Frequent", settings.JumpListCategoryLimit, out var frequentLinks);
                        if (frequentLinks != null) categories.Add(frequentLinks);
                    }
                }

                void ProcessCategory(KnownJumpListCategory categoryType, string categoryName, int categoryLimit, out JumpListCategory? category)
                {
                    var links = new List<LinkInfo>();
                    var objCollectionGuid = new Guid(Win32Consts.IID_IObjectCollection);
                    var iUnknownGuid = new Guid(Win32Consts.IID_IUnknown);
                    hr = automaticDestinationList.GetList((int)categoryType, (uint)categoryLimit, 0, ref objCollectionGuid, out var objectCollection);
                    if (hr.IsSuccess && objectCollection != null)
                    {
                        hr = objectCollection.GetCount(out var cnt);
                        if (hr.IsSuccess)
                        {
                            for (uint idx = 0; idx < cnt; idx++)
                            {
                                hr = objectCollection.GetAt(idx, ref iUnknownGuid, out var item);
                                if (hr.IsSuccess)
                                {
                                    var itemLink = item as IShellLinkW;
                                    if (itemLink == null && item is IShellItem2 shellItem)
                                    {
                                        hr = Shell32.SHGetIDListFromObject(shellItem, out var pidl);
                                        if (hr.IsSuccess)
                                        {
                                            var cshellLink = new CShellLink();
                                            // ReSharper disable once SuspiciousTypeConversion.Global
                                            itemLink = cshellLink as IShellLinkW;
                                            // ReSharper disable once UseNullPropagation
                                            if (itemLink != null)
                                            {
                                                itemLink.SetIDList(pidl);
                                            }
                                        }
                                    }
                                    if (itemLink != null)
                                    {
                                        if (!ParseLink(itemLink, appId, categoryName, links, installedApplications))
                                        {
                                            //something is wrong, we can continue here, no need to break
                                            //logged in ParseLink already
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (links.Count > 0)
                    {
                        category = new JumpListCategory(categoryName, categoryType);
                        category.Items.AddRange(links);
                    }
                    else
                    {
                        category = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogJumpListException(appId, ex);
            }

            LogJumpListProcessingEnd(appId, categories.Count);
            return categories.Count > 0 ? new JumpListCategories(categories.ToArray()) : JumpListCategories.Empty;
        }

        /// <summary>
        /// Parses the custom destinations file (.customDestinations-ms) and provides the categories and JumpList items
        /// </summary>
        /// <param name="fileName">Full path to custom destinations file</param>
        /// <param name="installedApplications">Information about installed applications</param>
        /// <returns>Array of JumpList categories and items retrieved from custom destination file</returns>
        private JumpListCategories ParseCustomDestinations(string fileName, InstalledApplications installedApplications)
        {
            LogJumpListProcessingStart(fileName);
            var categories = new List<JumpListCategory>();
            var source = Path.GetFileName(fileName); //file name without path
            try
            {
                var raw = File.ReadAllBytes(fileName); //read .customDestinations-ms content   
                using var input = new MemoryStream(raw);
                using var reader = new BinaryReader(input);
                var iStream = new NativeStreamWrapper(input);

                //header
                reader.ReadInt32(); //should be 02
                var categoryCnt = reader.ReadInt32(); //count of categories
                reader.ReadInt32(); //reserved

                for (var catIdx = 0; catIdx < categoryCnt; catIdx++)
                {
                    var categoryType = reader.ReadInt32();
                    var result = true;
                    switch (categoryType)
                    {
                        case 0: //custom category type - collection of custom destinations
                            var categoryTitle = ParseCategoryTitle(reader) ?? "Recent";
                            result = ParseCategoryLinks(reader, iStream, source, categoryTitle, out var categoryLinks, installedApplications, true);
                            if (result)
                            {
                                var category = new JumpListCategory(categoryTitle, KnownJumpListCategory.None);
                                category.Items.AddRange(categoryLinks);
                                categories.Add(category);
                            }
                            break;
                        case 1: //known category type - collection of known destinations (recent, frequent)
                            result = ParseKnownCategory(reader, out var knownCategory);
                            if (result)
                            {
                                var category = new JumpListCategory(knownCategory == KnownJumpListCategory.Frequent ? "Frequent" : "Recent", knownCategory);
                                categories.Add(category);
                            }
                            else
                            {
                                result = true; //don't break processing
                            }
                            break;
                        case 2: //custom tasks - collection of tasks
                            result = ParseCategoryLinks(reader, iStream, source, "Tasks", out var taskLinks, installedApplications, false);
                            if (result)
                            {
                                var category = new JumpListCategory("Tasks", KnownJumpListCategory.Tasks);
                                category.Items.AddRange(taskLinks);
                                categories.Add(category);
                            }
                            break;
                    }

                    if (!result)
                    {
                        //something is wrong, it doesn't make sense to continue
                        break;
                    }
                    ParseFooter(reader);
                }
            }
            catch (Exception ex)
            {
                LogJumpListException(source, ex);
            }

            LogJumpListProcessingEnd(fileName, categories.Count);
            return categories.Count > 0 ? new JumpListCategories(categories.ToArray()) : JumpListCategories.Empty;
        }

        /// <summary>
        /// Parses the category title from custom destinations file.
        /// It's number of characters + unicode chars
        /// </summary>
        /// <param name="reader">Reader used to access the custom destinations file stream</param>
        /// <returns>Category title parsed from custom destinations file</returns>
        private static string? ParseCategoryTitle(BinaryReader reader)
        {
            var charCnt = reader.ReadInt16();
            if (charCnt <= 0) return null;

            var charBuff = new byte[charCnt * 2];
            var l = reader.Read(charBuff, 0, charCnt * 2);

            var str = Encoding.Unicode.GetString(charBuff, 0, l);
            return str;

        }

        /// <summary>
        /// Parsers the JumpList items (<paramref name="links"/>) for a single <paramref name="category"/> from custom destinations file
        /// </summary>
        /// <param name="reader">Reader used to access the custom destinations file</param>
        /// <param name="iStream">Stream containing the link file(s)</param>
        /// <param name="source">Name of the source destinations file</param>
        /// <param name="category">Name of JumpList items category</param>
        /// <param name="links">Target list of JumpList items</param>
        /// <param name="installedApplications">Infomration about installed applications</param>
        /// <param name="applyLimit">Flag whether to apply <see cref="AppSettings.JumpListCategoryLimit"/></param>
        /// <returns>Returns False in case of error, otherwise true</returns>
        private bool ParseCategoryLinks(BinaryReader reader, IStream iStream, string source, string category, out List<LinkInfo> links, InstalledApplications installedApplications, bool applyLimit)
        {
            var categoryCounter = 0;
            var countItems = reader.ReadInt32();
            links = new List<LinkInfo>();
            for (var i = 0; i < Math.Min(countItems, 1000); i++) //have a hard limit big enough not to impact "normal" processing, but prevent long loops in case the format/parsing is not as expected and the count "makes no sense" (is too big)
            {
                if (!ParseLink(iStream, source, category, links, installedApplications, applyLimit && categoryCounter >= settings.JumpListCategoryLimit))
                {
                    //something is wrong, doesn't make sense to continue
                    LogJumpListSourceParsingError(source);
                    links.Clear();
                    return false;
                }
                categoryCounter++;
            }

            return true;
        }

        /// <summary>
        /// Parses single link file (JumpList item) from custom destinations file
        /// </summary>
        /// <param name="iStream">Stream containing the link file</param>
        /// <param name="source">Name of the source destinations file</param>
        /// <param name="category">Name of JumpList items category</param>
        /// <param name="links">Target list of JumpList items</param>
        /// <param name="installedApplications">Infomration about installed applications</param>
        /// <param name="categoryLimitReached">Flag whether <see cref="AppSettings.JumpListCategoryLimit"/> has been already reached</param>
        /// <returns>Returns False in case of error, otherwise true</returns>
        private bool ParseLink(IStream iStream, string source, string category, List<LinkInfo> links, InstalledApplications installedApplications, bool categoryLimitReached)
        {
            object? obj = null;
            try
            {
                //read IShellLink from Ole Stream - CLSID+serialized IShellLinkW
                var g = new Guid(Win32Consts.IID_IUnknown);
                var ret = Ole32.OleLoadFromStream(iStream, ref g, out obj);

                if (categoryLimitReached || !ret.IsSuccess || obj is not IShellLinkW link) return ret.IsSuccess; //when a category limit is reached, the links are still read from stream, but not processed

                return ParseLink(link, source, category, links, installedApplications);
            }
            catch (Exception ex)
            {
                LogJumpListException(source, ex);
                return false;
            }
            finally
            {
                if (obj != null && Marshal.IsComObject(obj))
                {
                    Marshal.ReleaseComObject(obj);
                }
            }
        }
        
        /// <summary>
        /// Parses single link (JumpList item) from automatic or custom destinations file
        /// </summary>
        /// <param name="link">Link retrieved from jumplist</param>
        /// <param name="source">Name of the source destinations file</param>
        /// <param name="category">Name of JumpList items category</param>
        /// <param name="links">Target list of JumpList items</param>
        /// <param name="installedApplications">Infomration about installed applications</param>
        /// <returns>Returns False in case of error, otherwise true</returns>
        private bool ParseLink(IShellLinkW link, string source, string category, List<LinkInfo> links, InstalledApplications installedApplications)
        {
            try
            {
                //get basic link information
                var sb = new StringBuilder(260);
                var data = new WIN32_FIND_DATAW();
                var ret = link.GetPath(sb, sb.Capacity, data, 0);
                var targetPath = ret.IsSuccess && sb.Length > 0 ? sb.ToString() : null;

                sb = new StringBuilder(1024);
                ret = link.GetArguments(sb, sb.Capacity);
                var arguments = ret.IsSuccess && sb.Length > 0 ? sb.ToString() : null;

                sb = new StringBuilder(1024);
                ret = link.GetDescription(sb, sb.Capacity);
                var description = ret.IsSuccess && sb.Length > 0 ? sb.ToString() : null;
                if (!string.IsNullOrEmpty(description))
                {
                    var newDescription = Resource.GetIndirectString(description, null);
                    if (newDescription != null) description = newDescription;
                }

                sb = new StringBuilder(1024);
                ret = link.GetWorkingDirectory(sb, sb.Capacity);
                var workingDirectory = ret.IsSuccess && sb.Length > 0 ? sb.ToString() : null;

                sb = new StringBuilder(1024);
                ret = link.GetIconLocation(sb, sb.Capacity, out var iconIndex);
                var iconLocation = ret.IsSuccess && sb.Length > 0 ? sb.ToString() : null;

                var icon =
                    !string.IsNullOrEmpty(iconLocation)
                        ? Resource.GetResourceIcon($"{iconLocation},{iconIndex}", IconSizeEnum.Small)
                        : null;

                string? title = null;
                var isStoreApp = false;

                if (!settings.JumpListUseTempFiles)
                {
                    //Try to get the information from shell properties of link
                    //ShellItem is created from link's ID list, so it represents the link target and it's used just to get icon  (if not overriden by the properties within the link)

                    // ReSharper disable once SuspiciousTypeConversion.Global
                    var linkPropertyStore = link as IPropertyStore;
                    var idListPtr = link.GetIDList();
                    var memShellLinkShellItem = Shell.Shell.CreateShellItemFromIdList(idListPtr);
                    ProcessLink(linkPropertyStore, memShellLinkShellItem);
                }


                // ReSharper disable once SuspiciousTypeConversion.Global
                if (link is IPersistFile persistFile && settings.JumpListUseTempFiles)
                {
                    //Try to get the information from shell properties of link ShellItem
                    //Save .lnk to temp file, retrieve IShellItem2 for the temp file, get the info and delete the temp file.
                    //The .lnk file is used as the icon source shell item here (if not overriden by the properties within the link)

                    var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".lnk");
                    persistFile.Save(tempFile, false);

                    var tmpFileShellItem = Shell.Shell.GetShellItemForPath(tempFile);
                    var tmpFilePropertyStore = tmpFileShellItem?.GetPropertyStore();
                    ProcessLink(tmpFilePropertyStore, tmpFileShellItem);

                    File.Delete(tempFile);
                }

                void ProcessLink(IPropertyStore? propertyStore, IShellItem2? iconSourceShellItem)
                {
                    if (iconSourceShellItem != null)
                    {
                        icon ??= Shell.Shell.GetShellItemBitmapSource(iconSourceShellItem, 32);
                    }

                    if (propertyStore == null) return;
                    
                    var propTitle = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_Title);
                    if (!string.IsNullOrEmpty(propTitle))
                    {
                        var newTitle = Resource.GetIndirectString(propTitle, null);
                        if (newTitle != null) title = newTitle;
                    }

                    //Check for Store (UWP) app and it's properties that can "override" the standard link ones
                    var appId = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_ID);
                    if (string.IsNullOrEmpty(appId))
                    {
                        var targetExecutable = Path.GetFileName(targetPath);
                        if (!string.IsNullOrEmpty(targetPath) && !string.IsNullOrEmpty(targetExecutable) && knownAppIds.TryGetValue(targetExecutable, out var knownAppId)) appId = knownAppId;
                    }

                    if (string.IsNullOrEmpty(appId)) return;

                    var packageFullName = installedApplications.GetPackageFullName(appId);
                    if (!string.IsNullOrEmpty(packageFullName))
                    {
                        isStoreApp = true;
                        targetPath = appId;
                    }

                    propTitle = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_Title); //try again, now with package info
                    if (!string.IsNullOrEmpty(propTitle))
                    {
                        var newTitle = Resource.GetIndirectString(propTitle, packageFullName);
                        if (newTitle != null) title = newTitle;
                    }

                    propTitle = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_DestListProvidedTitle);
                    if (!string.IsNullOrEmpty(propTitle))
                    {
                        var newTitle = Resource.GetIndirectString(propTitle, packageFullName);
                        if (newTitle != null) title = newTitle;
                    }

                    var propDescription = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_DestListProvidedDescription);
                    if (!string.IsNullOrEmpty(propDescription))
                    {
                        var newDescription = Resource.GetIndirectString(propDescription, packageFullName);
                        if (newDescription != null) description = newDescription;
                    }

                    var propLogo = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_DestListLogoUri);
                    if (!string.IsNullOrEmpty(propLogo) && isStoreApp)
                    {
                        if (packageFullName != null)
                        {
                            var packagePath = Package.GetPackagePath(packageFullName);
                            if (packagePath != null)
                            {
                                var imgAsset = Package.GetPackageImageAsset(packageFullName, propLogo, 32);
                                if (imgAsset != null)
                                {
                                    icon = new BitmapImage(new Uri(imgAsset));
                                }
                            }
                        }
                    }

                    var propActivationContext = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_ActivationContext);
                    if (!string.IsNullOrEmpty(propActivationContext))
                    {
                        arguments = propActivationContext;
                    }
                }


                title ??= description ?? (!string.IsNullOrEmpty(targetPath)
                    ? !string.IsNullOrEmpty(Path.GetFileNameWithoutExtension(targetPath)) ?
                        Path.GetFileNameWithoutExtension(targetPath) :
                        targetPath
                    : null);

                if (title != null)
                {
                    var linkInfo = new LinkInfo(
                        category, title, description,
                        targetPath, arguments, workingDirectory,
                        iconLocation, iconIndex, icon,
                        isStoreApp,
                        source);

                    if (links.Any(li => li.Hash == linkInfo.Hash)) return true; //stupid simple dedup
                    links.Add(linkInfo);
                    LogGotJumpListItem(source, $"{category}/{title}", $"{targetPath} {arguments}", icon != null);
                }
                else
                {
                    //is separator
                    var linkInfo = LinkInfo.Separator;
                    links.Add(linkInfo);
                }

                return true;
            }
            catch (Exception ex)
            {
                LogJumpListException(source, ex);
                return false;
            }
        }

        /// <summary>
        /// Parses the known category from custom destinations file.
        /// It's just a known category ID
        /// </summary>
        /// <param name="reader">Reader used to access the custom destinations file stream</param>
        /// <param name="knownCategory">The <see cref="KnownJumpListCategory"/> parsed from custom destinations file</param>
        /// <returns>Always true</returns>
        private static bool ParseKnownCategory(BinaryReader reader, out KnownJumpListCategory knownCategory)
        {
            // ReSharper disable once UnusedVariable
            var knownCategoryId = reader.ReadInt32();

            switch (knownCategoryId)
            {
                case 1:
                    knownCategory = KnownJumpListCategory.Frequent;
                    return true;
                case 2:
                    knownCategory = KnownJumpListCategory.Recent;
                    return true;
                default:
                    knownCategory = KnownJumpListCategory.None;
                    return false;
            }
        }

        /// <summary>
        /// Parses the category footer from customs destinations file
        /// Just reads and forgets the data to move forward within the stream
        /// </summary>
        /// <param name="reader">Reader used to access the custom destinations file</param>
        private static void ParseFooter(BinaryReader reader)
        {
            reader.ReadUInt32(); //should be AB FB BF BA
        }

        /// <summary>
        /// Known JumpList Category 
        /// </summary>
        private enum KnownJumpListCategory
        {
            /// <summary>
            /// Not a known category (custom category)
            /// </summary>
            None = -1,
            /// <summary>
            /// Pinned items
            /// </summary>
            Pinned = 0,
            /// <summary>
            /// Frequently used items
            /// </summary>
            Frequent = 1,
            /// <summary>
            /// Recently used items
            /// </summary>
            Recent = 2,
            /// <summary>
            /// Task items
            /// </summary>
            Tasks = 3,
        }

        /// <summary>
        /// Container for jumplist category items
        /// </summary>
        private class JumpListCategory
        {
            /// <summary>
            /// Name of the category
            /// </summary>
            private string Name { get; }
            /// <summary>
            /// Identifier of known JumpList category or <see cref="KnownJumpListCategory.None"/> for custom category
            /// </summary>
            public KnownJumpListCategory KnownCategory { get; }
            /// <summary>
            /// Items within the category
            /// </summary>
            public List<LinkInfo> Items { get; }

            /// <summary>
            /// CTOR
            /// </summary>
            /// <param name="name">Name of the category</param>
            /// <param name="knownCategory">Identifier of known JumpList category or <see cref="KnownJumpListCategory.None"/> for custom category</param>
            public JumpListCategory(string name, KnownJumpListCategory knownCategory)
            {
                Name = name;
                KnownCategory = knownCategory;
                Items = new List<LinkInfo>();
            }

            /// <summary>
            /// Returns string representation of the object
            /// </summary>
            /// <returns>String representation of the object</returns>
            public override string ToString()
            {
                return $"{Name} ({KnownCategory}) - {Items.Count} items";
            }
        }

        /// <summary>
        /// Container for jumplist categories retrieved from destination list
        /// </summary>
        private class JumpListCategories
        {
            /// <summary>
            /// Empty list of categories (no categories)
            /// </summary>
            public static JumpListCategories Empty { get; } = new JumpListCategories(Array.Empty<JumpListCategory>());
            /// <summary>
            /// Set of categories in the destination list
            /// </summary>
            public JumpListCategory[] Categories { get; }
            /// <summary>
            /// CTOR
            /// </summary>
            /// <param name="categories">Set of categories in the destination list</param>
            public JumpListCategories(JumpListCategory[] categories)
            {
                Categories = categories;
            }

            /// <summary>
            /// Check whether the destination list has a <see cref="KnownJumpListCategory"/> containing at least one item
            /// </summary>
            /// <param name="knownCategory">Identifier of known JumpList category</param>
            /// <param name="category">Contains a category container when the destination list contains given <paramref name="knownCategory"/> otherwise null</param>
            /// <returns>True when the destination list contains given <paramref name="knownCategory"/> otherwise false</returns>
            public bool HasCategory(KnownJumpListCategory knownCategory, out JumpListCategory? category)
            {
                category = Categories.FirstOrDefault(c => c.KnownCategory == knownCategory && c.Items.Count > 0);
                return category != null;
            }
        }

        /// <summary>
        /// Gets the information about the applications pinned to the taskbar
        /// </summary>
        /// <param name="knownFolders">Information about the known folder paths and GUIDs</param>
        /// <returns>Array of information about the applications pinned to the taskbar</returns>
        public PinnedAppInfo[] GetPinnedApplications(StringGuidPair[] knownFolders)
        {
            if (!settings.ShowPinnedApps) return Array.Empty<PinnedAppInfo>();

            var titlePropertyKey = new PropertyKey("9e5e05ac-1936-4a75-94f7-4704b8b01923", 0);

            var appInfos = new List<PinnedAppInfo>();
            var objType = Type.GetTypeFromCLSID(new Guid(Win32Consts.CLSID_TaskbanPin), false);
            if (objType == null) return Array.Empty<PinnedAppInfo>();

            var obj = Activator.CreateInstance(objType);
            if (obj is not IPinnedList3 pinnedList) return Array.Empty<PinnedAppInfo>();

            var hrs = pinnedList.EnumObjects(out var iel);
            if (!hrs.IsSuccess) return Array.Empty<PinnedAppInfo>();

            hrs = iel.Reset();
            if (!hrs.IsSuccess) return Array.Empty<PinnedAppInfo>();

            var order = 0;
            var iShellItem2Guid = new Guid(Win32Consts.IID_IShellItem2);
            do
            {
                hrs = iel.Next(1, out var pidl, out _);
                if (!hrs.IsS_OK) break; //S_FALSE is returned for end of enum, but it's also "success code", so explicit check needed here

                hrs = Shell32.SHCreateItemFromIDList(pidl, iShellItem2Guid, out var shellItem);
                if (!hrs.IsS_OK || shellItem == null) break;

                var shellProperties = shellItem.GetProperties();
                var type = shellProperties.IsStoreApp
                    ? PinnedAppInfo.PinnedAppTypeEnum.Package
                    : PinnedAppInfo.PinnedAppTypeEnum.Link;

                var title =
                    shellItem.GetPropertyValue<string>(titlePropertyKey) ??
                    shellItem.GetPropertyValue<string>(PropertyKey.PKEY_ItemNameDisplay) ??
                    "unknown";

                var appId = shellProperties.ApplicationUserModelId;
                var executable = GetExecutableFromLinkProps(shellProperties);
                if (appId == null && executable != null)
                {
                    //appId can be an executable full path, ensure that known folders are transformed to their GUIDs
                    appId = Shell.Shell.ReplaceKnownFolderWithGuid(executable);
                }

                var appInfo = new PinnedAppInfo(title, order, type, shellProperties, appId, executable);
                appInfos.Add(appInfo);
                order++;
                Marshal.FreeCoTaskMem(pidl);
                LogGotPinnedApplication(appInfo);
            } while (hrs.IsS_OK);

            return appInfos.ToArray();
        }

        /// <summary>
        /// Returns the executable path from link properties
        /// </summary>
        /// <param name="shellProperties">Link properties to check</param>
        /// <returns>Executable path if extracted from link properties</returns>
        private static string? GetExecutableFromLinkProps(ShellPropertiesSubset shellProperties)
        {
            if (shellProperties.IsStoreApp) return shellProperties.ParsingPath;

            var executable = shellProperties.LinkTargetParsingPath;
            if (executable != null)
            {
                if (File.Exists(executable) && Path.GetExtension(executable).ToLowerInvariant() == ".exe") return executable;

                //sometimes the LinkTargetParsingPath doesn't contain executable. Probably some issue with app or task bar. Try to get executable from link then

                // ReSharper disable SuspiciousTypeConversion.Global
                if (shellProperties.ParsingPath != null && File.Exists(shellProperties.ParsingPath) &&
                    new CShellLink() is (IShellLinkW link and IPersistFile linkPersistFile))
                // ReSharper restore SuspiciousTypeConversion.Global
                {
                    linkPersistFile.Load(shellProperties.ParsingPath, 0);

                    //get basic link information
                    var sb = new StringBuilder(260);
                    var data = new WIN32_FIND_DATAW();
                    var ret = link.GetPath(sb, sb.Capacity, data, 0);
                    executable = ret.IsSuccess && sb.Length > 0 ? sb.ToString() : null;
                }
            }

            return executable ?? shellProperties.ParsingPath;
        }
    }
}
