﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using EC_Website.Core.Entities.User;

namespace EC_Website.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public bool SignInAfterRegister { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Required to accept Terms of Use")]
            [Display(Name = "Terms of Use")]
            public bool AgreeTermsOfUsage { get; set; }
        }

        public void OnGet(string returnUrl = null, bool signInAfterRegister = true)
        {
            ReturnUrl = returnUrl;
            SignInAfterRegister = signInAfterRegister;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null, bool signInAfterRegister = true)
        {
            returnUrl ??= Url.Content("~/");
            var validCaptcha = await CheckCaptchaResponseAsync();

            if (!validCaptcha)
                ModelState.AddModelError("captcha", "Invalid captcha verification");

            if (!ModelState.IsValid) 
                return Page();

            var user = new ApplicationUser()
            {
                UserName = Input.Username,
                Email = Input.Email,
                ProfilePhotoPath = "/img/default_user_avatar.jpg",
                HeaderPhotoPath = "/img/default_user_header.jpg"
            };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail", 
                    null,
                    new { userId = user.Id, code },
                    Request.Scheme);

                await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                if (signInAfterRegister)
                {
                    await _signInManager.SignInAsync(user, false);
                }
                
                return LocalRedirect(returnUrl);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        private async Task<bool> CheckCaptchaResponseAsync()
        {
            const string captchaApiUrl = "https://www.google.com/recaptcha/api/siteverify";
            var captchaResponse = HttpContext.Request.Form["g-Recaptcha-Response"].ToString();
            var secretKey = _configuration.GetSection("GoogleRecaptchaV2:SecretKey").Value;
            var httpClient = new HttpClient();
            var postQueries = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("secret", secretKey),
                new KeyValuePair<string, string>("response", captchaResponse)
            };

            var response = await httpClient.PostAsync(new Uri(captchaApiUrl), new FormUrlEncodedContent(postQueries));
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonData = JObject.Parse(responseContent);
            return bool.Parse(jsonData["success"].ToString());
        }
    }
}