using Microsoft.Extensions.DependencyInjection;
using Employees.Application.Parsing;
using Employees.Application.Storage;
using Employees.Infrastructure.Parsing;
using Employees.Infrastructure.Storage;

namespace Employees.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<ICsvParser, CsvParser>();
            services.AddSingleton<IParsedDataStore, InMemoryParsedDataStore>();
            return services;
        }
    }
}
