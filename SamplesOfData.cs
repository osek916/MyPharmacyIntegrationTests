using MyPharmacy.Entities;
using System;
using System.Collections.Generic;

namespace MyPharmacyIntegrationTests
{
    public static class SamplesOfData
    {
        public static OrderForPharmacy ValidModelOrderForPharmacy()
        {


            var orderForPharmacy = new OrderForPharmacy()
            {
                OrderDescription = "Test Description",
                DateOfOrder = new DateTime(1999, 11, 13),
                DateOfReceipt = new DateTime(1999, 11, 15),

                Drugs = new List<Drug>()
                {
                    new Drug()
                    {
                        AmountOfPackages = 1,
                        Price = 10.33M,
                        DrugInformation = new DrugInformation()
                        {
                            DrugsName = "Ibuprom",
                            SubstancesName = "Ibuprofen",
                            Description = "DrugInformation test description",
                            LumpSumDrug = false,
                            MilligramsPerTablets = 1,
                            NumberOfTablets = 20,
                            PrescriptionRequired = false,
                            DrugCategories = new List<DrugCategory>()
                            {
                                new DrugCategory()
                                {
                                    CategoryName = "painkiller",
                                    Description = "new generation of painkillers",
                                }
                            }
                        }
                    }
                }
            };
            return orderForPharmacy;
        }
        public static List<Status> ValidModelStatuses()
        {
            var statuses = new List<Status>()
            {
                new Status()
                {
                    Name = "delivered"
                },
                new Status()
                {
                    Name = "during"
                },
                new Status()
                {
                    Name = "failed"
                }
            };
            return statuses;
        }

        public static List<DrugInformation> ValidModelDrugInformations()
        {
            var drugInformations = new List<DrugInformation>()
            {
                new DrugInformation()
                {
                    Description = "testtest",
                    DrugsName = "Ibuprom",
                    SubstancesName = "Ibuprofen",
                    LumpSumDrug = false,
                    MilligramsPerTablets = 1,
                    NumberOfTablets = 20,
                    PrescriptionRequired = false,
                },
                new DrugInformation()
                {
                    Description = "testtest",
                    DrugsName = "Benosen",
                    SubstancesName = "Difenhydramina",
                    LumpSumDrug = false,
                    MilligramsPerTablets = 1,
                    NumberOfTablets = 20,
                    PrescriptionRequired = true,
                }
            };

            return drugInformations;
        }

        public static Pharmacy ValidModelPharmacy()
        {
            var pharmacy = new Pharmacy()
            {
                Name = "Pod Slowikiem",
                ContactEmail = "poslowikiem@wp.pl",
                ContactNumber = "777777777",
                Address = new Address()
                {
                    City = "Szczecin",
                    Street = "Wolna 4",
                    PostalCode = "50-101"
                },
                HasPresciptionDrugs = true,
                CreatedByUserId = 1
            };

            return pharmacy;
        }

        /*
        public static OrderForPharmacy ValidModelOrderForPharmacy()
        {
            OrderForPharmacy orderForPharmacy = new OrderForPharmacy()
            {
                Drugs = new List<Drug>()
                {
                    new Drug()
                    {
                        AmountOfPackages = 1,
                        Price = 10.20M,
                        DrugInformation = new DrugInformation()
                        {
                            SubstancesName = "Ibuprofen",
                            DrugsName = "Ibuprom",
                            MilligramsPerTablets = 1,
                            NumberOfTablets = 20,
                            LumpSumDrug = false,
                            PrescriptionRequired = false,
                        }
                    },
                    new Drug()
                    {
                        AmountOfPackages = 2,
                        Price = 10.20M,
                        DrugInformation = new DrugInformation()
                        {
                            SubstancesName = "Difenhydramina",
                            DrugsName = "Benosen",
                            MilligramsPerTablets = 1,
                            NumberOfTablets = 20,
                            LumpSumDrug = false,
                            PrescriptionRequired = false,
                        }
                    }
                }
            };

            return orderForPharmacy; 
        }
        */
    }
}
