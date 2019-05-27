/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace EducationAccelerator.Validators
{
    public class NotEmptyCollectionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ICollection collection = value as ICollection;

            if (collection.Count == 0)
            {
                return new ValidationResult($"At least one {validationContext.MemberName} required");
            }
            return ValidationResult.Success;
        }
    }
}
