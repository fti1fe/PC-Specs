# Setup Guide for PC-Specs

## Required Packages (.NET 8.0 Windows)

| Package                                 | Version                  |
|------------------------------------------|--------------------------|
| LibreHardwareMonitorLib                  | 0.9.4                    |
| System.Data.SqlClient                    | 4.9.0                    |
| System.Management                        | 9.0.5                    |
| System.Windows.Forms.DataVisualization   | 1.0.0-prerelease.20110.1 |

## Installation

Open the terminal in your project folder and run these commands:

```powershell
# Install required packages
 dotnet add package LibreHardwareMonitorLib --version 0.9.4
 dotnet add package System.Data.SqlClient --version 4.9.0
 dotnet add package System.Management --version 9.0.5
 dotnet add package System.Windows.Forms.DataVisualization --version 1.0.0-prerelease.20110.1
```

After installing the packages, you can start the project as usual.

Good luck!
