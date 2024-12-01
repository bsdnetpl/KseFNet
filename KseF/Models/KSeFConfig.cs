using System.ComponentModel.DataAnnotations;

namespace KseF.Models
    {
    public class KSeFConfig
        {
        [Required]
        public string ApiUrl { get; set; } = "";

        [Required]
        public string Nip { get; set; } = "";

        [Required]
        public string ApiKey { get; set; } = "";

        [Required]
        public string PublicKeyPath { get; set; } = "";
        }
    }
