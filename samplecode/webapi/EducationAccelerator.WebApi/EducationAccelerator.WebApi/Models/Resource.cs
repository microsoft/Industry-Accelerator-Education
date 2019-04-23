/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using CsvHelper;
using EducationAccelerator.Vocabulary;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationAccelerator.Models
{
    public class Resource : BaseModel
    {
        internal override string ModelType()
        {
            return "resource";
        }

        internal override string UrlType()
        {
            return "resources";
        }

        public string Title { get; set; }

        [NotMapped]
        public RoleType[] Roles
        {
            get { return _roles == null ? null : JsonConvert.DeserializeObject<RoleType[]>(_roles); }
            set { _roles = JsonConvert.SerializeObject(value); }
        }
        private string _roles { get; set; }

        public Importance Importance { get; set; }

        [Required]
        public string VendorResourceId { get; set; }

        public string VendorId { get; set; }
        public string ApplicationId { get; set; }
        //public string CourseId { get; set; }

        public new void AsJson(JsonWriter writer, string baseUrl)
        {
            writer.WriteStartObject();

            base.AsJson(writer, baseUrl);

            if (Title != null)
            {
                writer.WritePropertyName("title");
                writer.WriteValue(Title);
            }

            if (Roles != null)
            {
                writer.WritePropertyName("roles");
                writer.WriteStartArray();
                foreach (var role in Roles)
                {
                    writer.WriteValue(Enum.GetName(typeof(RoleType), role));
                }
                writer.WriteEndArray();
            }

            writer.WritePropertyName("importance");
            writer.WriteValue(Enum.GetName(typeof(Importance), Importance));

            writer.WritePropertyName("vendorResourceId");
            writer.WriteValue(VendorResourceId);

            if (VendorId != null)
            {
                writer.WritePropertyName("vendorId");
                writer.WriteValue(VendorId);
            }

            if (ApplicationId != null)
            {
                writer.WritePropertyName("applicationId");
                writer.WriteValue(ApplicationId);
            }

            writer.WriteEndObject();
        }

        public static new void CsvHeader(CsvWriter writer)
        {
            BaseModel.CsvHeader(writer);

            writer.WriteField("title");
            writer.WriteField("vendorResourceId");
            writer.WriteField("vendorId");
            writer.WriteField("applicationId");

            writer.NextRecord();
        }

        public new void AsCsvRow(CsvWriter writer, bool bulk = true)
        {
            base.AsCsvRow(writer, bulk);

            writer.WriteField(Title);
            writer.WriteField(VendorResourceId);
            writer.WriteField(VendorId);
            writer.WriteField(ApplicationId);

            writer.NextRecord();
        }
    }
}
