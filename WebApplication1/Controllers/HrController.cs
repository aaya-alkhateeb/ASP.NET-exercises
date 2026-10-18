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
            var query = _context.Employees.Include(e => e.department).Include(e => e.User).AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(
                    e => e.User == null? 
                    false : 
                    (e.User.FirstName.Contains(searchQuery) || e.User.LastName.Contains(searchQuery)) || e.department.Name.Contains(searchQuery));
            }

            List<EmployeeView> employees = await query.Where(x => empDtype=="deleted" ? !x.Is_Active : x.Is_Active).OrderBy(x => x.User.FirstName).Select(e => new EmployeeView
            {
                Employee_Id = e.Employee_Id,
                Employee_Name = string.IsNullOrWhiteSpace( e.User.FirstName ?? "" + " " + e.User.LastName ?? "") ? "Name Not Specified" : e.User.FirstName + " " + e.User.LastName,
                salary = e.salary== null? "Not Specified" : e.salary.Value.ToString("C"),
                DepartmentName = e.department.Name ?? "No Department",
                HireDate = e.hireDate.ToString("dd - M - yyyy")
            }).ToListAsync();

            ViewBag.NumberOfEmployees = employees.Count;
            ViewBag.empDtype = empDtype;
            ViewBag.searchQuery = searchQuery;

            ViewBag.IsAdmin = IsAdmin();

            return View(employees);
        }

        public async Task<IActionResult> Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = await getDepartments();
            ViewBag.Users = await GetUsersWithNoEmployee();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID" , "salary" , "DepartmentId")]Employee employee)
        {

            if (!IsAdmin())
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            employee.hireDate = DateTime.Now;
            employee.Is_Active = true;
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Edit(int? id)
        {

            if (!IsAdmin())
            {
                return RedirectToAction(nameof(Index));
            }

            var employee = await FindEmployee(id);
            if (employee == null)
            {
                return RedirectToAction("Index");
            }
       
            ViewBag.Departments = await getDepartments();
            


            ViewBag.hasHireDate = !(employee.hireDate.Equals(DefaultDate));
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,Employee employee)
        {
            if (!IsAdmin())
            {
                return RedirectToAction(nameof(Index));
            }

            var dbEmp = await FindEmployee(id);

            if (dbEmp == null)
            {
                return RedirectToAction("Index");
            }
            if (!ModelState.IsValid || employee.salary == null)
            {
                if(employee.salary == null)
                {
                    ModelState.AddModelError("salary", "Salary is required");
                }

                ViewBag.Departments = await getDepartments();
                ViewBag.hasHireDate = !(dbEmp.hireDate.Equals(DefaultDate));
                return View(employee);
            }

            dbEmp.salary = employee.salary;
            dbEmp.DepartmentId = employee.DepartmentId;
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

            if (!IsAdmin())
            {
                return RedirectToAction(nameof(Index));
            }

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

            if (!IsAdmin())
            {
                return RedirectToAction(nameof(Index));
            }

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

        private async Task<SelectList> GetUsersWithNoEmployee()
        {
            var users = new SelectList(_context.Users.Include(u => u.Employee).Where(u=>u.Employee == null), "Id", "FirstName", "LastName");

            return users;
        }

        private bool IsAdmin()
        {
            if (User.Identity.IsAuthenticated && User.Identity.Name == "admin@example.com")
            {
                return true;
            }

            return false;
        }
    }
}

