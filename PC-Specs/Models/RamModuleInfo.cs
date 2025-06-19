namespace PC_Specs.Models
{
    /// <summary>
    /// Holds information about a single RAM module.
    /// </summary>
    public class RamModuleInfo
    {
        // The slot label, for example: "DIMM 0" or "ChannelA-DIMM0"
        public string DeviceLocator { get; set; }

        // The module capacity in bytes
        public ulong Capacity { get; set; }

        // The module speed in MHz, for example: 3200
        public uint Speed { get; set; }

        // The company that made the module
        public string Manufacturer { get; set; }

        // The part number of the module
        public string PartNumber { get; set; }

        // The form factor, for example: "DIMM", "SODIMM"
        public string FormFactor { get; set; }

        // The memory type, for example: "DDR4"
        public string MemoryType { get; set; }
    }
}
