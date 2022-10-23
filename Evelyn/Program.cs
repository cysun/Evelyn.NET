using Evelyn.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment;
var configuration = builder.Configuration;
var services = builder.Services;

// In production, this app will sit behind a Nginx reverse proxy with HTTPS
if (!environment.IsDevelopment())
    builder.WebHost.UseUrls("http://localhost:5010");

// Configure Services
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
services.AddAuthorization(options =>
{
    options.AddPolicy("IsAuthenticated", policyBuilder => policyBuilder.RequireAuthenticatedUser());
});

services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter("IsAuthenticated"));
});

services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
services.AddScoped<UserService>();
services.AddScoped<FileService>();
services.AddScoped<BookService>();
services.AddScoped<ChapterService>();
services.AddScoped<EBookService>();
services.AddScoped<BookmarkService>();

// Build App
var app = builder.Build();

// Configure Middleware Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        "default",
        "{controller=Account}/{action=Login}/{id?}");
});

// Run App
app.Run();
