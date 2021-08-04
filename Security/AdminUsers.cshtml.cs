using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace InnovationTest.Pages.Admin
{
    public class AdminUsersModel : PageModel
    {
        
        public string Result { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required(ErrorMessage = "Ulanyjynyň wezipesini saýlaň")]
            public string Roles { get; set; }

            [Required(ErrorMessage ="Email giriziň.")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage ="Paroly giriziň.")]
            [StringLength(100, ErrorMessage = "{0}yň yzynlygy azyndan {2} we  {1} simwoldan köp bolmaly däl.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Parol")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Paroly tassyklaň")]
            [Compare("Password", ErrorMessage = "Parol gabat gelmeýär.")]
            public string ConfirmPassword { get; set; }
        }

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AdminUsersModel> _logger;

        private readonly RoleManager<IdentityRole> roleManager;

        public AdminUsersModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<AdminUsersModel> logger,
             RoleManager<IdentityRole> _roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;

            roleManager = _roleManager;
        }
        public void OnGet(int kod)
        {
        if(kod==1)
                Result = "Täze ulanyjy döredildi.";
        else
            Result = "Täze ulanyjyny döretmek";
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
          
            returnUrl ??= Url.Content("~/");
            Result = "Ýalňyşlyk ýüze çykdy. Täze ulanyjy döredilmedi.";
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {


                    ////////////////////////
                    ///

                    IdentityResult IR;


                    if (roleManager == null)
                    {
                        throw new Exception("roleManager null");
                    }

                    if (!await roleManager.RoleExistsAsync(Input.Roles))
                    {
                        IR = await roleManager.CreateAsync(new IdentityRole(Input.Roles));
                    }

                    //var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

                    var userX = await _userManager.FindByIdAsync(user.Id);

                    if (user == null)
                    {
                        throw new Exception("The password was probably not strong enough!");
                    }

                    IR = await _userManager.AddToRoleAsync(user, Input.Roles);

                    // return IR;


                    //////////////////////


                    _logger.LogInformation("User created a new account with password.{usre}",Input.Email);

                    IPAddress iPAdress = Request.HttpContext.Connection.RemoteIpAddress;
                    _logger.LogInformation("User Ip adress: {IpAdress},", iPAdress.ToString());
                   
                    
                    //Input.Email = "";
                    //Input.Roles = "";
                    // ModelState.Clear();
                    return RedirectToPage("./AdminUsers" ,new  {kod=1 });

                }
                foreach (var error in result.Errors)
                {
                    Result = "Ýalňyşlyk ýüze çykdy. Täze ulanyjy döredilmedi.";
                    ModelState.AddModelError(string.Empty, error.Description);
             
                }


                // If we got this far, something failed, redisplay form
            }
            return Page();


        }
    }
}
