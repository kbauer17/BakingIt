using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BakingIt.Services;
using BakingIt.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with SQL Server connection
builder.Services.AddDbContext<BakingItContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => 
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,  // Retries up to 5 times
                maxRetryDelay: TimeSpan.FromSeconds(3), // Waits 10 seconds between attempts
                errorNumbersToAdd: null // Accepts additional error codes if needed
            );
        }));

// Add services to the container.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // tweak password/lockout settings here, e.g.:
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<BakingItContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews(options =>
    options.Filters.Add<BakingIt.Filters.AutoPopulateModelErrorsAttribute>())
    .AddRazorRuntimeCompilation(); // requires dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation --version 8.0.4
builder.Services.AddScoped(typeof(IBakingItRepository<>), typeof(EBakingItRepository<>));
builder.Services.AddScoped<IMeasureService, MeasureService>();
builder.Services.AddScoped<IPantryIngredientService, PantryIngredientService>();
builder.Services.AddScoped<IRecipeCalculationService, RecipeCalculationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
};

// If in Development and Seeder is set to true, Seed an admin and basic roles
if (app.Environment.IsDevelopment()
    && builder.Configuration.GetValue<bool>("SeedAdmin:Enabled"))
{
    using var scope = app.Services.CreateScope();
    var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    await IdentitySeeder.SeedAsync(scope.ServiceProvider, cfg);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
};

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
    {
        // Map explicit route for login for all users (authenticated or not)
        // This allows the Login getter method to apply logic depending upon authentication state of the User
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=Login}/{id?}"
        ).AllowAnonymous(); // Allow unauthenticated access to the Login action

        // Apply global authorization policy to all other routes
        // endpoints.MapControllerRoute(
        //     name: "authenticatedDefault",
        //     pattern: "{controller=Home}/{action=Index}/{id?}"
        // ).RequireAuthorization(_RequireAuthenticatedUserPolicy);
        
    });

app.Run();
