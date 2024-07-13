using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ChemKit.Datasources.pubchem.Services;
using ChemKit.Datasources.pubchem.Models;

namespace ChemKit.Datasources;

public static class DatasourceServiceCollectionExtensions
{
    public static IServiceCollection AddDatasourceServices(this IServiceCollection services)
    {
        services.AddSingleton<SubstanceService>();
        services.AddSingleton<CompoundService>();

        return services;
    }
}