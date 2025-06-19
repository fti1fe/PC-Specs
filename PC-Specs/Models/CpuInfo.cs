using System.Collections.Generic;

namespace PC_Specs.Models
{
    /// <summary>
    /// Holds information about the CPU.
    /// </summary>
    public class CpuInfo
    {
        // The name of the CPU, for example: "Intel Core i7-9700K"
        public string Name { get; set; }

        // The company that made the CPU, for example: "GenuineIntel"
        public string Manufacturer { get; set; }

        // How many physical cores the CPU has
        public uint NumberOfCores { get; set; }

        // How many logical processors (threads) the CPU has
        public uint NumberOfLogicalProcessors { get; set; }

        // The highest clock speed in MHz
        public uint MaxClockSpeed { get; set; }

        // (Optional) The name of the CPU socket, for example: "LGA1151"
        public string Socket { get; set; }

        // (Optional) L2 cache size in KB
        public uint L2CacheSize { get; set; }

        // (Optional) L3 cache size in KB
        public uint L3CacheSize { get; set; }

        // (Optional) The temperature of each core in degrees Celsius
        public List<float> CoreTemperatures { get; set; }

        // The clock rate of each core in MHz
        public List<float> CoreClockRates { get; set; }
    }
}
