using System;
using System.Windows;
using Microsoft.Extensions.Logging;
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.ViewModel
{
    /// <summary>
    /// The ViewModel for <see cref="MainWindow"/>.
    /// Encapsulates the data and logic related to "task bar applications/windows"
    ///  - pulling the list of them, switching the apps, presenting the thumbnails
    /// </summary>
    public partial class MainViewModel
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
        // 101 ShowThumbnail
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for ShowThumbnail
        /// </summary>
        private static readonly Action<ILogger, IntPtr, IntPtr, Rect, IntPtr, Exception?> __LogShowThumbnailDefinition =
            LoggerMessage.Define<IntPtr, IntPtr, Rect, IntPtr>(
                LogLevel.Debug,
                new EventId(101, nameof(LogShowThumbnail)),
                "Show thumbnail of hwnd:{sourceHwnd} in hwnd:{targetHwnd} at {targetRegion:#####} - handle is #{thumbnailHandle}",
                LogOptions);

        /// <summary>
        /// Logs record (Debug) when a window thumbnail is shown in popup
        /// </summary>
        /// <param name="sourceHwnd">HWND of the source window to present within the popup as a thumbnail</param>
        /// <param name="targetHwnd">HWND of the popup (thumbnail target)</param>
        /// <param name="targetRegion"><see cref="Rect"/> region of popup where to render the thumbnail to</param>
        /// <param name="thumbnailHandleIntPtr">Handle of the thumbnail returned from DWM api</param>

        private void LogShowThumbnail(IntPtr sourceHwnd, IntPtr targetHwnd, Rect targetRegion, IntPtr thumbnailHandleIntPtr)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                __LogShowThumbnailDefinition(logger, sourceHwnd, targetHwnd, targetRegion, thumbnailHandleIntPtr, null);
            }
        }

        //----------------------------------------------
        // 102 LogHideThumbnail
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogHideThumbnail
        /// </summary>
        private static readonly Action<ILogger, IntPtr, Exception?> __LogHideThumbnailDefinition =
            LoggerMessage.Define<IntPtr>(
                LogLevel.Debug,
                new EventId(102, nameof(LogHideThumbnail)),
                "Hide (unregister) thumbnail #{thumbnailHandle}",
                LogOptions);


        /// <summary>
        /// Logs record (Debug) when a window thumbnail is unregistered (hidden)
        /// </summary>
        /// <param name="thumbnailHandleIntPtr">Handle of the thumbnail</param>

        private void LogHideThumbnail(IntPtr thumbnailHandleIntPtr)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                __LogHideThumbnailDefinition(logger, thumbnailHandleIntPtr, null);

            }
        }

        

        //----------------------------------------------
        // 104 LogEnumeratedWindowInfo
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogEnumeratedWindowInfo
        /// </summary>
        private static readonly Action<ILogger, string, Exception?> __LogEnumeratedWindowInfoDefinition =
            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(104, nameof(LogEnumeratedWindowInfo)),
                "Enumerated window {windowInfo}",
                LogOptions);

        /// <summary>
        /// Logs record (Debug) about retrieved information about the window
        /// </summary>
        /// <param name="windowInfo">Information about window</param>
        private void LogEnumeratedWindowInfo(string windowInfo)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                __LogEnumeratedWindowInfoDefinition(logger, windowInfo, null);

            }
        }

        //----------------------------------------------
        // 201 LogSwitchApp
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogSwitchApp
        /// </summary>
        private static readonly Action<ILogger, IntPtr, string, bool, Exception?> __LogSwitchAppDefinition =
            LoggerMessage.Define<IntPtr, string, bool>(
                LogLevel.Information,
                new EventId(201, nameof(LogSwitchApp)),
                "Switched to app {title} (hwnd:{appHwnd}). Was minimized before: {isMinimized}",
                LogOptions);

        /// <summary>
        /// Logs record (Information) after switching the foreground app window based on the user interaction (click to app button) 
        /// </summary>
        /// <param name="appHwnd">HWND of the application window switched to foreground</param>
        /// <param name="title">Title of the application window switched to foreground</param>
        /// <param name="isMinimized">Information whether the application was minimized before the switch</param>
        private void LogSwitchApp(IntPtr appHwnd, string title, bool isMinimized)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                __LogSwitchAppDefinition(logger, appHwnd, title, isMinimized, null);

            }
        }

        //----------------------------------------------
        // 202 LogMinimizeApp
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogMinimizeApp
        /// </summary>
        private static readonly Action<ILogger, IntPtr, string, Exception?> __LogMinimizeAppDefinition =
            LoggerMessage.Define<IntPtr, string>(
                LogLevel.Information,
                new EventId(202, nameof(LogMinimizeApp)),
                "App {title} (hwnd:{appHwnd}) was minimized",
                LogOptions);

        /// <summary>
        /// Logs record (Information) after minimizing the app window based on the user interaction (click to app button) 
        /// </summary>
        /// <param name="appHwnd">HWND of the minimized application window</param>
        /// <param name="title">Title of the minimized application window</param>
        private void LogMinimizeApp(IntPtr appHwnd, string title)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                __LogMinimizeAppDefinition(logger, appHwnd, title, null);

            }
        }

       

        //----------------------------------------------
        // 900 LogWrongCommandParameter
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogWrongCommandParameter
        /// </summary>
        private static readonly Action<ILogger, string, Exception?> __LogWrongCommandParameterDefinition =
            LoggerMessage.Define<string>(
                LogLevel.Critical,
                new EventId(900, nameof(LogWrongCommandParameter)),
                "Command parameter must be `{commandParameterTypeName}`",
                LogOptions);

        /// <summary>
        /// Logs record (Critical) about the wrong parameter of <see cref="ShowThumbnailCommand"/> (null or wrong type)
        /// </summary>
        /// <param name="commandParameterTypeName">Name of expected <see cref="Type"/> of the command parameter value</param>
        private void LogWrongCommandParameter(string commandParameterTypeName)
        {
            if (logger.IsEnabled(LogLevel.Critical))
            {
                __LogWrongCommandParameterDefinition(logger, commandParameterTypeName, null);

            }
        }

        //----------------------------------------------
        // 901 CantStartApp
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogCantStartApp
        /// </summary>
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

    }
}
