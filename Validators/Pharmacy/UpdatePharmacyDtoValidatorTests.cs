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
    public class UpdatePharmacyDtoValidatorTests
    {
        private PharmacyDbContext _dbContext;
        public UpdatePharmacyDtoValidatorTests()
        {
            var builder = new DbContextOptionsBuilder<PharmacyDbContext>();
            builder.UseInMemoryDatabase("PharmacyTestDb");

            _dbContext = new PharmacyDbContext(builder.Options);
            Seed();
        }

        private void Seed()
        {
            var testPharmacies = new List<MyPharmacy.Entities.Pharmacy>()
            {
                new MyPharmacy.Entities.Pharmacy()
                {
                    ContactEmail = "test4@protonmail.com",
                    ContactNumber = "791020111",
                    Name = "AptekaNowa",
                    HasPresciptionDrugs = true,
                    Address = new Address()
                    {
                        City = "Gryfice",
                        PostalCode = "55432",
                        Street = "Apteczna 7"
                    }
                }
            };

            _dbContext.Pharmacies.AddRange(testPharmacies);
            _dbContext.SaveChanges();
        }

        /*
        [Fact]
        public void Validate_WithValidParameters_ReturnsSuccess()
        {
            //arrange

            var model = new UpdatePharmacyDto()
            {
                ContactEmail = "test4@protonmail.com",
                ContactNumber = "791020111",
                Name = "AptekaNowa",
                HasPresciptionDrugs = true,            
                City = "Gryfice",
                PostalCode = "55432",
                Street = "Apteczna 7"
                
            };

            var validator = new UpdatePharmacyDtoValidator(_dbContext);

            //act
            var result = validator.TestValidate(model);

            //assert

            result.ShouldNotHaveAnyValidationErrors();
        }
        */
        [Fact]
        public void Validate_WithInvalidParameters_ReturnsFailure()
        {
            //arrange

            var model = new UpdateDrugInformationDto()
            {
                DrugsName = "A",
                SubstancesName = "Paracetamol",
                NumberOfTablets = 10,
                MilligramsPerTablets = 500,
                LumpSumDrug = false,
                Description = "this is drug",
                PrescriptionRequired = false,
            };

            var validator = new UpdateDrugInformationDtoValidator(_dbContext);

            //act
            var result = validator.TestValidate(model);

            //assert

            result.ShouldHaveAnyValidationError();
        }

    }
}
