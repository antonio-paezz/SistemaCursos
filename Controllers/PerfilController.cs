using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeCursos.Data;
using SistemaDeCursos.Models;
using SistemaDeCursos.Services;
using SistemaDeCursos.ViewModels;
using System.Security.Claims;

namespace SistemaDeCursos.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly CloudinaryService _cloudinaryService;
        private readonly ActivityLogService _activityLogService;

        public PerfilController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            CloudinaryService cloudinaryService,
            ActivityLogService activityLogService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cloudinaryService = cloudinaryService;
            _activityLogService = activityLogService;
        }

        
        public async Task<IActionResult> Index()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return RedirectToAction("Login", "Login");

            var vm = await ConstruirViewModel(usuario);
            return View(vm);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(PerfilViewModel vm)
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return RedirectToAction("Login", "Login");

            if (!ModelState.IsValid)
                return View("Index", await ConstruirViewModel(usuario));

            
            if (usuario.UserName != vm.Nombre)
            {
                var existeUser = await _userManager.Users
                    .AnyAsync(u => u.UserName == vm.Nombre && u.Id != usuario.Id);

                if (existeUser)
                {
                    TempData["ErrorMessage"] = "El nombre de usuario ya está en uso.";
                    return RedirectToAction(nameof(Index));
                }

                usuario.UserName = vm.Nombre;
            }

            
            if (usuario.Email != vm.Email)
            {
                var existeEmail = await _userManager.Users
                    .AnyAsync(u => u.Email == vm.Email && u.Id != usuario.Id);

                if (existeEmail)
                {
                    TempData["ErrorMessage"] = "El email ya está en uso.";
                    return RedirectToAction(nameof(Index));
                }

                usuario.Email = vm.Email;
            }

            usuario.PhoneNumber = vm.Telefono;

            var result = await _userManager.UpdateAsync(usuario);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "No se pudo actualizar el perfil.";
                return RedirectToAction(nameof(Index));
            }

            await _signInManager.RefreshSignInAsync(usuario);

            _activityLogService.Registrar(usuario.Id, "Actualizó su perfil");

            TempData["SuccessMessage"] = "Perfil actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubirImagen(PerfilViewModel vm)
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return RedirectToAction("Login", "Login");

            if (vm.Foto == null)
            {
                TempData["ErrorMessage"] = "Debe seleccionar una imagen.";
                return RedirectToAction(nameof(Index));
            }

            var resultado = await _cloudinaryService.SubirArchivoAsync(vm.Foto);

            if (resultado == null)
            {
                TempData["ErrorMessage"] = "Error al subir la imagen.";
                return RedirectToAction(nameof(Index));
            }

            
            if (!string.IsNullOrEmpty(usuario.PublicId))
            {
                await _cloudinaryService.EliminarArchivo(usuario.PublicId, "image");
            }

            usuario.FotoUrl = resultado.UrlArchivo;
            usuario.PublicId = resultado.PublicId;

            await _userManager.UpdateAsync(usuario);
            await _signInManager.RefreshSignInAsync(usuario);

            _activityLogService.Registrar(usuario.Id, "Actualizó su foto de perfil");

            TempData["SuccessMessage"] = "Foto actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(PerfilViewModel vm)
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return RedirectToAction("Login", "Login");

            var result = await _userManager.ChangePasswordAsync(
                usuario,
                vm.ContraseñaActual,
                vm.NuevaContraseña);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "No se pudo cambiar la contraseña.";
                return RedirectToAction(nameof(Index));
            }

            await _signInManager.RefreshSignInAsync(usuario);

            _activityLogService.Registrar(usuario.Id, "Cambió su contraseña");

            TempData["SuccessMessage"] = "Contraseña actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        
        private async Task<PerfilViewModel> ConstruirViewModel(ApplicationUser usuario)
        {
            var logs = _activityLogService.ObtenerLogs(usuario.Id);

            return new PerfilViewModel
            {
                Nombre = usuario.UserName,
                Email = usuario.Email,
                Telefono = usuario.PhoneNumber,
                FotoUrl = usuario.FotoUrl,
                Logs = logs
            };
        }

    }
}
