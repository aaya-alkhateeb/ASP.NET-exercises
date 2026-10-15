using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class HrController : Controller
    {
        private readonly Context _context;
        private readonly DateTime DefaultDate = new DateTime(1, 1, 1, 0, 0, 0);

        public HrController(Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchQuery , string empDtype = "active")
        {
            var query = _context.Employees.Include(e => e.department).AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(e => e.Employee_Name.Contains(searchQuery) || e.department.Name.Contains(searchQuery));
            }

            List<EmployeeView> employees = await query.Where(x => empDtype=="deleted" ? !x.Is_Active : x.Is_Active).OrderBy(x => x.Employee_Name).Select(e => new EmployeeView
            {
                Employee_Id = e.Employee_Id,
                Employee_Name = e.Employee_Name,
                salary = e.salary,
                DepartmentName = e.department.Name ?? "No Department",
                HireDate = e.hireDate.ToString("dd - M - yyyy")
            }).ToListAsync();

            ViewBag.NumberOfEmployees = employees.Count;
            ViewBag.empDtype = empDtype;
            ViewBag.searchQuery = searchQuery;

            return View(employees);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = await getDepartments();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Employee_Name" , "salary" , "DepartmentId")]Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            employee.hireDate = DateTime.Now;
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Edit(int? id)
        {
            var employee = await FindEmployee(id);
            if (employee == null)
            {
                return RedirectToAction("Index");
            }
            if (employee.DepartmentId == null)
            {
                ViewBag.Departments = await getDepartments();
            }


            ViewBag.hasHireDate = !(employee.hireDate.Equals(DefaultDate));
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Employee employee)
        {
            var dbEmp = await FindEmployee(id);

            if (dbEmp == null)
            {
                return RedirectToAction("Index");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await getDepartments();
                return View(employee);
            }

            dbEmp.Employee_Name = employee.Employee_Name;
            dbEmp.salary = employee.salary;
            dbEmp.DepartmentId = employee.DepartmentId == null ? dbEmp.DepartmentId : employee.DepartmentId;
            dbEmp.hireDate = employee.hireDate.Equals(DefaultDate) ? dbEmp.hireDate : employee.hireDate;
            _context.Entry(dbEmp).State = EntityState.Modified;         
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDepartment([Bind("Name")]Department department)
        {
            if (!ModelState.IsValid)
            {
                return View(department);
            }

            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            var employee = await FindEmployee(id);
            if (employee == null)
            {
                return RedirectToAction("Index");
            }
            employee.Is_Active = false;
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int? id)
        {
            var employee = await FindEmployee(id);
            if (employee == null)
            {
                return RedirectToAction("Index");
            }
            employee.Is_Active = true;
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        private async Task<SelectList> getDepartments()
        {
            var departments = await _context.Departments.AsNoTracking().ToListAsync();
            return new SelectList(departments, "Id", "Name");
        }

        private async Task<Employee?> FindEmployee(int? id)
        {
            if (id == null) return null;
            return await _context.Employees.FindAsync(id);
        }
    }
}

