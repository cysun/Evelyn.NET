using Evelyn.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5001");

var configuration = builder.Configuration;

// Configure Services
var services = builder.Services;
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

app.UsePathBase(configuration.GetValue<string>("Application:PathBase"));
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
