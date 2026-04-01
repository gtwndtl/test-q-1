using ws.Models;
using ws.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));

builder.Services.AddSingleton<IUserRepository, UserRepository>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigins");
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    var (existing, _) = await userRepo.GetPaginatedAsync(new ws.Dto.RequestPaginatedUsersDto { Page = 1, PageSize = 1 });
    if (existing.Count == 0)
    {
        var seedUsers = new List<ws.Models.User>
        {
            new() { Firstname = "สมชาย", Lastname = "ใจดี", Address = "123 ถ.สุขุมวิท กรุงเทพฯ", DateOfBirth = new DateTime(1990, 3, 15), Age = 36 },
            new() { Firstname = "สมหญิง", Lastname = "รักเรียน", Address = "456 ถ.พหลโยธิน กรุงเทพฯ", DateOfBirth = new DateTime(1985, 7, 22), Age = 40 },
            new() { Firstname = "วิชัย", Lastname = "สุขสันต์", Address = "789 ถ.รามคำแหง กรุงเทพฯ", DateOfBirth = new DateTime(1995, 1, 10), Age = 31 },
            new() { Firstname = "นภา", Lastname = "พรมมา", Address = "321 ถ.เพชรบุรี กรุงเทพฯ", DateOfBirth = new DateTime(1992, 11, 5), Age = 33 },
            new() { Firstname = "ธนา", Lastname = "ศรีสุข", Address = "654 ถ.ลาดพร้าว กรุงเทพฯ", DateOfBirth = new DateTime(1988, 6, 18), Age = 37 },
            new() { Firstname = "พิมพ์", Lastname = "จันทร์แก้ว", Address = "987 ถ.รัชดาภิเษก กรุงเทพฯ", DateOfBirth = new DateTime(2000, 9, 30), Age = 25 },
            new() { Firstname = "อนุชา", Lastname = "มั่นคง", Address = "147 ถ.บางนา กรุงเทพฯ", DateOfBirth = new DateTime(1998, 4, 12), Age = 28 },
            new() { Firstname = "สุดา", Lastname = "แสงทอง", Address = "258 ถ.สาทร กรุงเทพฯ", DateOfBirth = new DateTime(1993, 8, 25), Age = 32 },
            new() { Firstname = "กิตติ", Lastname = "วงษ์สุวรรณ", Address = "369 ถ.สีลม กรุงเทพฯ", DateOfBirth = new DateTime(1997, 2, 14), Age = 29 },
            new() { Firstname = "ปราณี", Lastname = "ดีใจ", Address = "741 ถ.เจริญกรุง กรุงเทพฯ", DateOfBirth = new DateTime(1991, 12, 1), Age = 34 }
        };

        foreach (var user in seedUsers)
        {
            await userRepo.CreateAsync(user);
        }
        Console.WriteLine("Seeded 10 users successfully.");
    }
}

app.Run();

