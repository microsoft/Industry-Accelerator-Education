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
    public class Enrollment : BaseModel
    {
        internal override string ModelType()
        {
            return "enrollment";
        }

        internal override string UrlType()
        {
            return "enrollments";
        }

        // come back and add Primary when it's fixed
        public static readonly Dictionary<string, string> Fields = new Dictionary<string, string>()
        {
            ["sourcedid"] = "msk12_enrollmentid",
            ["createdat"] = "createdon",
            ["datelastmodified"] = "msk12_lastmodifieddate",
            ["status"] = "msk12_status",
            ["role"] = "msk12_role",
            ["begindate"] = "msk12_begindate",
            ["enddate"] = "msk12_enddate",
            ["user"] = "msk12_contact",
            ["school"] = "msk12_account",
            ["class"] = "msk12_classid",
            ["primary"] = "msk12_primaryteacher"
        };

        public static readonly string EntityName = "msk12_enrollment";
        public static readonly string EntitySetName = "msk12_enrollments";

        [Required]
        public RoleType Role { get; set; }

        public Boolean? Primary { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        public string IMSClassId { get; set; }
        public IMSClass IMSClass { get; set; }

        [Required]
        public string SchoolOrgId { get; set; }
        [ForeignKey("SchoolOrgId")]
        public Org School { get; set; }
        public Enrollment()
        {

        }

        public Enrollment(CrmEnrollment crmEnrollment)
        {
            Id = crmEnrollment.Id;
            CreatedAt = crmEnrollment.CreatedOn;
            UpdatedAt = crmEnrollment.msk12_lastmodifieddate;
            Status = crmEnrollment.msk12_status;
            Role = crmEnrollment.msk12_role;
            BeginDate = crmEnrollment.msk12_begindate;
            EndDate = crmEnrollment.msk12_enddate;
            Primary = crmEnrollment.msk12_primaryteacher;

            if (crmEnrollment._msk12_contact_value != null)
            {
                User = new User() { Id = crmEnrollment._msk12_contact_value };
            }

            if (crmEnrollment._msk12_account_value != null)
            {
                School = new Org() { Id = crmEnrollment._msk12_account_value };
            }

            if (crmEnrollment._msk12_classid_value != null)
            {
                IMSClass = new IMSClass() { Id = crmEnrollment._msk12_classid_value };
            }
    }

        public new void AsJson(JsonWriter writer, string baseUrl)
        {
            writer.WriteStartObject();
            base.AsJson(writer, baseUrl);

            if (User != null)
            {
                writer.WritePropertyName("user");
                User.AsJsonReference(writer, baseUrl);
            }

            if (IMSClass != null)
            {
                writer.WritePropertyName("class");
                IMSClass.AsJsonReference(writer, baseUrl);
            }

            if (School != null)
            {
                writer.WritePropertyName("school");
                School.AsJsonReference(writer, baseUrl);
            }

            writer.WritePropertyName("role");
            writer.WriteValue(Enum.GetName(typeof(RoleType), Role));

            if (Primary != null)
            {
                writer.WritePropertyName("primary");
                writer.WriteValue(Primary.ToString());
            }

            if (BeginDate != null)
            {
                writer.WritePropertyName("beginDate");
                writer.WriteValue(BeginDate.ToString("yyyy-MM-dd"));
            }

            if (EndDate != null)
            {
                writer.WritePropertyName("endDate");
                writer.WriteValue(EndDate.ToString("yyyy-MM-dd"));
            }

            writer.WriteEndObject();
            writer.Flush();
        }

        public static new void CsvHeader(CsvWriter writer)
        {
            BaseModel.CsvHeader(writer);
            writer.WriteField("classSourcedId");
            writer.WriteField("schoolSourcedId");
            writer.WriteField("userSourcedId");
            writer.WriteField("role");
            writer.WriteField("primary");
            writer.WriteField("beginDate");
            writer.WriteField("endDate");

            writer.NextRecord();
        }

        public new void AsCsvRow(CsvWriter writer, bool bulk = true)
        {
            base.AsCsvRow(writer, bulk);
            writer.WriteField(IMSClassId);
            writer.WriteField(SchoolOrgId);
            writer.WriteField(UserId);
            writer.WriteField(Role);
            writer.WriteField(Primary == null ? Primary.ToString() : "");
            writer.WriteField(BeginDate.ToString("yyyy-MM-dd"));
            writer.WriteField(EndDate.ToString("yyyy-MM-dd"));

            writer.NextRecord();
        }
    }

    public class CrmEnrollment : CrmBaseModel
    {
        public override string EntitySetName { get; } = "msk12_enrollments";
        public override string msk12_sourcedid { get; set; }

        [JsonProperty("msk12_enrollmentid")]
        public string Id { get; set; }
        [JsonIgnore]
        public DateTime CreatedOn { get; set; }
        public DateTime msk12_lastmodifieddate { get; set; }
        public StatusType msk12_status { get; set; }
        public RoleType msk12_role { get; set; }
        public DateTime msk12_begindate { get; set; }
        public DateTime msk12_enddate { get; set; }
        public string _msk12_contact_value { get; set; }
        public string _msk12_account_value { get; set; }
        public string _msk12_classid_value { get; set; }
        public bool? msk12_primaryteacher { get; set; }

        public CrmEnrollment() { }

        public CrmEnrollment(Enrollment e)
        {
            Id = msk12_sourcedid = e.Id;
            msk12_lastmodifieddate = e.UpdatedAt;
            msk12_status = e.Status;
            msk12_role = e.Role;
            msk12_begindate = e.BeginDate;
            msk12_enddate = e.EndDate;
            _msk12_contact_value = e.UserId;
            _msk12_account_value = e.SchoolOrgId;
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

                writer.WritePropertyName("msk12_primaryteacher");
                writer.WriteValue(msk12_primaryteacher);

                writer.WritePropertyName("msk12_begindate");
                writer.WriteValue(msk12_begindate.ToUniversalTime().ToString("o"));

                writer.WritePropertyName("msk12_enddate");
                writer.WriteValue(msk12_enddate.ToUniversalTime().ToString("o"));

                writer.WritePropertyName("msk12_role");
                writer.WriteValue((int)msk12_role);

                if (!string.IsNullOrEmpty(_msk12_contact_value))
                {
                    writer.WritePropertyName("msk12_contact@odata.bind");
                    writer.WriteValue($"/contacts(msk12_sourcedid='{_msk12_contact_value}')");
                }

                if (!string.IsNullOrEmpty(_msk12_account_value))
                {
                    writer.WritePropertyName("msk12_account@odata.bind");
                    writer.WriteValue($"/accounts(msk12_sourcedid='{_msk12_account_value}')");
                }

                if (!string.IsNullOrEmpty(_msk12_classid_value))
                {
                    writer.WritePropertyName("msk12_classid@odata.bind");
                    writer.WriteValue($"/msk12_classes(msk12_sourcedid='{_msk12_classid_value}')");
                }

                writer.WriteEndObject();
                writer.Flush();
            }
            return sb.ToString();
        }
    }
}
