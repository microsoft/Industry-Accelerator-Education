/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using EducationAccelerator.ActionResults;
using EducationAccelerator.Exceptions;
using EducationAccelerator.Models;
using EducationAccelerator.Serializers;
using EducationAccelerator.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace EducationAccelerator.Controllers
{
    [Route("ims/oneroster/v1p1")]
    public class BaseController : Controller
    {
        internal OneRosterSerializer serializer;
        internal ApiContext db;
        internal readonly string CrmBaseUrl;
        internal HttpClient _client;
        internal string BaseUrl() => $"{(Request.IsHttps ? "https" : "http")}://{Request.Host}/ims/oneroster/v1p1";
        internal string SortField() => Request.Query["sort"];
        internal bool SortDesc() => Request.Query["orderBy"] == "desc";
        internal int Offset() => QueryPositiveInt("offset", 0);
        internal int Limit() => QueryPositiveInt("limit", 100);
        internal IQueryable<T> ApplyPaging<T>(IQueryable<T> modelQuery) => modelQuery.Skip(Offset()).Take(Limit());
        internal List<OneRosterException> exceptions = new List<OneRosterException>();
        internal int? ResponseCount;

        internal int QueryPositiveInt(string name, int defaultValue)
        {
            int val;
            if (int.TryParse(Request.Query[name], out val) && val >= 0)
            {
                return val;
            }
            return defaultValue;
        }

        internal string BuildUrl<T>(string[] additionalFilters = null, List<LinkEntityData> linkedEntities = null)
            where T : BaseModel
        {
            var modelType = typeof(T);
            var entityName = (string)modelType.GetField("EntityName").GetValue(null);
            var entitySetName = (string)modelType.GetField("EntitySetName").GetValue(null);

            var url = new StringBuilder();
            url.Append($"{CrmBaseUrl}{entitySetName}?fetchXml=");
            url.Append(BuildFetch<T>(entityName, additionalFilters, linkedEntities));

            return url.ToString();
        }

        internal string BuildSingleRecordUrl<T>(string id)
            where T : BaseModel
        {
            var modelType = typeof(T);
            var entitySetName = (string)modelType.GetField("EntitySetName").GetValue(null);

            return $"{CrmBaseUrl}{entitySetName}({id})";
        }

        internal string BuildFetch<T>(string entityName, string[] additionalFilters, List<LinkEntityData> linkedEntities)
            where T : BaseModel
        {
            string filter = null;
            string filter2 = null;
            string links = null;
            string attributes = null;
            string paging = null;

            attributes = BaseModel.SelectAttributes<T>(Request.Query["fields"]);

            try
            {
                filter = BaseModel.BuildFilter(typeof(T), Request.Query["filter"]);
                filter2 = BaseModel.BuildFilter(typeof(T), new StringValues(additionalFilters));
            }
            catch (InvalidFilterFieldException e)
            {
                exceptions.Add(e);
            }

            if (linkedEntities != null && linkedEntities.Count > 0)
            {
                links = BuildLinkEntity<T>(linkedEntities);
            }

            try
            {
                paging = BaseModel.BuildPaging<T>(Request.Query["limit"], Request.Query["offset"]);
            }
            catch (InvalidFilterFieldException e)
            {
                exceptions.Add(e);
            }

            var fetch = $@"<fetch{paging}>
                <entity name='{entityName}'>
                    {attributes}
                    {filter}
                    {filter2}
                    {links}
                </entity>
            </fetch>";

            return Uri.EscapeDataString(fetch);
        }

        internal string BuildLinkEntity<T>(List<LinkEntityData> linkedEntities)
        {
            var sb = new StringBuilder();

            foreach (var link in linkedEntities)
            {
                string filter = null;

                var test = link.EntityType;
                try
                {
                    filter = BaseModel.BuildFilter(link.EntityType, new StringValues(link.Filters));
                }
                catch (InvalidFilterFieldException e)
                {
                    exceptions.Add(e);
                }

                var fetchPiece = $@"<link-entity name='{link.EntityName}' {link.JoinMapping} alias='{link.Alias}'>
                    {filter}
                </link-entity>";

                sb.Append(fetchPiece);
            }

            return sb.ToString();
        }

        internal void SerializeExceptions()
        {
            var writer = serializer.writer;

            if (writer.WriteState == WriteState.Object)
            {
                writer.WritePropertyName("statusInfoSet");
            }
            
            writer.WriteStartArray();

            foreach (var exception in exceptions)
            {
                exception.AsJson(writer, ControllerContext.ActionDescriptor.ActionName);
            }
            writer.WriteEndArray();
        }

        internal string FinishSerialization()
        {
            if (exceptions.Count > 0)
            {
                SerializeExceptions();
            }
            return serializer.Finish();
        }

        public BaseController(ApiContext _db, IOptions<CrmConnectionSettings> crmConnection = null)
        {
            db = _db;

            if (crmConnection != null)
            {
                CrmBaseUrl = crmConnection.Value.ApiUrl;
                _client = Helpers.GetHttpClientWithToken(crmConnection.Value);
            }
        }

        internal IActionResult JsonOk(string json)
        {
            return JsonOk(json, null);
        }

        internal IActionResult JsonOk(string json, int? count)
        {
            return JsonWithStatus(json, count, 200);
        }

        internal IActionResult JsonWithStatus(string json, int? count, int status)
        {
            if (exceptions.Any(e => e.GetType() == typeof(InvalidFilterFieldException)))
            {
                return ErrorResult();
            }
            
            return new OneRosterResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = status,
                count = count
            };
        }

        internal IActionResult ErrorResult()
        {
            serializer = new OneRosterSerializer("statusInfoSet");
            SerializeExceptions();

            return new OneRosterResult
            {
                Content = serializer.Finish(),
                ContentType = "application/json",
                StatusCode = 400
            };
        }
    }
}
