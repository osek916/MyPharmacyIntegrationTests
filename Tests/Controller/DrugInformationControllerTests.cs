using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyPharmacy;
using MyPharmacy.Entities;
using MyPharmacy.Models;
using MyPharmacyIntegrationTests.Helpers;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MyPharmacyIntegrationTests
{
    public class DrugInformationControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private WebApplicationFactory<Startup> _factory;
        private HttpClient _client;

        public DrugInformationControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<PharmacyDbContext>));

                        services.Remove(dbContextOptions);

                        services.AddMvc(options => options.Filters.Add(new FakeUserFilter()));
                        services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

                        services.AddDbContext<PharmacyDbContext>(option => option.UseInMemoryDatabase("PharmacyDb"));
                    });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateDrugInformation_WithValidModel_ReturnsCreatedStatus()
        {
            //arrange
            var drugInformation = new CreateDrugInformationDto()
            {
                DrugsName = "Apap",
                SubstancesName = "Paracetamol",
                Description = "This is drug",
                LumpSumDrug = false,
                MilligramsPerTablets = 500,
                NumberOfTablets = 10,
                PrescriptionRequired = false
            };

            var httpContent = HttpContentHelper.SerializeToJson(drugInformation);

            //act
            var response = await _client.PostAsync("/api/druginformation", httpContent);


            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateDrugInformation_WithInvalidModel_ReturnsBadRequest()
        {
            //arrange
            var drugInformation = new CreateDrugInformationDto()
            {
                DrugsName = "A",
                SubstancesName = "B",
                MilligramsPerTablets = 500,
                NumberOfTablets = 10,
                PrescriptionRequired = false
            };


            var httpContent = HttpContentHelper.SerializeToJson(drugInformation);

            //act
            var response = await _client.PostAsync("/api/druginformation", httpContent);


            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteById_WithInvalidModel_ReturnsNotFound()
        {
            //act
            var response = await _client.DeleteAsync("/api/druginformation/9999");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteById_WithExistDrugInformationId_ReturnsNoContent()
        {
            //arrange
            var drugInformation = new DrugInformation()
            {
                DrugsName = "Ibuprom",
                SubstancesName = "Ibuprofen"
            };

            SeedDrugInformation(drugInformation);

            //act
            var response = await _client.DeleteAsync("/api/druginformation/" + drugInformation.Id);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteById_WithBadDrugInformationId_ReturnsNotFound()
        {
            //arrange
            var drugInformation = new DrugInformation()
            {
                DrugsName = "Ibuprom",
                SubstancesName = "Ibuprofen"
            };

            SeedDrugInformation(drugInformation);

            //act
            var response = await _client.DeleteAsync("/api/druginformation/" + (drugInformation.Id + 1));

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateById_WithExistingInformationId_ReturnsOk()
        {
            //arrange
            var drugInformationToDatabase = new DrugInformation()
            {
                DrugsName = "testtest2",
                SubstancesName = "testtest2",
                Description = "testtest2",
                LumpSumDrug = true,
                PrescriptionRequired = true,
                NumberOfTablets = 20,
                MilligramsPerTablets = 2
            };

            SeedDrugInformation(drugInformationToDatabase);

            var drugInformationUpdated = new UpdateDrugInformationDto()
            {
                DrugsName = "Test1",
                SubstancesName = "Test1",
                Description = "testtest2",
                LumpSumDrug = true,
                PrescriptionRequired = true,
                NumberOfTablets = 30,
                MilligramsPerTablets = 1,
            };

            //act
            var httpContent = HttpContentHelper.SerializeToJson(drugInformationUpdated);

            var response = await _client.PutAsync("/api/druginformation/" + drugInformationToDatabase.Id, httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateById_WithNonExistentDrugInformationId_ReturnsNotFound()
        {
            //arrange
            var drugInformationToDatabase = new DrugInformation()
            {
                DrugsName = "Ibuprom",
                SubstancesName = "Ibuprofen"
            };

            SeedDrugInformation(drugInformationToDatabase);

            var drugInformationUpdated = new DrugInformation()
            {
                DrugsName = "Test1",
                SubstancesName = "Test1"
            };
            //act
            var httpContent = HttpContentHelper.SerializeToJson(drugInformationUpdated);

            var response = await _client.PutAsync("/api/drugcategory/" + drugInformationToDatabase.Id + 1, httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }


        private void SeedDrugInformation(DrugInformation drugInformation)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            _dbContext.DrugInformations.Add(drugInformation);
            _dbContext.SaveChanges();
        }
         
    }
}
