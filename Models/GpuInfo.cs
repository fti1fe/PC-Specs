using System.Collections.Generic;

namespace PC_Specs.Models
{
    /// <summary>
    /// Holds information about a single GPU.
    /// </summary>
    public class GpuInfo
    {
        // Name of the GPU, e.g. "NVIDIA GeForce RTX 3070"
        public string Name { get; set; }

        // Installed driver version
        public string DriverVersion { get; set; }

        // Name of the video processor
        public string VideoProcessor { get; set; }

        // Indicates if CUDA is supported
        public bool SupportsCuda { get; set; }

        // List of available temperature sensors (name + value)
        public List<(string Name, float Value)> Temperatures { get; set; }
    }
}