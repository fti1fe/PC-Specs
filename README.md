# PC Hardware Specification Tool Alpha

> **Disclaimer:**  
> This software is still very much under construction and should not be used at the moment. It is not reliable yet and is still being actively worked on.

## Overview
This Windows application shows detailed hardware information about your computer. It uses Windows Management Instrumentation and LibreHardwareMonitor to collect data. LibreHardwareMonitor is used to read sensor data such as CPU temperature, which is not available through WMI alone. You can see details about your CPU, RAM, GPU, storage, motherboard, operating system, audio devices, network adapters, and monitors. The tool also lets you update the CPU temperature live.

## Key Features
- Collects hardware information using WMI and LibreHardwareMonitor
- Shows all data in a clear, structured format
- Supports detection of:
  - Processor details (cores, speed, manufacturer, socket, cache, temperature)
  - RAM modules (size, speed, form factor, type)
  - Graphics cards (driver version, CUDA support)
  - Storage devices (capacity, interface type, media type)
  - Motherboard information
  - Operating system details
  - Audio devices (name, manufacturer, type)
  - Network adapters (name, MAC address, type)
  - Monitors (name, manufacturer, serial, resolution)
- Human-readable byte size formatting
- Button to update CPU temperature without reloading all data
- Displays detailed hardware information (CPU, GPU, RAM, storage, mainboard, network, audio, monitors)
- Live CPU core clock graphs
- NVIDIA GPU board partner and model detection (with PCI IDs)
- Modern Windows Forms UI
- Extensible model-service-UI architecture

### Supported GPU Temperature Sensors

The following GPU temperature sensors can be displayed (depending on your GPU and driver support):

- GPU Core
- GPU Hot Spot
- GPU Memory
- GPU VRM
- GPU (Chip/Die/Edge/Board/Memory Junction/VRAM/VRM/Hotspot/Power/Other)
- Any other temperature sensor reported by LibreHardwareMonitor

All available temperature sensors for each GPU will be listed with their names.

## PC Specs To Do

```
┌──────────────────────────────────────┐
│ [X] CPU temperature                  │
│ [X] GPU temperatures                 │
│ [X] Add graphs for cpu data          │
│ [ ] Add graphs for gpu data          │
│ [ ] Make graphs look better          │
│ [X] Clock rates per CPU core         │
│ [X] GPU clock rate                   │
│ [ ] VRAM clock rate                  │
│ [ ] More precise GPU model           │
│ [ ] Fix CUDA                         │
│ [ ] Fix monitor resolution           │
│ [ ] Add VRAM amount                  │
│ [ ] Make the UI more comfortable     │
└──────────────────────────────────────┘
```

## To-Do / Planned

- More detailed sensor data (fan speeds, voltages)
- Export to file (CSV, JSON)
- More UI features (dark mode, search, etc.)

## Requirements

- .NET 8.0 (Windows)
- NuGet: LibreHardwareMonitorLib, System.Management, System.Windows.Forms.DataVisualization, NvAPIWrapper.Net

## Usage

1. Build and run the solution in Visual Studio 2022 or newer.
2. Click "Load Specs" to display hardware information.
3. Use "Update All Data" to refresh.

## Notes

- Some features require admin rights for full sensor access.
- NVIDIA board partner detection only works with NVIDIA GPUs and requires NvAPIWrapper.Net.

## License

See LICENSE file.
