using FluentValidation.TestHelper;
using MyPharmacy.Entities;
using MyPharmacy.Models;
using MyPharmacy.Models.Validators.SearchEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MyPharmacyIntegrationTests.Validators
{
    public class SearchEngineDrugInformationQueryValidatorTests
    {
        public static IEnumerable<object[]> GetSampleValidModel()
        {
            var list = new List<SearchEngineDrugInformationQuery>()
            {
                new SearchEngineDrugInformationQuery()
                {
                    PageNumber = 1,
                    PageSize = 5,
                    Phrase = "Paracetamol",
                    SortBy = nameof(DrugInformation.SubstancesName),
                    SortDirection = SortDirection.ASC
                },
                new SearchEngineDrugInformationQuery()
                {
                    PageNumber = 2,
                    PageSize = 10,
                    Phrase = "Ibuprofen",
                    SortBy = nameof(DrugInformation.DrugsName),
                    SortDirection = SortDirection.DESC
                },
                new SearchEngineDrugInformationQuery()
                {
                    Phrase = "Apap",
                    SortBy = "DrugsName",
                },
                new SearchEngineDrugInformationQuery()
                {
                    Phrase = "Paracetamol",
                },
            };

            return list.Select(q => new object[] { q });
        }
        [Theory]
        [MemberData(nameof(GetSampleValidModel))]
        public void Validate_WithValidParameters_ReturnSuccess(SearchEngineDrugInformationQuery model)
        {
            //arrange
            var validator = new SearchEngineDrugInformationQueryValidator();
            
            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldNotHaveAnyValidationErrors();
        }


    }
}
