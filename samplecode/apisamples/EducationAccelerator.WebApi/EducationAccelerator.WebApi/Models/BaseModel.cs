/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using CsvHelper;
using EducationAccelerator.Exceptions;
using EducationAccelerator.Vocabulary;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace EducationAccelerator.Models
{
    public abstract class BaseModel
    {
        [Key]
        [Required]
        [JsonProperty("sourcedId")]
        public string Id { get; set; }

        [Required]
        public StatusType Status { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
        public string Metadata { get; set; }

        internal string Url(string baseUrl)
        {
            return $"{baseUrl}/{UrlType()}/{Id}";
        }
        internal abstract string ModelType();
        internal abstract string UrlType();
        //public abstract string GetCrmFieldName(string field);

        public BaseModel()
        {
            DateTime time = DateTime.Now;
            CreatedAt = time;
            UpdatedAt = time;
        }

        public void AsJsonReference(JsonWriter jw, string baseUrl)
        {
            jw.WriteStartObject();

            jw.WritePropertyName("href");
            jw.WriteValue(Url(baseUrl));
            jw.WritePropertyName("sourcedId");
            jw.WriteValue(Id);
            jw.WritePropertyName("type");
            jw.WriteValue(ModelType());

            jw.WriteEndObject();
        }

        public void AsJson(JsonWriter jw, string baseUrl)
        {
            jw.WritePropertyName("sourcedId");
            jw.WriteValue(Id);
            jw.WritePropertyName("status");
            jw.WriteValue(Enum.GetName(typeof(Vocabulary.StatusType), Status));
            jw.WritePropertyName("dateLastModified");
            jw.WriteValue(UpdatedAt.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffz"));

            if (!String.IsNullOrEmpty(Metadata))
            {
                jw.WritePropertyName("metadata");
                jw.WriteValue(Metadata);
            }
        }

        public static string BuildPaging<T>(StringValues limit, StringValues offset)
        {
            if (limit.Count < 1)
            {
                return null;
            }

            var sb = new StringBuilder();
            var hasLimit = int.TryParse(limit.First(), out int limitValue);

            if (hasLimit == true)
            {
                sb.Append($" count='{limit.First()}'");
            }

            if (offset.Count > 0)
            {
                var hasOffSet = int.TryParse(offset.First(), out int offsetValue);

                if (hasOffSet == true && hasLimit == true)
                {
                    sb.Append($" page='{offsetValue / limitValue + 1}'");
                }
            }

            return sb.ToString();
        }

        public static string SelectAttributes<T>(StringValues fields)
        {
            Type modelType = typeof(T);
            var attributeMap = (Dictionary<string, string>)modelType.GetField("Fields")?.GetValue(null);
            var attributes = fields.Count > 0 ? fields.ToArray() : attributeMap.Keys.ToArray();

            var fetch = new StringBuilder();

            foreach (var attribute in attributes)
            {
                if (attributeMap.ContainsKey(attribute))
                {
                    fetch.Append($"<attribute name='{attributeMap[attribute]}' />");
                }
            }

            return fetch.ToString();
        }

        public static string BuildFilter(Type modelType, StringValues filterValues)
        {
            var attributes = (Dictionary<string, string>)modelType.GetField("Fields").GetValue(null);

            if (filterValues.Count > 0)
            {
                var filter = filterValues.First();

                string logicalOperator;
                string[] pieces;

                if (filter.Contains(" OR "))
                {
                    logicalOperator = "or";
                    pieces = filter.Split(" OR ");
                }
                else
                {
                    logicalOperator = "and";
                    pieces = filter.Split(" AND ");
                }

                var conditions = pieces.Select(x => ParseFilter(modelType, x, attributes));

                return $@"<filter type='{logicalOperator}'>
                    {string.Join("", conditions)}
                </filter>";
            }

            return null;
        }

        private static Dictionary<string, string> ComparatorMap = new Dictionary<string, string>()
        {
            ["="] = "eq",
            ["!="] = "ne",
            [">"] = "gt",
            [">="] = "gte",
            ["<"] = "lt",
            ["<="] = "lte",
            ["~"] = "like"
        };

        // https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451997
        private static Regex filterMatcher = new Regex("(.*)(=|!=|>|>=|<|<=|~)'(.*)'");

        public static string ParseFilter(Type modelType, string filter, Dictionary<string, string> attributes)
        {
            var match = filterMatcher.Match(filter);

            if (!match.Success)
            {
                throw new InvalidFilterFieldException($"Filter: {filter}");
            }

            var dataFieldRaw = match.Groups[1].Value;
            var dataField = dataFieldRaw.ToLower();
            var predicate = match.Groups[2].Value;
            var value = match.Groups[3].Value.ToLower();

            // get the CRM field name
            // get the comparator
            // if the field is an enum, get the numerical value, otherwise just pass in the given value (any other types need special consideration?)

            var lookupMethod = modelType.GetMethod("LookupProperty", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            try
            {
                var comparator = ComparatorMap[predicate];
                var modelProp = (PropertyInfo)lookupMethod.Invoke(null, new object[] { modelType, dataField });

                var fieldType = modelProp.PropertyType;
                var getter = modelProp.GetMethod;
                var comparisonValue = fieldType.IsEnum ? ((int)Enum.Parse(fieldType, value)).ToString() : value;

                comparisonValue = comparator == "like" ? $"%{comparisonValue}%" : comparisonValue;

                return $"<condition attribute='{attributes[dataField]}' operator='{comparator}' value='{comparisonValue}' />";
            }
            catch (Exception e)
            {
                throw new InvalidFilterFieldException($"{dataFieldRaw}");
            }
        }

        private static Dictionary<string, string> PropertyMap = new Dictionary<string, string>()
        {
            ["datelastmodified"] = "UpdatedAt",
            ["sourcedid"] = "Id",
            ["class"] = "IMSClass"
        };

        public static PropertyInfo LookupProperty(Type modelType, string propName)
        {
            var modelProp = modelType.GetProperties().FirstOrDefault(prop => prop.Name.ToLower() == propName.ToLower());

            if (modelProp == null)
            {
                modelProp = modelType.GetProperties().FirstOrDefault(prop => prop.Name == PropertyMap[propName.ToLower()]);
            }

            return modelProp;
        }

        internal string GetOptionalJsonProperty(JObject json, string property, string defaultValue)
        {
            if (json.TryGetValue(property, out JToken value))
            {
                return (string)value;
            }
            return defaultValue;
        }

        public static void CsvHeader(CsvWriter writer)
        {
            writer.WriteField("sourcedId");

            writer.WriteField("status");
            writer.WriteField("dateLastModified");
        }

        public void AsCsvRow(CsvWriter writer, bool bulk = true)
        {
            writer.WriteField(Id);

            if (bulk)
            {
                writer.WriteField("");
                writer.WriteField("");
            }
            else
            {
                writer.WriteField(Status);
                writer.WriteField(UpdatedAt.ToString("yyyy-MM-dd"));
            }
        }
    }
}
