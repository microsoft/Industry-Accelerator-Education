/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using CsvHelper;
using EducationAccelerator.Vocabulary;
using Newtonsoft.Json;
using System;

namespace EducationAccelerator.Models
{
    public class Demographic : BaseModel
    {
        internal override string ModelType()
        {
            return "demographic";
        }

        internal override string UrlType()
        {
            return "demographic";
        }

        public DateTime BirthDate { get; set; }
        public Gender Sex { get; set; }
        public bool AmericanIndianOrAlaskaNative { get; set; }
        public bool Asian { get; set; }
        public bool BlackOrAfricanAmerican { get; set; }
        public bool NativeHawaiianOrOtherPacificIslander { get; set; }
        public bool White { get; set; }
        public bool DemographicRaceTwoOrMoreRaces { get; set; }
        public bool HispanicOrLatinoEthnicity { get; set; }
        public string CountryOfBirthCode { get; set; }
        public string StateOfBirthAbbreviation { get; set; }
        public string CityOfBirth { get; set; }
        public string PublicSchoolResidenceStatus { get; set; }

        public new void AsJson(JsonWriter writer, string baseUrl)
        {
            writer.WriteStartObject();
            base.AsJson(writer, baseUrl);

            if (BirthDate != null)
            {
                writer.WritePropertyName("birthDate");
                writer.WriteValue(BirthDate);
            }
            writer.WritePropertyName("sex");
            writer.WriteValue(Sex);

            writer.WritePropertyName("americanIndianOrAlaskaNative");
            writer.WriteValue(AmericanIndianOrAlaskaNative);
            
            writer.WritePropertyName("asian");
            writer.WriteValue(Asian);
            
            writer.WritePropertyName("blackOrAfricanAmerican");
            writer.WriteValue(BlackOrAfricanAmerican);
            
            writer.WritePropertyName("nativeHawaiianOrOtherPacificIslander");
            writer.WriteValue(NativeHawaiianOrOtherPacificIslander);
            
            writer.WritePropertyName("white");
            writer.WriteValue(White);
          
            writer.WritePropertyName("demographicRaceTwoOrMoreRaces");
            writer.WriteValue(DemographicRaceTwoOrMoreRaces);
            
            writer.WritePropertyName("hispanicOrLatinoEthnicity");
            writer.WriteValue(HispanicOrLatinoEthnicity);
            
            if (CountryOfBirthCode != null)
            {
                writer.WritePropertyName("countryOfBirthCode");
                writer.WriteValue(CountryOfBirthCode);
            }
            if (StateOfBirthAbbreviation != null)
            {
                writer.WritePropertyName("stateOfBirthAbbreviation");
                writer.WriteValue(StateOfBirthAbbreviation);
            }
            if (CityOfBirth != null)
            {
                writer.WritePropertyName("cityOfBirth");
                writer.WriteValue(CityOfBirth);
            }
            if (PublicSchoolResidenceStatus != null)
            {
                writer.WritePropertyName("publicSchoolResidenceStatus");
                writer.WriteValue(PublicSchoolResidenceStatus);
            }

            writer.WriteEndObject();
            writer.Flush();
        }

        public static new void CsvHeader(CsvWriter writer)
        {
            BaseModel.CsvHeader(writer);

            writer.WriteField("birthDate");
            writer.WriteField("gender");
            writer.WriteField("americanIndianOrAlaskaNative");
            writer.WriteField("asian");
            writer.WriteField("blackOrAfricanAmerican");
            writer.WriteField("nativeHawaiianOrOtherPacificIslander");
            writer.WriteField("white");
            writer.WriteField("demographicRaceTwoOrMoreRaces");
            writer.WriteField("hispanicOrLatinoEthnicity");
            writer.WriteField("countryOfBirthCode");
            writer.WriteField("stateOfBirthAbbreviation");
            writer.WriteField("cityOfBirth");
            writer.WriteField("publicSchoolResidenceStatus");
            
            writer.NextRecord();
        }

        public new void AsCsvRow(CsvWriter writer, bool bulk = true)
        {
            base.AsCsvRow(writer, bulk);

            writer.WriteField(BirthDate);
            writer.WriteField(Sex);
            writer.WriteField(AmericanIndianOrAlaskaNative);
            writer.WriteField(Asian);
            writer.WriteField(BlackOrAfricanAmerican);
            writer.WriteField(NativeHawaiianOrOtherPacificIslander);
            writer.WriteField(White);
            writer.WriteField(DemographicRaceTwoOrMoreRaces);
            writer.WriteField(HispanicOrLatinoEthnicity);
            writer.WriteField(CountryOfBirthCode);
            writer.WriteField(StateOfBirthAbbreviation);
            writer.WriteField(CityOfBirth);
            writer.WriteField(PublicSchoolResidenceStatus);

            writer.NextRecord();
        }
    }
}
