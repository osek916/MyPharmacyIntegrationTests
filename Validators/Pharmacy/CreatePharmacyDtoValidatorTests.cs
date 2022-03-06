using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using MyPharmacy.Entities;
using MyPharmacy.Models;
using MyPharmacy.Models.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MyPharmacyIntegrationTests.Validators.Pharmacy
{
    public class CreatePharmacyDtoValidatorTests
    {
        private PharmacyDbContext _dbContext;
        public CreatePharmacyDtoValidatorTests()
        {
            var builder = new DbContextOptionsBuilder<PharmacyDbContext>();
            builder.UseInMemoryDatabase("PharmacyTestDb");

            _dbContext = new PharmacyDbContext(builder.Options);
            Seed();
        }

        public static IEnumerable<object[]> GetSampleInvalidData()
        {
            var list = new List<CreatePharmacyDto>()
            {
                new CreatePharmacyDto()
                {
                 ContactEmail = "test4@protonmail.com",
                 City = "Gryfice",
                 ContactNumber = "791020111",
                 Name = "AptekaNowa",
                 HasPresciptionDrugs = true,
                 PostalCode = "55432",
                 Street = "Apteczna 7"
                },
                new CreatePharmacyDto()
                {
                 ContactEmail = "test2protonmail.com",
                 City = "Szczecin",
                 ContactNumber = "791330111",
                 Name = "AptekaPuls",
                 HasPresciptionDrugs = false,
                 PostalCode = "55542",
                 Street = "Uliczna 4"
                },
                new CreatePharmacyDto()
                {
                 ContactEmail = "apteka@protonmail.com",
                 City = "Nowogard",
                 ContactNumber = "65452011",
                 Name = "AptekaNowa",
                 HasPresciptionDrugs = false,
                 PostalCode = "54321",
                 Street = "Wolna 8"
                },
            };
            return list.Select(q => new object[] { q });
        }


        private void Seed()
        {
            var testPharmacies = new List<MyPharmacy.Entities.Pharmacy>()
            {
                new MyPharmacy.Entities.Pharmacy()
                {
                    ContactEmail = "test4@protonmail.com",
                    ContactNumber = "655333456",
                    Name = "AptekaSzczecińska",
                    HasPresciptionDrugs = true,
                    Address = new Address
                    {
                        City = "Szczecin",
                        PostalCode = "43567",
                        Street = "Szczecińska 5"
                    }
                }
            };
            _dbContext.Pharmacies.AddRange(testPharmacies);
            _dbContext.SaveChanges();
        }

        
        [Fact]
        public void Validate_WithValidParameters_ReturnsSuccess()
        {
            //arrange

            var model = new CreatePharmacyDto()
            {
                ContactEmail = "test5@protonmail.com",
                ContactNumber = "234543654",
                Name = "NewPharmacy",
                HasPresciptionDrugs = true,
                City = "Warszawa",
                PostalCode = "67343",
                Street = "Grudziącka 4"
            };

            var validator = new CreatePharmacyDtoValidator(_dbContext);

            //act
            var result = validator.TestValidate(model);

            //assert

            result.ShouldNotHaveAnyValidationErrors();
        }
        
        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]
        public void Validate_WithInvalidParameters_ReturnsFailure(CreatePharmacyDto model)
        {
            //arrange

            var validator = new CreatePharmacyDtoValidator(_dbContext);

            //act
            var result = validator.TestValidate(model);

            //assert

            result.ShouldHaveAnyValidationError();
        }
    }
}
