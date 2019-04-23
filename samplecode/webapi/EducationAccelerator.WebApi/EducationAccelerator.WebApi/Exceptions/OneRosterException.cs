/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using Newtonsoft.Json;
using System;

namespace EducationAccelerator.Exceptions
{
    public abstract class OneRosterException : Exception
    {
        public abstract void AsJson(JsonWriter writer, string operation);
    }
}
