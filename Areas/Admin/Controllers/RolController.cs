using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCursos.Data;
using SistemaDeCursos.Models;
using SistemaDeCursos.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace SistemaDeCursos.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class RolController : Controller
    {
        private readonly RoleManager<IdentityRole> _rolManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public RolController(RoleManager<IdentityRole> rolManager, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _rolManager = rolManager;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult VistaListaRoles()
        {

            var roles = _context.Roles.ToList();

            return View(roles);
        }

        public IActionResult BuscarRol(string filtro)
        {
            var roles = _rolManager.Roles.AsQueryable();
            if (!string.IsNullOrEmpty(filtro))
            {
                roles = roles.Where(r => r.Name.StartsWith(filtro));
            }

            var rolesFiltrados = roles.Select(r => new IdentityRole
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();

            return PartialView("_TablaRol", rolesFiltrados);
        }

        public async Task<IActionResult> UsuariosPorRol(string rolName, string SearchString)
        {
            ViewBag.RolName = rolName;
            if (rolName == null)
            {
                ModelState.AddModelError("", "No es válido el rol");
                return RedirectToAction("VistaListaRoles");
            }
            var rol = await _rolManager.FindByNameAsync(rolName);
            var usuariosDeRol = await _userManager.GetUsersInRoleAsync(rol.Name);

            if (!string.IsNullOrEmpty(SearchString))
            {
                usuariosDeRol.Where(u => u.UserName.StartsWith(SearchString) || u.Email.StartsWith(SearchString));
            }

            var viewmodel = new RolViewModel
            {
                IdRol = rol.Id,
                Nombre = rol.Name,
                Usuarios = usuariosDeRol
            };

            return View(viewmodel);
        }

        public async Task<IActionResult> ListaUsuariosPorRol(string rolName)
        {
            var usuarios = await _userManager.GetUsersInRoleAsync(rolName);
            var vm = new RolViewModel
            {
                Nombre = rolName,
                Usuarios = usuarios
            };
            return PartialView("_ListaUsuarios", vm);
        }

        public async Task<IActionResult> QuitarRol(string userId, string rolName)
        {
            if (userId == null || rolName == null)
            {
                return RedirectToAction("VistaListaRoles");
            }
            var usuario = await _userManager.FindByIdAsync(userId);
            var resultado = await _userManager.RemoveFromRoleAsync(usuario, rolName);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                RedirectToAction("VistaListaRoles");
            }

            return RedirectToAction("UsuariosPorRol", new { rolName });
        }


        public async Task<IActionResult> AsignarRol(List<string> ids, string rol)
        {
            if (ids == null || ids.Count() == 0)
            {
                return BadRequest("No se seleccionaron usuarios");
            }

            foreach (var id in ids)
            {
                var usuario = await _userManager.FindByIdAsync(id);
                if (usuario == null)
                {
                    return BadRequest("El usuario no se encontró");
                }

                var respuesta = await _userManager.AddToRoleAsync(usuario, rol);
                if (!respuesta.Succeeded)
                {
                    return BadRequest("Hubo un error al asignar el rol");
                }
            }


            return Ok(new { message = "Roles asignados correctamente" });
        }

        public IActionResult VistaCrearRol()
        {
            return View();
        }

        public IActionResult VistaEditarRol(string id)
        {
            var rol = _context.Roles.FirstOrDefault(r => r.Id == id);

            var viewmodel = new RolViewModel
            {
                IdRol = rol.Id,
                Nombre = rol.Name
            };

            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(RolViewModel viewmodel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Los datos no son válidos");
                return View("VistaCrearRol", viewmodel);
            }

            var rol = new IdentityRole
            {
                Name = viewmodel.Nombre
            };

            var respuesta = await _rolManager.CreateAsync(rol);

            if (respuesta.Succeeded)
            {
                TempData["SuccessMessage"] = "El rol se creó correctamente.";
                return RedirectToAction("VistaListaRoles", "Rol");
            }

            foreach (var error in respuesta.Errors)
            {
                TempData["ErrorMessage"] = error.Description;
            }

            return RedirectToAction("VistaListaRoles");
        }

        public async Task<IActionResult> Editar(RolViewModel viewmodel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Los datos ingresados son inválidos");
                return View("VistaEditarRol", viewmodel);
            }

            var rol = await _rolManager.FindByIdAsync(viewmodel.IdRol);

            var respuesta = await _rolManager.SetRoleNameAsync(rol, viewmodel.Nombre);

            if (respuesta.Succeeded)
            {
                await _rolManager.UpdateAsync(rol);
                TempData["SuccessMessage"] = "El rol se ha editado correctamente";
                return RedirectToAction("VistaListaRoles", "Rol");
            }

            foreach (var error in respuesta.Errors)
            {
                TempData["ErrorMessage"] = error.Description;
            }

            return RedirectToAction("VistaListaRoles");
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "No se encontró el Rol";
                return RedirectToAction("VistaListaRoles", "Rol");
            }

            var rol = await _rolManager.FindByIdAsync(id);
            var usuariosEnRol = await _userManager.GetUsersInRoleAsync(rol.Name);
            if (usuariosEnRol.Count > 0)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el rol poque tiene usuarios asignados";
                return RedirectToAction("VistaListaRoles");
            }


            var respuesta = await _rolManager.DeleteAsync(rol);

            if (respuesta.Succeeded)
            {
                TempData["SuccessMessage"] = "El rol se eliminó correctamente";
                return RedirectToAction("VistaListaRoles", "Rol");
            }

            foreach (var error in respuesta.Errors)
            {
                TempData["ErrorMessage"] = error.Description;
            }

            return RedirectToAction("VistaListaRoles");
        }
    }
}
