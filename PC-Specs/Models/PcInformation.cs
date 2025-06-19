using System.Collections.Generic;

namespace PC_Specs.Models
{
    /// <summary>
    /// Collects all PC specs in one object.
    /// </summary>
    public class PcInformation
    {
        // Info about the operating system
        public OperatingSystemInfo OperatingSystem { get; set; }

        // Info about the CPU
        public CpuInfo Cpu { get; set; }

        // Info about the mainboard
        public MainBoardInfo MainBoard { get; set; }

        // List of all RAM modules
        public List<RamModuleInfo> RamModules { get; set; }

        // List of all GPUs
        public List<GpuInfo> Gpus { get; set; }

        // List of all storage devices
        public List<StorageInfo> StorageDevices { get; set; }

        // List of all network adapters
        public List<NetworkAdapterInfo> NetworkAdapters { get; set; }

        // List of all audio devices
        public List<AudioDeviceInfo> AudioDevices { get; set; }

        // List of all monitors
        public List<MonitorInfo> Monitors { get; set; }

        // The constructor sets up the lists so they are not null
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
