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
using System.IO;
using System.Linq;
using System.Text;

namespace EducationAccelerator.Models
{
    public class User : BaseModel
    {
        internal override string ModelType()
        {
            return "user";
        }

        internal override string UrlType()
        {
            return "users";
        }

        public static readonly Dictionary<string, string> Fields = new Dictionary<string, string>()
        {
            ["sourcedid"] = "contactid",
            ["createdat"] = "createdon",
            ["datelastmodified"] = "msk12_lastmodifieddate",
            ["status"] = "msk12_status",
            ["username"] = "msk12_username",
            ["enableduser"] = "msk12_isenabled",
            ["givenname"] = "firstname",
            ["familyname"] = "lastname",
            ["middlename"] = "middlename",
            ["role"] = "msk12_role",
            ["identifier"] = "msk12_identifier",
            ["email"] = "emailaddress1",
            ["sms"] = "mobilephone",
            ["phone"] = "telephone1",
            ["grades"] = "msk12_grades"
        };

        public static readonly string EntityName = "contact";
        public static readonly string EntitySetName = "contacts";

        [Required]
        public string Username { get; set; }

        [NotMapped]
        public UserId[] UserIds
        {
            get { return _userIds == null ? null : JsonConvert.DeserializeObject<UserId[]>(_userIds); }
            set { _userIds = JsonConvert.SerializeObject(value); }
        }
        private string _userIds { get; set; }

        [Required]
        public Boolean EnabledUser { get; set; }

        [Required]
        public string GivenName { get; set; }

        [Required]
        public string FamilyName { get; set; }
        public string MiddleName { get; set; }

        [Required]
        public RoleType Role { get; set; }

        public string Identifier { get; set; }
        public string Email { get; set; }
        public string SMS { get; set; }
        public string Phone { get; set; }

        public List<UserAgent> UserAgents { get; set; }
        [NotEmptyCollection]
        public List<UserOrg> UserOrgs { get; set; }
        public List<Enrollment> Enrollments { get; set; }

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
        public string Pass { get; set; }

        public User() { }

        public User(CrmUser crmUser)
        {
            Id = crmUser.Id;
            CreatedAt = crmUser.CreatedOn;
            UpdatedAt = crmUser.msk12_lastmodifieddate;
            Status = crmUser.msk12_status;
            Username = crmUser.msk12_username;
            EnabledUser = crmUser.msk12_isenabled;
            GivenName = crmUser.FirstName;
            MiddleName = crmUser.MiddleName;
            FamilyName = crmUser.LastName;
            Role = crmUser.msk12_role;
            Identifier = crmUser.msk12_identifier;
            Email = crmUser.EmailAddress1;
            SMS = crmUser.MobilePhone;
            Phone = crmUser.Telephone1;
            Pass = crmUser.msk12_password;

            if (crmUser.msk12_grades != null)
            {
                Grades = crmUser.msk12_grades.Split(",");
            }
        }

        public new void AsJson(JsonWriter writer, string baseUrl)
        {
            writer.WriteStartObject();
            base.AsJson(writer, baseUrl);

            writer.WritePropertyName("username");
            writer.WriteValue(Username);

            if (UserIds != null && UserIds.Length > 0)
            {
                writer.WritePropertyName("userIds");
                writer.WriteStartArray();
                foreach (var userId in UserIds)
                {
                    userId.AsJson(writer, baseUrl);
                }
                writer.WriteEndArray();
            }

            writer.WritePropertyName("enabledUser");
            writer.WriteValue(EnabledUser.ToString());

            writer.WritePropertyName("givenName");
            writer.WriteValue(GivenName);

            writer.WritePropertyName("familyName");
            writer.WriteValue(FamilyName);

            if (!String.IsNullOrEmpty(MiddleName))
            {
                writer.WritePropertyName("middleName");
                writer.WriteValue(MiddleName);
            }

            writer.WritePropertyName("role");
            writer.WriteValue(Enum.GetName(typeof(Vocabulary.RoleType), Role));

            if (!String.IsNullOrEmpty(Identifier))
            {
                writer.WritePropertyName("identifier");
                writer.WriteValue(Identifier);
            }

            if (!String.IsNullOrEmpty(Email))
            {
                writer.WritePropertyName("email");
                writer.WriteValue(Email);
            }

            if (!String.IsNullOrEmpty(SMS))
            {
                writer.WritePropertyName("sms");
                writer.WriteValue(SMS);
            }

            if (!String.IsNullOrEmpty(Phone))
            {
                writer.WritePropertyName("phone");
                writer.WriteValue(Phone);
            }

            if (UserAgents != null && UserAgents.Count > 0)
            {
                writer.WritePropertyName("agents");
                writer.WriteStartArray();
                UserAgents.ForEach(ua => ua.Agent.AsJsonReference(writer, baseUrl));
                writer.WriteEndArray();
            }

            if (UserOrgs != null)
            {
                writer.WritePropertyName("orgs");
                writer.WriteStartArray();
                UserOrgs.ForEach(uo => uo.Org.AsJsonReference(writer, baseUrl));
                writer.WriteEndArray();
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

            if (!String.IsNullOrEmpty(Pass))
            {
                writer.WritePropertyName("password");
                writer.WriteValue(Pass);
            }

            writer.WriteEndObject();
            writer.Flush();
        }

        public static new void CsvHeader(CsvWriter writer)
        {
            BaseModel.CsvHeader(writer);
            
            writer.WriteField("enabledUser");
            writer.WriteField("orgSourcedIds");
            writer.WriteField("role");
            writer.WriteField("username");
            writer.WriteField("userIds");
            writer.WriteField("givenName");
            writer.WriteField("familyName");
            writer.WriteField("middleName");
            writer.WriteField("identifier");
            writer.WriteField("email");
            writer.WriteField("sms");
            writer.WriteField("phone");
            writer.WriteField("agentSourcedIds");
            writer.WriteField("grades");
            writer.WriteField("password");

            writer.NextRecord();
        }

        public new void AsCsvRow(CsvWriter writer, bool bulk = true)
        {
            base.AsCsvRow(writer, bulk);
            
            writer.WriteField(EnabledUser);
            writer.WriteField(String.Join(',', UserOrgs.Select(uo => uo.OrgId)));
            writer.WriteField(Role);
            writer.WriteField(Username);
            writer.WriteField(UserIds == null ? "" : String.Join(',', UserIds.Select(ui => $"{{{ui.Type}:{ui.Identifier}}}")));
            writer.WriteField(GivenName);
            writer.WriteField(FamilyName);
            writer.WriteField(MiddleName);
            writer.WriteField(Identifier);
            writer.WriteField(Email);
            writer.WriteField(SMS);
            writer.WriteField(Phone);
            writer.WriteField(String.Join(',', UserAgents.Select(ua => ua.AgentUserId)));
            writer.WriteField(String.Join(',', Grades));
            writer.WriteField(Pass);

            writer.NextRecord();
        }
    }

    public class CrmUser : CrmBaseModel
    {
        public override string EntitySetName { get; } = "contacts";
        public override string msk12_sourcedid { get; set; }

        [JsonProperty("contactid")]
        public string Id { get; set; }
        [JsonIgnore]
        public DateTime CreatedOn { get; set; }
        public DateTime msk12_lastmodifieddate { get; set; }
        public StatusType msk12_status { get; set; }
        public string msk12_username { get; set; }
        public bool msk12_isenabled { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public RoleType msk12_role { get; set; }
        public string msk12_identifier { get; set; }
        public string EmailAddress1 { get; set; }
        public string MobilePhone { get; set; }
        public string Telephone1 { get; set; }
        public string msk12_password { get; set; }
        public string msk12_grades { get; set; }

        public CrmUser() { }

        public CrmUser(User u)
        {
            Id = msk12_sourcedid = u.Id;
            msk12_lastmodifieddate = u.UpdatedAt;
            msk12_status = u.Status;
            msk12_username = u.Username;
            msk12_isenabled = u.EnabledUser;
            FirstName = u.GivenName;
            MiddleName = u.MiddleName;
            LastName = u.FamilyName;
            msk12_role = u.Role;
            msk12_identifier = u.Identifier;
            EmailAddress1 = u.Email;
            MobilePhone = u.SMS;
            Telephone1 = u.Phone;
            msk12_password = u.Pass;

            if (u.Grades != null && u.Grades.Length > 0)
            {
                msk12_grades = string.Join(",", u.Grades
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

                writer.WritePropertyName("msk12_username");
                writer.WriteValue(msk12_username);

                writer.WritePropertyName("msk12_isenabled");
                writer.WriteValue(msk12_isenabled);

                writer.WritePropertyName("firstname");
                writer.WriteValue(FirstName);

                writer.WritePropertyName("middlename");
                writer.WriteValue(MiddleName);

                writer.WritePropertyName("lastname");
                writer.WriteValue(LastName);

                writer.WritePropertyName("msk12_identifier");
                writer.WriteValue(msk12_identifier);

                writer.WritePropertyName("emailaddress1");
                writer.WriteValue(EmailAddress1);

                writer.WritePropertyName("mobilephone");
                writer.WriteValue(MobilePhone);

                writer.WritePropertyName("telephone1");
                writer.WriteValue(Telephone1);

                writer.WritePropertyName("msk12_password");
                writer.WriteValue(msk12_password);

                // Cannot upsert a null value for a multiselect
                if (msk12_grades != null)
                {
                    writer.WritePropertyName("msk12_grades");
                    writer.WriteValue(msk12_grades);
                }

                writer.WritePropertyName("msk12_role");
                writer.WriteValue((int)msk12_role);

                writer.WriteEndObject();
                writer.Flush();
            }
            return sb.ToString();
        }
    }
}
