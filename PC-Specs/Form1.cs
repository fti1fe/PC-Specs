using System;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using PC_Specs.Models;
using PC_Specs.Services;
using System.Collections.Generic;

namespace PC_Specs
{
    public partial class Form1 : Form
    {
        private readonly HardwareInfoService hardwareService = new HardwareInfoService();
        private Button btnLoadSpecs;
        private Button btnUpdateAllData;
        private TextBox txtOutput;
        private PcInformation lastPcInfo;
        private List<Chart> cpuCoreCharts = new List<Chart>();
        private Timer cpuClockUpdateTimer;
        private SplitContainer splitContainer;
        private Panel chartPanel;
        private Panel buttonPanel; // New field for the button panel

        private List<float> minClocks = new List<float>();
        private List<float> maxClocks = new List<float>();
        private List<float> lastClocks = new List<float>();
        private List<DateTime> chartStartTimes = new List<DateTime>();

        private class ChartOverlayLabels
        {
            public Label MinLabel { get; set; }
            public Label MaxLabel { get; set; }
            public Label CurrentLabel { get; set; }
        }
        private List<ChartOverlayLabels> chartOverlayLabels = new List<ChartOverlayLabels>();

        // Helper class for the overlay label block
        private class ChartOverlayLabelBlock
        {
            public Label InfoLabel { get; set; }
        }
        // Stores overlay label blocks for each chart
        private List<ChartOverlayLabelBlock> chartOverlayLabelBlocks = new List<ChartOverlayLabelBlock>();

        public Form1()
        {
            InitializeComponent();
            InitializeCustomUi();
        }

        private void InitializeCustomUi()
        {
            // Remove old controls that are not needed anymore
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl != headerPanel)
                    this.Controls.Remove(ctrl);
            }

            // Panel for the buttons at the top
            buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = System.Drawing.Color.Transparent
            };
            this.Controls.Add(buttonPanel);
            buttonPanel.BringToFront();

            // SplitContainer for left/right
            splitContainer = new SplitContainer
            {
                Orientation = Orientation.Vertical,
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent,
                FixedPanel = FixedPanel.None
            };
            this.Controls.Add(splitContainer);
            splitContainer.BringToFront();
            // 30% left, 70% right
            splitContainer.SplitterDistance = (int)(this.ClientSize.Width * 0.3);

            // Left side: Textbox for data
            txtOutput = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = System.Drawing.Color.White,
                ForeColor = System.Drawing.Color.FromArgb(30, 30, 30),
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular)
            };
            splitContainer.Panel1.Controls.Add(txtOutput);

            // Right side: Panel for charts (with its own scrollbar)
            chartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = System.Drawing.Color.White
            };
            splitContainer.Panel2.Controls.Add(chartPanel);

            // Buttons in the button panel
            btnLoadSpecs = new Button
            {
                Text = "Load Specs",
                Top = 15,
                Left = 30,
                Width = 120,
                Height = 40,
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(0, 120, 215),
                ForeColor = System.Drawing.Color.White,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand
            };
            btnLoadSpecs.Click += BtnLoadSpecs_Click;
            buttonPanel.Controls.Add(btnLoadSpecs);

            btnUpdateAllData = new Button
            {
                Text = "Update All Data",
                Top = 15,
                Left = btnLoadSpecs.Right + 20,
                Width = 160,
                Height = 40,
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(0, 120, 215),
                ForeColor = System.Drawing.Color.White,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand
            };
            btnUpdateAllData.Click += BtnUpdateAllData_Click;
            buttonPanel.Controls.Add(btnUpdateAllData);

            // No charts at the start
            CreateCpuCoreCharts(0);
        }

        private void CreateCpuCoreCharts(int coreCount)
        {
            // Remove old charts and labels
            foreach (var chart in cpuCoreCharts)
                chartPanel.Controls.Remove(chart);
            if (chartOverlayLabelBlocks != null)
            {
                foreach (var overlay in chartOverlayLabelBlocks)
                {
                    if (overlay != null)
                    {
                        chartPanel.Controls.Remove(overlay.InfoLabel);
                    }
                }
            }
            cpuCoreCharts.Clear();
            minClocks.Clear();
            maxClocks.Clear();
            lastClocks.Clear();
            chartStartTimes.Clear();
            chartOverlayLabelBlocks.Clear();

            int startY = 10;
            int chartHeight = 100;
            int chartWidth = 420; // Fixed width
            int left = 10;
            int labelOffset = 0; // Directly next to chart
            for (int i = 0; i < coreCount; i++)
            {
                var chart = new Chart
                {
                    Width = chartWidth,
                    Height = chartHeight,
                    Left = left,
                    Top = startY + i * (chartHeight + 30),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left, // Only anchor top and left
                    BackColor = System.Drawing.Color.Black
                };
                var chartArea = new ChartArea();
                chartArea.AxisX.Title = "";
                chartArea.AxisX.LabelStyle.Enabled = false;
                chartArea.AxisY.Title = "";
                chartArea.AxisY.Minimum = 0;
                chartArea.AxisY.LabelStyle.ForeColor = System.Drawing.Color.LightGray;
                chartArea.AxisX.LabelStyle.ForeColor = System.Drawing.Color.LightGray;
                chartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(40, 255, 255, 255);
                chartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(40, 255, 255, 255);
                chartArea.BackColor = System.Drawing.Color.Black;
                chartArea.Position = new ElementPosition(5, 10, 90, 80);
                chartArea.InnerPlotPosition = new ElementPosition(10, 5, 80, 85);
                chartArea.AxisX.LabelStyle.Angle = 0;
                chartArea.AxisX.LabelStyle.IsStaggered = false;
                chartArea.AxisX.IsLabelAutoFit = false;
                chart.ChartAreas.Add(chartArea);
                var series = new Series($"Core {i + 1}")
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2,
                    Color = System.Drawing.Color.Lime
                };
                chart.Series.Add(series);
                chart.Legends.Clear();
                var chartTitle = new Title($"CPU{(i + 1)} Clock, MHz", Docking.Top, new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold), System.Drawing.Color.LightGray);
                chart.Titles.Add(chartTitle);
                chartPanel.Controls.Add(chart);
                cpuCoreCharts.Add(chart);
                minClocks.Add(float.MaxValue);
                maxClocks.Add(float.MinValue);
                lastClocks.Add(0);
                chartStartTimes.Add(DateTime.Now);

                // Overlay label to the right of the chart, values stacked vertically
                var infoLabel = new Label
                {
                    AutoSize = false,
                    Width = 110,
                    Height = 54,
                    TextAlign = System.Drawing.ContentAlignment.TopLeft,
                    ForeColor = System.Drawing.Color.Black,
                    BackColor = System.Drawing.Color.White,
                    Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold),
                    Left = chart.Left + chart.Width + labelOffset,
                    Top = chart.Top + chart.Height / 2 - 27,
                    Parent = chartPanel,
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(6, 2, 6, 2)
                };
                chartPanel.Controls.Add(infoLabel);
                chartOverlayLabelBlocks.Add(new ChartOverlayLabelBlock { InfoLabel = infoLabel });
            }
        }

        private void UpdateChartOverlayLabels()
        {
            for (int i = 0; i < chartOverlayLabels.Count; i++)
            {
                var overlay = chartOverlayLabels[i];
                var min = minClocks[i] == float.MaxValue ? float.NaN : minClocks[i];
                var max = maxClocks[i] == float.MinValue ? float.NaN : maxClocks[i];
                var last = lastClocks[i];
                overlay.MinLabel.Text = float.IsNaN(min) ? "Min : -" : $"Min : {min:F0}";
                overlay.MaxLabel.Text = float.IsNaN(max) ? "Max : -" : $"Max : {max:F0}";
                overlay.CurrentLabel.Text = float.IsNaN(last) ? "-" : $"{last:F0}";
                // Adjust position if chart moves or gets bigger
                var chart = cpuCoreCharts[i];
                overlay.MinLabel.Left = chart.Left + 10;
                overlay.MinLabel.Top = chart.Top + 5;
                overlay.MaxLabel.Left = chart.Left + 80;
                overlay.MaxLabel.Top = chart.Top + 5;
                overlay.CurrentLabel.Left = chart.Left + chart.Width - 60;
                overlay.CurrentLabel.Top = chart.Top + chart.Height / 2 - 10;
            }
        }

        private void UpdateChartOverlayLabelBlocks()
        {
            for (int i = 0; i < chartOverlayLabelBlocks.Count; i++)
            {
                var overlay = chartOverlayLabelBlocks[i];
                var min = minClocks[i] == float.MaxValue ? float.NaN : minClocks[i];
                var max = maxClocks[i] == float.MinValue ? float.NaN : maxClocks[i];
                var last = lastClocks[i];
                overlay.InfoLabel.Text = $"Min: {(float.IsNaN(min) ? "-" : min.ToString("F0"))}{Environment.NewLine}Max: {(float.IsNaN(max) ? "-" : max.ToString("F0"))}{Environment.NewLine}Current: {(float.IsNaN(last) ? "-" : last.ToString("F0"))}";
                // Always position directly to the right of the chart
                var chart = cpuCoreCharts[i];
                overlay.InfoLabel.Left = chart.Left + chart.Width;
                overlay.InfoLabel.Top = chart.Top + chart.Height / 2 - overlay.InfoLabel.Height / 2;
            }
        }

        // Draw overlay in the PostPaint event (ChartGraphics!)
        private void DrawChartOverlayPostPaint(ChartPaintEventArgs e, Chart chart, int coreIdx)
        {
            if (coreIdx < 0 || coreIdx >= minClocks.Count || coreIdx >= maxClocks.Count || coreIdx >= lastClocks.Count)
                return;
            var area = chart.ChartAreas[0];
            var g = e.ChartGraphics.Graphics;
            var font = new System.Drawing.Font("Segoe UI", 8, System.Drawing.FontStyle.Bold);
            var min = minClocks[coreIdx] == float.MaxValue ? float.NaN : minClocks[coreIdx];
            var max = maxClocks[coreIdx] == float.MinValue ? float.NaN : maxClocks[coreIdx];
            var last = lastClocks[coreIdx];

            // Coordinates in the chart control (not ChartGraphics!)
            var caRect = chart.ClientRectangle;
            float margin = 8f;
            float topY = caRect.Top + margin;
            float leftX = caRect.Left + margin;
            float rightX = caRect.Right - margin;

            // Min/Max at the top left
            var minStr = float.IsNaN(min) ? "Min : -" : $"Min : {min:F0}";
            var maxStr = float.IsNaN(max) ? "Max : -" : $"Max : {max:F0}";
            var minSize = g.MeasureString(minStr, font);
            g.DrawString(minStr, font, new System.Drawing.SolidBrush(System.Drawing.Color.Lime), leftX, topY);
            g.DrawString(maxStr, font, new System.Drawing.SolidBrush(System.Drawing.Color.Red), leftX + minSize.Width + 12, topY);

            // Current value on the right, vertically centered
            var valueStr = float.IsNaN(last) ? "-" : $"{last:F0}";
            var valueFont = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            var valueSize = g.MeasureString(valueStr, valueFont);
            float midY = caRect.Top + caRect.Height / 2 - valueSize.Height / 2;
            g.DrawString(valueStr, valueFont, System.Drawing.Brushes.White, rightX - valueSize.Width, midY);
        }

        private void BtnLoadSpecs_Click(object sender, EventArgs e)
        {
            lastPcInfo = hardwareService.GetAllPcInformation();
            txtOutput.Text = lastPcInfo != null ? FormatPcInformation(lastPcInfo) : "No data found.";
            if (lastPcInfo?.Cpu?.CoreClockRates != null)
            {
                CreateCpuCoreCharts(lastPcInfo.Cpu.CoreClockRates.Count);
                StartCpuClockUpdateTimer();
            }
        }

        private void BtnUpdateAllData_Click(object sender, EventArgs e)
        {
            lastPcInfo = hardwareService.GetAllPcInformation();
            txtOutput.Text = lastPcInfo != null ? FormatPcInformation(lastPcInfo) : "No data found.";
            if (lastPcInfo?.Cpu?.CoreClockRates != null)
            {
                CreateCpuCoreCharts(lastPcInfo.Cpu.CoreClockRates.Count);
                StartCpuClockUpdateTimer();
            }
        }

        private void StartCpuClockUpdateTimer()
        {
            if (cpuClockUpdateTimer != null)
            {
                cpuClockUpdateTimer.Stop();
                cpuClockUpdateTimer.Dispose();
            }
            cpuClockUpdateTimer = new Timer();
            cpuClockUpdateTimer.Interval = 1000; // 1 second
            cpuClockUpdateTimer.Tick += CpuClockUpdateTimer_Tick;
            cpuClockUpdateTimer.Start();
        }

        private void CpuClockUpdateTimer_Tick(object sender, EventArgs e)
        {
            var cpu = hardwareService.GetCpuDetails();
            if (cpu?.CoreClockRates != null && cpuCoreCharts.Count == cpu.CoreClockRates.Count)
            {
                for (int i = 0; i < cpuCoreCharts.Count; i++)
                {
                    var chart = cpuCoreCharts[i];
                    var series = chart.Series[0];
                    float val = cpu.CoreClockRates[i];
                    var startTime = chartStartTimes[i];
                    double seconds = (DateTime.Now - startTime).TotalSeconds;
                    // Rolling buffer: remove all points < (seconds-10)
                    while (series.Points.Count > 0 && series.Points[0].XValue < seconds - 10)
                        series.Points.RemoveAt(0);
                    series.Points.AddXY(seconds, val);
                    // Update min/max/last
                    if (minClocks[i] == float.MaxValue) minClocks[i] = val;
                    if (maxClocks[i] == float.MinValue) maxClocks[i] = val;
                    if (val < minClocks[i]) minClocks[i] = val;
                    if (val > maxClocks[i]) maxClocks[i] = val;
                    lastClocks[i] = val;
                    // X axis always 10s window
                    var area = chart.ChartAreas[0];
                    area.AxisX.Minimum = Math.Max(0, seconds - 10);
                    area.AxisX.Maximum = Math.Max(10, seconds);
                    chart.Invalidate();
                }
                UpdateChartOverlayLabels(); // Update labels
                UpdateChartOverlayLabelBlocks(); // Update labels
            }
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
                sb.AppendLine($"  Socket: {info.Cpu.Socket}");
                sb.AppendLine($"  L2 Cache: {info.Cpu.L2CacheSize} KB");
                sb.AppendLine($"  L3 Cache: {info.Cpu.L3CacheSize} KB");
                // Only show CPU package temperature
                if (info.Cpu.CoreTemperatures != null && info.Cpu.CoreTemperatures.Count > 0)
                {
                    sb.AppendLine($"  CPU Temperature: {info.Cpu.CoreTemperatures[0]:F1} °C");
                }
                else
                {
                    sb.AppendLine("  CPU Temperature: n/a");
                }
                // Show per-core clock rates
                if (info.Cpu.CoreClockRates != null && info.Cpu.CoreClockRates.Count > 0)
                {
                    for (int i = 0; i < info.Cpu.CoreClockRates.Count; i++)
                    {
                        sb.AppendLine($"  Core {i + 1} Clock: {info.Cpu.CoreClockRates[i]:F0} MHz");
                    }
                }
                else
                {
                    sb.AppendLine("  Core Clocks: n/a");
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

            // GPUs:
            sb.AppendLine("GPUs:");
            if (info.Gpus != null)
            {
                int i = 1;
                foreach (var gpu in info.Gpus)
                {
                    sb.AppendLine($"  GPU {i++}:");
                    sb.AppendLine($"    Name: {gpu.Name}");
                    sb.AppendLine($"    Driver: {gpu.DriverVersion}");
                    sb.AppendLine($"    Processor: {gpu.VideoProcessor}");
                    sb.AppendLine($"    CUDA: {(gpu.SupportsCuda ? "Yes" : "No")}");
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
                    // GPU Clock Rates
                    if (gpu.ClockRates != null && gpu.ClockRates.Count > 0)
                    {
                        foreach (var clk in gpu.ClockRates)
                        {
                            sb.AppendLine($"    {clk.Name} Clock: {clk.Value:F0} MHz");
                        }
                    }
                    else
                    {
                        sb.AppendLine("    GPU Clock: n/a");
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

