using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MyPharmacy;
using MyPharmacy.Entities;
using MyPharmacy.Interfaces;
using System;
using System.Collections.Generic;

namespace MyPharmacyIntegrationTests
{
    public static class TestSeeder// <T> where T : IPharmacy
    {        
        /*
        public static void SeedGenericData(T table, WebApplicationFactory<Startup> _factory)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            if(table is IPharmacy)
            {
                Pharmacy p = (Pharmacy)table;
                _dbContext.Pharmacies.Add();
                _dbContext.SaveChanges();
            }   
        }
        */

        public static void SeedPharmacy(Pharmacy pharmacy, WebApplicationFactory<Startup> _factory)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            _dbContext.Pharmacies.Add(pharmacy);
            _dbContext.SaveChanges();
        }

        public static void SeedStatus(Status status, WebApplicationFactory<Startup> _factory)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            _dbContext.Statuses.Add(status);
            _dbContext.SaveChanges();
        }

        public static void SeedUser(User user, WebApplicationFactory<Startup> _factory)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        public static void SeedOrderForPharmacy(OrderForPharmacy orderForPharmacy, WebApplicationFactory<Startup> _factory)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            _dbContext.OrderForPharmacies.Add(orderForPharmacy);
            _dbContext.SaveChanges();
        }

        public static void SeedDrugInformation(DrugInformation drugInformation, WebApplicationFactory<Startup> _factory)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            _dbContext.DrugInformations.Add(drugInformation);
            _dbContext.SaveChanges();
        }


    }
}
