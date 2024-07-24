using Habanerio.Core.DBs.MongoDB.EFCore;
using Habanerio.Samples.Customers.MongoEFCore;
using Habanerio.Samples.Customers.MongoEFCore.Repositories;
using Microsoft.EntityFrameworkCore;

/******** Test Container ********
await using var mongoContainer = new MongoDbBuilder()
    .WithImage("mongo:6.0")
    .Build();

await mongoContainer.StartAsync();
var mongoConnectionString = mongoContainer.GetConnectionString();

var mongoDBSettings = builder.Configuration.GetSection("SampleMongoDBSettings").Find<MongoDbOptions>();
builder.Services.AddDbContext<SampleMongoDbContext>(options =>
   options.UseMongoDB(mongoConnectionString, mongoDBSettings.DatabaseName)
       .EnableSensitiveDataLogging()
       .EnableDetailedErrors());
********************************/




var builder = WebApplication.CreateBuilder(args);

//builder.AddMongoDBClient("sample-mongo");

// Add services to the container.
builder.Services.AddControllersWithViews();

/************* With DbContextOptions *************/

var mongoDBSettings = builder.Configuration.GetSection("SampleMongoDBSettings").Get<MongoDbOptions>();
//builder.Services.Configure<MongoDbOptions>(builder.Configuration.GetSection("SampleMongoDBSettings"));

builder.Services.AddDbContext<SampleMongoDbContext>(options =>
    options.UseMongoDB(mongoDBSettings.ConnectionString, mongoDBSettings.DatabaseName)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors());


/************* With IOptions<MongoDbOptions> *************
builder.Services.AddOptions<MongoDbOptions>()
    .BindConfiguration("SampleMongoDBSettings");
*************************************************/


builder.Services.AddScoped<ICustomersRepository, CustomersRepository>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
