/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using CsvHelper;
using EducationAccelerator.Validators;
using EducationAccelerator.Vocabulary;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;

namespace EducationAccelerator.Models
{
    public class IMSClass : BaseModel
    {
        internal override string ModelType()
        {
            return "class";
        }

        internal override string UrlType()
        {
            return "classes";
        }

        public static readonly Dictionary<string, string> Fields = new Dictionary<string, string>()
        {
            ["sourcedid"] = "msk12_classid",
            ["createdat"] = "createdon",
            ["datelastmodified"] = "msk12_lastmodifieddate",
            ["status"] = "msk12_status",
            ["title"] = "msk12_title",
            ["classcode"] = "msk12_classcode",
            ["type"] = "msk12_classtype",
            ["location"] = "msk12_location",
            ["school"] = "msk12_schoolid",
            ["course"] = "msk12_courseid",
            ["grades"] = "msk12_grades"
        };

        public static readonly string EntityName = "msk12_class";
        public static readonly string EntitySetName = "msk12_classes";

        [Required]
        public string Title { get; set; }

        public string IMSClassCode { get; set; }

        [Required]
        [JsonProperty("classType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public IMSClassType IMSClassType { get; set; }

        public string Location { get; set; }

        [Required]
        public string CourseId { get; set; }
        public Course Course { get; set; }

        [Required]
        public string SchoolOrgId { get; set; }
        [ForeignKey("SchoolOrgId")]
        public Org School { get; set; }

        // "terms"
        [NotEmptyCollection]
        public virtual List<IMSClassAcademicSession> IMSClassAcademicSessions { get; set; } 

        public virtual List<Enrollment> Enrollments { get; set; }

        [NotMapped]
        [Grades]
        public string[] Grades
        {
            get { return _grades == null ? null : JsonConvert.DeserializeObject<string[]>(_grades); }
            set
            {
                if (value?.GetType() == typeof(string[]))
                {
                    _grades = JsonConvert.SerializeObject(value);
                }
                else
                {
                    _grades = JsonConvert.SerializeObject(value.Select(x => Vocabulary.Grades.Members[x]));
                }
            }
        }
        private string _grades { get; set; }

        [NotMapped]
        public string[] Subjects
        {
            get {
                return SubjectCodes == null ? null : SubjectCodes.Select(code => Vocabulary.SubjectCodes.SubjectMap[code]).ToArray();
            }
        }

        [NotMapped]
        [SubjectCodes]
        public string[] SubjectCodes
        {
            get { return _subjectCodes == null ? null : JsonConvert.DeserializeObject<string[]>(_subjectCodes); }
            set { _subjectCodes = JsonConvert.SerializeObject(value); }
        }
        private string _subjectCodes { get; set; }

        [NotMapped]
        public string[] Periods
        {
            get { return _periods == null ? null : JsonConvert.DeserializeObject<string[]>(_periods); }
            set { _periods = JsonConvert.SerializeObject(value); }
        }
        private string _periods { get; set; }

        [NotMapped]
        public string[] Resources
        {
            get { return _resources == null ? null : JsonConvert.DeserializeObject<string[]>(_resources); }
            set { _resources = JsonConvert.SerializeObject(value); }
        }
        private string _resources { get; set; }

        public IMSClass() { }

        public IMSClass(CrmClass crmClass)
        {
            Id = crmClass.Id;
            CreatedAt = crmClass.CreatedOn;
            UpdatedAt = crmClass.msk12_lastmodifieddate;
            Status = crmClass.msk12_status;
            Title = crmClass.msk12_title;
            IMSClassCode = crmClass.msk12_classcode;
            IMSClassType = crmClass.msk12_classtype;
            Location = crmClass.msk12_location;

            if (crmClass.msk12_grades != null)
            {
                Grades = crmClass.msk12_grades.Split(",");
            }

            if (crmClass._msk12_schoolid_value != null)
            {
                School = new Org() { Id = crmClass._msk12_schoolid_value };
            }

            if (crmClass._msk12_courseid_value != null)
            {
                Course = new Course() { Id = crmClass._msk12_courseid_value };
            }
        }

        public new void AsJson(JsonWriter writer, string baseUrl)
        {
            writer.WriteStartObject();
            base.AsJson(writer, baseUrl);

            writer.WritePropertyName("title");
            writer.WriteValue(Title);

            if(!String.IsNullOrEmpty(IMSClassCode))
            {
                writer.WritePropertyName("classCode");
                writer.WriteValue(IMSClassCode);
            }

            writer.WritePropertyName("classType");
            writer.WriteValue(Enum.GetName(typeof(IMSClassType), IMSClassType));

            if (!String.IsNullOrEmpty(Location))
            {
                writer.WritePropertyName("location");
                writer.WriteValue(Location);
            }

            if (Grades != null && Grades.Length > 0)
            {
                writer.WritePropertyName("grades");
                writer.WriteStartArray();
                foreach(var grade in Grades)
                {
                    writer.WriteValue(grade);
                }
                writer.WriteEndArray();
            }

            if (Subjects != null && Subjects.Length > 0)
            {
                writer.WritePropertyName("subjects");
                writer.WriteStartArray();
                foreach (var subject in Subjects)
                {
                    writer.WriteValue(subject);
                }
                writer.WriteEndArray();
            }

            if (Course != null)
            {
                writer.WritePropertyName("course");
                Course.AsJsonReference(writer, baseUrl);
            }

            writer.WritePropertyName("resources");
            writer.WriteStartArray();
            if (Course != null && Course.Resources != null)
            {
                foreach (var resource in Course.Resources)
                {
                    writer.WriteValue(resource);
                }
            }
            if (Resources != null)
            {
                foreach (var resource in Resources)
                {
                    writer.WriteValue(resource);
                }
            }
            writer.WriteEndArray();

            if (School != null)
            {
                writer.WritePropertyName("school");
                School.AsJsonReference(writer, baseUrl);
            }

            writer.WritePropertyName("terms");
            writer.WriteStartArray();
            if (IMSClassAcademicSessions != null)
            {
                IMSClassAcademicSessions.ForEach(join => join.AcademicSession.AsJsonReference(writer, baseUrl));
            }
            writer.WriteEndArray();

            if (SubjectCodes != null && SubjectCodes.Length > 0)
            {
                writer.WritePropertyName("subjectCodes");
                writer.WriteStartArray();
                foreach (var subjectCode in SubjectCodes)
                {
                    writer.WriteValue(subjectCode);
                }
                writer.WriteEndArray();
            }

            if (Periods != null && Periods.Length > 0)
            {
                writer.WritePropertyName("periods");
                writer.WriteStartArray();
                foreach (var period in Periods)
                {
                    writer.WriteValue(period);
                }
                writer.WriteEndArray();
            }

            writer.WriteEndObject();
            writer.Flush();
        }

        public static new void CsvHeader(CsvWriter writer)
        {
            BaseModel.CsvHeader(writer);

            writer.WriteField("title");
            writer.WriteField("grades");
            writer.WriteField("courseSourcedId");
            writer.WriteField("classCode");
            writer.WriteField("classType");
            writer.WriteField("location");
            writer.WriteField("schoolSourcedId");
            writer.WriteField("termSourcedIds");
            writer.WriteField("subjects");
            writer.WriteField("subjectCodes");
            writer.WriteField("periods");

            writer.NextRecord();
        }

        public new void AsCsvRow(CsvWriter writer, bool bulk = true)
        {
            base.AsCsvRow(writer, bulk);

            writer.WriteField(Title);
            writer.WriteField(String.Join(',', Grades));
            writer.WriteField(CourseId);
            writer.WriteField(IMSClassCode);
            writer.WriteField(IMSClassType);
            writer.WriteField(Location);
            writer.WriteField(SchoolOrgId);
            writer.WriteField(String.Join(',', IMSClassAcademicSessions.Select(kas => kas.AcademicSessionId)));
            writer.WriteField(String.Join(',', Subjects));
            writer.WriteField(String.Join(',', SubjectCodes));
            writer.WriteField(String.Join(',', Periods));

            writer.NextRecord();
        }
    }

    public class CrmClass : CrmBaseModel
    {
        public override string EntitySetName { get; } = "msk12_classes";
        public override string msk12_sourcedid { get; set; }

        [JsonProperty("msk12_classid")]
        public string Id { get; set; }
        [JsonIgnore]
        public DateTime CreatedOn { get; set; }
        public DateTime msk12_lastmodifieddate { get; set; }
        public StatusType msk12_status { get; set; }
        public string msk12_title { get; set; }
        public string msk12_classcode { get; set; }
        public IMSClassType msk12_classtype { get; set; }
        public string msk12_location { get; set; }
        public string _msk12_schoolid_value { get; set; }
        public string _msk12_courseid_value { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public string msk12_grades { get; set; }

        public CrmClass() { }

        public CrmClass(IMSClass c)
        {
            Id = msk12_sourcedid = c.Id;
            msk12_lastmodifieddate = c.UpdatedAt;
            msk12_status = c.Status;
            msk12_title = c.Title;
            msk12_classcode = c.IMSClassCode;
            msk12_classtype = c.IMSClassType;
            msk12_location = c.Location;
            
            if (c.Grades != null && c.Grades.Length > 0)
            {
                msk12_grades = string.Join(",", c.Grades
                    .Where(g => Grades.Members.ContainsValue(g))
                    .Select(g => Grades.Members.FirstOrDefault(kvp => kvp.Value == g).Key)
                    .ToArray());
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

                writer.WritePropertyName("msk12_classcode");
                writer.WriteValue(msk12_classcode);

                writer.WritePropertyName("msk12_classtype");
                writer.WriteValue((int)msk12_classtype);

                writer.WritePropertyName("msk12_location");
                writer.WriteValue(msk12_location);

                // Cannot upsert a null value for a multiselect
                if (msk12_grades != null)
                {
                    writer.WritePropertyName("msk12_grades");
                    writer.WriteValue(msk12_grades);
                }

                if (!string.IsNullOrEmpty(_msk12_schoolid_value))
                {
                    writer.WritePropertyName("msk12_schoolid@odata.bind");
                    writer.WriteValue($"/accounts(msk12_sourcedid='{_msk12_schoolid_value}')");
                }

                if (!string.IsNullOrEmpty(_msk12_courseid_value))
                {
                    writer.WritePropertyName("msk12_courseid@odata.bind");
                    writer.WriteValue($"/msk12_courses(msk12_sourcedid='{_msk12_courseid_value}')");
                }

                writer.WriteEndObject();
                writer.Flush();
            }
            return sb.ToString();
        }
    }
}