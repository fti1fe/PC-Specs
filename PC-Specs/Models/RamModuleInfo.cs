namespace PC_Specs.Models
{
    /// <summary>
    /// Holds information about a single RAM module.
    /// </summary>
    public class RamModuleInfo
    {
        // Slot label, e.g. "DIMM 0" or "ChannelA-DIMM0"
        public string DeviceLocator { get; set; }

        // Module capacity in bytes
        public ulong Capacity { get; set; }

        // Module speed in MHz, e.g. 3200
        public uint Speed { get; set; }

        // Manufacturer of the module
        public string Manufacturer { get; set; }

        // Part number of the module
        public string PartNumber { get; set; }

        // Form factor, e.g. "DIMM", "SODIMM"
        public string FormFactor { get; set; }

        // Memory type, e.g. "DDR4"
        public string MemoryType { get; set; }
    }
}
