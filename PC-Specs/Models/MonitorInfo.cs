namespace PC_Specs.Models
{
    /// <summary>
    /// Holds information about a single monitor/display output.
    /// </summary>
    public class MonitorInfo
    {
        // Name or model of the monitor
        public string Name { get; set; }

        // Manufacturer of the monitor
        public string Manufacturer { get; set; }

        // Serial number of the monitor
        public string SerialNumber { get; set; }

        // Screen resolution, e.g. "1920x1080"
        public string Resolution { get; set; }

        // Optional: Connection type, e.g. "HDMI", "DisplayPort"
        public string ConnectionType { get; set; }
    }
}
