using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task9.Models {
    public class Prescription_Medicament {
        [Key]
        public int IdPMedicament { get; set; }
        public int IdMedicament { get; set; }
        public int IdPrescription { get; set; }
        public int Dose { get; set; }
        [Required]
        [MaxLength(100)]
        public string Details { get; set; }
        [ForeignKey(nameof(IdMedicament))]
        public Medicament Medicament { get; set; }
        [ForeignKey(nameof(IdPrescription))]
        public Prescription Prescription { get; set; }
    }
}
