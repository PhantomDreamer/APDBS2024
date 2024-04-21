using Microsoft.AspNetCore.Mvc;
using Task4.Models;

namespace Task4.Controllers;

[Route("api/visits")]
[ApiController]
public class VisitController : Controller {
    private static readonly List<Visit> visits = new() {
        new Visit { IdVisit = 1, dov = new DateTime(2008, 3, 9, 1, 30, 0), pet = new Pet { IdPet = 1, Name = "Fluffy", Category = "Dog", weight = 38.32, coveringColour = "Brown" }, desc = "dawfesdgdfgdf", price = 235.32 },
        new Visit { IdVisit = 2, dov = DateTime.Now, pet = new Pet { IdPet = 2, Name = "Ruffy", Category = "Cat", weight = 5.45, coveringColour = "Black" }, desc = "dwafdgfds", price = 42.32 },
        new Visit { IdVisit = 3, dov = DateTime.UtcNow, pet = new Pet { IdPet = 2, Name = "Ruffy", Category = "Cat", weight = 5.45, coveringColour = "Black" }, desc = "wfadfdgfd", price = 5243.32 }
    };
    
    [HttpGet]
    public IActionResult GetVisits() {
        return Ok(visits);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetPetVisit(int id) {
        List<Visit> petVisits = new();

        foreach(Visit vis in visits){
            if(vis.pet.IdPet == id) {
                petVisits.Add(vis);
            }
        }

        if (!petVisits.Any()) {
            return NotFound($"Pet with ID = {id} has no associated visits");
        }

        return Ok(petVisits);
    }

    [HttpPost]
    public IActionResult CreateVisit(Visit vis) {
        visits.Add(vis);
        return StatusCode(StatusCodes.Status201Created);
    }

}
