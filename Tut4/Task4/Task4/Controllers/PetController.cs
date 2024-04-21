using Microsoft.AspNetCore.Mvc;
using Task4.Models;

namespace Task4.Controllers;

[Route("api/pets")]
[ApiController]
public class StudentsController : ControllerBase {
    private static readonly List<Pet> pets = new()
    {
        new Pet { IdPet = 1, Name = "Fluffy", Category = "Dog", weight = 38.32, coveringColour = "Brown" },
        new Pet { IdPet = 2, Name = "Ruffy", Category = "Cat", weight = 5.45, coveringColour = "Black" },
        new Pet { IdPet = 3, Name = "Wuffy", Category = "Snake", weight = 4.4, coveringColour = "Olive" }
    };

    [HttpGet]
    public IActionResult GetPets([FromQuery] String? sortBy, [FromQuery] String? filterBy, [FromQuery] String? search) {
        if(search != null) {
            Pet result = pets.Find(p => p.Name == search);
            if(result != null) {
                return Ok(result);
            }
            else {
                return NotFound($"Pet with name = " + search + " was not found");
            }
        }
        switch ((sortBy)) {
            case "weight_asc":
                List<Pet> petsSortedAsc = pets.OrderBy(p => p.weight).ToList();
                return Ok(petsSortedAsc);
            case "weight_desc":
                List<Pet> petsSortedDesc = pets.OrderByDescending(p => p.weight).ToList();
                return Ok(petsSortedDesc);
            case null:
                break;
            default:
                return BadRequest("Invalid query parameter.");
        }
        switch ((filterBy)) {
            case "dog":
                return Ok(pets.FindAll(p => p.Category == "Dog"));
            case "cat":
                return Ok(pets.FindAll(p => p.Category == "Cat"));
            case "snake":
                return Ok(pets.FindAll(p => p.Category == "Snake"));
            case null:
                break;
            default:
                return BadRequest("Invalid query parameter.");
        }
        return Ok(pets);
    }


    [HttpGet("{id:int}")]
    public IActionResult GetPet(int id) {
        var pet = pets.FirstOrDefault(s => s.IdPet == id);

        if (pet == null) {
            return NotFound($"Pet with ID = {id} was not found");
        }

        return Ok(pet);
    }

    [HttpPost]
    public IActionResult CreatePet(Pet pet) {
        pets.Add(pet);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdatePet(int id, Pet pet) {
        var edittedPet = pets.FirstOrDefault(s => s.IdPet == id);

        if (edittedPet == null) {
            return NotFound($"Pet with ID = {id} was not found");
        }

        pets.Remove(edittedPet);
        pets.Add(pet);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeletePet(int id) {
        var studentToEdit = pets.FirstOrDefault(s => s.IdPet == id);

        if (studentToEdit == null) {
            return NoContent();
        }

        pets.Remove(studentToEdit);
        return NoContent();
    }

}
