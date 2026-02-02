using MongoDB.Driver;
using OpenApiUi;
using ChatApp.Models;

var builder = WebApplication.CreateBuilder(args);

// ---- MongoDB DI ----

// Register MongoClient
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    return new MongoClient("mongodb+srv://copyrighton21_db_user:wolverine65@cluster0.dad9rq0.mongodb.net/?appName=Cluster0&authSource=admin");
});

// Register Users collection
builder.Services.AddSingleton<IMongoCollection<Users>>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase("Chatters");

    // Use the generic collection type Users, which is BSON-mapped
    return database.GetCollection<Users>("users");
});

builder.Services.AddSingleton<IMongoCollection<Chats>>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase("Chatters");
    return database.GetCollection<Chats>("chats");
});


// ---- MVC + OpenAPI ----
builder.Services.AddControllersWithViews();
builder.Services.AddOpenApi();

// ---- Session ----
builder.Services.AddDistributedMemoryCache(); // session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); // session lifetime
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// ---- Middleware pipeline ----
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseOpenApiUi(config =>
    {
        config.OpenApiSpecPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // CSS/JS/images for Views

app.UseRouting();
app.UseSession(); // enable session before MVC
app.UseAuthorization();

// MVC route for website
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// API controllers
app.MapControllers();

app.Run();
