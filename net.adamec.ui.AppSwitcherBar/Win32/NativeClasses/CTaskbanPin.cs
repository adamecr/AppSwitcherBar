using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeClasses
{
    [ComImport]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid(Win32Consts.CLSID_TaskbanPin)]//90AA3A4E-1CBA-4233-B8BB-535773D48449
    internal class CTaskbanPin { }
}
