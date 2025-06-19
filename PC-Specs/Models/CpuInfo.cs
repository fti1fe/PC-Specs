using System.Collections.Generic;

namespace PC_Specs.Models
{
    /// <summary>
    /// Holds information about the CPU.
    /// </summary>
    public class CpuInfo
    {
        // Name of the CPU, e.g. "Intel Core i7-9700K"
        public string Name { get; set; }

        // Manufacturer, e.g. "GenuineIntel"
        public string Manufacturer { get; set; }

        // Number of physical cores
        public uint NumberOfCores { get; set; }

        // Number of logical processors (threads)
        public uint NumberOfLogicalProcessors { get; set; }

        // Maximum clock speed in MHz
        public uint MaxClockSpeed { get; set; }

        // Optional: CPU socket name, e.g. "LGA1151"
        public string Socket { get; set; }

        // Optional: L2 cache size in KB
        public uint L2CacheSize { get; set; }

        // Optional: L3 cache size in KB
        public uint L3CacheSize { get; set; }

        // Optional: Temperature per core in degrees Celsius
        public List<float> CoreTemperatures { get; set; }

        // Clock rate per core in MHz
        public List<float> CoreClockRates { get; set; }
    }
}
