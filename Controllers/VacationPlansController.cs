using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacationManagement.Data;
using VacationManagement.Models;

namespace VacationManagement.Controllers
{
    public class VacationPlansController : Controller
    {
        VacationDbContext _context;
        public VacationPlansController(VacationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.RequestVacations
                .Include(x => x.Employee)
                .Include(x => x.VacationType)
                .OrderByDescending(x => x.RequestData)
                .ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(VacationPlan model, int[] DayOfWeekCheckBox)
        {
            if (ModelState.IsValid)
            {
                var result = _context.VacationPlans
                    .Where(x => x.RequestVacation.EmployeeId == model.RequestVacation.EmployeeId
                    && x.VacationDate >= model.RequestVacation.StartDate
                    && x.VacationDate <= model.RequestVacation.EndDate).FirstOrDefault();
                if (result != null)
                {
                    ViewBag.ErrorVacation = false;
                    return View(model);
                }
                for (DateTime date = model.RequestVacation.StartDate;
                    date <= model.RequestVacation.EndDate; date = date.AddDays(1))
                {
                    /*
                     *DayOfWeekCheckBox  : ايام الدوام اللي علمت عليها
                     *(int)date.DayOfWeek : ايام الاجازة اللي حددتها
                     *IndexOf : لو ايام الاجازة موجوده في ايام الدوام هيرجع 1
                     *IndexOf : لو مش موجودة هيرجع -1 فمش هحفظ اليوم
                     */
                    if (Array.IndexOf(DayOfWeekCheckBox, (int)date.DayOfWeek) != -1)
                    {
                        model.Id = 0;
                        model.VacationDate = date;
                        model.RequestVacation.RequestData = DateTime.Now;
                        _context.VacationPlans.Add(model);
                        _context.SaveChanges();

                    }
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Edit(int? Id)
        {
            ViewBag.Employees = _context.Employees.OrderBy(x => x.Name).ToList();
            ViewBag.VacationType = _context.VacationTypes.OrderBy(x => x.VacationName).ToList();
            return View(_context.RequestVacations.Include(x => x.Employee)
                .Include(x => x.VacationType)
                .Include(x => x.VacationPlanList).FirstOrDefault(x => x.Id == Id));
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(RequestVacation model)
        {
            if (ModelState.IsValid)
            {
                if (model.Approved == true)
                {
                    model.DateApproved = DateTime.Now;
                    _context.RequestVacations.Update(model);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Employees = _context.Employees.OrderBy(x => x.Name).ToList();
            ViewBag.VacationType = _context.VacationTypes.OrderBy(x => x.VacationName).ToList();
            return View(model);
        }
        public IActionResult GetCountVacationEmployee (int? Id)
        {
            return Json(_context.VacationPlans.Where(x=>x.RequestVacationId==Id).Count());
        }
        public IActionResult GetVacationTypes(int? id)
        {
            return Json(_context.VacationTypes.OrderBy(x => x.Id).ToList());
        }
        public IActionResult Delete(int? id)
        {
            return View(_context.RequestVacations
                .Include(x => x.Employee)
                .Include(x => x.VacationType)
                .Include(x => x.VacationPlanList)
                .FirstOrDefault(x => x.Id == id));
        }
        [HttpPost]

        public IActionResult Delete(RequestVacation model)
        {
            if (model != null)
            {
                _context.RequestVacations.Remove(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public IActionResult ViewReportVacationPlan()
        {
            ViewBag.Employees = _context.Employees.ToList();
            return View();
        }
        public IActionResult ViewReportVacationPlan2()
        {
            ViewBag.Employees = _context.Employees.ToList();
            return View();
        }
        public IActionResult GetReportVacationPlan
            (int EmployeeId , DateTime FromDate , DateTime ToDate)
        {
            string Id = "";
            if (EmployeeId != 0 && EmployeeId.ToString() != "" )
            {
                Id = $"and Employees.Id = {EmployeeId}";
            }
            //var sqlQuery = _context.SqlDataTable($@"SELECT     dbo.Employees.Id, dbo.Employees.Name, dbo.Employees.VacationBalance,
            //         Sum(dbo.VacationTypes.NumberDays) AS TotalVacation,
            //         dbo.Employees.VacationBalance-Sum(dbo.VacationTypes.NumberDays) AS Total
            //               FROM        dbo.Employees INNER JOIN
            //               dbo.RequestVacations ON dbo.Employees.Id = dbo.RequestVacations.EmployeeId INNER JOIN
            //               dbo.VacationPlans    ON dbo.RequestVacations.Id = dbo.VacationPlans.RequestVacationId INNER JOIN
            //               dbo.VacationTypes    ON dbo.RequestVacations.VacationTypeId = dbo.VacationTypes.Id
            //      where dbo.VacationPlans.VacationDate between'"+FromDate.ToString("yyyy-MM-dd")+"'and '" + ToDate.ToString("yyyy-MM-dd") + "' "+ 
            //               " and RequestVacations.Approved ='True' " +
            //               $"{Id}group by  dbo.Employees.Id, dbo.Employees.Name, dbo.Employees.VacationBalance");

            string sqlQuery = $@"SELECT     dbo.Employees.Id, dbo.Employees.Name, dbo.Employees.VacationBalance,
		                   Sum(dbo.VacationTypes.NumberDays) AS TotalVacation,
		                   dbo.Employees.VacationBalance-Sum(dbo.VacationTypes.NumberDays) AS Total
                           FROM        dbo.Employees INNER JOIN
                           dbo.RequestVacations ON dbo.Employees.Id = dbo.RequestVacations.EmployeeId INNER JOIN
                           dbo.VacationPlans    ON dbo.RequestVacations.Id = dbo.VacationPlans.RequestVacationId INNER JOIN
                           dbo.VacationTypes    ON dbo.RequestVacations.VacationTypeId = dbo.VacationTypes.Id
			               where dbo.VacationPlans.VacationDate between'" + FromDate.ToString("yyyy-MM-dd") + "'and '" + ToDate.ToString("yyyy-MM-dd") + "' " +
                           " and RequestVacations.Approved ='True' " +
                           $"{Id}group by  dbo.Employees.Id, dbo.Employees.Name, dbo.Employees.VacationBalance";

            var SpGetData = _context.SpGetReportVacationPlans.FromSqlRaw(sqlQuery).ToList();


            ViewBag.Employees = _context.Employees.ToList();

            return View("ViewReportVacationPlan2", sqlQuery);
        }
    }
}
