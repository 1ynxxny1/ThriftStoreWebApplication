using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ThriftStoreWebApp.Models;
using ThriftStoreWebApp.Services;

namespace ThriftStoreWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public IActionResult Register()
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(dto);

            var user = new ApplicationUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "client");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(dto);
        }

        public async Task<IActionResult> Logout()
        {
            if (_signInManager.IsSignedIn(User))
                await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(dto);

            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, dto.RememberMe, false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ViewBag.ErrorMessage = "Invalid login attempt.";
            return View(dto);
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Index", "Home");

            var dto = new ProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };

            return View(dto);
        }

        [Authorize, HttpPost]
        public async Task<IActionResult> Profile(ProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please fill all the required fields with valid values.";
                return View(dto);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Index", "Home");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.UserName = dto.Email;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;

            var result = await _userManager.UpdateAsync(user);

            ViewBag.SuccessMessage = result.Succeeded
                ? "Profile updated successfully!"
                : "Unable to update the profile: " + result.Errors.First().Description;

            return View(dto);
        }

        [Authorize]
        public IActionResult Password() => View();

        [Authorize, HttpPost]
        public async Task<IActionResult> Password(PasswordDto dto)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Index", "Home");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (result.Succeeded)
                ViewBag.SuccessMessage = "Password updated successfully!";
            else
                ViewBag.ErrorMessage = "Error: " + result.Errors.First().Description;

            return View();
        }

        public IActionResult AccessDenied() => RedirectToAction("Index", "Home");

        public IActionResult ForgotPassword()
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([Required, EmailAddress] string email)
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("Index", "Home");

            ViewBag.Email = email;

            if (!ModelState.IsValid)
            {
                ViewBag.EmailError = ModelState["Email"]?.Errors.First().ErrorMessage ?? "Invalid Email Address";
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetUrl = Url.ActionLink("ResetPassword", "Account", new { token, email }) ?? "URL Error";

                string senderName = _configuration["BrevoSettings:SenderName"] ?? "";
                string senderEmail = _configuration["BrevoSettings:SenderEmail"] ?? "";
                string username = user.FirstName + " " + user.LastName;
                string subject = "Password Reset";
                string message = $"Dear {username},\n\nYou can reset your password using the following link:\n\n{resetUrl}\n\nBest Regards,\nThrift Avenue Team";

                EmailSender.SendEmail(senderName, senderEmail, username, email, message, subject);
            }

            ViewBag.SuccessMessage = "Please check your email and click on the password reset link.";
            return View();
        }

        public IActionResult ResetPassword(string? token)
        {
            if (_signInManager.IsSignedIn(User) || token == null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string? token, PasswordResetDto dto)
        {
            if (_signInManager.IsSignedIn(User) || token == null)
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(dto);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Token not valid!";
                return View(dto);
            }

            var result = await _userManager.ResetPasswordAsync(user, token, dto.Password);
            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Password reset successfully!";
            }
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(dto);
        }
    }
}
