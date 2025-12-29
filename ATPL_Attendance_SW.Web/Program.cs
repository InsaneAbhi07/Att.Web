var builder = WebApplication.CreateBuilder(args);

// =====================
// SERVICES (BEFORE BUILD)
// =====================

// MVC
builder.Services.AddControllersWithViews();

// COOKIE AUTHENTICATION
builder.Services.AddAuthentication("AttendanceCookie")
    .AddCookie("AttendanceCookie", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

// AUTHORIZATION
builder.Services.AddAuthorization();


// =====================
// BUILD APP
// =====================
var app = builder.Build();


// =====================
// MIDDLEWARE PIPELINE
// =====================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   // 👈 MUST be before Authorization
app.UseAuthorization();


// =====================
// ROUTING
// =====================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
