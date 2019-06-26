/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using Newtonsoft.Json;

namespace EducationAccelerator.Exceptions
{
    public class InvalidFilterFieldException : OneRosterException
    {
        private string field;
        public InvalidFilterFieldException(string _field)
        {
            field = _field;
        }
        public override void AsJson(JsonWriter writer, string operation)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("imsx_codeMajor");
            writer.WriteValue("failure");

            writer.WritePropertyName("imsx_severity");
            writer.WriteValue("error");

            writer.WritePropertyName("imsx_operationRefIdentifier");
            writer.WriteValue(operation);

            writer.WritePropertyName("imsx_description");
            writer.WriteValue(field);

            writer.WritePropertyName("imsx_codeMinor");
            writer.WriteValue("invalid_filter_field");

            writer.WriteEndObject();
        }
    }
}
