# PC Hardware Specification Tool Alpha

## Overview
This Windows application shows detailed hardware information about your computer. It uses Windows Management Instrumentation (WMI) and LibreHardwareMonitor to collect data. LibreHardwareMonitor is used to read sensor data such as CPU temperature, which is not available through WMI alone. You can see details about your CPU, RAM, GPU, storage, motherboard, operating system, audio devices, network adapters, and monitors. The tool also lets you update the CPU temperature live.

## Key Features
- Collects hardware information using WMI and LibreHardwareMonitor
- Shows all data in a clear, structured format
- Supports detection of:
  - Processor details (cores, speed, manufacturer, socket, cache, temperature)
  - RAM modules (size, speed, form factor, type)
  - Graphics cards (VRAM, driver version, CUDA support)
  - Storage devices (capacity, interface type, media type)
  - Motherboard information
  - Operating system details
  - Audio devices (name, manufacturer, type)
  - Network adapters (name, MAC address, type)
  - Monitors (name, manufacturer, serial, resolution)
- Human-readable byte size formatting
- Button to update CPU temperature without reloading all data

## PC Specs To Do

```
┌───────────────────────────────┐
│ [ ] CPU temperatures          │
│ [ ] GPU temperatures          │
│ [ ] Clock rates per CPU core  │
│ [ ] GPU clock rate            │
│ [ ] VRAM clock rate           │
│ [ ] More precise GPU model    │
│ [ ] Fix VRAM amount           │
│ [ ] Fix CUDA                  │
│ [ ] Fix monitor resolution    │
└───────────────────────────────┘
```
