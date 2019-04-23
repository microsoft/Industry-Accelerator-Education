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
    public class Org : BaseModel
    {
        internal override string ModelType()
        {
            return "org";
        }

        internal override string UrlType()
        {
            return "orgs";
        }

        public static readonly Dictionary<string, string> Fields = new Dictionary<string, string>()
        {
            ["sourcedid"] = "accountid",
            ["createdat"] = "createdon",
            ["datelastmodified"] = "msk12_lastmodifieddate",
            ["status"] = "msk12_status",
            ["name"] = "name",
            ["type"] = "msk12_accounttype",
            ["identifier"] = "msk12_identifier",
            ["parent"] = "parentaccountid"
        };

        public static readonly string EntityName = "account";
        public static readonly string EntitySetName = "accounts";

        [Required]
        public string Name { get; set; }

        [Required]
        public OrgType Type { get; set; }

        public string Identifier { get; set; }

        public string ParentOrgId { get; set; }
        
        public virtual Org Parent { get; set; }

        [InverseProperty("Parent")]
        public virtual List<Org> Children { get; set; }

        public Org() { }

        public Org(CrmOrg crmOrg)
        {
            Id = crmOrg.Id;
            CreatedAt = crmOrg.CreatedOn;
            UpdatedAt = crmOrg.msk12_lastmodifieddate;
            Status = crmOrg.msk12_status;
            Name = crmOrg.Name;
            Type = crmOrg.msk12_accounttype;
            Identifier = crmOrg.msk12_identifier;

            if (crmOrg._parentaccountid_value != null)
            {
                Parent = new Org() { Id = crmOrg._parentaccountid_value };
            }
        }

        public new void AsJson(JsonWriter writer, string baseUrl)
        {
            writer.WriteStartObject();
            base.AsJson(writer, baseUrl);

            writer.WritePropertyName("name");
            writer.WriteValue(Name);

            writer.WritePropertyName("type");
            writer.WriteValue(Enum.GetName(typeof(Vocabulary.OrgType), Type));

            if (!String.IsNullOrEmpty(Identifier))
            {
                writer.WritePropertyName("identifier");
                writer.WriteValue(Identifier);
            }

            if (Parent != null)
            {
                writer.WritePropertyName("parent");
                Parent.AsJsonReference(writer, baseUrl);
            }

            if (Children != null && Children.Count > 0)
            {
                writer.WritePropertyName("children");
                writer.WriteStartArray();
                Children.ForEach(child => child.AsJsonReference(writer, baseUrl));
                writer.WriteEndArray();
            }

            writer.WriteEndObject();
            writer.Flush();
        }

        public static new void CsvHeader(CsvWriter writer)
        {
            BaseModel.CsvHeader(writer);

            writer.WriteField("name");
            writer.WriteField("type");
            writer.WriteField("identifier");
            writer.WriteField("parentSourcedId");

            writer.NextRecord();
        }

        public new void AsCsvRow(CsvWriter writer, bool bulk = true)
        {
            base.AsCsvRow(writer, bulk);

            writer.WriteField(Name);
            writer.WriteField(Type);
            writer.WriteField(Identifier);
            writer.WriteField(ParentOrgId);

            writer.NextRecord();
        }
    }

    public class CrmOrg : CrmBaseModel
    {
        public override string EntitySetName { get; } = "accounts";
        public override string msk12_sourcedid { get; set; }

        [JsonProperty("accountid")]
        public string Id { get; set; }
        [JsonIgnore]
        public DateTime CreatedOn { get; set; }
        public DateTime msk12_lastmodifieddate { get; set; }
        public StatusType msk12_status { get; set; }
        public string Name { get; set; }
        public OrgType msk12_accounttype { get; set; }
        public string msk12_identifier { get; set; }
        public string _parentaccountid_value { get; set; }

        public CrmOrg() { }

        public CrmOrg(Org org)
        {
            Id = msk12_sourcedid = org.Id;
            msk12_lastmodifieddate = org.UpdatedAt;
            msk12_status = org.Status;
            Name = org.Name;
            msk12_accounttype = org.Type;
            msk12_identifier = org.Identifier;

            if (org.Parent != null)
            {
                _parentaccountid_value = org.Parent.Id;
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

                writer.WritePropertyName("name");
                writer.WriteValue(Name);

                writer.WritePropertyName("msk12_accounttype");
                writer.WriteValue(msk12_accounttype);

                writer.WritePropertyName("msk12_identifier");
                writer.WriteValue(msk12_identifier);

                if (!string.IsNullOrEmpty(_parentaccountid_value))
                {
                    writer.WritePropertyName("parentaccountid@odata.bind");
                    writer.WriteValue($"/accounts(msk12_sourcedid='{_parentaccountid_value}')");
                }

                writer.WriteEndObject();
                writer.Flush();
            }
            return sb.ToString();
        }
    }
}
