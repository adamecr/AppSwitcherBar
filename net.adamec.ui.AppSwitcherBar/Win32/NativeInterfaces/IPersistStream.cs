using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces
{
    /// <summary>
    /// Enables the saving and loading of objects that use a simple serial stream for their storage needs.
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid(Win32Consts.IID_IPersistStream)]
    internal interface IPersistStream
    {
        /// <summary>
        /// Retrieves the class identifier (CLSID) of the object.
        /// </summary>
        /// <param name="pClassId">A pointer to the location that receives the CLSID on return. The CLSID is a globally unique identifier (GUID) that uniquely represents an object class that defines the code that can manipulate the object's data.</param>
        /// <returns>If the method succeeds, the return value is S_OK. Otherwise, it is E_FAIL.</returns>
        HRESULT GetClassID(out Guid pClassId);

        /// <summary>
        /// Determines whether an object has changed since it was last saved to its stream.
        /// </summary>
        /// <returns>This method returns S_OK to indicate that the object has changed. Otherwise, it returns S_FALSE.</returns>
        [PreserveSig]
        HRESULT IsDirty();

        /// <summary>
        /// Initializes an object from the stream where it was saved previously.
        /// </summary>
        /// <param name="pStm">An IStream pointer to the stream from which the object should be loaded.</param>
        /// <returns>This method can return the following values: S_OK - The method completed successfully, E_OUTOFMEMORY - The object was not loaded due to lack of memory, E_FAIL - The object was not loaded due to some reason other than a lack of memory.</returns>
        [PreserveSig]
        HRESULT Load([In] IStream pStm);

        /// <summary>
        /// Saves an object to the specified stream.
        /// </summary>
        /// <param name="pStm">An IStream pointer to the stream into which the object should be saved.</param>
        /// <param name="fClearDirty">Indicates whether to clear the dirty flag after the save is complete. If TRUE, the flag should be cleared. If FALSE, the flag should be left unchanged.</param>
        /// <returns>This method can return the following values:
        /// S_OK - The method completed successfully,
        /// STG_E_CANTSAVE - The object could not save itself to the stream. This error could indicate, for example, that the object contains another object that is not serializable to a stream or that an ISequentialStream::Write call returned STG_E_CANTSAVE,
        /// STG_E_MEDIUMFULL - The object could not be saved because there is no space left on the storage device.</returns>
        HRESULT Save([In] IStream pStm, [In, MarshalAs(UnmanagedType.Bool)] bool fClearDirty);
        
        /// <summary>
        /// Retrieves the size of the stream needed to save the object.
        /// </summary>
        /// <param name="pcbSize">The size in bytes of the stream needed to save this object, in bytes.</param>
        /// <returns>This method returns S_OK to indicate that the size was retrieved successfully.</returns>
        HRESULT GetSizeMax(out long pcbSize);
    }
}
