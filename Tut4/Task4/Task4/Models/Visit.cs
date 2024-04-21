namespace Task4.Models;
public class Visit {
    public int IdVisit { get; set; }
    public DateTime dov { get; set; }
    public Pet pet { get; set; } = null!;
    public string desc { get; set; } = null!;
    public double price { get; set; }
}

