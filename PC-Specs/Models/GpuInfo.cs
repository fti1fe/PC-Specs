using System.Collections.Generic;

namespace PC_Specs.Models
{
    /// <summary>
    /// Holds information about a single GPU.
    /// </summary>
    public class GpuInfo
    {
        // The name of the GPU, for example: "NVIDIA GeForce RTX 3070"
        public string Name { get; set; }

        // The installed driver version
        public string DriverVersion { get; set; }

        // The name of the video processor
        public string VideoProcessor { get; set; }

        // Shows if CUDA is supported
        public bool SupportsCuda { get; set; }

        // List of all temperature sensors (name and value)
        public List<(string Name, float Value)> Temperatures { get; set; }

        // List of all GPU clock rates (name and value), for example: Core, Memory, Shader, etc.
        public List<(string Name, float Value)> ClockRates { get; set; }
    }
}


