using System.Security.Claims;
using HomestayWeb.Hubs;
using HomestayWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages
builder.Services.AddRazorPages();
builder.Services.AddSession(); // Thêm dòng này để kích hoạt Session

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;
    options.AccessDeniedPath = "/Forbidden";
    options.LogoutPath = "/Logout/Index";
    options.LoginPath = "/Login/Index";
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = "/signin-google";
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim("Role", Role.ADMIN.ToString()));
});

// Database
builder.Services.AddDbContext<ProjectHomeStayContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddSignalR();

var app = builder.Build();

// Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.MapGet("/signin-google", async (HttpContext http) =>
{
    var result = await http.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    if (result?.Principal is not null)
    {
        var email = result.Principal.FindFirstValue(ClaimTypes.Email);
        http.Response.Redirect("/Index");
    }
    else
    {
        http.Response.Redirect("/Login/Index");
    }
});
app.UseSession(); // Và dòng này để sử dụng Session

app.MapHub<ClientHub>("/clientHub");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();
