using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyPharmacy;
using MyPharmacy.Entities;
using MyPharmacy.Models.OrderForPharmacyDtos;
using MyPharmacyIntegrationTests.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MyPharmacyIntegrationTests.Tests.Controller
{
    public class OrderForPharmacyControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private WebApplicationFactory<Startup> _factory;
        private HttpClient _client;

        public OrderForPharmacyControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services
                    .SingleOrDefault(services => services.ServiceType == typeof(DbContextOptions<PharmacyDbContext>));

                    services.Remove(dbContextOptions);

                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                    services.AddMvc(options => options.Filters.Add(new FakeUserFilterPharmacist()));

                    services.AddDbContext<PharmacyDbContext>(option => option.UseInMemoryDatabase("PharmacyDb"));
                });
            });

            _client = _factory.CreateClient();

            /*
            List<Status> statuses = SamplesOfData.ValidModelStatuses();
            foreach (var status in statuses)
            {
                TestSeeder.SeedStatus(status, _factory);
            }
            */
        }

        /*
        [Theory]
        //[InlineData(new OrderForPharmacy() { Price = 10})]
        [InlineData(new OrderForPharmacy(){})]
        public async Task CreateOrderForPharmacy_WithValidModel_ReturnsCreatedStatus(OrderForPharmacy order)
        {
            //arrange

            //act

            //assert
        }
        */

        [Fact]
        public async Task CreateOrderForPharmacy_WithValidModel_ReturnsCreatedStatus()
        {
            //arrange
            var drugInformations = SamplesOfData.ValidModelDrugInformations();

            var pharmacy = SamplesOfData.ValidModelPharmacy();

            var createOrderForPharmacyDto = new CreateOrderForPharmacyDto()
            {
                AdditionalCosts = 10,
                OrderDescription = "test description",
                StatusName = "during",
                DrugsDtos = new List<CreateOrderForPharmacyDrugDto>()
                {
                    new CreateOrderForPharmacyDrugDto()
                    {
                        AmountOfPackages = 2,
                        SubstancesName = "Ibuprofen",
                        DrugsName = "Ibuprom",
                        Price = 10.20M,
                        MilligramsPerTablets = 1,
                        NumberOfTablets = 20
                    },
                    new CreateOrderForPharmacyDrugDto()
                    {
                        AmountOfPackages = 2,
                        SubstancesName = "Difenhydramina",
                        DrugsName = "Benosen",
                        Price = 10.20M,
                        MilligramsPerTablets = 1,
                        NumberOfTablets = 20
                    },
                }
            };

            //SeedPharmacy(pharmacy);
            TestSeeder.SeedPharmacy(pharmacy, _factory);
            foreach (var drugInformation in drugInformations)
            {
                TestSeeder.SeedDrugInformation(drugInformation, _factory);
            }
                        
            var httpContent = HttpContentHelper.SerializeToJson(createOrderForPharmacyDto);            

            //act
            var response = await _client.PostAsync("/api/orderforpharmacy", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        }
        /*
        [Fact]
        public async Task UpdateStatusOfOrder_WithValidModel_ReturnsOkStatus()
        {
            //arrange
            var orderForPharmacy = SamplesOfData.ValidModelOrderForPharmacy();
            orderForPharmacy.Status = new Status()
            {
                Name = "during"
            };
            TestSeeder.SeedOrderForPharmacy(orderForPharmacy, _factory);

            //act
            var response = await _client.PutAsync("/api/orderforpharmacy", httpContent);

            //assert


        }
        */
       
        
    }
}
