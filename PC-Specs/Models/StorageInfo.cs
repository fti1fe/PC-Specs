namespace PC_Specs.Models
{
    /// <summary>
    /// Holds information about a single storage device (HDD, SSD).
    /// </summary>
    public class StorageInfo
    {
        // The model name of the drive
        public string Model { get; set; }

        // The total capacity in bytes
        public ulong Size { get; set; }

        // The interface type, for example: "SATA", "NVMe", "SCSI", "IDE"
        public string InterfaceType { get; set; }

        // The media type, for example: "Fixed hard disk media", "SSD"
        public string MediaType { get; set; }
    }
}
