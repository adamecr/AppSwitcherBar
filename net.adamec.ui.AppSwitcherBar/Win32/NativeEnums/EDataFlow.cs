// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

/// <summary>
/// The EDataFlow enumeration defines constants that indicate the direction in which audio data flows between an audio endpoint device and an application.
/// </summary>
internal enum EDataFlow
{
    /// <summary>
    /// Audio rendering stream. Audio data flows from the application to the audio endpoint device, which renders the stream.
    /// </summary>
    Render = 0,
    /// <summary>
    /// Audio capture stream. Audio data flows from the audio endpoint device that captures the stream, to the application.
    /// </summary>
    Capture = 1,
    /// <summary>
    /// Audio rendering or capture stream. Audio data can flow either from the application to the audio endpoint device, or from the audio endpoint device to the application.
    /// </summary>
    All = 2,
    /*       
        /// <summary>
        /// The number of members in the EDataFlow enumeration (not counting the EDataFlow_enum_count member).
        /// </summary>
        DataFlowEnumCount = 3*/
}