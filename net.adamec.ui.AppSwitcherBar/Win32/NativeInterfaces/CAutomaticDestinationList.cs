using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

[ComImport]
[ClassInterface(ClassInterfaceType.None)]
[Guid(Win32Consts.CLSID_AutomaticDestinationList)]
internal class CAutomaticDestinationList { }