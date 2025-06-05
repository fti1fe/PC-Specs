using System.Collections.Generic;

namespace PC_Specs.Models
{
    /// <summary>
    /// Collects all PC specs in one object.
    /// </summary>
    public class PcInformation
    {
        // Operating system info
        public OperatingSystemInfo OperatingSystem { get; set; }

        // CPU info
        public CpuInfo Cpu { get; set; }

        // Mainboard info
        public MainBoardInfo MainBoard { get; set; }

        // List of RAM modules
        public List<RamModuleInfo> RamModules { get; set; }

        // List of GPUs
        public List<GpuInfo> Gpus { get; set; }

        // List of storage devices
        public List<StorageInfo> StorageDevices { get; set; }

        // List of network adapters
        public List<NetworkAdapterInfo> NetworkAdapters { get; set; }

        // List of audio devices
        public List<AudioDeviceInfo> AudioDevices { get; set; }

        // List of monitors
        public List<MonitorInfo> Monitors { get; set; }

        // Constructor initializes lists to avoid null references
        public PcInformation()
        {
            RamModules = new List<RamModuleInfo>();
            Gpus = new List<GpuInfo>();
            StorageDevices = new List<StorageInfo>();
            NetworkAdapters = new List<NetworkAdapterInfo>();
            AudioDevices = new List<AudioDeviceInfo>();
            Monitors = new List<MonitorInfo>();
        }
    }
}
