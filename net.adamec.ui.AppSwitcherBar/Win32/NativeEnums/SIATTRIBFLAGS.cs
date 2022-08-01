// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

/// <summary>ShellItem attribute flags.  SIATTRIBFLAGS_*</summary>
internal enum SIATTRIBFLAGS
{
    AND = 0x00000001,
    OR = 0x00000002,
    APPCOMPAT = 0x00000003,
    MASK = 0x00000003,
    ALLITEMS = 0x00004000
}