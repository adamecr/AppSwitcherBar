using System;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeStructs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeFolderDefinition
    {
        internal FolderCategory category;
        internal IntPtr name;
        internal IntPtr description;
        internal Guid parentId;
        internal IntPtr relativePath;
        internal IntPtr parsingName;
        internal IntPtr tooltip;
        internal IntPtr localizedName;
        internal IntPtr icon;
        internal IntPtr security;
        internal uint attributes;
        internal DefinitionOptions definitionOptions;
        internal Guid folderTypeId;
    }
}
