using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCursos.Models;

namespace SistemaDeCursos.Controllers
{
    public class RolController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Lista()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        public IActionResult VistaCrear()
        {
            return View();
        }

        public async Task<IActionResult> Crear(IdentityRole rol)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Debe ingresar todos los campos");
                return View("VistaCrear", rol);
            }

            var respuesta = await _roleManager.CreateAsync(rol);

            if (!respuesta.Succeeded)
            {
                foreach (var error in respuesta.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(rol);
            }

            return RedirectToAction("Lista");
        }
    }
}
