using System.ComponentModel.DataAnnotations;
using Task9.Models;

namespace Task9.DTO {
    public class PrescriptionDTO {
        [Required]
        public List<Prescription_Medicament> pms { get; set; }
        [Required]
        public DateTime date { get; set; }
        [Required]
        public DateTime dueDate { get; set; }
        [Required]
        public Doctor doc { get; set; }
        [Required]
        public Patient pat { get; set; }
    }
}
