using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyPharmacy;
using MyPharmacy.Entities;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MyPharmacyIntegrationTests
{
    public class SearchEngineControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;

        public SearchEngineControllerTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<PharmacyDbContext>));

                    services.Remove(dbContextOptions);

                    services.AddDbContext<PharmacyDbContext>(options => options.UseInMemoryDatabase("PharmacyDb"));
                });
            })
                .CreateClient();
        }

        //[Fact]
        [Theory]
        [InlineData("SortBy=DrugsName&PageNumber=3&PageSize=5")]
        [InlineData("SortBy=DrugsName&PageNumber=10&PageSize=10")]
        [InlineData("SortBy=DrugsName&PageNumber=15&PageSize=15")]
        [InlineData("SortBy=DrugsName&PageNumber=20&PageSize=20")]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetDrugInformations_WithQueryParameters_ReturnsOkResult(string queryParams)
        {
            // act

            var response  = await _client.GetAsync("/api/searchengine/druginformation?" + queryParams);
            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        
        [Theory]
        [InlineData("SortBy=DrugsName&PageNumber=1&PageSize=2")]
        [InlineData("SortBy=DrugsName&PageNumber=1&PageSize=33")]

        public async Task GetDrugInformations_WithInvalidQueryParams_ReturnsBadRequest(string queryParams)
        {
            //act
            var response = await _client.GetAsync("/api/searchengine/druginformation?" + queryParams);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }



        [Theory]
        [InlineData("HasPresciptionDrugs=false&SortBy=City&PageSize=5&PageNumber=2&Phrase=Warszawa")]
        [InlineData("HasPresciptionDrugs=true&SortBy=Name&PageSize=10&PageNumber=1")]
        [InlineData("HasPresciptionDrugs=false&PageSize=5&PageNumber=2")]
        [InlineData("PageSize=5&PageNumber=2")]

        public async Task GetPharmacies_WithQueryParameters_ReturnsOkResult(string queryParams)
        {
            //act
            var response = await _client.GetAsync("/api/searchengine/pharmacy?" + queryParams);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }


        /*
        [Theory]
        [InlineData("HasPresciptionDrugs=false&SortBy=City&PageSize=5&PageNumber=2&Phrase=")]
        [InlineData(null)]
        [InlineData("")]

        public async Task GetPharmacies_WithInvalidQueryParams_ReturnsNotFound(string queryParams)
        {
            //act
            var response = await _client.GetAsync("/searchengine/pharmacy?" + queryParams);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
        */
        [Theory]
        [InlineData("HasPresciptionDrugs=false&SortBy=City&PageSize=6&PageNumber=2&Phrase=Warszawa")]

        public async Task GetPharmacies_WithInvalidQueryParams_ReturnsBadRequest(string queryParams)
        {
            //act
            var response = await _client.GetAsync("api/searchengine/pharmacy?" + queryParams);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }


        [Theory]
        [InlineData("City=Warszawa&SortDirection=DESC&PharmaciesSortBy&Phrase=metylofenidat&PageNumber=1&PageSize=10")]
        [InlineData("PharmaciesSortBy&Phrase=metylofenidat&PageNumber=2&PageSize=10")]
        [InlineData("SortDirection=ASC&PharmaciesSortBy=City&Phrase=apap&PageNumber=1&PageSize=15")]
        [InlineData("SortDirection=ASC&PharmaciesSortBy=Name&Phrase=apap&PageNumber=1&PageSize=15")]
        public async Task GetPharmaciesWithDrug_WithQueryParameters_ReturnsOkResult(string queryParams)
        {
            //act
            var response = await _client.GetAsync("api/searchengine/pharmacywithdrug?" + queryParams);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("City = Warszawa & SortDirection = DESC  & Phrase = metylofenidat & PageNumber = 1 & PageSize = 9")]
        [InlineData("City = Warszawa & SortDirection = DESC  & PageNumber = 1 & PageSize = 9")]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetPharmaciesWithDrug_WithInvalidQueryParams_ReturnsNotFound(string queryParams)
        {
            //act
            var response = await _client.GetAsync("/searchengine/pharmacywithdrug?" + queryParams);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

    }
}
