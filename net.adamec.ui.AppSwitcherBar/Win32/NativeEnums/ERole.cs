// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

/// <summary>
/// The ERole enumeration defines constants that indicate the role that the system has assigned to an audio endpoint device.
/// </summary>
internal enum ERole
{
    /// <summary>
    /// Games, system notification sounds, and voice commands.
    /// </summary>
    Console = 0,
    /// <summary>
    /// Music, movies, narration, and live music recording.
    /// </summary>
    Multimedia = 1,
    /// <summary>
    /// Voice communications (talking to another person).
    /// </summary>
    Communications = 2,
    /*
        /// <summary>
        /// The number of members in the ERole enumeration (not counting the ERole_enum_count member).
        /// </summary>
        ERoleEnumCount = 3*/
}