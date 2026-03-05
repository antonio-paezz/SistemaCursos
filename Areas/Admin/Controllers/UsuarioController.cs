using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SistemaDeCursos.Areas.Admin.ViewModels;
using SistemaDeCursos.Data;
using SistemaDeCursos.Models;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaDeCursos.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuarioController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Buscar(string filtro, string modo)
        {
            ViewBag.Modo = modo;
            var usuarios = _userManager.Users.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                usuarios = usuarios.Where(u =>
                u.UserName.StartsWith(filtro) ||
                u.Email.StartsWith(filtro));
            }

            var resultado = usuarios
                    .Take(10)
                    .ToList();

            return PartialView("_ListaUsuarios", resultado);
        }

        public async Task<IActionResult> Index(int pagina = 1)
        {
			int registrosPorPagina = 10;

			var totalRegistros = await _userManager.Users.CountAsync();

			var usuarios = await _userManager.Users
				.OrderBy(i => i.Id)
				.Skip((pagina - 1) * registrosPorPagina)
				.Take(registrosPorPagina)
				.ToListAsync();

			ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);
			ViewBag.PaginaActual = pagina;

			return View(usuarios);
        }

        public IActionResult VistaAgregar()
        {
            return View();
        }

        public async Task<IActionResult> Agregar(AgregarUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "No se completaron todos los datos";
                return View(model);
            }

            var usuario = new ApplicationUser
            {
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                UserName = model.UserName,
                Email = model.Email,
                Pais = model.Pais,
                Provincia = model.Provincia,
                Ciudad = model.Ciudad
            };

            var resultado = await _userManager.CreateAsync(usuario, model.Contraseña);

            if (resultado.Succeeded)
            {
                TempData["SuccessMessagge"] = "El usuario se agregó con éxito";
                return RedirectToAction("Index");
            }

            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public IActionResult VistaEditar(string id)
        {
            var usuario = _userManager.Users.FirstOrDefault(u => u.Id == id);

            if (usuario == null)
            {
                TempData["ErrorMessagge"] = "No se encontró el usuario";
                return RedirectToAction("Index");
            }

            var usuvm = new EditarUsuarioViewModel
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                UserName = usuario.UserName,
                Email = usuario.Email,
                Pais = usuario.Pais,
                Provincia = usuario.Provincia,
                Ciudad = usuario.Ciudad
            };

            return View(usuvm);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(EditarUsuarioViewModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (model.Id == userId)
            {
                TempData["ErrorMessage"] = "No podés editarte a vos mismo";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Los datos no son válidos");
                return View("VistaEditar" ,model);
            }

            var usuario = await _userManager.FindByIdAsync(model.Id);

            if (usuario == null)
            {
                TempData["ErrorMessage"] = "No se encontró el usuario";
                return RedirectToAction("Index");
            }

            var existe = _userManager.Users.Any(u =>
                u.Id != model.Id &&
                (u.UserName == model.UserName || u.Email == model.Email)
            );

            if (existe)
            {
                TempData["ErrorMessage"] = "Ya hay un usuario con ese email o usuario";
                return View("VistaEditar", model);
            }

            usuario.Nombre = model.Nombre;
            usuario.Apellido = model.Apellido;
            usuario.UserName = model.UserName;
            usuario.Email = model.Email;
            usuario.Pais = model.Pais;

            var resultado = await _userManager.UpdateAsync(usuario);

            if (resultado.Succeeded)
            {
                TempData["SuccessMessage"] = "El usuaro se editó con éxito";
                return RedirectToAction("Index");
            }

            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("VistaEditar",model);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(string id)
        {

            if (id == null)
            {
                TempData["ErrorMessage"] = "No se encontró el usuario";
                return RedirectToAction("Index");
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id == userId)
            {
                TempData["ErrorMessage"] = "No podés eliminarte a vos mismo";
                return RedirectToAction("Index");
            }

            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                TempData["ErrorMessage"] = "No se encontró el usuario";
                return RedirectToAction("Index");
            }

            var tieneInscripciones = _context.Inscripcion
                .Any(i => i.Curso.InstructorId == id);

            if (tieneInscripciones)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el usuario porque tiene alumnos inscriptos.";
                return RedirectToAction("Index");
            }

            var tieneCompras = _context.Compra
                .Any(c => c.UsuarioId == id);

            if (tieneCompras)
            {
                usuario.LockoutEnabled = true;
                usuario.LockoutEnd = DateTimeOffset.MaxValue;

                await _userManager.UpdateAsync(usuario);

                TempData["SuccessMessage"] =
                    "El usuario tiene compras asociadas y fue deshabilitado.";

                return RedirectToAction("Index");
            }

            var respuesta = await _userManager.DeleteAsync(usuario);

            if (respuesta.Succeeded)
            {
                TempData["SuccessMessage"] = "El usuario se eliminó con éxito";
                return RedirectToAction("Index");
            }

            foreach (var error in respuesta.Errors)
            {
                TempData["ErrorMessage"] = error.Description;
            }

            return RedirectToAction("Index");
        }



        public IActionResult ModalBuscarUsuarios(string rolName, string modo)
        {
            ViewBag.RolName = rolName;
            var usuarios = _userManager.Users
                .ToList();

            ViewBag.Modo = modo; // pasar el modo a la vista
            return PartialView("_ModalBuscarUsuarios", usuarios);
        }

        public async Task<IActionResult> AsignarRol(RolesDeUsuarioViewModel viewmodel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "El modelo no es válido");
                return RedirectToAction("VistaListaUsuario");
            }
            var usuario = await _userManager.FindByIdAsync(viewmodel.UsuarioId);

            if (usuario == null)
            {
                ModelState.AddModelError("", "No se encontró el usuario");
                return RedirectToAction("VistaListaUsuario");
            }
            var roles = (await _userManager.GetRolesAsync(usuario)).ToList();

            foreach (var rol in roles)
            {
                if (rol == viewmodel.RolName)
                {
                    TempData["Error"] = "El Rol ya está asignado al usuario.";
                    return RedirectToAction("RolesDeUsuario", new { userId = viewmodel.UsuarioId });
                }
            }

            var resultado = await _userManager.AddToRoleAsync(usuario, viewmodel.RolName);
            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    return RedirectToAction("VistaListaUsuario");
                }
            }
            TempData["SuccessMessage"] = "Se asigno el rol correctamente";
            return RedirectToAction("RolesDeUsuario", new { userId = viewmodel.UsuarioId });
        }

        public async Task<IActionResult> QuitarRol(string userId, string rolName)
        {
            if (userId == null || rolName == null)
            {
                return RedirectToAction("RolesDeUsuario", new { userId = userId });
            }
            var usuario = await _userManager.FindByIdAsync(userId);
            var resultado = await _userManager.RemoveFromRoleAsync(usuario, rolName);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                RedirectToAction("RolesDeUsuario", new { userId = userId });
            }
            TempData["SuccessMessage"] = "El rol se eliminó con exito";
            return RedirectToAction("RolesDeUsuario", new { userId = userId });
        }


		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> RolesDeUsuario(string userId)
		{
			var usuario = await _userManager.FindByIdAsync(userId);
			var rolesDeUsuario = (await _userManager.GetRolesAsync(usuario)).ToList();
			var roles = _roleManager.Roles.ToList();
			var viewmodel = new RolesDeUsuarioViewModel
			{
				UsuarioId = usuario.Id,
				RolesDeUsuario = rolesDeUsuario,
				Roles = roles
			};

			return View(viewmodel);
		}

        
    }
}
