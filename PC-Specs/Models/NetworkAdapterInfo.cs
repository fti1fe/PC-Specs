namespace PC_Specs.Models
{
    /// <summary>
    /// Holds information about a single network adapter.
    /// </summary>
    public class NetworkAdapterInfo
    {
        // The name of the network adapter, for example: "Intel(R) Ethernet Connection"
        public string Name { get; set; }

        // The company that made the adapter
        public string Manufacturer { get; set; }

        // The MAC address
        public string MacAddress { get; set; }

        // The adapter type, for example: "Ethernet", "Wireless"
        public string AdapterType { get; set; }

        // (Optional) The IP address (IPv4 or IPv6)
        public string IpAddress { get; set; }
    }
}
