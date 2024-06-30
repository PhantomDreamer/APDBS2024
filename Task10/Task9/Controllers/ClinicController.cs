using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task9.DTO;
using Task9.Models;
using Task9.Services;

namespace Task9.Controllers {
[Route("api/clinicController")]
[ApiController]
public class ClinicController : Controller {
        ClinicService cs = new ClinicService();


        public ClinicController(ClinicService _cs) {
            cs = _cs;
        }

        [HttpGet("{patientId}")]
        public IActionResult GetPatient(int patientId) {
            var patientDetails =  cs.GetPatient(patientId);
            return Ok(patientDetails);
        }
        [Route("Prescription")]
        [HttpPost]
        public IActionResult CreatePrescription(PrescriptionDTO pdto) {
            var prescriptionDetails = cs.CreatePrescription(pdto.pms, pdto.date, pdto.dueDate, pdto.doc, pdto.pat);
            return Ok(prescriptionDetails);
        }
        [HttpPost("register")]
        public IActionResult RegisterUser(UserDTO udto) {
            User user = cs.RegisterUser(udto);
            return Ok(user);
        }
    }
}
