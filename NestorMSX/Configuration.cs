using System.Collections.Generic;

namespace Konamiman.NestorMSX
{
    /// <summary>
    /// Configuration to be passed to MsxEmulationEnvironment.
    /// </summary>
    public class Configuration
    {
        public string KeymapFile { get; set; }

        public string ColorsFile { get; set; }

        public decimal CpuSpeedInMHz { get; set; }

        public decimal VdpFrequencyMultiplier { get; set; }

        public decimal DisplayZoomLevel { get; set; }

        public int HorizontalMarginInPixels { get; set; }

        public int VerticalMarginInPixels { get; set; }

        public IDictionary<string, object> GlobalPluginsConfig { get; set; }

        public IDictionary<string, object> SharedPluginsConfig { get; set; }

        public int VramSizeInKb { get; set; }
    }
}
