using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Data.SqlClient;
using Task6.Models;

namespace Task6.Controllers {
    [Route("api/WarehouseController")]
    [ApiController]
    public class WarehouseController : ControllerBase {

        [HttpPost]
        public IActionResult CreateProductWH(ProductWH p) { //I'm assuming we cannot use Entity Framework for this task, so it might look stragne
            if (p.Amount == 0) {
                return BadRequest("The product amount cannot be 0");
            }

            using var connection = new SqlConnection("Data Source=db-mssql16.pjwstk.edu.pl;Integrated Security=SSPI");          //I'm unable to connect to the school database due to some techincal issue, but it was tested on a local database
            connection.Open();

            using var checkproduct = new SqlCommand("SELECT * FROM PRODUCT WHERE IdProduct = @IdProduct");
            checkproduct.Parameters.AddWithValue("@IdProduct", p.IdProduct);
            checkproduct.Connection = connection;
            var dr = checkproduct.ExecuteReader();
            if (!dr.HasRows) {
                return NotFound("Such product does not exist");
            }
            dr.Close();

            checkproduct.Dispose();

            using var checkwarehouse = new SqlCommand("SELECT * FROM WAREHOUSE WHERE IdWarehouse = @IdWarehouse");
            checkwarehouse.Parameters.AddWithValue("@IdWarehouse", p.IdWarehouse);
            checkwarehouse.Connection = connection;
            dr = checkwarehouse.ExecuteReader();
            if (!dr.HasRows) {
                return NotFound("Such warehouse doesn't exist");
            }

            checkproduct.Dispose();

            using var checkorder = new SqlCommand("SELECT * FROM ORDER WHERE Amount = @Amount AND IdProduct = @IdProduct");
            checkorder.Parameters.AddWithValue("@IdProduct", p.IdProduct);
            checkorder.Parameters.AddWithValue("@Amount", p.Amount);
            checkorder.Connection = connection;
            dr = checkorder.ExecuteReader();
            DateTime productdate = new DateTime();
            string idorder = "";

            if (dr.HasRows) {
                while (dr.Read()) {
                    idorder = dr["IdOrder"].ToString();
                    productdate = Convert.ToDateTime(dr["CreatedAt"].ToString());
                }
                if (DateTime.Compare((p.CreatedAt), productdate) <= 0) {
                    return Conflict("Date of the order is higher than the date of the request");
                }
            }
            else {
                return NotFound("The order does not exist");
            }

            checkorder.Dispose();

            using var checkcomp = new SqlCommand("SELECT * FROM Product_Warehouse WHERE IdOrder = @IdOrder");
            checkcomp.Parameters.AddWithValue("@IdOrder", idorder);
            checkcomp.Connection = connection;
            dr = checkcomp.ExecuteReader();
            if (dr.HasRows) {
                return BadRequest("This order has already been completed");
            }

            checkcomp.Dispose();

            DbTransaction tran1 = connection.BeginTransaction();
            using var updt = new SqlCommand("UPDATE ORDER SET FulfilledAt = '@FulfilledAt' WHERE IdOrder = @IdOrder");
            updt.Parameters.AddWithValue("@IdOrder", idorder);
            updt.Parameters.AddWithValue("@FulfilledAt", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"));
            updt.Transaction = (SqlTransaction)tran1;
            try {
                updt.Connection = connection;
                dr = updt.ExecuteReader();
                tran1.Commit();
            }
            catch (SqlException e) {
                tran1.Rollback();
                return Conflict("Error during the transaction");
            }

            updt.Dispose();

            using var getprice = new SqlCommand("SELECT Price FROM Product WHERE IdProduct = @IdProduct");
            getprice.Parameters.AddWithValue("@IdProduct", p.IdProduct);
            getprice.Connection = connection;
            dr = getprice.ExecuteReader();
            double price = 0;
            while (dr.Read()) {
                price = Convert.ToDouble(dr["Price"]);
            }

            getprice.Dispose();

            DbTransaction tran2 = connection.BeginTransaction();
            using var ins = new SqlCommand("INSERT INTO Product_Warehouse(IdWarehouse ,IdProduct, IdOrder, Amount, Price, CreatedAt) VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @WHPrice, '@CreatedAt');");
            String dateCreated = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            ins.Parameters.AddWithValue("@IdWarehouse", p.IdWarehouse);
            ins.Parameters.AddWithValue("@IdProduct", p.IdProduct);
            ins.Parameters.AddWithValue("@IdOrder", idorder);
            ins.Parameters.AddWithValue("@Amount", p.Amount);
            ins.Parameters.AddWithValue("@WHPrice", (price * p.Amount));
            ins.Parameters.AddWithValue("@CreatedAt", dateCreated);
            ins.Transaction = (SqlTransaction)tran2;
            try {
                ins.Connection = connection;
                dr = ins.ExecuteReader();
                tran2.Commit();
            }
            catch (SqlException e) {
                tran2.Rollback();
                return Conflict("Error during the transaction");
            }

            ins.Dispose();
            using var maxid = new SqlCommand("SELECT IdProductWarehouse FROM Product_Warehouse WHERE CreatedAt = @Created");
            maxid.Connection = connection;

            dr = maxid.ExecuteReader();
            int id = Convert.ToInt32(dr["IdProductWarehouse"]);

            maxid.Dispose();
            connection.Dispose();

            return Ok(id);
        }
        [Route("PostProcedure")]
        [HttpPost]
        public IActionResult CreateProductFromProcedure(ProductWH p) {
            using (var con = new SqlConnection("Data Source=db-mssql16.pjwstk.edu.pl;Integrated Security=SSPI")) {
                using (var com = new SqlCommand("AddProductToWarehouse", con)) {
                    com.Parameters.AddWithValue("@IdProduct", p.IdProduct);
                    com.Parameters.AddWithValue("@IdWarehouse", p.IdWarehouse);
                    com.Parameters.AddWithValue("@Amount", p.Amount);
                    com.Parameters.AddWithValue("@CreatedAt", p.CreatedAt);

                    con.Open();;
                    var dr = com.ExecuteReader();

                    con.Dispose();
                    return Ok();
                }
            }
        }
    }
}
