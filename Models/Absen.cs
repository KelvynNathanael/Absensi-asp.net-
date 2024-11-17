public class Absen
{
    public int Id { get; set; }
    public int Employee_Id { get; set; }
    public DateTime Tanggal_Absen { get; set; }

    public virtual Employee Employee { get; set; }
}
