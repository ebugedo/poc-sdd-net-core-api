using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using poc_sdd_net_core_api.Application.Mappings;
using poc_sdd_net_core_api.Domain.Interfaces;
using poc_sdd_net_core_api.Infrastructure.Data;
using poc_sdd_net_core_api.Infrastructure.Repositories;

namespace poc_sdd_net_core_api.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        // Autofac registrations
        builder.RegisterType<ClientRepository>().As<IClientRepository>().InstancePerLifetimeScope();
        builder.RegisterType<ProjectRepository>().As<IProjectRepository>().InstancePerLifetimeScope();
        builder.RegisterType<SectorRepository>().As<ISectorRepository>().InstancePerLifetimeScope();
        // IUnitOfWork -> ApplicationDbContext (already registered as DbContext)
        builder.Register(c => c.Resolve<ApplicationDbContext>()).As<IUnitOfWork>().InstancePerLifetimeScope();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        // EF Core + PostgreSQL
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Infrastructure")));

        // AutoMapper - scans ClientMappingProfile
        services.AddAutoMapper(typeof(ClientMappingProfile).Assembly);

        // CQRS - MediatR (commands + queries, handlers auto-descubiertos)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ClientMappingProfile).Assembly));

        // Swagger - siempre habilitado para POC (útil tras nginx)
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "POC SDD API", Version = "v1" });
        });

        services.AddEndpointsApiExplorer();

        // Health checks
        services.AddHealthChecks();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Aplicar migraciones automáticamente al iniciar (crea tablas en primera ejecución)
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
        }

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "POC SDD API v1"));

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });
    }
}
