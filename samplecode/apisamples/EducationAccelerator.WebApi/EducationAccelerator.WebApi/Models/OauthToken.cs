/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using System;
using System.ComponentModel.DataAnnotations;

namespace EducationAccelerator.Models
{
    public class OauthToken
    {
        [Key, Required]
        public string Value { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public bool CanBeUsed()
        {
            long elapsedTicks = DateTime.Now.Ticks - CreatedAt.Ticks;
            var elapsedSpan = new TimeSpan(elapsedTicks);
            return elapsedSpan.TotalMinutes < 1440;
        }
    }
}
