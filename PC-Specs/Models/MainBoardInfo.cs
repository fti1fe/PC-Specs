namespace PC_Specs.Models
{
    /// <summary>
    /// Holds information about the mainboard.
    /// </summary>
    public class MainBoardInfo
    {
        // The company that made the mainboard
        public string Manufacturer { get; set; }

        // The product or model name of the mainboard
        public string Product { get; set; }

        // The serial number of the mainboard
        public string SerialNumber { get; set; }

        // The version of the mainboard
        public string Version { get; set; }
    }
}
