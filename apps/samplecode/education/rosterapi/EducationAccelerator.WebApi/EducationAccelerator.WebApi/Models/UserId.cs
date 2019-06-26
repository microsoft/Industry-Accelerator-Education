/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using Newtonsoft.Json;

namespace EducationAccelerator.Models
{
    public class UserId
    {
        public string Type { get; set; }
        public string Identifier { get; set; }

        public void AsJson(JsonWriter writer, string baseUrl)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("type");
            writer.WriteValue(Type);
            writer.WritePropertyName("identifier");
            writer.WriteValue(Identifier);

            writer.WriteEndObject();

        }
    }
}
