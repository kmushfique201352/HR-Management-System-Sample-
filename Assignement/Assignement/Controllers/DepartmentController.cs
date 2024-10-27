using Assignement.DataAccess.UnitOfWork;
using Assignement.Model.DTO;
using Assignement.Model.Entity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace Assignement.MVC.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(string Id)
        {
            var departments = await _unitOfWork.DepartmentRepo.GetAllAsync();

            if (!string.IsNullOrEmpty(Id))
            {
                departments = departments.Where(d => d.Id == Id).ToList();
            }

            return View(departments);
        }

        public async Task<IActionResult> TotalDepartmentCount()
        {
            var total = (await _unitOfWork.DepartmentRepo.GetAllAsync()).Count();
            return Ok(total);
        }

        public async Task<IActionResult> GetByID(string Id)
        {
            var department = await _unitOfWork.DepartmentRepo.GetByIdAsync(Id);
            if (department is null)
            {
                return NotFound();
            }
            return Ok(department);
        }

        [HttpGet]
        public async Task<IActionResult> DepartmentSave(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return View();
            }

            var department = await _unitOfWork.DepartmentRepo.GetByIdAsync(Id);
            if (department is null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> DepartmentSave(Department model)
        {
            if (ModelState.IsValid)
            {
                model.UpBy = "Uploader Name Here"; 
                model.CreatedAt = DateTime.Now;

                if (!string.IsNullOrEmpty(model.Id))
                {
                    var existingDepartment = await _unitOfWork.DepartmentRepo.GetByIdAsync(model.Id);
                    if (existingDepartment != null)
                    {
                        existingDepartment.Name = model.Name;
                        existingDepartment.UpBy = model.UpBy;
                        existingDepartment.IsDeleted = model.IsDeleted;
                        _unitOfWork.DepartmentRepo.Update(existingDepartment);
                    }
                }
                else
                {
                    await _unitOfWork.DepartmentRepo.AddAsync(model);
                }

                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> DepartmentDelete(string Id)
        {
            var department = await _unitOfWork.DepartmentRepo.GetByIdAsync(Id);
            if (department is null)
            {
                return NotFound();
            }

            _unitOfWork.DepartmentRepo.Delete(department);
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
