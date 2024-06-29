using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Task5.Models;

namespace Task5.Controllers {
    [Route("api/animals")]
    [ApiController]
    public class AnimalsController : ControllerBase {
        [HttpGet]
        public IActionResult GetAnimals([FromQuery] string orderBy = "name") {
            var validParams = new List<string> { "name", "description", "category", "area" };

            if (!validParams.Contains(orderBy.ToLower())) {
                return BadRequest("Invalid parameter");
            }

            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();

            using var connection = new SqlConnection("Data Source=db-mssql16.pjwstk.edu.pl;Integrated Security=SSPI;");
            connection.Open();

            using var cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT IdAnimal, Name, Description, Category, Area FROM Animals ORDER BY @orderBy ASC";
            cmd.Parameters.AddWithValue("@orderBy", orderBy);

            var dr = cmd.ExecuteReader();
            var animals = new List<Animal>();

            while (dr.Read()) {
                var animal = new Animal {
                    IdAnimal = (int)dr["IdAnimal"],
                    Name = dr.IsDBNull(1) ? null : dr["Name"].ToString(),
                    Description = dr["Description"].ToString(),
                    Category = dr.IsDBNull(3) ? null : dr["Category"].ToString(),
                    Area = dr.IsDBNull(4) ? null : dr["Area"].ToString()
                };
                animals.Add(animal);
            }
            cmd.Dispose();
            connection.Dispose();

            return Ok(animals);
        }

        [HttpPost]
        public IActionResult CreateAnimal(Animal animal) {
            using var connection = new SqlConnection("ConnectionStrings:DefaultConnection");
            connection.Open();

            using var cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO Animal(Name, Description, Category, Area) VALUES(@Name, @Description, @Category, @Area)";
            cmd.Parameters.AddWithValue("@Name", animal.Name);
            cmd.Parameters.AddWithValue("@Description", animal.Description);
            cmd.Parameters.AddWithValue("@Category", animal.Category);
            cmd.Parameters.AddWithValue("@Area", animal.Area);

            cmd.Dispose();
            connection.Dispose();

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{idAnimal:int}")]
        public IActionResult UpdateAnimal(int idAnimal, Animal animal) {
            if(idAnimal != animal.IdAnimal) {
                return BadRequest("The ID of the specified animal and the data do not match");
            }
            using var connection = new SqlConnection("ConnectionStrings:DefaultConnection");
            connection.Open();

            using var cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE Animal SET Name=@Name, Description=@Description, Category=@Category, Area=@Area WHERE IdAnimal = @IdAnimal";
            cmd.Parameters.AddWithValue("@IdAnimal", animal.IdAnimal);
            cmd.Parameters.AddWithValue("@Name", animal.Name);
            cmd.Parameters.AddWithValue("@Description", animal.Description);
            cmd.Parameters.AddWithValue("@Category", animal.Category);
            cmd.Parameters.AddWithValue("@Area", animal.Area);

            cmd.Dispose();
            connection.Dispose();

            return NoContent();
        }

        [HttpDelete("{idAnimal:int}")]
        public IActionResult DeletePet(int idAnimal) {
            using var connection = new SqlConnection("ConnectionStrings:DefaultConnection");
            connection.Open();

            using var cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "DELETE FROM Animal WHERE IdAnimal = @IdAnimal";
            cmd.Parameters.AddWithValue("@IdAnimal", idAnimal);

            cmd.Dispose();
            connection.Dispose();

            return NoContent();
        }
    }
}
