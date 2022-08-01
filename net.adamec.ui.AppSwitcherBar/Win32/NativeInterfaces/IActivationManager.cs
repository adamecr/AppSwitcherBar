using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces
{
    /// <summary>
    /// Provides methods which activate Windows Store apps for the Launch, File, and Protocol extensions. You will normally use this interface in debuggers and design tools.
    /// </summary>
    [ComImport, Guid(Win32Consts.IID_IApplicationActivationManager), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IApplicationActivationManager
    {
        /// <summary>
        /// Activates the specified Windows Store app for the generic launch contract (Windows.Launch) in the current session.
        /// </summary>
        /// <param name="appUserModelId">The application user model ID of the Windows Store app.</param>
        /// <param name="arguments">A pointer to an optional, app-specific, argument string.</param>
        /// <param name="options">One or more of the flags used to support design mode, debugging, and testing scenarios.</param>
        /// <param name="processId">A pointer to a value that, when this method returns successfully, receives the process ID of the app instance that fulfills this contract.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT ActivateApplication([In] string appUserModelId, [In] string? arguments, [In] ActivateOptions options, [Out] out uint processId);

        /// <summary>
        /// Activates the specified Windows Store app for the file contract (Windows.File).
        /// </summary>
        /// <param name="appUserModelId">The application user model ID of the Windows Store app.</param>
        /// <param name="itemArray">A pointer to an array of Shell items, each representing a file. This value is converted to a VectorView of StorageItem objects that is passed to the app through FileActivatedEventArgs.</param>
        /// <param name="verb">The verb being applied to the file or files specified by itemArray.</param>
        /// <param name="processId">A pointer to a value that, when this method returns successfully, receives the process ID of the app instance that fulfills this contract.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT ActivateForFile([In] string appUserModelId, [In] IntPtr /*IShellItemArray* */ itemArray, [In] string verb, [Out] out uint processId);

        /// <summary>
        /// Activates the specified Windows Store app for the protocol contract (Windows.Protocol).
        /// </summary>
        /// <param name="appUserModelId">The application user model ID of the Windows Store app.</param>
        /// <param name="itemArray">A pointer to an array of a single Shell item. The first item in the array is converted into a Uri object that is passed to the app through ProtocolActivatedEventArgs. Any items in the array except for the first element are ignored.</param>
        /// <param name="processId">A pointer to a value that, when this method returns successfully, receives the process ID of the app instance that fulfills this contract.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT ActivateForProtocol([In] string appUserModelId, [In] IntPtr /* IShellItemArray* */itemArray, [Out] out uint processId);
    }

    /// <summary>
    /// Provides methods which activate Windows Store apps for the Launch, File, and Protocol extensions.
    /// </summary>
    [ComImport, Guid("45BA127D-10A8-46EA-8AB7-56EA9078943C")]
    internal class ApplicationActivationManager : IApplicationActivationManager
    {
        /// <summary>
        /// Activates the specified Windows Store app for the generic launch contract (Windows.Launch) in the current session.
        /// </summary>
        /// <param name="appUserModelId">The application user model ID of the Windows Store app.</param>
        /// <param name="arguments">A pointer to an optional, app-specific, argument string.</param>
        /// <param name="options">One or more of the flags used to support design mode, debugging, and testing scenarios.</param>
        /// <param name="processId">A pointer to a value that, when this method returns successfully, receives the process ID of the app instance that fulfills this contract.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public extern HRESULT ActivateApplication([In] string appUserModelId, [In] string? arguments, [In] ActivateOptions options, [Out] out uint processId);

        /// <summary>
        /// Activates the specified Windows Store app for the file contract (Windows.File).
        /// </summary>
        /// <param name="appUserModelId">The application user model ID of the Windows Store app.</param>
        /// <param name="itemArray">A pointer to an array of Shell items, each representing a file. This value is converted to a VectorView of StorageItem objects that is passed to the app through FileActivatedEventArgs.</param>
        /// <param name="verb">The verb being applied to the file or files specified by itemArray.</param>
        /// <param name="processId">A pointer to a value that, when this method returns successfully, receives the process ID of the app instance that fulfills this contract.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public extern HRESULT ActivateForFile([In] string appUserModelId, [In] IntPtr itemArray, [In] string verb, [Out] out uint processId);

        /// <summary>
        /// Activates the specified Windows Store app for the protocol contract (Windows.Protocol).
        /// </summary>
        /// <param name="appUserModelId">The application user model ID of the Windows Store app.</param>
        /// <param name="itemArray">A pointer to an array of a single Shell item. The first item in the array is converted into a Uri object that is passed to the app through ProtocolActivatedEventArgs. Any items in the array except for the first element are ignored.</param>
        /// <param name="processId">A pointer to a value that, when this method returns successfully, receives the process ID of the app instance that fulfills this contract.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public extern HRESULT ActivateForProtocol([In] string appUserModelId, [In] IntPtr itemArray, [Out] out uint processId);
    }
}
