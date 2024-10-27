using Assignement.DataAccess.UnitOfWork;
using Assignement.Model.DTO;
using Assignement.Model.Entity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace Assignement.MVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> EmployeeView()
        {
            var employees = await _unitOfWork.EmployeeRepo.GetAllAsync();
            var departments = await _unitOfWork.DepartmentRepo.GetAllAsync();
            var designations = await _unitOfWork.DesignationRepo.GetAllAsync();

            var employeeList = from e in employees
                               join d in departments on e.DeptId equals d.Id into ed
                               from dept in ed.DefaultIfEmpty()
                               join desig in designations on e.DesigID equals desig.Id into desg
                               from designation in desg.DefaultIfEmpty()
                               select new
                               {
                                   e.Id,
                                   e.Name,
                                   e.Address,
                                   e.City,
                                   e.Phone,
                                   DepartmentName = dept?.Name ?? "No Department",
                                   DesignationName = designation?.Name ?? "No Designation"
                               };

            return View(employeeList);
        }



        public async Task<IActionResult> TotalEmployeeCount()
        {
            var total = await _unitOfWork.EmployeeRepo.GetAllAsync();
            return Ok(total.Count());
        }

        public async Task<IActionResult> GetByEmpID(string id)
        {
            var employee = await _unitOfWork.EmployeeRepo.GetByIdAsync(id);
            if (employee is null) return NotFound();
            return Ok(employee);
        }
        public async Task<IActionResult> EmployeeList(string searchName, string searchDepartment, string searchDesignation)
        {
            var employees = await _unitOfWork.EmployeeRepo.GetAllAsync();
            var departments = await _unitOfWork.DepartmentRepo.GetAllAsync();
            var designations = await _unitOfWork.DesignationRepo.GetAllAsync();

            var employeeList = from e in employees
                               join d in departments on e.DeptId equals d.Id into ed
                               from dept in ed.DefaultIfEmpty()
                               join des in designations on e.DesigID equals des.Id into desj
                               from designation in desj.DefaultIfEmpty()
                               where (string.IsNullOrEmpty(searchName) || e.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase)) &&
                                     (string.IsNullOrEmpty(searchDepartment) || (dept?.Name.Contains(searchDepartment, StringComparison.OrdinalIgnoreCase) == true)) &&
                                     (string.IsNullOrEmpty(searchDesignation) || (designation?.Name.Contains(searchDesignation, StringComparison.OrdinalIgnoreCase) == true))
                               select new
                               {
                                   e.Id,
                                   e.Name,
                                   e.Phone,
                                   DepartmentName = dept?.Name ?? "No Department",
                                   DesignationName = designation?.Name ?? "No Designation"
                               };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_EmployeeListPartial", employeeList);
            }

            return View(employeeList);
        }





        [HttpGet]
        public async Task<IActionResult> EmployeeSave(string id)
        {
            ViewBag.Departments = await _unitOfWork.DepartmentRepo.GetAllAsync();
            ViewBag.Designations = await _unitOfWork.DesignationRepo.GetAllAsync();

            if (string.IsNullOrEmpty(id)) return View();

            var employee = await _unitOfWork.EmployeeRepo.GetByIdAsync(id);
            if (employee is null) return NotFound();

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> EmployeeSave(Employee_List_DTO model)
        {
            var departments = await _unitOfWork.DepartmentRepo.GetAllAsync();
            if (!departments.Any(d => d.Id == model.DeptId))
            {
                ModelState.AddModelError("DeptId", "The selected department does not exist.");
                return View(model);
            }

            var designations = await _unitOfWork.DesignationRepo.GetAllAsync();
            if (!string.IsNullOrEmpty(model.DesigID) && !designations.Any(d => d.Id == model.DesigID))
            {
                ModelState.AddModelError("DesigID", "The selected designation does not exist.");
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.Id))
            {
                var employee = await _unitOfWork.EmployeeRepo.GetByIdAsync(model.Id);
                if (employee is null) return NotFound();

                employee.Name = model.Name;
                employee.Address = model.Address;
                employee.City = model.City;
                employee.Phone = model.Phone;
                employee.DeptId = model.DeptId;
                employee.DesigID = model.DesigID;

                _unitOfWork.EmployeeRepo.Update(employee);
            }
            else
            {
                var newEmployee = new Employee
                {
                    Name = model.Name,
                    Address = model.Address,
                    City = model.City,
                    Phone = model.Phone,
                    DeptId = model.DeptId,
                    DesigID = model.DesigID
                };
                await _unitOfWork.EmployeeRepo.AddAsync(newEmployee);
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(EmployeeView));
        }


        [HttpGet]
        public async Task<IActionResult> EmployeeDelete(string id)
        {
            var employee = await _unitOfWork.EmployeeRepo.GetByIdAsync(id);
            if (employee is null) return NotFound();

            _unitOfWork.EmployeeRepo.Delete(employee);
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(EmployeeView));
        }
    }
}
