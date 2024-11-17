public class Employee
{
    public int Id { get; set; }
    public string Nik { get; set; }
    public string Name { get; set; }

    public virtual ICollection<Absen> Absens { get; set; }
}
