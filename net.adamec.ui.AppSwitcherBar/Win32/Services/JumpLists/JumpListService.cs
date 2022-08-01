using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.ViewModel;
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
    /// </summary>
    public class JumpListService : IJumpListService
    {
        #region Logging
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// Logger used
        /// </summary>
        private readonly ILogger logger;
        // 1xxx - JumpList Service (19xx Errors/Exceptions)
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
        // 1901 JumpList Exception
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogJumpListException
        /// </summary>
        private static readonly Action<ILogger, string, Exception?> __LogJumpListExceptionDefinition =
            LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(1901, nameof(LogJumpListException)),
                "Error while processing the JumpList {source}",
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

        // ReSharper restore InconsistentNaming
        #endregion

        /// <summary>
        /// Application settings
        /// </summary>
        private readonly IAppSettings settings;

        /// <summary>
        /// Internal CTOR
        /// Directly used by <see cref="ViewModelLocator"/> when creating a design time instance.
        /// Internally called by public "DI bound" CTOR
        /// </summary>
        /// <param name="settings">Application setting</param>
        /// <param name="logger">Logger to be used</param>
        internal JumpListService(IAppSettings settings, ILogger logger)
        {
            this.logger = logger;
            this.settings = settings;
        }

        /// <summary>
        /// CTOR used by DI
        /// </summary>
        /// <param name="options">Application settings configuration</param>
        /// <param name="logger">Logger used</param>
        // ReSharper disable once UnusedMember.Global
        public JumpListService(IOptions<AppSettings> options, ILogger<JumpListService> logger) : this(options.Value, logger)
        {
            //used from DI - DI populates the parameters and the internal CTOR is called then
        }


        /// <summary>
        /// Checks whether there is automatic and/or custom destinations file for given <paramref name="appId"/> and
        /// if yes, parses the file(s) and provides the retrieved JumpList items
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
            var customDestinationsFile =
                Path.Combine(recentDir, "CustomDestinations", $"{hashStr}.customDestinations-ms");
            var automaticDestinationsFile = Path.Combine(recentDir, "AutomaticDestinations",
                $"{hashStr}.automaticDestinations-ms");
            var customDestinationsFileExists = File.Exists(customDestinationsFile);
            var automaticDestinationsFileExists = File.Exists(automaticDestinationsFile);


            // ReSharper disable once InvertIf
            if (customDestinationsFileExists || automaticDestinationsFileExists)
            {
                //get the jumplist items

                if (automaticDestinationsFileExists)
                {
                    var automaticDestinations =
                        ParseAutomaticDestinations(automaticDestinationsFile, installedApplications);
                    if (automaticDestinations != null) jumplistItems.AddRange(automaticDestinations);
                }

                // ReSharper disable once InvertIf
                if (customDestinationsFileExists)
                {
                    var customDestinations =
                        ParseCustomDestinations(customDestinationsFile, installedApplications);
                    if (customDestinations != null) jumplistItems.AddRange(customDestinations);
                }
            }

            return jumplistItems.ToArray();
        }

        /// <summary>
        /// Parses the automatic destinations file (.automaticDestinations-ms) and provides the JumpList items
        /// </summary>
        /// <remarks>When "JumpList" feature flag is not set, it skips the processing and an empty array is returned</remarks>
        /// <param name="fileName">Full path to automatic destinations file</param>
        /// <param name="installedApplications">Information about installed applications</param>
        /// <returns>Array of JumpList items retrieved from automatic destination file</returns>
        private LinkInfo[]? ParseAutomaticDestinations(string fileName, InstalledApplications installedApplications)
        {
            LogJumpListProcessingStart(fileName);
            var links = new List<LinkInfo>();
            var source = Path.GetFileName(fileName); //file name without path

            StorageInfo? storageRoot = null;
            try
            {
                storageRoot = GetStorageRoot(fileName); //open the file as Compound Storare
                if (storageRoot != null)
                {
                    var clsId = new Guid("00021401-0000-0000-C000-000000000046"); //Shortcut/link
                    var clsBytes = clsId.ToByteArray();
                    const string category = "Recent";
                    var categoryCounter = 0;

                    foreach (var streamInfo in storageRoot.GetStreams())
                    {
                        if (streamInfo.Name == "DestList") continue; //ignore DestList - it can be usefull to check whether the item is pinned sometimes in future

                        //other streams are the link files corresponding to individual items of the JumpList
                        using var tmpStream = streamInfo.GetStream(FileMode.Open, FileAccess.Read);
                        var raw = ReadAllBytesFromStream(tmpStream);

                        //add CLSID so it can be parsed using OleLoadFromStream in ParseLink method
                        var oleStreamData = new byte[clsBytes.Length + raw.Length];
                        Buffer.BlockCopy(clsBytes, 0, oleStreamData, 0, clsBytes.Length);
                        Buffer.BlockCopy(raw, 0, oleStreamData, clsBytes.Length, raw.Length);

                        //parse link from stream
                        using var input = new MemoryStream(oleStreamData);
                        var iStream = new NativeStreamWrapper(input);
                        ParseLink(iStream, source, category, links, installedApplications, categoryCounter >= settings.JumpListCategoryLimit);
                        categoryCounter++;
                    }
                }
            }
            catch (Exception ex)
            {
                LogJumpListException(source, ex);
            }
            finally
            {
                if (storageRoot != null)
                {
                    CloseStorageRoot(storageRoot);
                }
            }

            LogJumpListProcessingEnd(fileName, links.Count);
            return links.Count > 0 ? links.ToArray() : null;
        }

        /// <summary>
        /// Parses the custom destinations file (.customDestinations-ms) and provides the JumpList items
        /// </summary>
        /// <remarks>When "JumpList" feature flag is not set, it skips the processing and an empty array is returned</remarks>
        /// <param name="fileName">Full path to custom destinations file</param>
        /// <param name="installedApplications">Information about installed applications</param>
        /// <returns>Array of JumpList items retrieved from automatic destination file</returns>
        private LinkInfo[]? ParseCustomDestinations(string fileName, InstalledApplications installedApplications)
        {
            LogJumpListProcessingStart(fileName);
            var links = new List<LinkInfo>();
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
                    switch (categoryType)
                    {
                        case 0: //custom category type - collection of custom destinations
                            var categoryTitle = ParseCategoryTitle(reader) ?? "Recent";
                            ParseCategoryLinks(reader, iStream, source, categoryTitle, links, installedApplications, true);
                            break;
                        case 1: //known category type - collection of known destinations (recent, frequent)
                            ParseCategoryLinks(reader, iStream, source, "Recent", links, installedApplications, true);
                            break;
                        case 2: //custom tasks - collection of tasks
                            ParseCategoryLinks(reader, iStream, source, "Tasks", links, installedApplications, false);
                            break;
                    }
                    ParseFooter(reader);
                }
            }
            catch (Exception ex)
            {
                LogJumpListException(source, ex);
            }

            LogJumpListProcessingEnd(fileName, links.Count);
            return links.Count > 0 ? links.ToArray() : null;
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
        private void ParseCategoryLinks(BinaryReader reader, IStream iStream, string source, string category, List<LinkInfo> links, InstalledApplications installedApplications, bool applyLimit)
        {
            var categoryCounter = 0;
            var countItems = reader.ReadInt32();
            for (var i = 0; i < countItems; i++)
            {
                ParseLink(iStream, source, category, links, installedApplications, applyLimit && categoryCounter >= settings.JumpListCategoryLimit);
                categoryCounter++;
            }
        }

        /// <summary>
        /// Parses single link file (JumpList item) from automatic or custom destinations file
        /// </summary>
        /// <param name="iStream">Stream containing the link file</param>
        /// <param name="source">Name of the source destinations file</param>
        /// <param name="category">Name of JumpList items category</param>
        /// <param name="links">Target list of JumpList items</param>
        /// <param name="installedApplications">Infomration about installed applications</param>
        /// <param name="categoryLimitReached">Flag whether <see cref="AppSettings.JumpListCategoryLimit"/> has been already reached</param>
        private void ParseLink(IStream iStream, string source, string category, List<LinkInfo> links, InstalledApplications installedApplications, bool categoryLimitReached)
        {
            var g = new Guid(Win32Consts.IID_IUnknown);
            object? obj = null;
            try
            {
                //read IShellLink from Ole Stream - CLSID+serialized IShellLinkW
                var ret = Ole32.OleLoadFromStream(iStream, ref g, out obj);
                if (categoryLimitReached || !ret.IsSuccess || obj is not IShellLinkW link) return; //when a category limit is reached, the links are still read from stream, but not processed


                //get basig link information
                var sb = new StringBuilder(260);
                var data = new WIN32_FIND_DATAW();
                ret = link.GetPath(sb, sb.Capacity, data, 0);
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

                if (obj is IPersistFile persistFile)
                {
                    //try to get information from shell properties.

                    //It seems, that the easiest way is to save .lnk to temp file, retrieve IShellItem2 for the temp file, get the info and delete the temp file.
                    //In-memory operations often failed at Win API level, probably due to wrong memory handling in my code

                    var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".lnk");
                    persistFile.Save(tempFile, false);

                    var shellItem = Shell.Shell.GetShellItemForPath(tempFile);
                    if (shellItem != null)
                    {
                        var propTitle = shellItem.GetPropertyValue<string>(PropertyKey.PKEY_Title);
                        if (!string.IsNullOrEmpty(propTitle))
                        {
                            var newTitle = Resource.GetIndirectString(propTitle, null);
                            if (newTitle != null) title = newTitle;
                        }

                        icon ??= Shell.Shell.GetShellItemBitmapSource(shellItem, 32);

                        //Check for Store (UWP) app and it's properties that can "override" the standard link ones
                        var appId = shellItem.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_ID);
                        if (!string.IsNullOrEmpty(appId))
                        {
                            isStoreApp = true;
                            targetPath = appId;
                            var packageFullName = installedApplications.GetPackageFullName(appId);


                            propTitle = shellItem.GetPropertyValue<string>(PropertyKey
                                .PKEY_Title); //try again, now with package info
                            if (!string.IsNullOrEmpty(propTitle))
                            {
                                var newTitle = Resource.GetIndirectString(propTitle, packageFullName);
                                if (newTitle != null) title = newTitle;
                            }

                            propTitle = shellItem.GetPropertyValue<string>(PropertyKey
                                .PKEY_AppUserModel_DestListProvidedTitle);
                            if (!string.IsNullOrEmpty(propTitle))
                            {
                                var newTitle = Resource.GetIndirectString(propTitle, packageFullName);
                                if (newTitle != null) title = newTitle;
                            }

                            var propDescription =
                                shellItem.GetPropertyValue<string>(PropertyKey
                                    .PKEY_AppUserModel_DestListProvidedDescription);
                            if (!string.IsNullOrEmpty(propDescription))
                            {
                                var newDescription = Resource.GetIndirectString(propDescription, packageFullName);
                                if (newDescription != null) description = newDescription;
                            }

                            var propLogo =
                                shellItem.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_DestListLogoUri);
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

                            var propActivationContext =
                                shellItem.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_ActivationContext);
                            if (!string.IsNullOrEmpty(propActivationContext))
                            {
                                arguments = propActivationContext;
                            }
                        }
                    }

                    File.Delete(tempFile);
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
                    links.Add(linkInfo);

                    LogGotJumpListItem(source, $"{category}/{title}", $"{targetPath} {arguments}", icon != null);

                }
                else
                {
                    //is separator
                    var linkInfo = LinkInfo.Separator;
                    links.Add(linkInfo);
                }
            }
            catch (Exception ex)
            {
                LogJumpListException(source, ex);
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
        /// Parses the category footer from customs destinations file
        /// Just reads and forgets the data to move forward within the stream
        /// </summary>
        /// <param name="reader">Reader used to access the custom destinations file</param>
        private static void ParseFooter(BinaryReader reader)
        {
            reader.ReadUInt32(); //should be AB FB BF BA
        }


        /// <summary>
        /// Opens the <paramref name="fileName"/> as Compound Storage and retuns the Storage Root
        /// providing the access to streams within the compound storage
        /// </summary>
        /// <param name="fileName">Full path to the file to open</param>
        /// <returns>Storage Root of Compound Storage persisted in <paramref name="fileName"/> </returns>
        public static StorageInfo? GetStorageRoot(string fileName)
        {
            //HACK: non public method, so use the custom invoker
            var storageRoot = (StorageInfo?)InvokeStorageRootMethod(null, "Open", fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return storageRoot;
        }

        /// <summary>
        /// Closes the Compound Storage file
        /// </summary>
        /// <param name="storageRoot">Storage Root of the Compound Storage to be closed</param>
        public static void CloseStorageRoot(StorageInfo storageRoot)
        {
            //HACK: non public method, so use the custom invoker
            InvokeStorageRootMethod(storageRoot, "Close");
        }

        /// <summary>
        /// Invokes the non public method of MS internal System.IO.Packaging.StorageRoot
        /// </summary>
        /// <param name="storageRoot">Instance to invoke method of or null for static methods</param>
        /// <param name="methodName">Name of the method</param>
        /// <param name="methodArgs">Array of method arguments</param>
        /// <returns>The method return value</returns>
        private static object? InvokeStorageRootMethod(StorageInfo? storageRoot, string methodName, params object[] methodArgs)
        {
            //https://www.pinvoke.net/default.aspx/ole32.stgopenstorage
            //We need the StorageRoot class to directly open an OSS file.  Unfortunately, it's internal.
            //So we'll have to use Reflection to access it.  This code was inspired by:
            //http://henbo.spaces.live.com/blog/cns!2E073207A544E12!200.entry
            //Note: In early WinFX CTPs the StorageRoot class was public because it was documented
            //here: http://msdn2.microsoft.com/en-us/library/aa480157.aspx

            var storageRootType = typeof(StorageInfo).Assembly.GetType("System.IO.Packaging.StorageRoot", true, false);

            var result = storageRootType?.InvokeMember(
                methodName,
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
                null,
                storageRoot,
                methodArgs);
            return result;
        }

        /// <summary>
        /// Read all bytes from given <param name="stream"></param>
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <returns>Content of <paramref name="stream"/> as byte array</returns>
        private static byte[] ReadAllBytesFromStream(Stream stream)
        {
            if (stream is MemoryStream ms) return ms.ToArray();

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
