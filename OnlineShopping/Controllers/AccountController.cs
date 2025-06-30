using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Common;
using OnlineShopping.Models;
using OnlineShopping.Services.Interfaces;
using OnlineShopping.Utilities;
using OnlineShopping.Utilities;
using OnlineShopping.ViewModels;
using System.Threading.Tasks;

namespace OnlineShopping.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<AppUser>userManager, 
            SignInManager<AppUser>signInManager,
            RoleManager<IdentityRole>roleManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = new AppUser
            {
                Name = registerVM.Name,
                Surname = registerVM.Surname,
                UserName = registerVM.UserName,
                Email = registerVM.Email,
            };
           var result=await  _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(user, UserRole.Member.ToString());

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink=Url.Action(nameof(ComfirmEmail),"Account",new {token,Email=user.Email},Request.Scheme);
            await _emailService.SendMailAsync(user.Email, "Email Confirmation",confirmationLink);
            //await _signInManager.SignInAsync(user, false);



            return RedirectToAction(nameof(SuccesfullyRegisteredEmail),"Account");
        }
        public async Task<IActionResult> ComfirmEmail(string token,string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);
            if (user is null) return NotFound();
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            await _signInManager.SignInAsync(user, false);
            return View();
        }

        public IActionResult SuccesfullyRegisteredEmail()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM,string? returnUrl)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginVM.UsernameOrEmail || u.Email == loginVM.UsernameOrEmail);
            if(user is null)
            {
                ModelState.AddModelError(string.Empty,"Username,Email or password is incorrect");
                    return View();
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.IsPersistant, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Try 10 minutes later");
                return View();
            }
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Please comfirm your email");
                return View();
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Username,Email or password is incorrect");
                return View();
            }
            if (returnUrl is null) return RedirectToAction("index","home");
            return RedirectToAction(returnUrl);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }


        public async Task<IActionResult> CreateRole()
        {
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = role.ToString()
                    });
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPasswordVM)
        {
            if (!ModelState.IsValid) return View(forgotPasswordVM);
            var user = await _userManager.FindByEmailAsync(forgotPasswordVM.Email);
            if (user is null) return View(forgotPasswordVM);
            //https://localhost:7214/Account/ResetPassword/userId?token=resetToken
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string link = Url.Action("ResetPassword", "Account", new { userId = user.Id, Token = token }, HttpContext.Request.Scheme);
            return Json(link);
        }

        public async Task<IActionResult> ResetPassword(string userId,string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token)) return BadRequest();
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM,string userId,string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token)) return BadRequest();
            if (!ModelState.IsValid) return View(resetPasswordVM);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();
            var identityUser = await _userManager.ResetPasswordAsync(user, token, resetPasswordVM.ConfirmPassword);
                return RedirectToAction(nameof(Login));

        }



    }
}
