namespace PC_Specs.Models
{
    /// <summary>
    /// Holds information about a single storage device (HDD, SSD).
    /// </summary>
    public class StorageInfo
    {
        // Model name of the drive
        public string Model { get; set; }

        // Total capacity in bytes
        public ulong Size { get; set; }

        // Interface type, e.g. "SATA", "NVMe", "SCSI", "IDE"
        public string InterfaceType { get; set; }

        // Media type, e.g. "Fixed hard disk media", "SSD"
        public string MediaType { get; set; }
    }
}
