using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyPharmacy;
using MyPharmacy.Entities;
using MyPharmacy.Models.OrderForPharmacyDtos;
using MyPharmacyIntegrationTests.Helpers;
using System;
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

        }


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
        
        [Fact]
        public async Task UpdateStatusOfOrder_WithValidModel_ReturnsOkStatus()
        {
            //arrange
            string status = "delivered";
            var pharmacy = SamplesOfData.ValidModelPharmacy();
            TestSeeder.SeedPharmacy(pharmacy, _factory);

            var orderForPharmacy = SamplesOfData.ValidModelOrderForPharmacy();
            orderForPharmacy.Status = new Status()
            {
                Name = "during",
            };
            orderForPharmacy.PharmacyId = 1;

            TestSeeder.SeedOrderForPharmacy(orderForPharmacy, _factory);

            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            int id = _dbContext.OrderForPharmacies.Select(x => x.Id).First();
            //act
            var httpContent = HttpContentHelper.SerializeToJson(status);

            var response = await _client
                .PutAsync($"/api/orderforpharmacy/status/{id}", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateStatusOfOrder_WithInvalidStatus_ReturnsBadRequestStatus()
        {
            //arrange
            string status = "this status doesn't exist";
            var pharmacy = SamplesOfData.ValidModelPharmacy();
            TestSeeder.SeedPharmacy(pharmacy, _factory);

            var orderForPharmacy = SamplesOfData.ValidModelOrderForPharmacy();
            orderForPharmacy.Status = new Status()
            {
                Name = "during",
            };
            orderForPharmacy.PharmacyId = 1;

            TestSeeder.SeedOrderForPharmacy(orderForPharmacy, _factory);

            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            int id = _dbContext.OrderForPharmacies.Select(x => x.Id).First();
            //act
            var httpContent = HttpContentHelper.SerializeToJson(status);

            var response = await _client
                .PutAsync($"/api/orderforpharmacy/status/{id}", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateDateOfReceiptOfOrder_WithValidModel_ReturnsOkStatus()
        {
            //arrange
            DateTime newDateOfReceipt = new DateTime(2020, 11, 20, 12, 22, 22);
            var pharmacy = SamplesOfData.ValidModelPharmacy();
            TestSeeder.SeedPharmacy(pharmacy, _factory);

            var orderForPharmacy = SamplesOfData.ValidModelOrderForPharmacy(); //DateOfReceipt = new DateTime(1999, 11, 15)
            orderForPharmacy.PharmacyId = 1;

            TestSeeder.SeedOrderForPharmacy(orderForPharmacy, _factory);

            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            int id = _dbContext.OrderForPharmacies.Select(x => x.Id).First();
            //act
            var httpContent = HttpContentHelper.SerializeToJson(newDateOfReceipt);

            var response = await _client
                .PutAsync($"/api/orderforpharmacy/dateofreceipt/{id}", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateDateOfReceiptOfOrder_WithInvalidDateOfReceipt_ReturnsBadRequestStatus()
        {
            //arrange
            //New dateOfReceipt is lower than dateOfOrder
            DateTime newDateOfReceipt = new DateTime(1993, 11, 20, 12, 22, 22);
            var pharmacy = SamplesOfData.ValidModelPharmacy();
            TestSeeder.SeedPharmacy(pharmacy, _factory);

            var orderForPharmacy = SamplesOfData.ValidModelOrderForPharmacy(); //DateOfReceipt = new DateTime(1999, 11, 15)
            orderForPharmacy.PharmacyId = 1;

            TestSeeder.SeedOrderForPharmacy(orderForPharmacy, _factory);

            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            int id = _dbContext.OrderForPharmacies.Select(x => x.Id).First();
            //act
            var httpContent = HttpContentHelper.SerializeToJson(newDateOfReceipt);

            var response = await _client
                .PutAsync($"/api/orderforpharmacy/dateofreceipt/{id}", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddDrugToOrder_WithValidDrugToExistDrugInformation_ReturnsOkStatus()
        {
            //arrange
            var pharmacy = SamplesOfData.ValidModelPharmacy();
            TestSeeder.SeedPharmacy(pharmacy, _factory);

            var orderForPharmacy = SamplesOfData.ValidModelOrderForPharmacy();
            orderForPharmacy.PharmacyId = 1;

            TestSeeder.SeedOrderForPharmacy(orderForPharmacy, _factory);

            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            int id = _dbContext.OrderForPharmacies.Select(x => x.Id).First();

            AddDrugToOrderDto addDrugOrderDto = new AddDrugToOrderDto()
            {
                DrugsName = "Ibuprom",
                SubstancesName = "Ibuprofen",
                NumberOfTablets = 20,
                MilligramsPerTablets = 1,
                AmountOfPackages = 2,
                AdditionalCosts = 0,
                Price = 10.33M
            };

            //act
            var httpContent = HttpContentHelper.SerializeToJson(addDrugOrderDto);

            var response = await _client
                .PutAsync($"/api/orderforpharmacy/drug/{id}", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }


        [Fact]
        public async Task AddDrugToOrder_WitInvalidDrugToExistDrugInformation_ReturnsBadRequestStatus()
        {
            //arrange
            var pharmacy = SamplesOfData.ValidModelPharmacy();
            TestSeeder.SeedPharmacy(pharmacy, _factory);

            var orderForPharmacy = SamplesOfData.ValidModelOrderForPharmacy();
            orderForPharmacy.PharmacyId = 1;

            TestSeeder.SeedOrderForPharmacy(orderForPharmacy, _factory);

            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            int id = _dbContext.OrderForPharmacies.Select(x => x.Id).First();

            AddDrugToOrderDto addDrugOrderDto = new AddDrugToOrderDto()
            {
                DrugsName = "Ibuprom",
                SubstancesName = "Ibuprofen",
                NumberOfTablets = 20,
                MilligramsPerTablets = 1,
                AmountOfPackages = -2,
                AdditionalCosts = 0,
                Price = 10.33M
            };

            //act
            var httpContent = HttpContentHelper.SerializeToJson(addDrugOrderDto);

            var response = await _client
                .PutAsync($"/api/orderforpharmacy/drug/{id}", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task AddDrugToOrder_WithValidDrug_ReturnsOkStatus()
        {
            //arrange
            var secondDrugInformation = new DrugInformation()
            {
                DrugsName = "New Drug",
                SubstancesName = "newdrug",
                NumberOfTablets = 20,
                MilligramsPerTablets = 1,
                Description = "new description",
                LumpSumDrug = false,
                PrescriptionRequired = true
            };

            TestSeeder.SeedDrugInformation(secondDrugInformation, _factory);

            var pharmacy = SamplesOfData.ValidModelPharmacy();
            TestSeeder.SeedPharmacy(pharmacy, _factory);

            var orderForPharmacy = SamplesOfData.ValidModelOrderForPharmacy();
            orderForPharmacy.PharmacyId = 1;

            TestSeeder.SeedOrderForPharmacy(orderForPharmacy, _factory);

            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            int id = _dbContext.OrderForPharmacies.Select(x => x.Id).First();            

            AddDrugToOrderDto addDrugOrderDto = new AddDrugToOrderDto()
            {
                DrugsName = "New Drug",
                SubstancesName = "newdrug",
                NumberOfTablets = 20,
                MilligramsPerTablets = 1,
                AmountOfPackages = 2,
                AdditionalCosts = 0,
                Price = 10.33M
            };

            //act
            var httpContent = HttpContentHelper.SerializeToJson(addDrugOrderDto);

            var response = await _client
                .PutAsync($"/api/orderforpharmacy/drug/{id}", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
           

        [Fact]
        public async Task DeleteById_WithValidModel_ReturnsNoContentStatus()
        {
            //arrange
            var pharmacy = SamplesOfData.ValidModelPharmacy();
            TestSeeder.SeedPharmacy(pharmacy, _factory);

            var orderForPharmacy = SamplesOfData.ValidModelOrderForPharmacy(); 
            orderForPharmacy.PharmacyId = 1;

            TestSeeder.SeedOrderForPharmacy(orderForPharmacy, _factory);

            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            int id = _dbContext.OrderForPharmacies.Select(x => x.Id).First();    

            //act
            var response = await _client
                .DeleteAsync($"/api/orderforpharmacy/{id}");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteById_WithInvalidModel_ReturnsNotFoundStatus()
        {
            //arrange
            var pharmacy = SamplesOfData.ValidModelPharmacy();
            TestSeeder.SeedPharmacy(pharmacy, _factory);

            var orderForPharmacy = SamplesOfData.ValidModelOrderForPharmacy();
            orderForPharmacy.PharmacyId = 1;

            TestSeeder.SeedOrderForPharmacy(orderForPharmacy, _factory);

            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PharmacyDbContext>();

            int id = _dbContext.OrderForPharmacies.Select(x => x.Id).First();

            //act
            var response = await _client
                .DeleteAsync($"/api/orderforpharmacy/{id + 1}");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

    }
}
