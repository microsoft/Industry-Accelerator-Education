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
    public class LineItemCategory : BaseModel
    {
        internal override string ModelType()
        {
            return "category";
        }

        internal override string UrlType()
        {
            return "categories";
        }

        [Required]
        public string Title { get; set; }

        public new void AsJson(JsonWriter writer, string baseUrl)
        {
            writer.WriteStartObject();
            base.AsJson(writer, baseUrl);

            writer.WritePropertyName("title");
            writer.WriteValue(Title);

            writer.WriteEndObject();
            writer.Flush();
        }

        public bool UpdateWithJson(JObject json)
        {
            try
            {
                Status = (Vocabulary.StatusType)Enum.Parse(typeof(Vocabulary.StatusType), (string)json["status"]);
                Metadata = (string)json["metadata"];
                UpdatedAt = DateTime.Now;
                Title = (string)json["lineItem"]["title"];
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
            writer.NextRecord();
        }

        public new void AsCsvRow(CsvWriter writer, bool bulk = true)
        {
            base.AsCsvRow(writer, bulk);
            writer.WriteField(Title);
            writer.NextRecord();
        }
    }
}
