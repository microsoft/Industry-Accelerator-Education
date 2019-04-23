/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using System;
using System.ComponentModel.DataAnnotations;

namespace EducationAccelerator.Models
{
    public class OauthNonce
    {
        [Key, Required]
        public string Value { get; set; }

        [Required]
        public DateTime UsedAt { get; set; }

        public bool CanBeUsed()
        {
            long elapsedTicks = DateTime.Now.Ticks - UsedAt.Ticks;

            var elapsedSpan = new TimeSpan(elapsedTicks);

            return elapsedSpan.Minutes > 90;
        }
    }
}
