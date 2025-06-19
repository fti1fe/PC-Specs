# PC Hardware Specification Tool Alpha

## Overview
This Windows application retrieves and displays detailed hardware specifications of a computer system using Windows Management Instrumentation (WMI). Built with C# and .NET, it provides comprehensive information about key components including CPU, RAM, GPU, storage devices, motherboard, and operating system.

## Key Features
- Retrieves hardware specifications via WMI queries
- Displays information in a structured format
- Supports detection of:
  - Processor details (cores, speed, manufacturer)
  - RAM modules (size, speed, form factor)
  - Graphics cards (VRAM, driver version)
  - Storage devices (capacity, interface type)
  - Motherboard information
  - Operating system details
  - (and more)
- Byte size formatting for human-readable output

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
