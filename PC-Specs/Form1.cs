﻿using System;
using System.Text;
using System.Windows.Forms;
using PC_Specs.Models;
using PC_Specs.Services;

namespace PC_Specs
{
    public partial class Form1 : Form
    {
        private readonly HardwareInfoService hardwareService = new HardwareInfoService();
        private Button btnLoadSpecs;
        private Button btnUpdateCpuTemp; // NEU
        private TextBox txtOutput;
        private PcInformation lastPcInfo; // NEU

        public Form1()
        {
            InitializeComponent();
            InitializeCustomUi();
        }

        private void InitializeCustomUi()
        {
            // Button to load hardware data
            btnLoadSpecs = new Button
            {
                Text = "Load Specs",
                Top = 10,
                Left = 10,
                Width = 100
            };
            btnLoadSpecs.Click += BtnLoadSpecs_Click;
            this.Controls.Add(btnLoadSpecs);

            // Button to update only CPU temperature
            btnUpdateCpuTemp = new Button
            {
                Text = "Update CPU Temperature",
                Top = 10,
                Left = btnLoadSpecs.Right + 10,
                Width = 170
            };
            btnUpdateCpuTemp.Click += BtnUpdateCpuTemp_Click;
            this.Controls.Add(btnUpdateCpuTemp);

            // Large TextBox to display hardware data
            txtOutput = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                Top = btnLoadSpecs.Bottom + 10,
                Left = 10,
                Width = this.ClientSize.Width - 20,
                Height = this.ClientSize.Height - btnLoadSpecs.Bottom - 30,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true
            };
            this.Controls.Add(txtOutput);
        }

        private void BtnLoadSpecs_Click(object sender, EventArgs e)
        {
            lastPcInfo = hardwareService.GetAllPcInformation();
            txtOutput.Text = lastPcInfo != null ? FormatPcInformation(lastPcInfo) : "No data found.";
        }

        private void BtnUpdateCpuTemp_Click(object sender, EventArgs e)
        {
            if (lastPcInfo == null)
            {
                // If nothing loaded yet, load everything
                BtnLoadSpecs_Click(sender, e);
                return;
            }
            // Only update CPU temperature
            var cpu = hardwareService.GetCpuDetails();
            if (cpu != null)
            {
                lastPcInfo.Cpu.CoreTemperatures = cpu.CoreTemperatures;
                // Optional: update more CPU fields if wanted
            }
            txtOutput.Text = FormatPcInformation(lastPcInfo);
        }

        private string FormatPcInformation(PcInformation info)
        {
            var sb = new StringBuilder();
            // OS
            sb.AppendLine("Operating System:");
            if (info.OperatingSystem != null)
            {
                sb.AppendLine($"  Name: {info.OperatingSystem.Caption}");
                sb.AppendLine($"  Version: {info.OperatingSystem.Version}");
                sb.AppendLine($"  Architecture: {info.OperatingSystem.OSArchitecture}");
            }
            sb.AppendLine();

            // CPU
            sb.AppendLine("CPU:");
            if (info.Cpu != null)
            {
                sb.AppendLine($"  Name: {info.Cpu.Name}");
                sb.AppendLine($"  Manufacturer: {info.Cpu.Manufacturer}");
                sb.AppendLine($"  Cores: {info.Cpu.NumberOfCores}");
                sb.AppendLine($"  Logical Processors: {info.Cpu.NumberOfLogicalProcessors}");
                sb.AppendLine($"  Max Clock Speed: {info.Cpu.MaxClockSpeed} MHz");
                sb.AppendLine($"  Socket: {info.Cpu.Socket}");
                sb.AppendLine($"  L2 Cache: {info.Cpu.L2CacheSize} KB");
                sb.AppendLine($"  L3 Cache: {info.Cpu.L3CacheSize} KB");
                // Only show CPU Package temperature
                if (info.Cpu.CoreTemperatures != null && info.Cpu.CoreTemperatures.Count > 0)
                {
                    sb.AppendLine($"  CPU Temperature: {info.Cpu.CoreTemperatures[0]:F1} °C");
                }
                else
                {
                    sb.AppendLine("  CPU Temperature: n/a");
                }
            }
            sb.AppendLine();

            // Mainboard
            sb.AppendLine("Mainboard:");
            if (info.MainBoard != null)
            {
                sb.AppendLine($"  Manufacturer: {info.MainBoard.Manufacturer}");
                sb.AppendLine($"  Product: {info.MainBoard.Product}");
                sb.AppendLine($"  Serial: {info.MainBoard.SerialNumber}");
                sb.AppendLine($"  Version: {info.MainBoard.Version}");
            }
            sb.AppendLine();

            // RAM
            sb.AppendLine("RAM Modules:");
            if (info.RamModules != null)
            {
                int i = 1;
                foreach (var ram in info.RamModules)
                {
                    sb.AppendLine($"  Module {i++}:");
                    sb.AppendLine($"    Device: {ram.DeviceLocator}");
                    sb.AppendLine($"    Capacity: {ram.Capacity / (1024 * 1024)} MB");
                    sb.AppendLine($"    Speed: {ram.Speed} MHz");
                    sb.AppendLine($"    Manufacturer: {ram.Manufacturer}");
                    sb.AppendLine($"    Part Number: {ram.PartNumber}");
                    sb.AppendLine($"    Form Factor: {ram.FormFactor}");
                    sb.AppendLine($"    Memory Type: {ram.MemoryType}");
                }
            }
            sb.AppendLine();

            // GPUs
            sb.AppendLine("GPUs:");
            if (info.Gpus != null)
            {
                int i = 1;
                foreach (var gpu in info.Gpus)
                {
                    sb.AppendLine($"  GPU {i++}:");
                    sb.AppendLine($"    Name: {gpu.Name}");
                    sb.AppendLine($"    RAM: {gpu.AdapterRAM / (1024 * 1024)} MB");
                    sb.AppendLine($"    Driver: {gpu.DriverVersion}");
                    sb.AppendLine($"    Processor: {gpu.VideoProcessor}");
                    sb.AppendLine($"    CUDA: {(gpu.SupportsCuda ? "Yes" : "No")}");
                    // NEW: GPU Temperaturen
                    if (gpu.Temperatures != null && gpu.Temperatures.Count > 0)
                    {
                        foreach (var t in gpu.Temperatures)
                        {
                            sb.AppendLine($"    {t.Name} Temperature: {t.Value:F1} °C");
                        }
                    }
                    else
                    {
                        sb.AppendLine("    GPU Temperature: n/a");
                    }
                }
            }
            sb.AppendLine();

            // Storage
            sb.AppendLine("Storage Devices:");
            if (info.StorageDevices != null)
            {
                int i = 1;
                foreach (var s in info.StorageDevices)
                {
                    sb.AppendLine($"  Device {i++}:");
                    sb.AppendLine($"    Model: {s.Model}");
                    sb.AppendLine($"    Size: {s.Size / (1024 * 1024 * 1024)} GB");
                    sb.AppendLine($"    Interface: {s.InterfaceType}");
                    sb.AppendLine($"    Media Type: {s.MediaType}");
                }
            }
            sb.AppendLine();

            // Audio Devices
            sb.AppendLine("Audio Devices:");
            if (info.AudioDevices != null)
            {
                int i = 1;
                foreach (var audio in info.AudioDevices)
                {
                    sb.AppendLine($"  Device {i++}:");
                    sb.AppendLine($"    Name: {audio.Name}");
                    sb.AppendLine($"    Manufacturer: {audio.Manufacturer}");
                    sb.AppendLine($"    Type: {audio.DeviceType}");
                    sb.AppendLine($"    Device ID: {audio.DeviceId}");
                }
            }
            sb.AppendLine();

            // Network Adapters
            sb.AppendLine("Network Adapters:");
            if (info.NetworkAdapters != null)
            {
                int i = 1;
                foreach (var net in info.NetworkAdapters)
                {
                    sb.AppendLine($"  Adapter {i++}:");
                    sb.AppendLine($"    Name: {net.Name}");
                    sb.AppendLine($"    Manufacturer: {net.Manufacturer}");
                    sb.AppendLine($"    MAC Address: {net.MacAddress}");
                    sb.AppendLine($"    Type: {net.AdapterType}");
                    sb.AppendLine($"    IP Address: {net.IpAddress}");
                }
            }
            sb.AppendLine();

            // Monitors
            sb.AppendLine("Monitors:");
            if (info.Monitors != null)
            {
                int i = 1;
                foreach (var mon in info.Monitors)
                {
                    sb.AppendLine($"  Monitor {i++}:");
                    sb.AppendLine($"    Name: {mon.Name}");
                    sb.AppendLine($"    Manufacturer: {mon.Manufacturer}");
                    sb.AppendLine($"    Serial Number: {mon.SerialNumber}");
                    sb.AppendLine($"    Resolution: {mon.Resolution}");
                    sb.AppendLine($"    Connection Type: {mon.ConnectionType}");
                }
            }
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
