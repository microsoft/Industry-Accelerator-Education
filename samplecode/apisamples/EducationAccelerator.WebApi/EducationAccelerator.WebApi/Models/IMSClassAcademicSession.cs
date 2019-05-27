/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using System;

namespace EducationAccelerator.Models
{
    public class IMSClassAcademicSession : BaseModel
    {
        internal override string ModelType()
        {
            throw new NotImplementedException();
        }

        internal override string UrlType()
        {
            throw new NotImplementedException();
        }

        public string IMSClassId { get; set; }
        public IMSClass IMSClass { get; set; }

        public string AcademicSessionId { get; set; }
        public AcademicSession AcademicSession { get; set; }
    }
}
