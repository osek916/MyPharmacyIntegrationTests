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
    public class CreateDrugInformationDtoValidatorTests
    {
        private PharmacyDbContext _dbContext;
        public CreateDrugInformationDtoValidatorTests()
        {
            var builder = new DbContextOptionsBuilder<PharmacyDbContext>();
            builder.UseInMemoryDatabase("PharmacyTestDb");

            _dbContext = new PharmacyDbContext(builder.Options);
            Seed();
        }

        public static IEnumerable<object[]> GetSampleInvalidData()
        {
            var list = new List<CreateDrugInformationDto>()
            {
                new CreateDrugInformationDto()
                {
                    DrugsName = "Apap",
                    SubstancesName = "Paracetamol",
                    NumberOfTablets = 10,
                    MilligramsPerTablets = 500,
                    LumpSumDrug = false,
                    Description = "this is drug",
                    PrescriptionRequired = false,
                },
                new CreateDrugInformationDto()
                {
                    DrugsName = "A",
                    SubstancesName = "Paracetamol",
                    NumberOfTablets = 10,
                    MilligramsPerTablets = 500,
                    LumpSumDrug = false,
                    Description = "this is drug",
                    PrescriptionRequired = false,
                },
                new CreateDrugInformationDto()
                {
                    DrugsName = "Apap",
                    NumberOfTablets = 10,
                    MilligramsPerTablets = 500,
                    LumpSumDrug = false,
                    Description = "this is drug",
                    PrescriptionRequired = false,
                }
            };
            return list.Select(q => new object[] { q });
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

            var model = new CreateDrugInformationDto()
            {
                DrugsName = "Apap",
                SubstancesName = "Paracetamol",
                NumberOfTablets = 20,
                MilligramsPerTablets = 500,
                LumpSumDrug = false,
                Description = "this is drug",
                PrescriptionRequired = false,
            };

            var validator = new CreateDrugInformationDtoValidator(_dbContext);

            //act
            var result = validator.TestValidate(model);

            //assert

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]
        public void Validate_WithInvalidParameters_ReturnsFailure(CreateDrugInformationDto model)
        {
            //arrange

            var validator = new CreateDrugInformationDtoValidator(_dbContext);

            //act
            var result = validator.TestValidate(model);

            //assert

            result.ShouldHaveAnyValidationError();
        }
    }
}
