using WebFrameworks_CA2.Components;
using WebFrameworks_CA2.Components.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpClient<MovieService>()
    .Services.AddScoped<MovieService>();
    
builder.Services
    .AddHttpClient<CinemaService>()
    .Services.AddScoped<CinemaService>();


// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();