/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EducationAccelerator.Validators
{
    public class SubjectCodesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string[] subjectCodes = value as string[];
            if (subjectCodes.All(subjectCode => Vocabulary.SubjectCodes.SubjectMap.Keys.Contains(subjectCode)))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid subjectCode entry");
        }
    }
}
