namespace PC_Specs.Models
{
    /// <summary>
    /// Holds information about a single monitor/display output.
    /// </summary>
    public class MonitorInfo
    {
        // The name or model of the monitor
        public string Name { get; set; }

        // The company that made the monitor
        public string Manufacturer { get; set; }

        // The serial number of the monitor
        public string SerialNumber { get; set; }

        // The screen resolution, for example: "1920x1080"
        public string Resolution { get; set; }

        // (Optional) The connection type, for example: "HDMI", "DisplayPort"
        public string ConnectionType { get; set; }
    }
}
