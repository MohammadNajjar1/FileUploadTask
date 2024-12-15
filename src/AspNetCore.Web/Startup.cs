using AutoMapper;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using AspNetCore.Components.Extensions;
using AspNetCore.Components.Logging;
using AspNetCore.Components.Mail;
using AspNetCore.Components.Mvc;
using AspNetCore.Components.Security;
using AspNetCore.Controllers;
using AspNetCore.Data;
using AspNetCore.Data.Migrations;
using AspNetCore.Objects;
using AspNetCore.Resources;
using AspNetCore.Services;
using AspNetCore.Validators;
using NonFactors.Mvc.Grid;
using System.Diagnostics.CodeAnalysis;

namespace AspNetCore.Web;

[ExcludeFromCodeCoverage]
public class Startup
{
    private IConfiguration Config { get; }
    private IHostEnvironment Environment { get; }

    public Startup(IHostEnvironment host)
    {
        Environment = host;
        Config = new ConfigurationBuilder()
            .SetBasePath(host.ContentRootPath)
            .AddJsonFile("configuration.json")
            .AddJsonFile($"configuration.{host.EnvironmentName.ToLower()}.json", optional: true)
            .AddEnvironmentVariables($"AspNetCore__{host.EnvironmentName}__")
            .Build();
    }

    public void Configure(IApplicationBuilder app)
    {
        SeedDatabase(app);

        RegisterResources();
        RegisterConstants(app);
        RegisterMiddleware(app);
    }
    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureMvc(services);
        ConfigureOptions(services);
        ConfigureDependencies(services);
    }

    private void ConfigureMvc(IServiceCollection services)
    {
        services
            .AddMvc()
            .AddMvcOptions(options =>
            {
                options.Filters.Add<LanguageFilter>();
                options.Filters.Add<TransactionFilter>();
                options.Filters.Add<AuthorizationFilter>();
                ModelMessagesProvider.Set(options.ModelBindingMessageProvider);
                options.ModelMetadataDetailsProviders.Add(new DisplayMetadataProvider());
                options.ModelBinderProviders.Insert(4, new TrimmingModelBinderProvider());
            })
            .AddRazorOptions(options => options.ViewLocationExpanders.Add(new ViewLocationExpander()))
            .AddViewOptions(options => options.ClientModelValidatorProviders.Add(new ClientValidatorProvider()));

        services.AddAuthentication("Cookies").AddCookie(authentication =>
        {
            authentication.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            authentication.Cookie.Name = Config["Cookies:Auth"];
            authentication.Events = new AuthenticationEvents();
            authentication.Cookie.HttpOnly = true;
        });

        services.AddMvcGrid(filters =>
        {
            filters.BooleanFalseOptionText = () => Resource.ForString("No");
            filters.BooleanTrueOptionText = () => Resource.ForString("Yes");
        });

        services.AddLogging(builder =>
        {
            builder.AddConfiguration(Config.GetSection("Logging"));

            if (Environment.IsDevelopment())
                builder.AddConsole();
            else
                builder.AddProvider(new FileLoggerProvider(Config["Logging:File:Path"], Config.GetValue<Int64>("Logging:File:RollSize")));
        });
    }
    private void ConfigureOptions(IServiceCollection services)
    {
        services.Configure<RouteOptions>(options => options.ConstraintMap["slug"] = typeof(SlugifyTransformer));
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.Configure<MailConfiguration>(Config.GetSection("Mail"));
        services.Configure<CookieTempDataProviderOptions>(tempData =>
        {
            tempData.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            tempData.Cookie.Name = Config["Cookies:TempData"];
            tempData.Cookie.HttpOnly = true;
        });
        services.Configure<AntiforgeryOptions>(antiforgery =>
        {
            antiforgery.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            antiforgery.Cookie.Name = Config["Cookies:Antiforgery"];
            antiforgery.FormFieldName = "_Token_";
            antiforgery.Cookie.HttpOnly = true;
        });
        services.Configure<SessionOptions>(session =>
        {
            session.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            session.Cookie.Name = Config["Cookies:Session"];
            session.Cookie.HttpOnly = true;
        });
    }
    private void ConfigureDependencies(IServiceCollection services)
    {
        services.AddSession();
        services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

        services.AddScoped<Configuration>();
        services.AddDbContext<DbContext, Context>(options =>
        {
            options.EnableSensitiveDataLogging();
            options.UseSqlServer(Config["Data:Connection"]);
        });
        services.AddScoped<IUnitOfWork>(provider => new AuditedUnitOfWork(
            provider.GetRequiredService<DbContext>(),
            provider.GetRequiredService<IMapper>(),
            provider.GetRequiredService<IHttpContextAccessor>().HttpContext?.User.Id() ?? 0));

        services.AddSingleton<IHasher, BCrypter>();
        services.AddSingleton<IMailClient, SmtpMailClient>();
        services.AddSingleton<IValidationAttributeAdapterProvider, ValidationAdapterProvider>();
        services.AddSingleton<IAuthorization>(provider => new Authorization(typeof(AController).Assembly, provider));
        services.AddSingleton(new MapperConfiguration(mapper => mapper.AddMaps(typeof(AView).Assembly)).CreateMapper());

        Language[] supported = Config.GetSection("Languages:Supported").Get<Language[]>();
        services.AddSingleton<ILanguages>(new Languages(Config["Languages:Default"], supported));

        services.AddSingleton<ISiteMap>(provider => new SiteMap(
            File.ReadAllText(Config["SiteMap:Path"]), provider.GetRequiredService<IAuthorization>()));

        services.AddScopedImplementations<IService>();
        services.AddScopedImplementations<IValidator>();

        IServiceProvider provider = services.BuildServiceProvider();
        LinkGenerator generator = provider.GetRequiredService<LinkGenerator>();

        services.Remove(services.First(descriptor => descriptor.ServiceType == typeof(LinkGenerator)));
        services.AddSingleton<LinkGenerator>(new DefaultLinkGenerator(generator));
    }

    private void RegisterResources()
    {
        if (Config["Resources:Path"] is String path && Directory.Exists(path))
            foreach (String resource in Directory.GetFiles(path, "*.json", SearchOption.AllDirectories))
            {
                String type = Path.GetFileNameWithoutExtension(resource);
                String language = Path.GetExtension(type).TrimStart('.');
                type = Path.GetFileNameWithoutExtension(type);

                Resource.Set(type).Override(language, File.ReadAllText(resource));
            }

        foreach (Type view in typeof(AView).Assembly.GetTypes())
        {
            Type type = view;

            while (typeof(AView).IsAssignableFrom(type.BaseType))
            {
                Resource.Set(view.Name).Inherit(Resource.Set(type.BaseType!.Name));

                type = type.BaseType;
            }
        }
    }
    private void RegisterConstants(IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }
    private void RegisterMiddleware(IApplicationBuilder app)
    {
        if (Environment.IsDevelopment())
            app.UseMiddleware<DeveloperExceptionPageMiddleware>();
        else
            app.UseMiddleware<ErrorResponseMiddleware>();

        app.UseMiddleware<SecureHeadersMiddleware>();

        app.UseHttpsRedirection();

        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = response => response.Context.Response.Headers["Cache-Control"] = "max-age=8640000"
        });

        app.UseSession();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute("MultiArea", "{language}/{area:slug:exists}/{controller:slug}/{action:slug=Index}/{id:long?}");
            endpoints.MapControllerRoute("DefaultArea", "{area:slug:exists}/{controller:slug}/{action:slug=Index}/{id:long?}");
            endpoints.MapControllerRoute("Multi", "{language}/{controller:slug}/{action:slug=Index}/{id:long?}");
            endpoints.MapControllerRoute("Default", "{controller:slug}/{action:slug=Index}/{id:long?}");
            endpoints.MapControllerRoute("Home", "{controller:slug=Home}/{action:slug=Index}");
        });
    }

    private void SeedDatabase(IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using Configuration configuration = scope.ServiceProvider.GetRequiredService<Configuration>();

        configuration.Migrate();
    }
}
