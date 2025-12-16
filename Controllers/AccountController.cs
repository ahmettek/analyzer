using System.Security.Claims;
using DotNet8.WebApi.Data;
using DotNet8.WebApi.Entities;
using DotNet8.WebApi.Models.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNet8.WebApi.Controllers;

public class AccountController(AppDbContext dbContext) : Controller
{
    [HttpPost]
    public async Task<ActionResult> SignUp([FromBody] SignUpRequestDto request, CancellationToken cancellationToken)
    {
        var existingAccount = await dbContext.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

        if (existingAccount != null)
        {
            return View("SignUp", new ResponseBase 
            { 
                Message = "Mail adresinizi veya şifrenizi hatalı girdiniz" 
            });
        }

        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(cancellationToken);

        await CreateIdentityAsync(request.Email, account.Id);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Login(string email, CancellationToken cancellationToken)
    {
        var account = await dbContext.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Email == email, cancellationToken);

        if (account != null && BCrypt.Net.BCrypt.Verify("123456", account.Password))
        {
            await CreateIdentityAsync(email, account.Id);
            return RedirectToAction("Index", "Home");
        }

        return View("Login", new ResponseBase { Success = false, Message = "Email veya şifreniz yanlış." });
    }

    [HttpGet]
    public async Task<IActionResult> Test(string email, CancellationToken cancellationToken)
    {
        var account = await dbContext.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Email == email, cancellationToken);

        return Json(new ResponseBase { Success = false, Message = account?.Email ?? string.Empty });
    }

    private async Task CreateIdentityAsync(string email, Guid accountId)
    {
        var claims = new List<Claim>
        {
            new("AccountId", accountId.ToString()),
            new("Email", email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var props = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(3)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
    }
}

