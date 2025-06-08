using System;
using System.Collections.Generic;
using System.Management;
using PC_Specs.Models;

namespace PC_Specs.Services
{
    /// <summary>
    /// Service for retrieving hardware information using WMI.
    /// </summary>
    public class HardwareInfoService
    {
        public CpuInfo GetCpuDetails()
        {
            // Retrieves CPU information from Win32_Processor
            try
            {
                using (var searcher = new ManagementObjectSearcher("select * from Win32_Processor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return new CpuInfo
                        {
                            Name = obj["Name"]?.ToString(),
                            Manufacturer = obj["Manufacturer"]?.ToString(),
                            NumberOfCores = obj["NumberOfCores"] != null ? Convert.ToUInt32(obj["NumberOfCores"]) : 0,
                            NumberOfLogicalProcessors = obj["NumberOfLogicalProcessors"] != null ? Convert.ToUInt32(obj["NumberOfLogicalProcessors"]) : 0,
                            MaxClockSpeed = obj["MaxClockSpeed"] != null ? Convert.ToUInt32(obj["MaxClockSpeed"]) : 0,
                            Socket = obj["SocketDesignation"]?.ToString(),
                            L2CacheSize = obj["L2CacheSize"] != null ? Convert.ToUInt32(obj["L2CacheSize"]) : 0,
                            L3CacheSize = obj["L3CacheSize"] != null ? Convert.ToUInt32(obj["L3CacheSize"]) : 0
                        };
                    }
                }
            }
            catch { }
            return null;
        }

        public List<RamModuleInfo> GetRamModulesDetails()
        {
            // Retrieves RAM modules information from Win32_PhysicalMemory
            var ramModules = new List<RamModuleInfo>();
            try
            {
                using (var searcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        ramModules.Add(new RamModuleInfo
                        {
                            DeviceLocator = obj["DeviceLocator"]?.ToString(),
                            Capacity = obj["Capacity"] != null ? Convert.ToUInt64(obj["Capacity"]) : 0,
                            Speed = obj["Speed"] != null ? Convert.ToUInt32(obj["Speed"]) : 0,
                            Manufacturer = obj["Manufacturer"]?.ToString(),
                            PartNumber = obj["PartNumber"]?.ToString(),
                            FormFactor = obj["FormFactor"] != null ? GetFormFactorString(Convert.ToInt32(obj["FormFactor"])) : null,
                            MemoryType = obj["MemoryType"] != null ? GetMemoryTypeString(Convert.ToInt32(obj["MemoryType"])) : null
                        });
                    }
                }
            }
            catch { }
            return ramModules;
        }

        // Returns a string representation for RAM form factor
        private string GetFormFactorString(int formFactor)
        {
            switch (formFactor)
            {
                case 8: return "DIMM";
                case 12: return "SODIMM";
                default: return formFactor.ToString();
            }
        }

        // Returns a string representation for RAM memory type
        private string GetMemoryTypeString(int memoryType)
        {
            switch (memoryType)
            {
                case 24: return "DDR3";
                case 26: return "DDR4";
                case 20: return "DDR";
                case 21: return "DDR2";
                default: return memoryType.ToString();
            }
        }

        public MainBoardInfo GetMainboardDetails()
        {
            // Retrieves mainboard information from Win32_BaseBoard
            try
            {
                using (var searcher = new ManagementObjectSearcher("select * from Win32_BaseBoard"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return new MainBoardInfo
                        {
                            Manufacturer = obj["Manufacturer"]?.ToString(),
                            Product = obj["Product"]?.ToString(),
                            SerialNumber = obj["SerialNumber"]?.ToString(),
                            Version = obj["Version"]?.ToString()
                        };
                    }
                }
            }
            catch { }
            return null;
        }

        public List<GpuInfo> GetGpuDetails()
        {
            // Retrieves GPU information from Win32_VideoController
            var gpus = new List<GpuInfo>();
            try
            {
                using (var searcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        gpus.Add(new GpuInfo
                        {
                            Name = obj["Name"]?.ToString(),
                            AdapterRAM = obj["AdapterRAM"] != null ? Convert.ToUInt64(obj["AdapterRAM"]) : 0,
                            DriverVersion = obj["DriverVersion"]?.ToString(),
                            VideoProcessor = obj["VideoProcessor"]?.ToString(),
                            SupportsCuda = obj["VideoProcessor"] != null && obj["VideoProcessor"].ToString().ToLower().Contains("cuda")
                        });
                    }
                }
            }
            catch { }
            return gpus;
        }

        public List<StorageInfo> GetStorageDetails()
        {
            // Retrieves storage information from Win32_DiskDrive
            var storages = new List<StorageInfo>();
            try
            {
                using (var searcher = new ManagementObjectSearcher("select * from Win32_DiskDrive"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        storages.Add(new StorageInfo
                        {
                            Model = obj["Model"]?.ToString(),
                            Size = obj["Size"] != null ? Convert.ToUInt64(obj["Size"]) : 0,
                            InterfaceType = obj["InterfaceType"]?.ToString(),
                            MediaType = obj["MediaType"]?.ToString()
                        });
                    }
                }
            }
            catch { }
            return storages;
        }

        public OperatingSystemInfo GetOperatingSystemDetails()
        {
            // Retrieves OS information from Win32_OperatingSystem
            try
            {
                using (var searcher = new ManagementObjectSearcher("select * from Win32_OperatingSystem"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return new OperatingSystemInfo
                        {
                            Caption = obj["Caption"]?.ToString(),
                            Version = obj["Version"]?.ToString(),
                            OSArchitecture = obj["OSArchitecture"]?.ToString()
                        };
                    }
                }
            }
            catch { }
            return null;
        }

        public List<NetworkAdapterInfo> GetNetworkAdapterDetails()
        {
            var adapters = new List<NetworkAdapterInfo>();
            try
            {
                using (var searcher = new ManagementObjectSearcher("select * from Win32_NetworkAdapter where PhysicalAdapter = true"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        adapters.Add(new NetworkAdapterInfo
                        {
                            Name = obj["Name"]?.ToString(),
                            Manufacturer = obj["Manufacturer"]?.ToString(),
                            MacAddress = obj["MACAddress"]?.ToString(),
                            AdapterType = obj["AdapterType"]?.ToString(),
                            // IP address is not directly available in Win32_NetworkAdapter, so leave empty or extend if needed
                            IpAddress = null
                        });
                    }
                }
            }
            catch { }
            return adapters;
        }

        public List<AudioDeviceInfo> GetAudioDeviceDetails()
        {
            var devices = new List<AudioDeviceInfo>();
            try
            {
                using (var searcher = new ManagementObjectSearcher("select * from Win32_SoundDevice"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        devices.Add(new AudioDeviceInfo
                        {
                            Name = obj["Name"]?.ToString(),
                            Manufacturer = obj["Manufacturer"]?.ToString(),
                            DeviceType = "Output", // Win32_SoundDevice does not provide type, so mark as "Output"
                            DeviceId = obj["DeviceID"]?.ToString()
                        });
                    }
                }
            }
            catch { }
            return devices;
        }

        public List<MonitorInfo> GetMonitorDetails()
        {
            var monitors = new List<MonitorInfo>();
            try
            {
                using (var searcher = new ManagementObjectSearcher("select * from Win32_DesktopMonitor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        monitors.Add(new MonitorInfo
                        {
                            Name = obj["Name"]?.ToString(),
                            Manufacturer = obj["MonitorManufacturer"]?.ToString(),
                            SerialNumber = obj["DeviceID"]?.ToString(),
                            Resolution = (obj["ScreenWidth"] != null && obj["ScreenHeight"] != null)
                                ? $"{obj["ScreenWidth"]}x{obj["ScreenHeight"]}"
                                : null,
                            ConnectionType = null // Not directly available
                        });
                    }
                }
            }
            catch { }
            return monitors;
        }

        public PcInformation GetAllPcInformation()
        {
            // Collects all hardware information into one object
            var pcInfo = new PcInformation
            {
                OperatingSystem = GetOperatingSystemDetails(),
                Cpu = GetCpuDetails(),
                MainBoard = GetMainboardDetails(),
                RamModules = GetRamModulesDetails(),
                Gpus = GetGpuDetails(),
                StorageDevices = GetStorageDetails(),
                NetworkAdapters = GetNetworkAdapterDetails(),
                AudioDevices = GetAudioDeviceDetails(),
                Monitors = GetMonitorDetails()
            };
            return pcInfo;
        }
    }
}
