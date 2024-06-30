using Task9.Context;
using Task9.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Task9.Services {
    public class ClinicService {
        task9Context context = new task9Context();
        public bool PatientExists(int IdPatient) {
            return context.Patients.Any(x => x.IdPatient == IdPatient);
        }
        public bool MedicationExists(int IdMedicament) {
            if(!context.Medicaments.Any(x => x.IdMedicament == IdMedicament)) {
                throw new Exception($"There is no such medication");
            }
            return true;
        }

        public bool CheckCount(Prescription p) {
            if (p.Prescription_Medicaments.Count > 10) {
                throw new Exception($"A prescription cannot contain more than 10 medicaments");
            }
            return true;
        }

        public bool CreatePatient(Patient pat) {
            if (PatientExists(pat.IdPatient)) {
                return true;
            }

            context.Patients.Add(pat);
            return false;
        }

        public Doctor GetDoctorData(int IdDoctor) {
            return context.Doctors.Where(x => x.IdDoctor == IdDoctor).First();
        }

        public Prescription CreatePrescription(List<Prescription_Medicament> pms, DateTime date, DateTime dueDate, Doctor doc, Patient pat) {
            if (PatientExists(pat.IdPatient)) {
                CreatePatient(pat);
            }
            foreach(Prescription_Medicament m in pms) {
                MedicationExists(m.IdMedicament);
            }
            if (pat == null || doc == null || pms == null) {
                throw new Exception($"Nulls are not allowed for the parameters");
            }
            else if (date > dueDate) {
                throw new Exception($"The date cannot be higher than due date");
            }
            else if (pms.Count > 10) {
                throw new Exception($"You cannot have more than 10 medications");
            }
            var pres = new Prescription {
                IdDoctor = doc.IdDoctor,
                IdPatient = pat.IdPatient,
                DueDate = dueDate,
                Date = date
            };
            return pres;
        }

        public IEnumerable<Patient> GetPatient(int IdPatient) {
            if (!PatientExists(IdPatient)) {
                throw new Exception($"Such patient does not exist");
            }
            var patient = context.Patients.Include(p => p.Prescriptions).ThenInclude(pr => pr.Doctor)
                .Include(p => p.Prescriptions).ThenInclude(pr => pr.Prescription_Medicaments)
                .Include(p => p.Prescriptions).ThenInclude(pr => pr.Prescription_Medicaments).ThenInclude(pm => pm.Medicament)
                .Where(p => p.IdPatient == IdPatient);

            return patient;
        }


    }
}
