/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using CsvHelper;
using EducationAccelerator.Vocabulary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;

namespace EducationAccelerator.Models
{
    public class AcademicSession : BaseModel
    {
        internal override string ModelType()
        {
            return "academicSession";
        }

        internal override string UrlType()
        {
            return "academicSessions";
        }

        public static readonly Dictionary<string, string> Fields = new Dictionary<string, string>()
        {
            ["sourcedid"] = "msk12_academicsessionid",
            ["createdat"] = "createdon",
            ["datelastmodified"] = "msk12_lastmodifieddate",
            ["status"] = "msk12_status",
            ["title"] = "msk12_title",
            ["startdate"] = "msk12_startdate",
            ["enddate"] = "msk12_enddate",
            ["type"] = "msk12_sessiontype",
            ["parent"] = "msk12_parentsession",
            ["schoolyear"] = "msk12_schoolyear"
        };

        public static readonly string EntityName = "msk12_academicsession";
        public static readonly string EntitySetName = "msk12_academicsessions";

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public SessionType Type { get; set; }

        [Required]
        public string SchoolYear { get; set; }

        // Associations
        public string ParentAcademicSessionId { get; set; }
        public virtual AcademicSession ParentAcademicSession { get; set; }
        [InverseProperty("ParentAcademicSession")]
        public List<AcademicSession> Children { get; set; }

        public AcademicSession() { }

        public AcademicSession(CrmAcademicSession crmSession)
        {
            Id = crmSession.Id;
            Status = crmSession.msk12_status;
            CreatedAt = crmSession.CreatedOn;
            UpdatedAt = crmSession.msk12_lastmodifieddate;
            Title = crmSession.msk12_title;
            StartDate = crmSession.msk12_startdate;
            EndDate = crmSession.msk12_enddate;
            Type = crmSession.msk12_sessiontype;
            SchoolYear = crmSession.msk12_schoolyear;

            if (crmSession._msk12_parentsession_value != null)
            {
                ParentAcademicSession = new AcademicSession() { Id = crmSession._msk12_parentsession_value };
            }
        }
        public new void AsJson(JsonWriter writer, string baseUrl)
        {
            writer.WriteStartObject();
            base.AsJson(writer, baseUrl);

            writer.WritePropertyName("title");
            writer.WriteValue(Title);
            writer.WritePropertyName("startDate");
            writer.WriteValue(StartDate.ToString("yyyy-MM-dd"));
            writer.WritePropertyName("endDate");
            writer.WriteValue(EndDate.ToString("yyyy-MM-dd"));
            writer.WritePropertyName("type");
            writer.WriteValue(Enum.GetName(typeof(Vocabulary.SessionType), Type));

            if (ParentAcademicSession != null)
            {
                writer.WritePropertyName("parent");
                ParentAcademicSession.AsJsonReference(writer, baseUrl);
            }

            if (Children != null && Children.Count > 0)
            {
                writer.WritePropertyName("children");
                writer.WriteStartArray();
                Children.ForEach(child => child.AsJsonReference(writer, baseUrl));
                writer.WriteEndArray();
            }

            writer.WritePropertyName("schoolYear");
            writer.WriteValue(SchoolYear);

            writer.WriteEndObject();
            writer.Flush();
        }

        public static new void CsvHeader(CsvWriter writer)
        {
            BaseModel.CsvHeader(writer);

            writer.WriteField("title");
            writer.WriteField("type");
            writer.WriteField("startDate");
            writer.WriteField("endDate");
            writer.WriteField("parentSourcedId");
            writer.WriteField("schoolYear");

            writer.NextRecord();
        }

        public new void AsCsvRow(CsvWriter writer, bool bulk = true)
        {
            base.AsCsvRow(writer, bulk);

            writer.WriteField(Title);
            writer.WriteField(Type);
            writer.WriteField(StartDate.ToString("yyyy-MM-dd"));
            writer.WriteField(EndDate.ToString("yyyy-MM-dd"));
            writer.WriteField(ParentAcademicSessionId);
            writer.WriteField(SchoolYear);

            writer.NextRecord();
        }
    }

    public class CrmAcademicSession : CrmBaseModel
    {
        public override string EntitySetName { get; } = "msk12_academicsessions";
        public override string msk12_sourcedid { get; set; }

        [JsonProperty("msk12_academicsessionid")]
        public string Id { get; set; }
        [JsonIgnore]
        public DateTime CreatedOn { get; set; }
        public DateTime msk12_lastmodifieddate { get; set; }
        public StatusType msk12_status { get; set; }
        public string msk12_title { get; set; }
        public DateTime msk12_startdate { get; set; }
        public DateTime msk12_enddate { get; set; }
        public SessionType msk12_sessiontype { get; set; }
        public string _msk12_parentsession_value { get; set; }
        public string msk12_schoolyear { get; set; }

        public CrmAcademicSession() { }

        public CrmAcademicSession(AcademicSession session)
        {
            Id = msk12_sourcedid = session.Id;
            msk12_status = session.Status;
            msk12_lastmodifieddate = session.UpdatedAt;
            msk12_startdate = session.StartDate;
            msk12_enddate = session.EndDate;
            msk12_sessiontype = session.Type;

            if (session.ParentAcademicSession != null)
            {
                _msk12_parentsession_value = session.ParentAcademicSession.Id;
            }
        }

        public override string ToJson()
        {
            var sb = new StringBuilder();
            using (var writer = new JsonTextWriter(new StringWriter(sb)))
            {
                writer.WriteStartObject();

                writer.WritePropertyName("msk12_sourcedid");
                writer.WriteValue(Id);

                writer.WritePropertyName("msk12_lastmodifieddate");
                writer.WriteValue(msk12_lastmodifieddate.ToUniversalTime().ToString("o"));

                writer.WritePropertyName("msk12_status");
                writer.WriteValue(msk12_status);

                writer.WritePropertyName("msk12_title");
                writer.WriteValue(msk12_title);

                writer.WritePropertyName("msk12_startdate");
                writer.WriteValue(msk12_startdate.ToUniversalTime().ToString("o"));

                writer.WritePropertyName("msk12_enddate");
                writer.WriteValue(msk12_enddate.ToUniversalTime().ToString("o"));

                writer.WritePropertyName("msk12_schoolyear");
                writer.WriteValue(msk12_schoolyear);

                writer.WritePropertyName("msk12_sessiontype");
                writer.WriteValue((int)msk12_sessiontype);

                if (!string.IsNullOrEmpty(_msk12_parentsession_value))
                {
                    writer.WritePropertyName("msk12_parentsession@odata.bind");
                    writer.WriteValue($"/{EntitySetName}(msk12_sourcedid='{_msk12_parentsession_value}')");
                }

                writer.WriteEndObject();
                writer.Flush();
            }
            return sb.ToString();
        }
    }
}
