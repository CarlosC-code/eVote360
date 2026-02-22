using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using eVote360.Core.Application;
using eVote360.Core.Application.Interface;
using eVote360.Infrastructure.Persistence;
using eVote360.Middlewares;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddPersistenceLayerIoc(builder.Configuration);
builder.Services.AddApplicationLayerIoc();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<eVote360.Core.Application.Mapping.AdminProfile>();
    cfg.AddProfile<eVote360.Core.Application.Mapping.DirigenteProfile>();
    cfg.AddProfile<eVote360.Core.Application.Mapping.EleccionesProfile>();
    cfg.AddProfile<eVote360.Core.Application.Mapping.VotacionProfile>();
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserSession, UserSession>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Elector}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
