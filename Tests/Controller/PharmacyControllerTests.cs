using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyPharmacy;
using MyPharmacy.Entities;
using MyPharmacy.Models;
using MyPharmacyIntegrationTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MyPharmacyIntegrationTests.Tests.Controller
{
    public class PharmacyControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private WebApplicationFactory<Startup> _factory;
        private HttpClient _client;

        public PharmacyControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services.SingleOrDefault(services => services.ServiceType == typeof(DbContextOptions<PharmacyDbContext>));

                    services.Remove(dbContextOptions);

                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                    services.AddMvc(options => options.Filters.Add(new FakeUserFilter()));

                    services.AddDbContext<PharmacyDbContext>(option => option.UseInMemoryDatabase("PharmacyDb"));
                });
            });

            _client = _factory.CreateClient();
        }

        //[Fact]
        //public async Task CreatePharmacy_WithValidModel_ReturnsCreatedStatus()
        //{
        //    //arrange
        //    var user = new User()
        //    {
        //        FirstName = "Manager",
        //        LastName = "Dwulit",
        //        Gender = 'f',
        //        Email = "manager@protonmail",
        //        Nationality = "German",
        //        DateOfBirth = new DateTime(11 - 11 - 2010),
        //        RoleId = 3
        //    };

        //    SeedUser(user);

        //    var pharmacy = new CreatePharmacyDto()
        //    {
        //        Name = "TestPharmacy",
        //        City = "Szczecin",
        //        ContactEmail = "pharmacy@protonmail.com",
        //        ContactNumber = "999111222",
        //        Street = "apteczna 3",
        //        HasPresciptionDrugs = true,
        //        PostalCode = "75644"
        //    };

        //    var httpContent = HttpContentHelper.SerializeToJson(pharmacy);

        //    //act
        //    var response = await _client.PostAsync("/api/pharmacy", httpContent);


        //    response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        //    response.Headers.Location.Should().NotBeNull();
        //}

        [Theory]
        [InlineData(3)]
        public async Task Delete_WithExistDrugInformationId_ReturnsNoContent(int roleId)
        {
            //arrange
            
            var user = new User()
            {
                Email = "kamillosiak@protonmail.com",
                PasswordHash = "thisispassword1!",
                FirstName = "Kamil",
                LastName = "Losiak",
                DateOfBirth = new DateTime(1996 - 11 - 11),
                Gender = 'm',
                Nationality = "Poland",
                RoleId = roleId
            };

            SeedUser(user);

            var pharmacy = new Pharmacy()
            {
                Name = "TestPharmacy",
                ContactEmail = "pharmacy@protonmail.com",
                ContactNumber = "999111222",
                HasPresciptionDrugs = true,
                Address = new Address()
                {
                    City = "Szczecin",
                    Street = "apteczna 3",
                    PostalCode = "75644"
                },
                CreatedByUserId = user.Id
            };

            SeedPharmacy(pharmacy);

            //act
            var response = await _client.DeleteAsync("/api/pharmacy?bodyid=" + user.Id);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }


        private void SeedUser(User user)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        private void SeedPharmacy(Pharmacy pharmacy)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            _dbContext.Pharmacies.Add(pharmacy);
            _dbContext.SaveChanges();
        }
        
    }
}
