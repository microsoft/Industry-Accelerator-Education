/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using CsvHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;

namespace EducationAccelerator.Models
{
    public class LineItem : BaseModel
    {
        internal override string ModelType()
        {
            return "lineItem";
        }

        internal override string UrlType()
        {
            return "lineItems";
        }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime AssignDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public string IMSClassId { get; set; }
        public IMSClass IMSClass { get; set; }

        [Required]
        public string LineItemCategoryId { get; set; }
        public LineItemCategory LineItemCategory { get; set; }

        [Required]
        public string AcademicSessionId { get; set; }
        public AcademicSession AcademicSession { get; set; }

        [Required]
        public float ResultValueMin { get; set; }

        [Required]
        public float ResultValueMax { get; set; }

        public new void AsJson(JsonWriter writer, string baseUrl)
        {
            writer.WriteStartObject();
            base.AsJson(writer, baseUrl);

            writer.WritePropertyName("title");
            writer.WriteValue(Title);

            if (!String.IsNullOrEmpty(Description))
            {
                writer.WritePropertyName("description");
                writer.WriteValue(Description);
            }

            writer.WritePropertyName("assignDate");
            writer.WriteValue(AssignDate.ToString("yyyy-MM-dd"));

            writer.WritePropertyName("dueDate");
            writer.WriteValue(DueDate.ToString("yyyy-MM-dd"));

            writer.WritePropertyName("category");
            LineItemCategory.AsJsonReference(writer, baseUrl);

            writer.WritePropertyName("class");
            IMSClass.AsJsonReference(writer, baseUrl);

            writer.WritePropertyName("gradingPeriod");
            AcademicSession.AsJsonReference(writer, baseUrl);

            writer.WritePropertyName("resultValueMin");
            writer.WriteValue(ResultValueMin.ToString());

            writer.WritePropertyName("resultValueMax");
            writer.WriteValue(ResultValueMax.ToString());

            writer.WriteEndObject();
            writer.Flush();
        }

        public bool UpdateWithJson(JObject json)
        {
            try
            {
                Status = (Vocabulary.StatusType)Enum.Parse(typeof(Vocabulary.StatusType), GetOptionalJsonProperty(json, "status", "active"));
                Metadata = GetOptionalJsonProperty(json, "metadata", null);
                UpdatedAt = DateTime.Now;
                Title = (string)json["title"];
                Description = (string)json["description"];
                AssignDate = DateTime.Parse((string)json["assignDate"]);
                DueDate = DateTime.Parse((string)json["dueDate"]);
                IMSClassId = (string)json["class"]["sourcedId"];
                LineItemCategoryId = (string)json["category"]["sourcedId"];
                AcademicSessionId = (string)json["gradingPeriod"]["sourcedId"];
                ResultValueMin = (float)json["resultValueMin"];
                ResultValueMax = (float)json["resultValueMax"];
            }
            catch (NullReferenceException)
            {
                return false;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            return true;
        }

        public static new void CsvHeader(CsvWriter writer)
        {
            BaseModel.CsvHeader(writer);

            writer.WriteField("title");
            writer.WriteField("description");
            writer.WriteField("assignDate");
            writer.WriteField("dueDate");
            writer.WriteField("classSourcedId");
            writer.WriteField("categorySourcedId");
            writer.WriteField("gradingPeriodSourcedId");
            writer.WriteField("resultValueMin");
            writer.WriteField("resultValueMax");

            writer.NextRecord();
        }

        public new void AsCsvRow(CsvWriter writer, bool bulk = true)
        {
            base.AsCsvRow(writer, bulk);

            writer.WriteField(Title);
            writer.WriteField(Description);
            writer.WriteField(AssignDate.ToString("yyyy-MM-dd"));
            writer.WriteField(DueDate.ToString("yyyy-MM-dd"));
            writer.WriteField(IMSClassId);
            writer.WriteField(LineItemCategoryId);
            writer.WriteField(AcademicSessionId);
            writer.WriteField(ResultValueMin);
            writer.WriteField(ResultValueMax);

            writer.NextRecord();
        }
    }
}
