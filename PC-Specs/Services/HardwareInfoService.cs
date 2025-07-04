using System;
using System.Collections.Generic;
using System.Management;
using PC_Specs.Models;
using LibreHardwareMonitor.Hardware; // NEU

using System.Linq;
using NvAPIWrapper;
using NvAPIWrapper.GPU;

namespace PC_Specs.Services
{
    /// <summary>
    /// Service for retrieving hardware information using WMI.
    /// </summary>
    public class HardwareInfoService
    {
        public CpuInfo GetCpuDetails()
        {
            CpuInfo cpu = null;
            try
            {
                using (var searcher = new ManagementObjectSearcher("select * from Win32_Processor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        cpu = new CpuInfo
                        {
                            Name = obj["Name"]?.ToString(),
                            Manufacturer = obj["Manufacturer"]?.ToString(),
                            NumberOfCores = obj["NumberOfCores"] != null ? Convert.ToUInt32(obj["NumberOfCores"]) : 0,
                            NumberOfLogicalProcessors = obj["NumberOfLogicalProcessors"] != null ? Convert.ToUInt32(obj["NumberOfLogicalProcessors"]) : 0,
                            MaxClockSpeed = obj["MaxClockSpeed"] != null ? Convert.ToUInt32(obj["MaxClockSpeed"]) : 0,
                            Socket = obj["SocketDesignation"]?.ToString(),
                            L2CacheSize = obj["L2CacheSize"] != null ? Convert.ToUInt32(obj["L2CacheSize"]) : 0,
                            L3CacheSize = obj["L3CacheSize"] != null ? Convert.ToUInt32(obj["L3CacheSize"]) : 0,
                            CoreTemperatures = GetCpuCoreTemperatures(),
                            CoreClockRates = GetCpuCoreClockRates()
                        };
                        break;
                    }
                }
            }
            catch { }
            return cpu;
        }

        // Read CPU Package temperature (with fallback) using LibreHardwareMonitorLib
        private List<float> GetCpuCoreTemperatures()
        {
            var temps = new List<float>();
            try
            {
                Computer computer = new Computer
                {
                    IsCpuEnabled = true
                };
                computer.Open();
                foreach (var hardware in computer.Hardware)
                {
                    if (hardware.HardwareType == LibreHardwareMonitor.Hardware.HardwareType.Cpu)
                    {
                        hardware.Update();
                        // Try to find "CPU Package" first
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == LibreHardwareMonitor.Hardware.SensorType.Temperature &&
                                (sensor.Name.Equals("CPU Package", StringComparison.OrdinalIgnoreCase) ||
                                 sensor.Name.Equals("Package", StringComparison.OrdinalIgnoreCase)))
                            {
                                if (sensor.Value.HasValue)
                                {
                                    temps.Add(sensor.Value.Value);
                                    break;
                                }
                            }
                        }
                        // Fallback: take first available temperature sensor if no package sensor found
                        if (temps.Count == 0)
                        {
                            foreach (var sensor in hardware.Sensors)
                            {
                                if (sensor.SensorType == LibreHardwareMonitor.Hardware.SensorType.Temperature &&
                                    sensor.Value.HasValue)
                                {
                                    temps.Add(sensor.Value.Value);
                                    break;
                                }
                            }
                        }
                    }
                }
                computer.Close();
            }
            catch { }
            return temps;
        }

        // Read per-core clock rates using LibreHardwareMonitorLib
        private List<float> GetCpuCoreClockRates()
        {
            var clocks = new List<float>();
            try
            {
                Computer computer = new Computer
                {
                    IsCpuEnabled = true
                };
                computer.Open();
                foreach (var hardware in computer.Hardware)
                {
                    if (hardware.HardwareType == LibreHardwareMonitor.Hardware.HardwareType.Cpu)
                    {
                        hardware.Update();
                        // Find all "Clock" sensors with "Core #" in the name
                        var coreClocks = hardware.Sensors
                            .Where(s => s.SensorType == SensorType.Clock && s.Name.StartsWith("Core", StringComparison.OrdinalIgnoreCase) && s.Value.HasValue)
                            .OrderBy(s => s.Name) // "Core #1", "Core #2", etc.
                            .ToList();

                        // If no per-core clocks, try "CPU Core" (average)
                        if (coreClocks.Count > 0)
                        {
                            foreach (var sensor in coreClocks)
                                clocks.Add(sensor.Value.Value);
                        }
                        else
                        {
                            // Fallback: try "CPU Core" or "Bus Speed"
                            var cpuCoreClock = hardware.Sensors.FirstOrDefault(s =>
                                s.SensorType == SensorType.Clock &&
                                (s.Name.Equals("CPU Core", StringComparison.OrdinalIgnoreCase) ||
                                 s.Name.Equals("Core", StringComparison.OrdinalIgnoreCase)) &&
                                s.Value.HasValue);
                            if (cpuCoreClock != null)
                                clocks.Add(cpuCoreClock.Value.Value);
                        }
                        break;
                    }
                }
                computer.Close();
            }
            catch { }
            return clocks;
        }

        public List<RamModuleInfo> GetRamModulesDetails()
        {
            // Get RAM modules information from Win32_PhysicalMemory
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
                case 34: return "DDR5";
                default: return memoryType.ToString();
            }
        }

        public MainBoardInfo GetMainboardDetails()
        {
            // Get mainboard information from Win32_BaseBoard
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
            // Get GPU information from Win32_VideoController
            var gpus = new List<GpuInfo>();
            try
            {
                // Read GPU temperatures (LibreHardwareMonitor)
                var gpuTemps = GetGpuTemperaturesFromLHM();
                // Read GPU clock rates (LibreHardwareMonitor)
                var gpuClocks = GetGpuClockRatesFromLHM();

                using (var searcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
                {
                    int idx = 0;
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        var gpu = new GpuInfo
                        {
                            Name = obj["Name"]?.ToString(),
                            DriverVersion = obj["DriverVersion"]?.ToString(),
                            VideoProcessor = obj["VideoProcessor"]?.ToString(),
                            SupportsCuda = obj["VideoProcessor"] != null && obj["VideoProcessor"].ToString().ToLower().Contains("cuda"),
                            Temperatures = gpuTemps.Count > idx ? gpuTemps[idx] : new List<(string, float)>(),
                            ClockRates = gpuClocks.Count > idx ? gpuClocks[idx] : new List<(string, float)>()
                        };
                        gpus.Add(gpu);
                        idx++;
                    }
                }
            }
            catch { }

            // Extend NVIDIA GPU information
            EnrichNvidiaGpuInfo(gpus);
            return gpus;
        }

        private void EnrichNvidiaGpuInfo(List<GpuInfo> gpus)
        {
            try
            {
                // Only initialize NvAPI if NVIDIA GPUs are present
                bool hasNvidiaGpu = gpus.Any(g => g.Name != null && g.Name.ToUpper().Contains("NVIDIA"));
                if (!hasNvidiaGpu)
                    return;

                NVIDIA.Initialize();
                var physicalGPUs = PhysicalGPU.GetPhysicalGPUs();

                foreach (var gpu in gpus)
                {
                    // Check if NVIDIA GPU
                    if (gpu.Name != null && gpu.Name.ToUpper().Contains("NVIDIA"))
                    {
                        gpu.IsNvidia = true;
                        // Remove "NVIDIA" from name for better matching
                        string cleanGpuName = gpu.Name.Replace("NVIDIA", "").Trim();
                        PhysicalGPU matchingNvGpu = null;
                        // Try different matching strategies
                        foreach (var nvGpu in physicalGPUs)
                        {
                            // Exact match
                            if (nvGpu.FullName.Equals(cleanGpuName, StringComparison.OrdinalIgnoreCase))
                            {
                                matchingNvGpu = nvGpu;
                                break;
                            }
                            // Partial match
                            else if (nvGpu.FullName.Contains(cleanGpuName) || cleanGpuName.Contains(nvGpu.FullName))
                            {
                                matchingNvGpu = nvGpu;
                                break;
                            }
                        }
                        // If not found and only one NVIDIA GPU present
                        if (matchingNvGpu == null && physicalGPUs.Length == 1 && gpus.Count(g => g.IsNvidia) == 1)
                        {
                            matchingNvGpu = physicalGPUs[0];
                        }
                        if (matchingNvGpu != null)
                        {
                            try
                            {
                                // Try to get PCI IDs via reflection for compatibility with different NvAPIWrapper.Net versions
                                try
                                {
                                    var t = matchingNvGpu.GetType();
                                    object vendorIdObj = t.GetProperty("VendorId")?.GetValue(matchingNvGpu);
                                    object deviceIdObj = t.GetProperty("DeviceId")?.GetValue(matchingNvGpu);
                                    object subVendorIdObj = t.GetProperty("SubsystemVendorId")?.GetValue(matchingNvGpu);
                                    object subIdObj = t.GetProperty("SubsystemId")?.GetValue(matchingNvGpu);
                                    gpu.VendorId = vendorIdObj != null ? Convert.ToUInt32(vendorIdObj) : (uint?)null;
                                    gpu.DeviceId = deviceIdObj != null ? Convert.ToUInt32(deviceIdObj) : (uint?)null;
                                    gpu.SubsystemVendorId = subVendorIdObj != null ? Convert.ToUInt32(subVendorIdObj) : (uint?)null;
                                    gpu.SubsystemId = subIdObj != null ? Convert.ToUInt32(subIdObj) : (uint?)null;
                                }
                                catch { }
                                gpu.BoardPartner = NvidiaBoardPartnerMap.GetBoardPartnerName(gpu.SubsystemVendorId);
                                if (gpu.BoardPartner != "Unknown" && gpu.BoardPartner != "NVIDIA (Founders Edition)")
                                {
                                    gpu.BoardModel = $"{gpu.BoardPartner} {matchingNvGpu.FullName}";
                                }
                                else
                                {
                                    gpu.BoardModel = matchingNvGpu.FullName;
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error getting NVIDIA GPU details for {gpu.Name}: {ex.Message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NvAPI error: {ex.Message}");
            }
            finally
            {
                try
                {
                    NVIDIA.Unload();
                }
                catch { }
            }
        }

        // Read GPU temperatures (all sensors, for each GPU)
        private List<List<(string Name, float Value)>> GetGpuTemperaturesFromLHM()
        {
            var result = new List<List<(string, float)>>();
            try
            {
                Computer computer = new Computer
                {
                    IsGpuEnabled = true
                };
                computer.Open();
                foreach (var hardware in computer.Hardware)
                {
                    if (hardware.HardwareType == LibreHardwareMonitor.Hardware.HardwareType.GpuNvidia ||
                        hardware.HardwareType == LibreHardwareMonitor.Hardware.HardwareType.GpuAmd ||
                        hardware.HardwareType == LibreHardwareMonitor.Hardware.HardwareType.GpuIntel)
                    {
                        hardware.Update();
                        var temps = new List<(string, float)>();
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == LibreHardwareMonitor.Hardware.SensorType.Temperature && sensor.Value.HasValue)
                            {
                                temps.Add((sensor.Name, sensor.Value.Value));
                            }
                        }
                        result.Add(temps);
                    }
                }
                computer.Close();
            }
            catch { }
            return result;
        }

        // Read GPU clock rates (all sensors, for each GPU)
        private List<List<(string Name, float Value)>> GetGpuClockRatesFromLHM()
        {
            var result = new List<List<(string, float)>>();
            try
            {
                Computer computer = new Computer
                {
                    IsGpuEnabled = true
                };
                computer.Open();
                foreach (var hardware in computer.Hardware)
                {
                    if (hardware.HardwareType == LibreHardwareMonitor.Hardware.HardwareType.GpuNvidia ||
                        hardware.HardwareType == LibreHardwareMonitor.Hardware.HardwareType.GpuAmd ||
                        hardware.HardwareType == LibreHardwareMonitor.Hardware.HardwareType.GpuIntel)
                    {
                        hardware.Update();
                        var clocks = new List<(string, float)>();
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == LibreHardwareMonitor.Hardware.SensorType.Clock && sensor.Value.HasValue)
                            {
                                clocks.Add((sensor.Name, sensor.Value.Value));
                            }
                        }
                        result.Add(clocks);
                    }
                }
                computer.Close();
            }
            catch { }
            return result;
        }

        public List<StorageInfo> GetStorageDetails()
        {
            // Get storage information from Win32_DiskDrive
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
            // Get OS information from Win32_OperatingSystem
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
                            // IP address is not directly available in Win32_NetworkAdapter, so leave empty or add if needed
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
                        string deviceId = obj["DeviceID"]?.ToString();
                        string resolution = null;

                        // Try to match with System.Windows.Forms.Screen for real resolution
                        foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                        {
                            // DeviceName is like "\\.\DISPLAY1", DeviceID is like "DesktopMonitor1"
                            if (screen.DeviceName.EndsWith(deviceId?.Replace("DesktopMonitor", "DISPLAY")))
                            {
                                resolution = $"{screen.Bounds.Width}x{screen.Bounds.Height}";
                                break;
                            }
                        }
                        // Fallback: use WMI fields if available
                        if (resolution == null && obj["ScreenWidth"] != null && obj["ScreenHeight"] != null)
                        {
                            resolution = $"{obj["ScreenWidth"]}x{obj["ScreenHeight"]}";
                        }

                        monitors.Add(new MonitorInfo
                        {
                            Name = obj["Name"]?.ToString(),
                            Manufacturer = obj["MonitorManufacturer"]?.ToString(),
                            SerialNumber = obj["DeviceID"]?.ToString(),
                            Resolution = resolution,
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

