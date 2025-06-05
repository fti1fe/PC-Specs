namespace PC_Specs.Models
{
    /// <summary>
    /// Holds information about a single network adapter.
    /// </summary>
    public class NetworkAdapterInfo
    {
        // Name of the network adapter, e.g. "Intel(R) Ethernet Connection"
        public string Name { get; set; }

        // Manufacturer of the adapter
        public string Manufacturer { get; set; }

        // MAC address
        public string MacAddress { get; set; }

        // Adapter type, e.g. "Ethernet", "Wireless"
        public string AdapterType { get; set; }

        // Optional: IP address (IPv4 or IPv6)
        public string IpAddress { get; set; }
    }
}
