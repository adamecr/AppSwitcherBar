namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums
{
    /// <summary>
    /// The AudioSessionState enumeration defines constants that indicate the current state of an audio session.
    /// </summary>
    internal enum AudioSessionState
    {
        /// <summary>
        /// The audio session is inactive. (It contains at least one stream, but none of the streams in the session is currently running.)
        /// </summary>
        Inactive = 0,
        /// <summary>
        /// The audio session is active. (At least one of the streams in the session is running.)
        /// </summary>
        Active = 1,
        /// <summary>
        /// The audio session has expired. (It contains no streams.)
        /// </summary>
        Expired = 2
    }
}
