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
        private Panel buttonPanel; // Neues Feld für das Button-Panel

        private List<float> minClocks = new List<float>();
        private List<float> maxClocks = new List<float>();
        private List<float> lastClocks = new List<float>();
        private List<DateTime> chartStartTimes = new List<DateTime>();

        public Form1()
        {
            InitializeComponent();
            InitializeCustomUi();
        }

        private void InitializeCustomUi()
        {
            // Entferne bisherige Controls, die nicht mehr gebraucht werden
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl != headerPanel)
                    this.Controls.Remove(ctrl);
            }

            // Panel für Buttons oben
            buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = System.Drawing.Color.Transparent
            };
            this.Controls.Add(buttonPanel);
            buttonPanel.BringToFront();

            // SplitContainer für links/rechts
            splitContainer = new SplitContainer
            {
                Orientation = Orientation.Vertical,
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent,
                FixedPanel = FixedPanel.None
            };
            this.Controls.Add(splitContainer);
            splitContainer.BringToFront();
            // 30% links, 70% rechts
            splitContainer.SplitterDistance = (int)(this.ClientSize.Width * 0.3);

            // Linke Seite: Textbox für Daten
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

            // Rechte Seite: Panel für Charts (mit eigenem Scrollbar)
            chartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = System.Drawing.Color.White
            };
            splitContainer.Panel2.Controls.Add(chartPanel);

            // Buttons im Button-Panel
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

            // Initial keine Charts
            CreateCpuCoreCharts(0);
        }

        private void CreateCpuCoreCharts(int coreCount)
        {
            // Entferne alte Charts
            foreach (var chart in cpuCoreCharts)
                chartPanel.Controls.Remove(chart);
            cpuCoreCharts.Clear();
            minClocks.Clear();
            maxClocks.Clear();
            lastClocks.Clear();
            chartStartTimes.Clear();

            int startY = 10;
            int chartHeight = 100;
            int chartWidth = 420;
            int left = 10;
            for (int i = 0; i < coreCount; i++)
            {
                var chart = new Chart
                {
                    Width = chartWidth,
                    Height = chartHeight,
                    Left = left,
                    Top = startY + i * (chartHeight + 30),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
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
                // Mehr Rand links/rechts
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
                chart.PostPaint += (s, e) => DrawChartOverlay(e, i);
                chartPanel.Controls.Add(chart);
                cpuCoreCharts.Add(chart);
                minClocks.Add(float.MaxValue);
                maxClocks.Add(float.MinValue);
                lastClocks.Add(0);
                chartStartTimes.Add(DateTime.Now);
            }
        }

        private void DrawChartOverlay(ChartPaintEventArgs e, int coreIdx)
        {
            if (coreIdx < 0 || coreIdx >= minClocks.Count || coreIdx >= maxClocks.Count || coreIdx >= lastClocks.Count)
                return;
            var chart = e.Chart;
            var area = chart.ChartAreas[0];
            var g = e.ChartGraphics.Graphics;
            var font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
            var min = minClocks[coreIdx];
            var max = maxClocks[coreIdx];
            var last = lastClocks[coreIdx];

            // Koordinaten im Plotbereich
            float right = area.Position.X + area.Position.Width;
            float bottom = area.Position.Y + area.Position.Height;
            var absTopLeft = e.ChartGraphics.GetAbsolutePoint(new System.Drawing.PointF(area.Position.X, area.Position.Y));
            var absTopRight = e.ChartGraphics.GetAbsolutePoint(new System.Drawing.PointF(right, area.Position.Y));
            // Min/Max links oben
            var minStr = $"Min : {min:F0}";
            var maxStr = $"Max : {max:F0}";
            var minSize = g.MeasureString(minStr, font);
            g.DrawString(minStr, font, System.Drawing.Brushes.Lime, absTopLeft.X + 5, absTopLeft.Y + 5);
            g.DrawString(maxStr, font, System.Drawing.Brushes.Red, absTopLeft.X + 15 + minSize.Width, absTopLeft.Y + 5);
            // Titel oben rechts
            var title = "CPU clock, MHz";
            var titleSize = g.MeasureString(title, font);
            g.DrawString(title, font, System.Drawing.Brushes.LightGray, absTopRight.X - titleSize.Width - 10, absTopRight.Y + 5);
            // Aktueller Wert rechts neben der Linie (letzter Punkt)
            var valueStr = $"{last:F0}";
            var valueFont = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            var valueSize = g.MeasureString(valueStr, valueFont);
            var series = chart.Series[0];
            if (series.Points.Count > 0)
            {
                var lastPoint = series.Points[series.Points.Count - 1];
                double xVal = lastPoint.XValue;
                double yVal = lastPoint.YValues[0];
                var plotX = (float)area.AxisX.ValueToPixelPosition(xVal);
                var plotY = (float)area.AxisY.ValueToPixelPosition(yVal);
                g.DrawString(valueStr, valueFont, System.Drawing.Brushes.White, plotX + 10, plotY - valueSize.Height / 2);
            }
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
            cpuClockUpdateTimer.Interval = 1000; // 1 Sekunde
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
                    // Rollender Buffer: Entferne alle Punkte < (seconds-10)
                    while (series.Points.Count > 0 && series.Points[0].XValue < seconds - 10)
                        series.Points.RemoveAt(0);
                    series.Points.AddXY(seconds, val);
                    // Min/Max/Last aktualisieren
                    if (val < minClocks[i]) minClocks[i] = val;
                    if (val > maxClocks[i]) maxClocks[i] = val;
                    lastClocks[i] = val;
                    // X-Achse immer 10s Fenster
                    var area = chart.ChartAreas[0];
                    area.AxisX.Minimum = Math.Max(0, seconds - 10);
                    area.AxisX.Maximum = Math.Max(10, seconds);
                    chart.Invalidate();
                }
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
                // Only show CPU Package temperature
                if (info.Cpu.CoreTemperatures != null && info.Cpu.CoreTemperatures.Count > 0)
                {
                    sb.AppendLine($"  CPU Temperature: {info.Cpu.CoreTemperatures[0]:F1} °C");
                }
                else
                {
                    sb.AppendLine("  CPU Temperature: n/a");
                }
                // NEW: Show per-core clock rates
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
                    // NEW: GPU Clock Rates
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

