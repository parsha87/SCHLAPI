using System.Collections.Generic;

namespace Scheduling.ViewModels
{
    public class FirmwareUpdateViewModel
    {
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Firmware
    {
        public string section { get; set; }
        public string version { get; set; }
        public string fu { get; set; }
        public string size { get; set; }
        public string crc { get; set; }
    }

    public class MyJsonDatum
    {
        public TinyN TinyN { get; set; }
    }

    public class RootFirmware
    {
        public string Error_ID { get; set; }
        public string Packet_ID { get; set; }
        public string Sub_Type { get; set; }
        public string Device_Type { get; set; }
        public string MyData { get; set; }
        public List<MyJsonDatum> MyJsonData { get; set; }
    }

    public class SectionFirmware
    {
        public List<Firmware> firmwares { get; set; }
    }

    public class TinyN
    {
        public string totalSections { get; set; }
        public string systemVersion { get; set; }
        public SectionFirmware sectionFirmware { get; set; }
    }


}
