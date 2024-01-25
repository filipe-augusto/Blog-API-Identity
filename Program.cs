using Blog;
using Blog.Data;
using Blog.Services;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);
ConfigureAuthentication(builder);

ConfigureMvc(builder);
ConfigureService(builder);


builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();
LoadConfiguration(app);


//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//    Console.WriteLine("Estou em desenvolvimento");
//}

app.UseAuthentication();//1° quem voce é 
app.UseAuthorization();//2° o que pode fazer
app.UseStaticFiles();//para uploads de arquivos
app.MapControllers();
app.Run();


void LoadConfiguration(WebApplication app)
{
    Configuration.Jwtkey = app.Configuration.GetValue<string>("Jwtkey");
    Configuration.ApiKeyName = app.Configuration.GetValue<string>("ApiKeyName");
    Configuration.ApiKey = app.Configuration.GetValue<string>("ApiKey");

   // var stmp = new Configuration.SmtpConfiguration(); não usando por enquanto
   // app.Configuration.GetSection("StmpConfiguration").Bind(stmp);
}

void ConfigureAuthentication(WebApplicationBuilder builder)
{
    var key = Encoding.ASCII.GetBytes(Configuration.Jwtkey);

    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });
}

void ConfigureMvc(WebApplicationBuilder builder)
{
    builder.Services.AddMemoryCache();
    builder.Services.AddResponseCompression(option =>
    {
        option.Providers.Add<GzipCompressionProvider>();

    });
    builder.Services.Configure<GzipCompressionProviderOptions>(Options => {
        Options.Level = CompressionLevel.Optimal;
    });


    builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options => 
    {
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(x => {

        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    });





}

void ConfigureService(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");




    builder.Services.AddDbContext<BlogDataContext>(options =>
    {
        options.UseSqlServer(connectionString);
    });//AddDbContext para dbcontext
    builder.Services.AddTransient<TokenService>(); //sempre cria um novo
                                                   //builder.Services.AddScoped(); //por transação - pode usar para o dbcontext,pois se usar o AddTransient para criar uma nova conexão com um pouco não é uma boa pratica abrir varias conexões com o banco
                                                   //builder.Services.AddSingleton(); // singleton => 1 por app. Sempre esta na aplicação.uma vez que você chamou o objeto ele carrega para memoria e só quando pare a aplicação e rode ela novamente ai ele vai tira ele todo da memoria.

   // builder.Services.AddTransient<EmailService>();
}



//dotnet add package microsoft.entityframeworkcore.sqlserver
//dotnet add package microsoft.entityframeworkcore.design
//dotnet add package Microsoft.AspnetCore.Authentication
//dotnet add package Microsoft.AspnetCore.Authentication.JwtBearer
//dotnet add package SecureIdentity


//------------------------------------------
//    alteração no banco
//     dotnet ef migrations add createdatabase
//        dotnet ef database update