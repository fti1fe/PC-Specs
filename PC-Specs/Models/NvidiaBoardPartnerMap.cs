using System.Collections.Generic;

namespace PC_Specs.Models
{
    /// <summary>
    /// Mapping of PCI Subsystem Vendor IDs to board partner names for NVIDIA GPUs
    /// </summary>
    public static class NvidiaBoardPartnerMap
    {
        private static readonly Dictionary<uint, string> VendorMap = new Dictionary<uint, string>
        {
            { 0x1043, "ASUS" },
            { 0x1458, "Gigabyte" },
            { 0x1462, "MSI" },
            { 0x3842, "EVGA" },
            { 0x196E, "PNY" },
            { 0x1569, "Palit" },
            { 0x19DA, "Zotac" },
            { 0x1682, "XFX" },
            { 0x148C, "PowerColor" },
            { 0x1DA2, "Sapphire" },
            { 0x10DE, "NVIDIA (Founders Edition)" },
            { 0x1849, "ASRock" },
            { 0x7377, "Colorful" },
            { 0x19F1, "BFG Tech" },
            { 0x1B4C, "KFA2/Galax" },
            { 0x154B, "PNY" },
            { 0x1ACC, "Point of View" },
            { 0x10B0, "Gainward" },
            { 0x1642, "Bitland" },
            { 0x1B13, "Leadtek" },
            { 0x107D, "Leadtek" }
        };

        public static string GetBoardPartnerName(uint? subVendorId)
        {
            if (!subVendorId.HasValue)
                return "Unknown";

            if (VendorMap.TryGetValue(subVendorId.Value, out string vendor))
                return vendor;

            // Fallback: try to resolve vendor name for common IDs
            switch (subVendorId.Value)
            {
                case 0x10DE:
                    return "NVIDIA (Founders Edition)";
                case 0x1043:
                    return "ASUS";
                case 0x1458:
                    return "Gigabyte";
                case 0x1462:
                    return "MSI";
                case 0x3842:
                    return "EVGA";
                case 0x196E:
                case 0x154B:
                    return "PNY";
                case 0x19DA:
                    return "Zotac";
                case 0x1569:
                    return "Palit";
                case 0x1B4C:
                    return "KFA2/Galax";
                case 0x1849:
                    return "ASRock";
                case 0x7377:
                    return "Colorful";
                case 0x19F1:
                    return "BFG Tech";
                case 0x1ACC:
                    return "Point of View";
                case 0x10B0:
                    return "Gainward";
                case 0x1642:
                    return "Bitland";
                case 0x1B13:
                case 0x107D:
                    return "Leadtek";
                case 0x148C:
                    return "PowerColor";
                case 0x1DA2:
                    return "Sapphire";
                case 0x1682:
                    return "XFX";
                default:
                    return $"Unknown (0x{subVendorId.Value:X4})";
            }
        }
    }
}
