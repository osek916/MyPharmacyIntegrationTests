using Microsoft.AspNetCore.Mvc.Testing;
using MyPharmacy;
using Xunit;
using FluentAssertions;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MyPharmacy.Controllers;
using System.Collections.Generic;
using System;
using FluentValidation;

namespace MyPharmacyIntegrationTests.Tests
{
    public class StartupTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private List<Type> _controllerNames;
        private readonly WebApplicationFactory<Startup> _factory;

        public StartupTests(WebApplicationFactory<Startup> factory)
        {
             _controllerNames = typeof(Startup)
                .Assembly
                .GetTypes()
                .Where(c => c.IsSubclassOf(typeof(ControllerBase)))
                .ToList();


            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    _controllerNames.ForEach(c => services.AddScoped(c));
                });
            });
        }

        [Fact]
        public void ConfigureServices_Controllers_RegistersAllDependencies()
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();

            _controllerNames.ForEach(c =>
            {
                var controller = scope.ServiceProvider.GetService(c);
                controller.Should().NotBeNull();
            });
        }       
    }
}
