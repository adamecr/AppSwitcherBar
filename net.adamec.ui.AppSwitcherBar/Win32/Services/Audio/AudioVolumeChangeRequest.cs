namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Audio
{
    /// <summary>
    /// Audio device volume change request
    /// </summary>
    public class AudioVolumeChangeRequest
    {
        /// <summary>
        /// Audio device ID
        /// </summary>
        public string DeviceId { get; }
        /// <summary>
        /// Requested delta (for volume being scalar 0.0-0.1)
        /// </summary>
        public float Delta { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="deviceId">Audio device ID</param>
        /// <param name="delta">Requested delta</param>
        public AudioVolumeChangeRequest(string deviceId, float delta)
        {
            DeviceId = deviceId;
            Delta = delta;
        }
    }
}
