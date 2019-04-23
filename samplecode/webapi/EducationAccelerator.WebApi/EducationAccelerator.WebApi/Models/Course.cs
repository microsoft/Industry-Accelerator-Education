/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using CsvHelper;
using EducationAccelerator.Validators;
using EducationAccelerator.Vocabulary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace EducationAccelerator.Models
{
    public class Course : BaseModel
    {
        internal override string ModelType()
        {
            return "course";
        }

        internal override string UrlType()
        {
            return "courses";
        }

        public static readonly Dictionary<string, string> Fields = new Dictionary<string, string>
        {
            ["sourcedid"] = "msk12_courseid",
            ["createdat"] = "createdon",
            ["datelastmodified"] = "msk12_lastmodifieddate",
            ["status"] = "msk12_status",
            ["title"] = "msk12_title",
            ["schoolyearacademicsession"] = "msk12_academicsession",
            ["coursecode"] = "msk12_coursecode",
            ["org"] = "msk12_account",
            ["grades"] = "msk12_grades"
        };

        public static readonly string EntityName = "msk12_course";
        public static readonly string EntitySetName = "msk12_courses";

        [Required]
        public string Title { get; set; }

        // SchoolYear
        public string SchoolYearAcademicSessionId { get; set; }
        public AcademicSession SchoolYearAcademicSession { get; set; }

        public string CourseCode { get; set; }

        [Required]
        public string OrgId { get; set; }
        public Org Org { get; set; }

        [NotMapped]
        public string[] Resources
        {
            get { return _resources == null ? null : JsonConvert.DeserializeObject<string[]>(_resources); }
            set { _resources = JsonConvert.SerializeObject(value); }
        }
        private string _resources { get; set; }

        [NotMapped]
        [Grades]
        public string[] Grades
        {
            get { return _grades == null ? null : JsonConvert.DeserializeObject<string[]>(_grades); }
            set { _grades = JsonConvert.SerializeObject(value.Select(x => Vocabulary.Grades.Members[x])); }
        }
        private string _grades { get; set; }

        [NotMapped]
        public string[] Subjects => SubjectCodes?.Select(code => Vocabulary.SubjectCodes.SubjectMap[code]).ToArray();

        [NotMapped]
        [SubjectCodes]
        public string[] SubjectCodes
        {
            get { return _subjectCodes == null ? null : JsonConvert.DeserializeObject<string[]>(_subjectCodes); }
            set { _subjectCodes = JsonConvert.SerializeObject(value); }
        }
        private string _subjectCodes { get; set; }

        public Course() { }

        public Course(CrmCourse crmCourse)
        {
            Id = crmCourse.Id;
            CreatedAt = crmCourse.CreatedOn;
            UpdatedAt = crmCourse.msk12_lastmodifieddate;
            Status = crmCourse.msk12_status;
            Title = crmCourse.msk12_title;
            CourseCode = crmCourse.msk12_coursecode;

            if (crmCourse.msk12_grades != null)
            {
                Grades = crmCourse.msk12_grades.Split(",");
            }

            if (crmCourse._msk12_academicsession_value != null)
            {
                SchoolYearAcademicSession = new AcademicSession() { Id = crmCourse._msk12_academicsession_value };
            }

            if (crmCourse._msk12_account_value != null)
            {
                Org = new Org() { Id = crmCourse._msk12_account_value };
            }
        }

        public new void AsJson(JsonWriter writer, string baseUrl)
        {
            writer.WriteStartObject();
            base.AsJson(writer, baseUrl);

            writer.WritePropertyName("title");
            writer.WriteValue(Title);

            if (SchoolYearAcademicSession != null)
            {
                writer.WritePropertyName("schoolYear");
                SchoolYearAcademicSession.AsJsonReference(writer, baseUrl);
            }

            if (!String.IsNullOrEmpty(CourseCode))
            {
                writer.WritePropertyName("courseCode");
                writer.WriteValue(CourseCode);
            }

            if (Grades != null && Grades.Length > 0)
            {
                writer.WritePropertyName("grades");
                writer.WriteStartArray();
                foreach (var grade in Grades)
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

                writer.WritePropertyName("subjectCodes");
                writer.WriteStartArray();
                foreach (var subjectCode in SubjectCodes)
                {
                    writer.WriteValue(subjectCode);
                }
                writer.WriteEndArray();
            }

            if (Resources != null && Resources.Length > 0)
            {
                writer.WritePropertyName("resources");
                writer.WriteStartArray();
                foreach (var resource in Resources)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("href");
                    writer.WriteValue(baseUrl + "/resources/" + resource);
                    writer.WritePropertyName("sourceId");
                    writer.WriteValue(resource);
                    writer.WritePropertyName("type");
                    writer.WriteValue("resource");
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }

            if (Org != null)
            {
                writer.WritePropertyName("org");
                Org.AsJsonReference(writer, baseUrl);
            }

            writer.WriteEndObject();
            writer.Flush();
        }

        public static new void CsvHeader(CsvWriter writer)
        {
            BaseModel.CsvHeader(writer);
            writer.WriteField("schoolYearSourcedId");
            writer.WriteField("title");
            writer.WriteField("courseCode");
            writer.WriteField("grades");
            writer.WriteField("orgSourcedId");
            writer.WriteField("subjects");
            writer.WriteField("subjectCodes");

            writer.NextRecord();
        }

        public new void AsCsvRow(CsvWriter writer, bool bulk = true)
        {
            base.AsCsvRow(writer, bulk);
            writer.WriteField(SchoolYearAcademicSessionId);
            writer.WriteField(Title);
            writer.WriteField(CourseCode);
            writer.WriteField(String.Join(',', Grades));
            writer.WriteField(OrgId);
            writer.WriteField(String.Join(',', Subjects));
            writer.WriteField(String.Join(',', SubjectCodes));

            writer.NextRecord();
        }
    }

    public class CrmCourse
    {
        [JsonProperty("msk12_courseid")]
        public string Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime msk12_lastmodifieddate { get; set; }
        public StatusType msk12_status { get; set; }
        public string msk12_title { get; set; }
        public string _msk12_academicsession_value { get; set; }
        public string msk12_coursecode { get; set; }
        public string _msk12_account_value { get; set; }
        public string msk12_grades { get; set; }
    }
}
