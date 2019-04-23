/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EducationAccelerator.Validators
{
    public class GradesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string[] grades = value as string[];
            if(grades.All(grade => Vocabulary.Grades.Members.ContainsKey(grade)))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid grade entry");
        }
    }
}
