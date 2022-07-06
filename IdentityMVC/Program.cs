using IdentityMVC.ClaimProviders;
using IdentityMVC.CustomMembershipValidations;
using IdentityMVC.Helpers.MailHelpers;
using IdentityMVC.Models;
using IdentityMVC.Models.Entities;
using IdentityMVC.RequirementClaims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
// Add services to the container.

//CONF
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddSingleton(typeof(PasswordResetService));
builder.Services.AddSingleton(typeof(EmailConfirmationService));


builder.Services.AddDbContext<AppDbContext>(optionsAction =>
{
    optionsAction.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddIdentity<User, Role>(idenytityOptions =>
{

    //PASSWORD OPTIONS
    idenytityOptions.Password.RequiredLength = 4; //miniumum karakter default=6
    idenytityOptions.Password.RequireNonAlphanumeric = false;//?.! gibi karakter zounlu olmasın. default=true
    idenytityOptions.Password.RequireLowercase = false;//küçük karaker zounlu olmasın. default=true
    idenytityOptions.Password.RequireUppercase = false;//büyük karakter zorunlu olmasın. default=true
    idenytityOptions.Password.RequireDigit = false;//rakam zorunlu değil. default=true
    idenytityOptions.Password.RequiredUniqueChars = 1;//1 adet benzersiz (farklı karakter olsun) default=1
    //PASSWORD OPTIONS

    //USERNAME OPTIONS
    idenytityOptions.User.AllowedUserNameCharacters = "abcdefghıijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
    idenytityOptions.User.RequireUniqueEmail = true; //benzersiz emaial default= false
    //USERNAME OPTIONS


    //PasswordContainsValidator ile custom PASSWORD OPTIONS.
    //CustomUserValidator ile custom USER OPTIONS.
    //CustomIdentityErrorDescriber ile errorları türkçeleştirme.
    //AddEntityFrameworkStores ile veri tabanı senkronizasyonu.
}).AddPasswordValidator<CustomPasswordValidator>().AddUserValidator<CustomUserValidator>().AddDefaultTokenProviders()./*AddErrorDescriber<CustomIdentityErrorDescriber>().*/AddEntityFrameworkStores<AppDbContext>();

CookieBuilder cookieBuilder = new CookieBuilder()
{
    Name = "CustomIdentityCookie", //cookie ismi
    HttpOnly = true,//
    SameSite = SameSiteMode.Lax,//lax=> url değişirse cookie gonderir,strict=> hiçbir şekilde göndermez, none=> url değişmese de gönderir. güvnli değildir. örn facebook strict yaparsa farklı siteden facebook linkine tıklanınca tekrar login ister, fakat  lax yapılırsa farklı sitdeen gelen GET isteğini kabul eder ve oturum açmaya gerek kalmaz. 
    SecurePolicy = CookieSecurePolicy.SameAsRequest, //hangisinden cookile oluştuysa (http veya https) üzerinden gönderilir. always=>yalnızca https üzerinden gönderir. //none => http üzerinden gönderir. 

};


builder.Services.ConfigureApplicationCookie(cookieAuthenticationOptions =>
{
    cookieAuthenticationOptions.LoginPath = new PathString("/Auths/Login");//cookie'si yoksa giris yap sayfasi
    cookieAuthenticationOptions.LogoutPath = new PathString("/Auths/LogOut");//cookie'si yoksa giris yap sayfasi

    cookieAuthenticationOptions.Cookie = cookieBuilder;
    cookieAuthenticationOptions.SlidingExpiration = true;//Expiration sürsinin yarısı geçtiyse expriation süresi ekler.
    cookieAuthenticationOptions.ExpireTimeSpan = TimeSpan.FromDays(2);
    cookieAuthenticationOptions.AccessDeniedPath= "/Auths/AccessDenied";
});

builder.Services.AddScoped<IClaimsTransformation, ClaimProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, ExpireDateExhangeHandler>();
//dinamik olarak claim ekleme


builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});




builder.Services.AddAuthorization(authorizationOptions =>
{
    //authorizationOptions.AddPolicy("MinAge", authPolicyBuilder =>
    //{
    //    authPolicyBuilder.RequireClaim("birthdate", "10/21/1999 12:00:00 AM");
    //});

    //CLAIM'I DINAMIK OLARAK VERIP YETKILENDIRME(her isteke calısır.)
    authorizationOptions.AddPolicy("EnoughAge", authPolicyBuilder =>
    {
        authPolicyBuilder.RequireClaim("enoughage");
    });


    //CLAIM'I VERİTABANI (userclaims) ILE YETKILENDIRME
    authorizationOptions.AddPolicy("FreeExchangePolicy",authPolicyBuilder =>
    {
        authPolicyBuilder.AddRequirements(new ExpireDateExchangeRequirement());
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "Administration",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
   

    //endpoints.MapAreaControllerRoute(
    //    name: "AdminRoute",
    //    areaName: "administration",
    //    pattern: "admin/{controller=Home}/{action=Index}"
    //    );
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
