using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StudentInfoLoginRoles.Components;
using StudentInfoLoginRoles.Models;
using StudentInfoLoginRoles.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//changing this to AddIdentity from AddDefaultIdentity to enable roles
//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddSignalR();
var app = builder.Build();
//add seed data method call to create roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await StudentInfoLoginRoles.Data.SeedData.InitializeAsync(services);
    // Assign admin role to Admin user
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var adminUser = await userManager.FindByEmailAsync("admin@asdf.com");//password is asdf1234A/1
    if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.MapRazorComponents<ImageGenerator>()
    .AddInteractiveServerRenderMode();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chathub");
app.Run();
