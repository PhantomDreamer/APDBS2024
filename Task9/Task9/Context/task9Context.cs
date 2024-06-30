using Microsoft.EntityFrameworkCore;
using Task9.Models;

namespace Task9.Context {
    public partial class task9Context : DbContext {
        public task9Context() {

        }
        public task9Context(DbContextOptions<task9Context> options) : base(options) {

        }


        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Prescription_Medicament> Prescription_Medicaments { get; set; }
    }
}
