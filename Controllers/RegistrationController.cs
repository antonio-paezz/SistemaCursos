using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCursos.Models;
using SistemaDeCursos.ViewModels;

namespace SistemaDeCursos.Controllers
{
	public class RegistrationController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;

        public RegistrationController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Registration()
		{
			return View();
		}

		public async Task<IActionResult> Registrarse(RegistrationViewModel viewmodel) 
		{
			if (!ModelState.IsValid)
			{
				TempData["Error"] = "El usuario ingresado no es válido";
				return View("Registration", viewmodel);
			}

			var usuario = new ApplicationUser
			{
				Nombre = viewmodel.Nombre,
				Apellido = viewmodel.Apellido,
				UserName = viewmodel.NombreDeUsuario,
				Email = viewmodel.Email,	
				Pais = viewmodel.Pais,
				Provincia = viewmodel.Provincia,
				Ciudad = viewmodel.Ciudad
			};

			var respuesta = await _userManager.CreateAsync(usuario);

            if (!respuesta.Succeeded)
            {
                foreach (var error in respuesta.Errors)
                {
					ModelState.AddModelError("", error.Description);
					return View("Registration",viewmodel);
                }
            }

			var agregarContraseña = await _userManager.AddPasswordAsync(usuario, viewmodel.Contraseña);
			if (!agregarContraseña.Succeeded)
			{
				foreach (var error in respuesta.Errors)
				{
					ModelState.AddModelError("", error.Description);
					return View("Registration", viewmodel);

				}

			}

			TempData["SuccessMessage"] = "Se registró correctamente";

			return RedirectToAction("Index","Home");
		}
	}
}
