using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCursos.Models;
using SistemaDeCursos.ViewModels;

namespace SistemaDeCursos.Controllers
{
	public class LoginController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
			_userManager = userManager;
			_signInManager = signInManager;
        }

        public IActionResult Login()
		{
			return View();
		}

		public async Task<IActionResult> IniciarSesion(LoginViewModel viewmodel) 
		{
            if (!ModelState.IsValid)
            {
				ModelState.AddModelError("", "Los datos ingresados son incorrectos");
				return View("Login", viewmodel);
			}

			var user = _userManager.Users.FirstOrDefault(u => u.Email == viewmodel.Email);

			if (user == null) 
			{
				ModelState.AddModelError("", "No se encontró el usuario");
				return View("Login", viewmodel);
			}

			var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, viewmodel.Contraseña, true);

			if (!passwordCheck.Succeeded)
			{
				if(passwordCheck.IsNotAllowed)
				{
					ModelState.AddModelError("", "No tiene permitido iniciar sesión");
					return View("Login", viewmodel);
				}
                else if (passwordCheck.IsLockedOut)
				{
					ModelState.AddModelError("", "Bloqueo por intentos fallidos");
					return View("Login", viewmodel);
				}
				else
				{
					ModelState.AddModelError("", "La contraseña es incorrecta");
					return View("Login", viewmodel);
				}
            }

			await _signInManager.SignInAsync(user, true);
			TempData["SuccessMessage"] = "Se inicio sesión correctamente";

            return RedirectToAction("Index", "Home"); 
		}

		public async Task<IActionResult> CerrarSesion()
		{
			await _signInManager.SignOutAsync();
			

			return RedirectToAction("Index", "Home");
		}


		public IActionResult AccessDenied()
		{
			return View();
		}

    }
}
