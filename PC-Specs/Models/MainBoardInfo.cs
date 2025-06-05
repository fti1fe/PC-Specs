namespace PC_Specs.Models
{
    /// <summary>
    /// Holds information about the mainboard.
    /// </summary>
    public class MainBoardInfo
    {
        // Manufacturer of the mainboard
        public string Manufacturer { get; set; }

        // Product/model name of the mainboard
        public string Product { get; set; }

        // Serial number of the mainboard
        public string SerialNumber { get; set; }

        // Version of the mainboard
        public string Version { get; set; }
    }
}
