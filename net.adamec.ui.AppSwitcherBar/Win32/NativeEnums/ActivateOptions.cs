
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
using System;


namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums
{
    /// <summary>
    /// One or more of the following flags used to support design mode, debugging, and testing scenarios. 
    /// </summary>
    [Flags]
    internal enum ActivateOptions
    {
        /// <summary>
        /// No flags set
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// The app is being activated for design mode, so it can't create its normal window.
        /// The creation of the app's window must be done by design tools that load the necessary components by communicating
        /// with a designer-specified service on the site chain established through the activation manager.
        /// Note that this means that the splash screen seen during regular activations won't be seen.
        /// Note that you must enable debug mode on the app's package to successfully use design mode.
        /// </summary>
        DesignMode = 0x00000001,
        /// <summary>
        /// Do not show an error dialog if the app fails to activate.                                
        /// </summary>
        NoErrorUI = 0x00000002, 
        /// <summary>
        /// Do not display the app's splash screen when the app is activated.
        /// You must enable debug mode on the app's package when you use this flag; otherwise, the PLM will terminate the app after a few seconds.
        /// </summary>
        NoSplashScreen = 0x00000004,
        /// <summary>
        /// The application is being activated in prelaunch mode. This value is supported starting in Windows 10.
        /// </summary>
        PreLaunch = 0x2000000
    }
}
