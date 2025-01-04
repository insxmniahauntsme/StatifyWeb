using StatifyWeb.Interfaces;
using StatifyWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient("SpotifyClient", client =>
{
	var baseUrl = builder.Configuration["SpotifyAPI:base-url"];
	if (string.IsNullOrWhiteSpace(baseUrl))
	{
		throw new Exception("Couldn't get API base URL.");
	}

	client.BaseAddress = new Uri(baseUrl);
	client.Timeout = TimeSpan.FromSeconds(30);
	client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHttpClient("LocalhostClient", client =>
{
	client.BaseAddress = new Uri("https://localhost:8081/");
	client.Timeout = TimeSpan.FromSeconds(30);
	client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddTransient<IAccessTokenService, AccessTokenService>();

builder.Services.AddCors(options =>
{
	options.AddPolicy("StatifyClient", policy =>
	{
		policy.WithOrigins("https://localhost:8081") 
			.AllowAnyMethod() 
			.AllowAnyHeader(); 
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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseCors("StatifyClient");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();