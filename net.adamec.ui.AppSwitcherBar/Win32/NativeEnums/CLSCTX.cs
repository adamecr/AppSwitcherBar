using System;
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

/// <summary>
/// Values that are used in activation calls to indicate the execution contexts in which an object is to be run. 
/// </summary>
[Flags]
internal enum CLSCTX : uint
{
    /// <summary>
    /// The code that creates and manages objects of this class is a DLL that runs in the same process as the caller of the function specifying the class context.
    /// </summary>
    INPROC_SERVER = 0x1,
    /// <summary>
    /// The code that manages objects of this class is an in-process handler. This is a DLL that runs in the client process and implements client-side structures of this class when instances of the class are accessed remotely.
    /// </summary>
    INPROC_HANDLER = 0x2,
    /// <summary>
    /// The EXE code that creates and manages objects of this class runs on same machine but is loaded in a separate process space.
    /// </summary>
    LOCAL_SERVER = 0x4,
    /// <summary>
    /// Obsolete.
    /// </summary>
    INPROC_SERVER16 = 0x8,
    /// <summary>
    /// A remote context. The LocalServer32 or LocalService code that creates and manages objects of this class is run on a different computer.
    /// </summary>
    REMOTE_SERVER = 0x10,
    /// <summary>
    /// Obsolete.
    /// </summary>
    INPROC_HANDLER16 = 0x20,
    /// <summary>
    /// Reserved.
    /// </summary>
    RESERVED1 = 0x40,
    /// <summary>
    /// Reserved.
    /// </summary>
    RESERVED2 = 0x80,
    /// <summary>
    /// Reserved.
    /// </summary>
    RESERVED3 = 0x100,
    /// <summary>
    /// Reserved.
    /// </summary>
    RESERVED4 = 0x200,
    /// <summary>
    /// Disables the downloading of code from the directory service or the Internet. This flag cannot be set at the same time as CLSCTX_ENABLE_CODE_DOWNLOAD.
    /// </summary>
    NO_CODE_DOWNLOAD = 0x400,
    /// <summary>
    /// Reserved.
    /// </summary>
    RESERVED5 = 0x800,
    /// <summary>
    /// Specify if you want the activation to fail if it uses custom marshalling.
    /// </summary>
    NO_CUSTOM_MARSHAL = 0x1000,
    /// <summary>
    /// Enables the downloading of code from the directory service or the Internet. This flag cannot be set at the same time as CLSCTX_NO_CODE_DOWNLOAD.
    /// </summary>
    ENABLE_CODE_DOWNLOAD = 0x2000,
    /// <summary>
    /// The CLSCTX_NO_FAILURE_LOG can be used to override the logging of failures in CoCreateInstanceEx.
    /// </summary>
    NO_FAILURE_LOG = 0x4000,
    /// <summary>
    /// Disables activate-as-activator (AAA) activations for this activation only.
    /// </summary>
    DISABLE_AAA = 0x8000,
    /// <summary>
    /// Enables activate-as-activator (AAA) activations for this activation only.
    /// </summary>
    ENABLE_AAA = 0x10000,
    /// <summary>
    /// Begin this activation from the default context of the current apartment.
    /// </summary>
    FROM_DEFAULT_CONTEXT = 0x20000,
    /// <summary>
    /// Activate or connect to a 32 bit version of the server; fail if one is not registered.
    /// </summary>
    ACTIVATE_32_BIT_SERVER= 0x40000,
    /// <summary>
    /// Activate or connect to a 64 bit version of the server; fail if one is not registered.
    /// </summary>
    ACTIVATE_64_BIT_SERVER = 0x80000,
    /// <summary>
    /// When this flag is specified, COM uses the impersonation token of the thread, if one is present, for the activation request made by the thread. When this flag is not specified or if the thread does not have an impersonation token, COM uses the process token of the thread's process for the activation request made by the thread.
    /// </summary>
    ENABLE_CLOAKING = 0x100000,
    /// <summary>
    /// Indicates activation is for an app container.
    /// </summary>
    APPCONTAINER = 0x400000,
    INPROC = INPROC_SERVER | INPROC_HANDLER,
    SERVER = INPROC_SERVER | LOCAL_SERVER | REMOTE_SERVER,
    ALL = SERVER | INPROC_HANDLER
}