using System;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

/// <summary>Flags for Known Folder APIs.  KF_FLAG_*</summary>
/// <remarks>native enum was called KNOWN_FOLDER_FLAG</remarks>
[Flags]
internal enum KF_FLAG : uint
{
    DEFAULT = 0x00000000,

    // Make sure that the folder already exists or create it and apply security specified in folder definition
    // If folder can not be created then function will return failure and no folder path (IDList) will be returned
    // If folder is located on the network the function may take long time to execute
    CREATE = 0x00008000,

    // If this flag is specified then the folder path is returned and no verification is performed
    // Use this flag is you want to get folder's path (IDList) and do not need to verify folder's existence
    //
    // If this flag is NOT specified then Known Folder API will try to verify that the folder exists
    //     If folder does not exist or can not be accessed then function will return failure and no folder path (IDList) will be returned
    //     If folder is located on the network the function may take long time to execute
    DONT_VERIFY = 0x00004000,

    // Set folder path as is and do not try to substitute parts of the path with environments variables.
    // If flag is not specified then Known Folder will try to replace parts of the path with some
    // known environment variables (%USERPROFILE%, %APPDATA% etc.)
    DONT_UNEXPAND = 0x00002000,

    // Get file system based IDList if available. If the flag is not specified the Known Folder API
    // will try to return aliased IDList by default. Example for FOLDERID_Documents -
    // Aliased - [desktop]\[user]\[Documents] - exact location is determined by shell namespace layout and might change
    // Non aliased - [desktop]\[computer]\[disk_c]\[users]\[user]\[Documents] - location is determined by folder location in the file system
    NO_ALIAS = 0x00001000,

    // Initialize the folder with desktop.ini settings
    // If folder can not be initialized then function will return failure and no folder path will be returned
    // If folder is located on the network the function may take long time to execute
    INIT = 0x00000800,

    // Get the default path, will also verify folder existence unless KF_FLAG_DONT_VERIFY is also specified
    DEFAULT_PATH = 0x00000400,

    // Get the not-parent-relative default path. Only valid with KF_FLAG_DEFAULT_PATH
    NOT_PARENT_RELATIVE = 0x00000200,

    // Build simple IDList
    SIMPLE_IDLIST = 0x00000100,

    // only return the aliased IDLists, don't fallback to file system path
    ALIAS_ONLY = 0x80000000,
}