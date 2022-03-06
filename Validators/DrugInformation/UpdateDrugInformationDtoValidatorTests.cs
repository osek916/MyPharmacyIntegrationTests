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

namespace MyPharmacyIntegrationTests.Validators
{
    public class UpdateDrugInformationDtoValidatorTests
    {
        private PharmacyDbContext _dbContext;
        public UpdateDrugInformationDtoValidatorTests()
        {
            var builder = new DbContextOptionsBuilder<PharmacyDbContext>();
            builder.UseInMemoryDatabase("PharmacyTestDb");

            _dbContext = new PharmacyDbContext(builder.Options);
            Seed();
        }

        private void Seed()
        {
            var testDrugInformations = new List<DrugInformation>()
            {
                new DrugInformation()
                {
                    DrugsName = "Apap",
                    SubstancesName = "Paracetamol",
                    NumberOfTablets = 10,
                    MilligramsPerTablets = 500,
                    LumpSumDrug = false,
                    Description = "this is drug",
                    PrescriptionRequired = false,
                }
            };

            _dbContext.DrugInformations.AddRange(testDrugInformations);
            _dbContext.SaveChanges();
        }
        [Fact]
        public void Validate_WithValidParameters_ReturnsSuccess()
        {
            //arrange

            var model = new UpdateDrugInformationDto()
            {
                DrugsName = "Apap",
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

            result.ShouldNotHaveAnyValidationErrors();
        }

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
