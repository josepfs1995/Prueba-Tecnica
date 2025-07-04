using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Navisaf.Application.Common.Interfaces;
using Navisaf.Application.Common.PipelineBehavior;
using Navisaf.Application.Features.Orders.Command;
using Navisaf.Infra.Persistence;

namespace Navisaf.Web.Configurations;

public static class ApplicationConfiguration
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NavisafDbContext>(option =>
        {
            option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IApplicationDbContext>( provider => provider.GetRequiredService<NavisafDbContext>());
        services.AddValidatorsFromAssembly(typeof(CreateOrderCommandValidator).Assembly);
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<CreateOrderCommand>();
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggerBehavior<,>));
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });
        return services;
    }
}