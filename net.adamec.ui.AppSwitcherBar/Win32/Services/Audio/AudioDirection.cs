namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Audio
{
    /// <summary>
    /// Indicates the direction in which audio data flows between an audio endpoint device and an application.
    /// </summary>
    public enum AudioDirection
    {
        /// <summary>
        /// Audio rendering stream. Audio data flows from the application to the audio endpoint device, which renders the stream.
        /// </summary>
        Render = 0,
        /// <summary>
        /// Audio capture stream. Audio data flows from the audio endpoint device that captures the stream, to the application.
        /// </summary>
        Capture = 1,
    }
}
