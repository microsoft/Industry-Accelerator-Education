/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using System;

namespace EducationAccelerator.Models
{
    public class UserOrg : BaseModel
    {
        internal override string ModelType()
        {
            throw new NotImplementedException();
        }

        internal override string UrlType()
        {
            throw new NotImplementedException();
        }

        public string UserId { get; set; }
        public User User { get; set; }

        public string OrgId { get; set; }
        public Org Org { get; set; }
    }
}
