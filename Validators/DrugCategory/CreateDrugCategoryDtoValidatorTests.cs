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
    public class CreateDrugCategoryDtoValidatorTests
    {
        private PharmacyDbContext _dbContext;
        public CreateDrugCategoryDtoValidatorTests()
        {
            var builder = new DbContextOptionsBuilder<PharmacyDbContext>();
            builder.UseInMemoryDatabase("PharmacyTestDb");

            _dbContext = new PharmacyDbContext(builder.Options);
            Seed();
        }

        public static IEnumerable<object[]> GetSampleInvalidData()
        {
            var list = new List<CreateDrugCategoryDto>()
            {
                new CreateDrugCategoryDto()
                {
                    CategoryName = "te",
                    Description = "testtest1"
                },
                new CreateDrugCategoryDto()
                {
                    CategoryName = "testtest2testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest",
                    Description = "testtest2"
                },
            };
            return list.Select(q => new object[] { q });
        }

        public static IEnumerable<object[]> GetSampleValidData()
        {
            var list = new List<CreateDrugCategoryDto>()
            {
                new CreateDrugCategoryDto()
                {
                    CategoryName = "test1",
                    Description = "testtest1"
                },
                new CreateDrugCategoryDto()
                {
                    CategoryName = "test2",
                    Description = "testtest2"
                },
                new CreateDrugCategoryDto()
                {
                    CategoryName = "test3",
                    Description = "testtest3"
                }
            };
            return list.Select(q => new object[] { q });
        }
        private void Seed()
        {
            var testDrugCategory = new List<DrugCategory>()
            {
                new DrugCategory()
                {
                    CategoryName = "test4",
                    Description = "testtest4"
                }
            };

            _dbContext.DrugCategories.AddRange(testDrugCategory);
            _dbContext.SaveChanges();
        }

        [Fact]
        public void Validate_WithValidParameters_ReturnsSuccess()
        {
            //arrange

            var model = new CreateDrugCategoryDto()
            {
                CategoryName = "test5",
                Description = "testtest5"
            };

            var validator = new CreateDrugCategoryDtoValidator(_dbContext);

            //act
            var result = validator.TestValidate(model);

            //assert

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]
        public void Validate_WithInvalidParameters_ReturnsFailure(CreateDrugCategoryDto model)
        {
            //arrange

            var validator = new CreateDrugCategoryDtoValidator(_dbContext);

            //act
            var result = validator.TestValidate(model);

            //assert

            result.ShouldHaveAnyValidationError();
        }
    }
}
