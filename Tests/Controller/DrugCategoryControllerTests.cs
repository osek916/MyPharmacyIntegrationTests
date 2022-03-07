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
    public class DrugCategoryControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private WebApplicationFactory<Startup> _factory;
        private HttpClient _client;

        public DrugCategoryControllerTests(WebApplicationFactory<Startup> factory)
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

        [Fact]
        public async Task CreateDrugCategory_WithValidModel_ReturnsCreatedStatus()
        {
            //arrange
            var drugCategory = new CreateDrugCategoryDto()
            {
                CategoryName = "AntyAllergicxx",
                Description = "This is antyallergic category"
            };

            var httpContent = HttpContentHelper.SerializeToJson(drugCategory);

            //act
            var response = await _client.PostAsync("/api/drugcategory", httpContent);

            
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
           // response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateDrugCategory_WithInvalidModel_ReturnsBadRequest()
        {
            //arrange
            var drugCategory = new CreateDrugCategoryDto()
            {
                CategoryName = "aa",
                Description ="This has invalid data because CategoryName has too few letters"
            };


            var httpContent = HttpContentHelper.SerializeToJson(drugCategory);

            //act
            var response = await _client.PostAsync("/api/drugcategory", httpContent);


            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteById_WithInvalidModel_ReturnsNotFound()
        {
            //act
            var response = await _client.DeleteAsync("/api/drugcategory/9999");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteById_WithExistDrugCategoryId_ReturnsNoContent()
        {
            //arrange
            var drugCategory = new DrugCategory()
            {
                CategoryName = "hypnotic",
                Description = "This is hypnotic drug category"
            };

            SeedDrugCategory(drugCategory);

            //act
            var response = await _client.DeleteAsync("/api/drugcategory/" + drugCategory.Id);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteById_WithBadDrugCategoryId_ReturnsNotFound()
        {
            //arrange
            var drugCategory = new DrugCategory()
            {
                CategoryName = "hypnotic",
                Description = "This is hypnotic drug category"
            };
            SeedDrugCategory(drugCategory);

            //act
            var response = await _client.DeleteAsync("/api/drugcategory/" + (drugCategory.Id + 1));

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateById_WithExistingDrugCategoryId_ReturnsOk()
        {
            //arrange
            var drugCategoryToDatabase = new DrugCategory()
            {
                CategoryName = "hypnotic",
                Description = "This is hypnotic drug category"
            };
            SeedDrugCategory(drugCategoryToDatabase);

            var drugCategoryUpdated = new UpdateDrugCategoryDto()
            {
                CategoryName = "Upadeted hypnotic",
                Description = "This is hypnotic drug category"
            };
            //act
            var httpContent = HttpContentHelper.SerializeToJson(drugCategoryUpdated);

            var response = await _client.PutAsync("/api/drugcategory/" + drugCategoryToDatabase.Id, httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateById_WithNonExistentDrugCategoryId_ReturnsNotFound()
        {
            //arrange
            var drugCategoryToDatabase = new DrugCategory()
            {
                CategoryName = "test2",
                Description = "This is test2 drug category"
            };
            SeedDrugCategory(drugCategoryToDatabase);

            var drugCategoryUpdated = new DrugCategory()
            {
                CategoryName = "Upadeted test2",
                Description = "This is test2 drug category"
            };
            //act
            var httpContent = HttpContentHelper.SerializeToJson(drugCategoryUpdated);

            var response = await _client.PutAsync("/api/drugcategory/" + drugCategoryToDatabase.Id + 1, httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        private void SeedDrugCategory(DrugCategory drugCategory)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            _dbContext.DrugCategories.Add(drugCategory);
            _dbContext.SaveChanges();
        }
    }
}
