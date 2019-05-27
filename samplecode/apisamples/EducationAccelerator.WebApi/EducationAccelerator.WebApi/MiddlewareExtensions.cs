/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using Microsoft.AspNetCore.Builder;

namespace EducationAccelerator
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseOauthMessageSigning(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Middlewares.OAuth>();
        }
    }
}
