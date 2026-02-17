using Ledgerium.Infrastructure.Persistence;
using Ledgerium.Web.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL") 
             ?? builder.Configuration.GetValue<string>("Seq:Url");

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()  // поддержка BeginScope
    .Enrich.WithMachineName() // имя инстанса
    .Enrich.WithProperty("Application", "Ledgerium")
    .WriteTo.Console()
    .WriteTo.Seq(seqUrl) // Seq из Docker Compose
    .CreateLogger();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // слушаем все интерфейсы внутри контейнера
});

builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WalletDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});

var modules = new Ledgerium.Application.Abstractions.IModule[]
{
    new Ledgerium.Application.ApplicationModule(),
    new Ledgerium.Infrastructure.InfrastructureModule()
};

foreach (var module in modules)
{
    module.Register(builder.Services, builder.Configuration);
}

var app = builder.Build();
var isMigrate = args.Contains("--migrate");
if (isMigrate)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
    await db.Database.MigrateAsync();
}

app.UseMiddleware<ProblemDetailsExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();