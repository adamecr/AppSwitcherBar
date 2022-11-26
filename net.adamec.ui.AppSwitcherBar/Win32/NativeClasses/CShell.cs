using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using System.Runtime.InteropServices;

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeClasses
{
  
    [ComImport]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid(Win32Consts.CLSID_Shell)]
    internal class CShell { }
}

