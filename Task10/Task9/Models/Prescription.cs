using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task9.Models {
    public class Prescription {
        [Key]
        public int IdPrescription { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public int IdDoctor { get; set; }
        [ForeignKey(nameof(IdDoctor))]
        public Doctor Doctor { get; set; }
        public int IdPatient { get; set; }
        [ForeignKey(nameof(IdPatient))]
        public Patient Patient { get; set; }


        public ICollection<Prescription_Medicament> Prescription_Medicaments { get; set; } = new HashSet<Prescription_Medicament>();
    }
}
