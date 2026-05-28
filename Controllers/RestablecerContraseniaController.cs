using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCursos.ViewModels;
using SistemaDeCursos.Models;

namespace SistemaDeCursos.Controllers
{
	public class RestablecerContraseniaController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public RestablecerContraseniaController(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		public IActionResult VistaCambiarContrasenia()
		{
			return View();
		}

		public async Task<IActionResult> ConfirmarEmail(RestablecerContraseniaViewModel viewmodel)
		{
			if (viewmodel.Email == null)
			{
				ModelState.AddModelError("", "Debe ingresar email");
				return View("VistaCambiarContrasenia", viewmodel);
			}

			var usuario = await _userManager.FindByEmailAsync(viewmodel.Email);
			if (usuario == null)
			{
				ModelState.AddModelError("", "El email no coincide con ningun usuario");
				return View("VistaCambiarContrasenia", viewmodel);
			}

			return View("_PartialContrasenia", viewmodel);
		}

		[HttpPost]
		public async Task<IActionResult> RestablecerContrasenia(RestablecerContraseniaViewModel viewmodel)
		{
			if (!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Debe ingresar todos los campos");
				return View("_PartialContrasenia", viewmodel);
			}

			if (viewmodel.NuevaContrasenia != viewmodel.ConfirmarContrasenia)
			{
				ModelState.AddModelError("ConfirmarContrasenia", "Las contraseñas no coinciden");
				return View("_PartialContrasenia", viewmodel);
			}

			var usuario = await _userManager.FindByEmailAsync(viewmodel.Email);

			if (usuario == null)
			{
				ModelState.AddModelError("", "Error al encontrar al usuario");
				return View("_PartialContrasenia");
			}


			var existe = await _userManager.CheckPasswordAsync(usuario, viewmodel.NuevaContrasenia);

			if (existe)
			{
				ModelState.AddModelError("", "La contraseña no puede ser igual a la anterior");
				return View("_PartialContrasenia", viewmodel);
			}

			var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);

			var respuesta = await _userManager.ResetPasswordAsync(usuario, token, viewmodel.NuevaContrasenia);

			if (respuesta.Succeeded)
			{
				TempData["SuccessMessage"] = "Se cambió la contraseña correctamente";
				return RedirectToAction("Login", "Login");
			}

			foreach (var error in respuesta.Errors)
			{
				ModelState.AddModelError("NuevaContrasenia", error.Description);
			}

			return View("_PartialContrasenia", viewmodel);
		}
	}
}
