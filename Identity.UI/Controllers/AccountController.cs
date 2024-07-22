using Identity.Application.DTOs.AppUserDTOs;
using Identity.Application.Services.AppUserServices;
using Identity.Domain.Utilities.Concretes;
using Identity.UI.Models.IdentityVMs;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAppUserService _appUserService;
        private const string customerPassword = "Password.1";

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IAppUserService appUserService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appUserService = appUserService;
        }


        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Register()
        {
            var registerVM = new RegisterVM();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model) 
        { 
            if(!ModelState.IsValid)
            {
                Console.Out.WriteLineAsync("Üyelik İşlemleri sırasında bir hata oluştu.");
                return View(model);
            }
            var result = await _appUserService.CreateAsync(model.Adapt<AppUserCreateDTO>());
            if(result.IsSuccess)
            {
                Console.Out.WriteLineAsync("Üyelik başarıyla oluşturuldu.");
                return RedirectToAction("RegisterSuccess");
            }
            Console.Out.WriteLineAsync("Üyelik İşlemleri sırasında bir hata oluştu.");
            return RedirectToAction("RegisterFailed");

        }
        public async Task<IActionResult> ConfirmEmail(string customerId, string code)
        {
            var customer = await _userManager.FindByIdAsync(customerId);
            await _userManager.ConfirmEmailAsync(customer, code);
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model) 
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customer = await _userManager.FindByEmailAsync(model.Email);
            if (customer == null)
            {
                Console.Out.WriteLineAsync("Kullanıcı veya şifre hatalı");
                return RedirectToAction("LoginFailed");
            }

            var checkPassword = await _signInManager.PasswordSignInAsync(customer, model.Password, false, false);
            if (!checkPassword.Succeeded)
            {
                Console.Out.WriteLineAsync("Kullanıcı veya şifre hatalı");
                return RedirectToAction("LoginFailed");
            }

            var userRole = await _userManager.GetRolesAsync(customer);
            if (userRole == null)
            {
                Console.Out.WriteLineAsync("Kullanıcı veya şifre hatalı");
                return RedirectToAction("LoginFailed");
            }

            return RedirectToAction("LoginSuccess");

        }
        public async Task<IActionResult> RegisterSuccess()
        {
            return View();
        }

        public async Task<IActionResult> RegisterFailed()
        {
            return View();
        }
        public async Task<IActionResult> LoginSuccess()
        {
            return View();
        }

        public async Task<IActionResult> LoginFailed()
        {
            return View();
        }
    }
}
