/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducationAccelerator.ActionResults
{
    public class OneRosterResult : ContentResult
    {
        public int? count;
        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (count != null)
            {
                context.HttpContext.Response.Headers.Add("X-Total-Count", $"{count}");
            }
            
            return base.ExecuteResultAsync(context);
        }
    }
}
