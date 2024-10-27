using Assignement.DataAccess;
using Assignement.Model.DTO;
using Assignement.Model.Entity;
using Assignement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Assignement.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AssignmentDBcontext _context;

        public HomeController(ILogger<HomeController> logger, AssignmentDBcontext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> EmployeeList()
        {
            var emplist = await _context.Employees.Include(e => e.Dept).ToListAsync();
            return Ok(emplist);
        }

        public async Task<IActionResult> Index(string Id)
        {
            var query = _context.Departments.AsQueryable();

            if (!string.IsNullOrEmpty(Id))
            {
                query = query.Where(x => x.Id == Id);
            }

            var Deptlist = await query.ToListAsync();

            return View(Deptlist);
        }

        public async Task<IActionResult> TotalDeptlist()
        {
            var total = await _context.Departments.CountAsync();
            return Ok(total);
        }

        public async Task<IActionResult> GetByID(string Id)
        {
            var byidtotal = await _context.Departments.SingleOrDefaultAsync(x => x.Id == Id);
            if (byidtotal is null)
            {
                return NotFound();
            }
            return Ok(byidtotal);
        }

        public async Task<IActionResult> GetByIDview(string Id)
        {
            var total = await _context.Departments.CountAsync();
            var byidtotal = await _context.Departments.SingleOrDefaultAsync(x => x.Id == Id);
            if (byidtotal is null)
            {
                return NotFound();
            }
            ViewBag.Total = total;
            return View(byidtotal);
        }

        public async Task<IActionResult> Department2()
        {
            var departments = await _context.Departments
                .Select(x => new Department_List_DTO { Id = x.Id, Name = x.Name })
                .ToListAsync();
            return Ok(departments);
        }

        [HttpGet]
        public async Task<IActionResult> DepartmentSave(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return View();
            }

            var edit = await _context.Departments.SingleOrDefaultAsync(x => x.Id == Id);
            if (edit is null)
            {
                return NotFound();
            }

            return View(edit);
        }

        [HttpPost]
        public async Task<IActionResult> DepartmentSave(Department_List_DTO model)
        {
            if (!string.IsNullOrEmpty(model.Id))
            {
                var department = await _context.Departments.SingleOrDefaultAsync(x => x.Id == model.Id);
                if (department is null)
                {
                    return NotFound();
                }

                department.Name = model.Name;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var newDepartment = new Department { Name = model.Name };
            _context.Departments.Add(newDepartment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DepartmentDelete(string Id)
        {
            var department = await _context.Departments.SingleOrDefaultAsync(x => x.Id == Id);
            if (department is null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Privacy(string Id)
        {
            var query = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(Id))
            {
                query = query.Where(x => x.Id == Id);
            }

            var emplist = await query.ToListAsync();

            return View(emplist);
        }

        public async Task<IActionResult> TotalEmplist()
        {
            var total = await _context.Employees.CountAsync();
            return Ok(total);
        }

        public async Task<IActionResult> GetByEmpID(string Id)
        {
            var emp = await _context.Employees.SingleOrDefaultAsync(e => e.Id == Id);
            if (emp is null)
            {
                return NotFound();
            }
            return Ok(emp);
        }

        public async Task<IActionResult> EmpByIDview(string Id)
        {
            var total = await _context.Employees.CountAsync();
            var emp = await _context.Employees.SingleOrDefaultAsync(e => e.Id == Id);
            if (emp is null)
            {
                return NotFound();
            }
            ViewBag.Total = total;
            return View(emp);
        }

        public async Task<IActionResult> Employee2()
        {
            var employees = await _context.Employees
                .Select(x => new Employee_List_DTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    City = x.City,
                    Phone = x.Phone
                }).ToListAsync();
            return Ok(employees);
        }

        [HttpGet]
        public async Task<IActionResult> EmployeeSave(string Id)
        {
            ViewBag.Departments = await _context.Departments.ToListAsync();

            if (string.IsNullOrEmpty(Id))
            {
                return View();
            }

            var edit = await _context.Employees.SingleOrDefaultAsync(x => x.Id == Id);
            if (edit is null)
            {
                return NotFound();
            }

            return View(edit);
        }

        [HttpPost]
        public async Task<IActionResult> EmployeeSave(Employee_List_DTO model)
        {
            if (!await _context.Departments.AnyAsync(d => d.Id == model.DeptId))
            {
                ModelState.AddModelError("DeptId", "The selected department does not exist.");
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.Id))
            {
                var edit = await _context.Employees.SingleOrDefaultAsync(x => x.Id == model.Id);
                if (edit is null)
                {
                    return NotFound();
                }

                edit.Name = model.Name;
                edit.Address = model.Address;
                edit.City = model.City;
                edit.Phone = model.Phone;
                edit.DeptId = model.DeptId;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Privacy));
            }

            var employee = new Employee
            {
                Name = model.Name,
                Address = model.Address,
                City = model.City,
                Phone = model.Phone,
                DeptId = model.DeptId
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Privacy));
        }

        [HttpGet]
        public async Task<IActionResult> EmployeeDelete(string Id)
        {
            var edit = await _context.Employees.SingleOrDefaultAsync(x => x.Id == Id);
            if (edit is null)
            {
                return NotFound();
            }

            _context.Employees.Remove(edit);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Privacy));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
