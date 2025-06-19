namespace PC_Specs.Models
{
    /// <summary>
    /// Holds basic information about the operating system.
    /// </summary>
    public class OperatingSystemInfo
    {
        // The full name, for example: "Microsoft Windows 11 Pro"
        public string Caption { get; set; }

        // The version number
        public string Version { get; set; }

        // The OS architecture, for example: "64-Bit"
        public string OSArchitecture { get; set; }
    }
}
