using GadgetMarket;
using GadgetMarket.Repositories;
using GadgetMarket.Web;
using GadgetMarket.Web.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<AppExceptionFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddTransient<UsersRepository>();
//builder.Services.AddScoped<UsersRepository>();

builder.Services.AddSingleton<IUsersRepository, UsersRepository>();
builder.Services.AddSingleton<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddSingleton<IGadgetsRepository, GadgetsRepository>();
builder.Services.AddSingleton<DbConfig>(services =>
{
    var configuration = services.GetRequiredService<IConfiguration>();
    return new DbConfig
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection")
    };
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = Auth.ISSUER,
            ValidateAudience = true,
            ValidAudience = Auth.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = Auth.GetSecurityKey(),
            ValidateIssuerSigningKey = true
        };
    });


var app = builder.Build();

app.UseCors(x => x
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .SetIsOriginAllowed(origin => true)
                  .AllowCredentials());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
