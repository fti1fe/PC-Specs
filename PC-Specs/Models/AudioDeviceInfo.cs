namespace PC_Specs
{
    /// <summary>
    /// Holds information about a single audio device.
    /// </summary>
    public class AudioDeviceInfo
    {
        // Name of the audio device, e.g. "Realtek High Definition Audio"
        public string Name { get; set; }

        // Manufacturer of the audio device
        public string Manufacturer { get; set; }

        // Device type, e.g. "Output", "Input"
        public string DeviceType { get; set; }

        // Optional: Device ID or PNPDeviceID
        public string DeviceId { get; set; }
    }
}
