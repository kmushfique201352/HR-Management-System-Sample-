using Microsoft.AspNetCore.Mvc;
using Assignement.DataAccess.UnitOfWork;
using Assignement.Model.Entity;
using System.Threading.Tasks;

namespace Assignement.MVC.Controllers
{
    public class DesignationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DesignationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> DesignationSave(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View(new Designation());
            }

            var designation = await _unitOfWork.DesignationRepo.GetByIdAsync(id);
            if (designation == null)
            {
                return NotFound();
            }

            return View(designation);
        }

        [HttpPost]
        public async Task<IActionResult> DesignationSave(Designation model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); 
            }

            var existingDesignation = await _unitOfWork.DesignationRepo.FirstOrDefaultAsync(d => d.Name == model.Name);

            if (existingDesignation != null)
            {
                ModelState.AddModelError("Name", "A designation with this name already exists.");
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.Id))
            {
                var designation = await _unitOfWork.DesignationRepo.GetByIdAsync(model.Id);
                if (designation == null)
                {
                    var newDesignation = new Designation
                    {
                        Name = model.Name
                    };
                    await _unitOfWork.DesignationRepo.AddAsync(newDesignation);

                    await _unitOfWork.SaveAsync();
                    return RedirectToAction(nameof(Designation));
                }

                designation.Name = model.Name;
                _unitOfWork.DesignationRepo.Update(designation);
            }
            else
            {
                var newDesignation = new Designation
                {
                    Name = model.Name
                };
                await _unitOfWork.DesignationRepo.AddAsync(newDesignation);
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Designation));
        }


        public async Task<IActionResult> Designation()
        {
            var designations = await _unitOfWork.DesignationRepo.GetAllAsync();
            return View(designations);
        }

        [HttpGet]
        public async Task<IActionResult> DesignationDelete(string id)
        {
            var designation = await _unitOfWork.DesignationRepo.GetByIdAsync(id);
            if (designation == null)
            {
                return NotFound();
            }

            _unitOfWork.DesignationRepo.Delete(designation);
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Designation));
        }
    }
}
