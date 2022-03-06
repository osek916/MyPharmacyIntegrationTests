using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyPharmacy;
using MyPharmacy.Entities;
using MyPharmacy.Models;
using MyPharmacy.Services;
using MyPharmacyIntegrationTests.Helpers;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MyPharmacyIntegrationTests
{
    public class AccountControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private Mock<IAccountService> _accountServiceMock = new Mock<IAccountService>();
        private HttpClient _client;

        public AccountControllerTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<PharmacyDbContext>));

                    services.Remove(dbContextOptions);
                    services.AddSingleton<IAccountService>(_accountServiceMock.Object);

                    services.AddDbContext<PharmacyDbContext>(option => option.UseInMemoryDatabase("PharmacyDb"));
                });
            }).CreateClient();
        }

        [Fact]
        public async Task RegisterUser_WithValidModel_ReturnsOk()
        {
            //arrange
            var user = new UserRegisterDto()
            {
                Email = "kamillosiak@protonmail.com",
                Password = "thisispassword1!",
                ConfirmPassword = "thisispassword1!",
                FirstName = "Kamil",
                LastName = "Losiak",
                DateOfBirth = new DateTime(1996 - 11 - 11),
                Gender = 'm',
                Nationality = "Poland",
                RoleId = 1
            };

            var httpContent = HttpContentHelper.SerializeToJson(user);

            //act
            var response = await _client.PostAsync("/api/account/register", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task RegisterUser_WithInvalidPasswordWithoutSpecialCharAndNumber_ReturnsBadRequest()
        {
            //arrange
            var user = new UserRegisterDto()
            {
                Email = "kamillosiak@protonmail.com",
                Password = "thisispassword",
                ConfirmPassword = "thisispassword",
                FirstName = "Kamil",
                LastName = "Losiak",
                DateOfBirth = new DateTime(1996 - 11 - 11),
                Gender = 'm',
                Nationality = "Poland",
                RoleId = 1
            };

            var httpContent = HttpContentHelper.SerializeToJson(user);

            //act
            var response = await _client.PostAsync("/api/account/register", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_WithExistedUserParameters_ReturnsOk()
        {
            //arrange
            _accountServiceMock
                .Setup(a => a.GenerateJwt(It.IsAny<LoginDto>()))
                .Returns("jwt");

            var loginDto = new LoginDto()
            {
                Email = "kamillosiak@protonmail.com",
                Password = "thisispassword"
            };


            var httpContent = HttpContentHelper.SerializeToJson(loginDto);

            //act
            var response = await _client.PostAsync("/api/account/login", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}
