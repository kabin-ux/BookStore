using BookStore.Entities;
using BookStore.Services;
using BookStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDBContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookService, BookService>();
//builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddIdentity<Users, Roles>().AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.AddScoped<JwtTokenService>();


var jwtConfig = builder.Configuration.GetSection(JwtOptions.SectionName);

builder.Services.AddOptions<JwtOptions>()
.Bind(jwtConfig)
.ValidateDataAnnotations();

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    var jwtOptions = jwtConfig.Get<JwtOptions>()!;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtOptions.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtOptions.Audience,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = jwtOptions.SymmetricSecurityKey,

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
