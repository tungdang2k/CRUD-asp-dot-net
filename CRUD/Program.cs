using CRUD.Data;
using CRUD.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration
                               .GetConnectionString("Redis");

    return ConnectionMultiplexer.Connect(configuration!);
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfire((sp, config) =>
{ 
    var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("DBConnection");
    config.UseSqlServerStorage(connectionString);
});
builder.Services.AddHangfireServer();


builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<RedisQueueService>();
builder.Services.AddScoped<StudentJobService>();
builder.Services.AddHostedService<QueueWorkerService>();
builder.Services.AddSingleton<QueueMonitorService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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

app.UseAuthorization();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=SinhVien}/{action=Index}/{id?}");

app.UseHangfireDashboard("/hangfire");
RecurringJob.AddOrUpdate<QueueMonitorService>(
    "RedisQueueMonitor",
    x => x.CheckQueueAsync(),
    Cron.Minutely);
app.Run();
