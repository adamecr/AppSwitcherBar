using System;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums
{
    /// <summary>
    /// Specifies option settings. Used with IShellLinkDataList::GetFlags and IShellLinkDataList::SetFlags.
    /// </summary>
    [Flags]
    internal enum SHELL_LINK_DATA_FLAGS : uint
    {
        /// <summary>Default value used when no other flag is explicitly set.</summary>
        SLDF_DEFAULT = 0x00000000,

        /// <summary>The Shell link was saved with an ID list.</summary>
        SLDF_HAS_ID_LIST = 0x00000001,

        /// <summary>
        /// The Shell link was saved with link information to enable distributed tracking. This information is used by .lnk files to
        /// locate the target if the targets's path has changed. It includes information such as volume label and serial number, although
        /// the specific stored information can change from release to release.
        /// </summary>
        SLDF_HAS_LINK_INFO = 0x00000002,

        /// <summary>The link has a name.</summary>
        SLDF_HAS_NAME = 0x00000004,

        /// <summary>The link has a relative path.</summary>
        SLDF_HAS_RELPATH = 0x00000008,

        /// <summary>The link has a working directory.</summary>
        SLDF_HAS_WORKINGDIR = 0x00000010,

        /// <summary>The link has arguments.</summary>
        SLDF_HAS_ARGS = 0x00000020,

        /// <summary>The link has an icon location.</summary>
        SLDF_HAS_ICONLOCATION = 0x00000040,

        /// <summary>Stored strings are Unicode.</summary>
        SLDF_UNICODE = 0x00000080,

        /// <summary>
        /// Prevents the storage of link tracking information. If this flag is set, it is less likely, though not impossible, that a
        /// target can be found by the link if that target is moved.
        /// </summary>
        SLDF_FORCE_NO_LINKINFO = 0x00000100,

        /// <summary>The link contains expandable environment strings such as %windir%.</summary>
        SLDF_HAS_EXP_SZ = 0x00000200,

        /// <summary>Causes a 16-bit target application to run in a separate Virtual DOS Machine (VDM)/Windows on Windows (WOW).</summary>
        SLDF_RUN_IN_SEPARATE = 0x00000400,

        /// <summary>Not supported. Note that as of Windows Vista, this value is no longer defined.</summary>
        SLDF_HAS_LOGO3ID = 0x00000800,

        /// <summary>The link is a special Windows Installer link.</summary>
        SLDF_HAS_DARWINID = 0x00001000,

        /// <summary>Causes the target application to run as a different user.</summary>
        SLDF_RUNAS_USER = 0x00002000,

        /// <summary>The icon path in the link contains an expandable environment string such as such as %windir%.</summary>
        SLDF_HAS_EXP_ICON_SZ = 0x00004000,

        /// <summary>Prevents the use of ID list alias mapping when parsing the ID list from the path.</summary>
        SLDF_NO_PIDL_ALIAS = 0x00008000,

        /// <summary>Forces the use of the UNC name (a full network resource name), rather than the local name.</summary>
        SLDF_FORCE_UNCNAME = 0x00010000,

        /// <summary>
        /// Causes the target of this link to launch with a shim layer active. A shim is an intermediate DLL that facilitates
        /// compatibility between otherwise incompatible software services. Shims are typically used to provide version compatibility.
        /// </summary>
        SLDF_RUN_WITH_SHIMLAYER = 0x00020000,

        /// <summary>Introduced in Windows Vista. Disable object ID distributed tracking information.</summary>
        SLDF_FORCE_NO_LINKTRACK = 0x00040000,

        /// <summary>Introduced in Windows Vista. Enable the caching of target metadata into the link file.</summary>
        SLDF_ENABLE_TARGET_METADATA = 0x000800000,

        /// <summary>Introduced in Windows 7. Disable shell link tracking.</summary>
        SLDF_DISABLE_LINK_PATH_TRACKING = 0x00100000,

        /// <summary>Introduced in Windows Vista. Disable known folder tracking information.</summary>
        SLDF_DISABLE_KNOWNFOLDER_RELATIVE_TRACKING = 0x00200000,

        /// <summary>Introduced in Windows 7. Disable known folder alias mapping when loading the IDList during deserialization.</summary>
        SLDF_NO_KF_ALIAS = 0x00400000,

        /// <summary>Introduced in Windows 7. Allow link to point to another shell link as long as this does not create cycles.</summary>
        SLDF_ALLOW_LINK_TO_LINK = 0x00800000,

        /// <summary>Introduced in Windows 7. Remove alias when saving the IDList.</summary>
        SLDF_UNALIAS_ON_SAVE = 0x01000000,

        /// <summary>
        /// Introduced in Windows 7. Recalculate the IDList from the path with the environmental variables at load time, rather than
        /// persisting the IDList.
        /// </summary>
        SLDF_PREFER_ENVIRONMENT_PATH = 0x02000000,

        /// <summary>
        /// Introduced in Windows 7. If the target is a UNC location on a local machine, keep the local IDList target in addition to the
        /// remote target.
        /// </summary>
        SLDF_KEEP_LOCAL_IDLIST_FOR_UNC_TARGET = 0x04000000,

        /// <summary>
        /// Introduced in Windows 8. Persist the target IDlist in its volume-ID-relative form to avoid a dependency on drive letters.
        /// </summary>
        SLDF_PERSIST_VOLUME_ID_RELATIVE = 0x08000000,
    }
}
