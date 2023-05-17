var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddHsts(options =>
//{
//    options.Preload = true;
//    options.IncludeSubDomains = true;
//    options.MaxAge = TimeSpan.FromDays(365);
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseForwardedHeaders();
    app.UseHsts();
    //app.UseXfo(option =>
    //{
    //    option.Deny();
    //});

}
//app.UseForwardedHeaders();
//app.UseHsts();
//app.UseCors(
//    builder => builder
//    .AllowAnyOrigin()
//    .AllowAnyMethod()
//    .AllowAnyHeader()
//    );
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "upgrade-insecure-requests");
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "NotFound",
    pattern: "{controller=Errors}/{action=Error404}");

app.Run();
