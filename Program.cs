using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BakingIt.Services;

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
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation(); // requires dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation --version 8.0.4
builder.Services.AddScoped(typeof(IBakingItRepository<>), typeof(EBakingItRepository<>));
builder.Services.AddScoped<IMeasureService, MeasureService>();
builder.Services.AddScoped<IPantryIngredientService, PantryIngredientService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
};

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

app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
