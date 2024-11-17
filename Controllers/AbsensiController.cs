using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class AbsensiController : Controller
{
    private readonly AppDbContext _context;
    public AbsensiController(AppDbContext context)
    {
        _context = context;
    }

    //data gabungan employee dan absen (nomor 2)
    public IActionResult Index()
    {
        var data = _context.Absens
                .Include(a => a.Employee)
                .Select(a => new Dictionary<string, object>
                {
                    { "Nik", a.Employee.Nik },
                    { "Name", a.Employee.Name },
                    { "TanggalAbsen", a.Tanggal_Absen }
                }).ToList();
        return View(data);
    }

    //Form Tambah Data Absen (nomor 1)
    public IActionResult Create()
    {
        var data = _context.Employees
            .Select(a => new Dictionary<string, object>
                {
                    { "Id", a.Id },
                    { "Nik", a.Nik },
                    { "Name", a.Name }
                }).ToList();;
        return View(data);
    }
    [HttpPost]
    public IActionResult Create(int employeeId, DateTime tanggalAbsen)
    {
        var absen = new Absen
        {
            Employee_Id = employeeId,
            Tanggal_Absen = tanggalAbsen
        };

        _context.Absens.Add(absen);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }


    // data absensi pertanggal (nomor 3)
    public ActionResult perTanggal()
{
    var data = _context.Absens
                .GroupBy(a => a.Employee_Id)
                .Select(g => new 
                {
                    Employee = g.FirstOrDefault().Employee,
                    Absensi = g.Select(a => a.Tanggal_Absen).ToList()
                }).ToList();

    var dates = _context.Absens.Select(a => a.Tanggal_Absen).Distinct().OrderBy(d => d).ToList();

    var result = new List<Dictionary<string, object>>();

    foreach (var d in data)
    {
        var absenList = new List<string>();
        int total = 0;

        //cek jika absensi ada di tanggal
        for (int i = 0; i < dates.Count; i++)
        {
            if (d.Absensi.Contains(dates[i]))
            {
                absenList.Add("X");
                total++;
            }
            else
            {
                absenList.Add("");
            }
        }

        var employeeData = new Dictionary<string, object>
        {
            { "nik", d.Employee.Nik },
            { "name", d.Employee.Name },
            { "absensi", absenList },
            { "total", total }
        };

        result.Add(employeeData);
    }

    ViewBag.Dates = dates;
    return View(result);
}

//absensi perbulan (nomor 4)
public ActionResult PerBulan()
{
    var absens = _context.Absens
                    .Include(a => a.Employee) 
                    .ToList();

    var data = absens
                .GroupBy(a => a.Employee_Id)
                .Select(g => new
                {
                    Employee = g.FirstOrDefault().Employee,
                    Absensi = g.GroupBy(a => a.Tanggal_Absen.ToString("yyyyMM"))
                                .Select(monthGroup => new
                                {
                                    Month = monthGroup.Key,
                                    Count = monthGroup.Count()
                                }).ToList()
                }).ToList();

    var months = absens
                .Select(a => a.Tanggal_Absen.ToString("yyyyMM"))
                .Distinct()
                .OrderBy(m => m)
                .ToList();

    var result = new List<Dictionary<string, object>>();

    foreach (var d in data)
    {
        var absensiPerBulan = new List<int>();
        int total = 0;

        foreach (var month in months)
        {
            var count = d.Absensi.FirstOrDefault(a => a.Month == month)?.Count ?? 0;
            absensiPerBulan.Add(count);
            total += count;
        }

        var employeeData = new Dictionary<string, object>
        {
            { "nik", d.Employee.Nik },
            { "name", d.Employee.Name },
            { "absensi", absensiPerBulan },
            { "total", total }
        };

        result.Add(employeeData);
    }

    ViewBag.Months = months;
    return View(result);
}

}
