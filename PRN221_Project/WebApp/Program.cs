using Microsoft.EntityFrameworkCore;
using DataAccessLayer.DbContext;
using System.Net;
using System.Net.Mail;
using WebApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews(); // Ensure this is added

// Add session services
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30); // Adjust timeout as needed
});

// Configure SMTP settings
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// Register EmailService
builder.Services.AddTransient<EmailService>();
builder.Services.Configure<ImageUploadSettings>(builder.Configuration.GetSection("ImageUploadSettings"));
builder.Services.AddTransient<ImageUploadService>();
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session middleware
app.UseSession();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapHub<ChatHubs>("/ChatHubs");

// Add default route for controllers
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public class SmtpSettings
{
	public string Host { get; set; }
	public int Port { get; set; }
	public string Username { get; set; }
	public string Password { get; set; }
	public bool EnableSsl { get; set; }
	public bool UseDefaultCredentials { get; set; }
}

public class ImageUploadSettings
{
	public string UploadPath { get; set; }
	public List<string> AllowedExtensions { get; set; }
	public long MaxFileSize { get; set; }
}
