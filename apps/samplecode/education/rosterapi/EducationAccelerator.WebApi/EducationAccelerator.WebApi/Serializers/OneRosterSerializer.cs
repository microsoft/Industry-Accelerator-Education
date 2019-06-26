/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace EducationAccelerator.Serializers
{
    public class OneRosterSerializer
    {
        private string _root;
        private StringBuilder _sb;

        public JsonWriter writer { get; }
        public string Finish()
        {
            writer.WriteEndObject();
            return _sb.ToString();
        }

        public OneRosterSerializer(string root)
        {
            _root = root;
            _sb = new StringBuilder();
            writer = new JsonTextWriter(new StringWriter(_sb));
            writer.WriteStartObject();
            writer.WritePropertyName(root);
        }
    }
}
