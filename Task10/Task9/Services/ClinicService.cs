using Task9.Context;
using Task9.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Task9.DTO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.IdentityModel.Tokens.Jwt;

namespace Task9.Services {
    public class ClinicService {
        task9Context context = new task9Context();
        public bool PatientExists(int IdPatient) {
            return context.Patients.Any(x => x.IdPatient == IdPatient);
        }
        public bool MedicationExists(int IdMedicament) {
            if (!context.Medicaments.Any(x => x.IdMedicament == IdMedicament)) {
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
            foreach (Prescription_Medicament m in pms) {
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

        public User RegisterUser(UserDTO udto) {
            if (context.Users.Any(x => x.Login == udto.Login)) {
                throw new Exception($"Login already exists");
            }

            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: udto.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
                ));
            string saltBase64 = Convert.ToBase64String(salt);

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(randomNumber);
            }

            var user = new User {
                Login = udto.Login,
                Password = hashed,
                Salt = saltBase64,
                RefreshToken = Convert.ToBase64String(randomNumber),
                RefreshTokenTimer = DateTime.Now.AddHours(4)
            };
            context.Users.Add(user);

            return user;
        }

        public void LoginUser(UserDTO udto) {
            if (!context.Users.Any(x => x.Login == udto.Login)) {
                throw new Exception($"Login already exists");
            }

            User user = context.Users.Where(x => x.Login == udto.Login).First();
            byte[] saltBytes = Convert.FromBase64String(user.Salt);

            string UnhashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: user.Password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
                ));

            if (user.Password != UnhashedPassword) {
                throw new UnauthorizedAccessException("Authentication failure - wrong password");
            }

            //JwtSecurityToken token = new JwtSecurityToken
            //(
            //    issuer: "https://localhost:5001",
            //    audience: "https://localhost:5001",
            //    expires: DateTime.Now.AddMinutes(10),
            //    //signingCredentials: ,
            //    //claims: 
            //);

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(randomNumber);
            }
            user.RefreshToken = Convert.ToBase64String(randomNumber);
            user.RefreshTokenTimer = DateTime.Now.AddHours(4);

            User old = context.Users.Where(x => x.Login == udto.Login).First();
            context.Entry(old).CurrentValues.SetValues(user);
        }

        public void updateRefresh() {



        }
    }
}
