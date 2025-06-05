namespace PC_Specs.Models
{
    /// <summary>
    /// Holds basic information about the operating system.
    /// </summary>
    public class OperatingSystemInfo
    {
        // Full name, e.g. "Microsoft Windows 11 Pro"
        public string Caption { get; set; }

        // Version number
        public string Version { get; set; }

        // OS architecture, e.g. "64-Bit"
        public string OSArchitecture { get; set; }
    }
}
