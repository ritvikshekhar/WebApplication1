using System.ComponentModel.DataAnnotations;

namespace project.data
{
    public class DeviceData
    {
        public int DeviceID { get; set; }

        [Required]
        public string SerialNumber { get; set; }

        [Required]
        public string DeviceName { get; set; }

        [Required]
        public string SupportLabelIdentifier { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
