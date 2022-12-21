using net.adamec.ui.AppSwitcherBar.Win32.Services.Audio;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace net.adamec.ui.AppSwitcherBar.Dto
{
    /// <summary>
    /// Information about audio device
    /// </summary>
    public class AudioDeviceInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// Flag whether the device info is empty (has no device)
        /// </summary>
        private bool isEmpty;
        /// <summary>
        /// Flag whether the device info is empty (has no device)
        /// </summary>
        public bool IsEmpty
        {
            get => isEmpty;
            private set
            {
                if (value == isEmpty) return;
                isEmpty = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasDevice));
            }
        }

        /// <summary>
        /// Flag whether the device info has a device
        /// </summary>
        public bool HasDevice => !isEmpty;

        /// <summary>
        /// Device ID
        /// </summary>
        private string deviceId;

        /// <summary>
        /// Device ID
        /// </summary>
        public string DeviceId
        {
            get => deviceId;
            private set
            {
                if (value == deviceId) return;
                deviceId = value;
                IsEmpty = !string.IsNullOrEmpty(deviceId);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Name of the device
        /// </summary>
        private string device;
        /// <summary>
        /// Name of the device
        /// </summary>
       public string Device
        {
            get => device;
            private set
            {
                if (value == device) return;
                device = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Caption));
            }
        }

        /// <summary>
        /// Device caption - name with volume
        /// </summary>
      
        public string Caption => $"{Device}: {Volume}%";

        /// <summary>
        /// Audion flow direction
        /// </summary>
        private AudioDirection direction;

        /// <summary>
        /// Audion flow direction
        /// </summary>
        public AudioDirection Direction
        {
            get => direction;
            private set
            {
                if (value == direction) return;
                direction = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Flag whether the device is mutable
        /// </summary>
        private bool isMutable;
        /// <summary>
        /// Flag whether the device is mutable
        /// </summary>
        public bool IsMutable
        {
            get => isMutable;
            private set
            {
                if (value == isMutable) return;
                isMutable = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Flag whether the device is muted
        /// </summary>
        private bool isMuted;
        /// <summary>
        /// Flag whether the device is muted
        /// </summary>
        public bool IsMuted
        {
            get => isMuted;
            private set
            {
                if (value == isMuted) return;
                isMuted = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Device volume in percent
        /// </summary>
        private byte volume;
        /// <summary>
        /// Device volume in percent
        /// </summary>
        public byte Volume
        {
            get => volume;
            private set
            {
                if (value == volume) return;
                volume = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Caption));
            }
        }

        /// <summary>
        /// Flag whether the device is active (has any active session)
        /// </summary>
        private bool isActive;
        /// <summary>
        /// Flag whether the device is active (has any active session)
        /// </summary>
        public bool IsActive
        {
            get => isActive;
            set
            {
                if (value == isActive) return;
                isActive = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// CTOR
        /// </summary>
        public AudioDeviceInfo()
        {
            device = string.Empty;
            deviceId = string.Empty;
            isEmpty = true;
        }

        /// <summary>
        /// Updates the <see cref="AudioDeviceInfo"/> from <see cref="AudioDevice"/> source
        /// </summary>
        /// <param name="source">Source of the information about device</param>
        /// <returns>This object</returns>
        internal AudioDeviceInfo UpdateFrom(AudioDevice? source)
        {
            if (source == null)
            {
                DeviceId = string.Empty;
                Device = string.Empty;
                IsMutable = false;
                IsMuted = true;
                Volume = 0;
                IsActive=false;
                IsEmpty = true;


                return this;
            }

            DeviceId = source.Id;
            Device = source.Name;
            IsMutable = source.IsMutable;
            IsMuted = source.IsMuted;
            Direction = source.Direction;
            Volume = (byte)Math.Round(source.Volume * 100, 0);
            IsActive = source.IsActive;
            IsEmpty = false;
            return this;
        }

        /// <summary>
        /// Updates the <see cref="AudioDeviceInfo"/> from another device info source
        /// </summary>
        /// <param name="source">Source of the information about device</param>
        /// <returns>This object</returns>
        public AudioDeviceInfo UpdateFrom(AudioDeviceInfo source)
        {
            DeviceId = source.DeviceId;
            Device = source.Device;
            Direction = source.Direction;
            IsMutable = source.IsMutable;
            IsMuted = source.IsMuted;
            Volume = source.Volume;
            IsActive = source.IsActive;
            IsEmpty = source.IsEmpty;
            return this;
        }

        /// <summary>
        /// Makes the <see cref="AudioDeviceInfo"/> empty
        /// </summary>
        /// <returns>This object</returns>
        public AudioDeviceInfo MakeEmpty()
        {
            return UpdateFrom((AudioDevice?)null);
        }

        /// <summary>
        /// Occurs when a property value changes
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raise <see cref="PropertyChanged"/> event for given <paramref name="propertyName"/>
        /// </summary>
        /// <param name="propertyName">Name of the property changed</param>
        // ReSharper disable once UnusedMember.Global
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
