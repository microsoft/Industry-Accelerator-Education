/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

#define TRACE
using EducationAccelerator.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EducationAccelerator.Middlewares
{
    public class OAuth
    {
        public static string OAUTH_CONSUMER_KEY = "contoso";
        public static string OAUTH_CONSUMER_SECRET = "contoso-secret";

        private readonly RequestDelegate _next;

        public OAuth(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ApiContext db)
        {
            if (!context.Request.Path.StartsWithSegments("/ims/oneroster"))
            {
                Trace.TraceInformation($"Non-OneRoster route; bypassing oauth");
                await _next(context);
                return;
            }
            Trace.TraceInformation($"Checking oauth for path {context.Request.Path}");
            int validationResult = Verify(context, db);
            if(validationResult == 0)
            {
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = validationResult;
                await context.Response.WriteAsync("");
            }
        }

        #region Parameter Parsing

        private static Regex headerMatcher = new Regex("\\w*?\\s*?([\\w]*)=\"([\\w%\\.\\-]*)\"");
        private static KeyValuePair<string, string> ParseHeaderFragment(string pair)
        {
            if(pair.Contains("Bearer"))
            {
                var pairArray = pair.Split(" ");
                return new KeyValuePair<string, string>(
                    Uri.EscapeDataString(pairArray[0]),
                    Uri.EscapeDataString(pairArray[1])
                );
            }
            var match = headerMatcher.Match(pair);

            return new KeyValuePair<string, string>(
                Uri.EscapeDataString(match.Groups[1].Value),
                Uri.EscapeDataString(match.Groups[2].Value)
            );
        }

        private static List<KeyValuePair<string, string>> getHeaderParams(HttpRequest request)
        {
            var authHeaders = request.Headers.GetCommaSeparatedValues("Authorization");

            var pairs = new List<KeyValuePair<string, string>>();

            foreach (var authHeader in authHeaders)
            {
                var kvp = ParseHeaderFragment(authHeader);
                if (kvp.Key != "realm")
                {
                    pairs.Add(new KeyValuePair<string, string>(kvp.Key, Uri.UnescapeDataString(kvp.Value)));
                }
            }

            return pairs;
        }

        private static List<KeyValuePair<string, string>> getQueryParams(HttpRequest request)
        {
            var pairs = new List<KeyValuePair<string, string>>();

            var queryValues = request.Query.ToList();
            foreach (var pair in queryValues)
            {
                var values = pair.Value;
                foreach (var value in pair.Value)
                {
                    pairs.Add(new KeyValuePair<string, string>(
                        pair.Key,
                        Uri.EscapeDataString(value ?? "")
                    ));
                }
            }

            return pairs;
        }

        #endregion

        /// <summary>
        /// Returns 0 if oauth signature is valid
        /// Returns 400 if unsupported parameter, unsupported signature method, missing parameter, dupe parameter
        /// Returns 401 if invalid key, timestamp, token, signature, or nonce
        /// </summary>
        /// <param name="context"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static int Verify(HttpContext context, ApiContext db)
        {
            var request = context.Request;
            // Setup the variables necessary to recreate the OAuth 1.0 signature 
            string httpMethod = request.Method.ToUpper();

            var url = SignatureBaseStringUri(request);

            // Collect header and querystring params
            // OneRoster endpoints don't support urlencoded body
            var headerParams = getHeaderParams(request);
            var queryParams = getQueryParams(request);
            var combinedParams = headerParams.Concat(queryParams).ToList();

            // Generate and accept or reject the signature
            try
            {
                // if bearer token is used, then other fields are unnecessary
                var token = combinedParams.FirstOrDefault(kvp => kvp.Key == "Bearer").Value;

                if (VerifyBearerToken(token, db))
                {
                    return 0;
                }

                var nonce = combinedParams.First(kvp => kvp.Key == "oauth_nonce").Value;
                var timestamp = combinedParams.First(kvp => kvp.Key == "oauth_timestamp").Value;
                var clientSignature = combinedParams.First(kvp => kvp.Key == "oauth_signature").Value;
                var signatureMethod = combinedParams.First(kvp => kvp.Key == "oauth_signature_method").Value.ToUpper();
                var clientId = combinedParams.First(kvp => kvp.Key == "oauth_consumer_key").Value;

                if (!IsValidTimestamp(timestamp))
                {
                    Trace.TraceError($"Bad timestamp: {timestamp}");
                    return 401;
                }
                if (!LatchNonce(nonce, db))
                {
                    Trace.TraceError($"Bad nonce: {nonce}");
                    return 401;
                }
                if (clientId != OAUTH_CONSUMER_KEY)
                {
                    Trace.TraceError($"Bad consumer key");
                    return 401;
                }

                var normalizedParams = NormalizeParams(combinedParams);
                var signatureBaseString = $"{httpMethod}&{url}&{normalizedParams}";

                if (signatureMethod != "HMAC-SHA1" &&
                    signatureMethod != "HMAC-SHA2" &&
                    signatureMethod != "HMAC-SHA256")
                {
                    Trace.TraceError($"Bad signing method: {signatureMethod}");
                    return 400;
                }

                var hmac = GenerateHmac(signatureBaseString, signatureMethod, OAUTH_CONSUMER_SECRET);
                if (hmac != clientSignature)
                {
                    Trace.TraceError($"Signature mismatch: {hmac} | {clientSignature}, signatureBaseString is {signatureBaseString}");
                    return 401;
                }

                return 0;
            }
            catch(InvalidOperationException)
            {
                return 400;
            }
        }

        public static string GenerateBearerToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        private static bool VerifyBearerToken(string token, ApiContext db)
        {
            var existingToken = db.OauthTokens.SingleOrDefault(n => n.Value == token);
            return existingToken != null && existingToken.CanBeUsed();
        }

        private static bool IsValidTimestamp(string timestamp)
        {
            var now = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
            var then = long.Parse(timestamp);

            return Math.Abs(now - then) < 45 * 60;
        }

        private static bool LatchNonce(string nonce, ApiContext db)
        {
            var existingNonce = db.OauthNonces.SingleOrDefault(n => n.Value == nonce);

            if (existingNonce != null && DateTime.Now.Subtract(existingNonce.UsedAt).Minutes < 90)
            {
                return false;
            }

            OauthNonce used;

            if (existingNonce != null)
            {
                used = existingNonce;
            }
            else
            {
                used = new OauthNonce();
                db.Add(used);
            }

            used.Value = nonce;
            used.UsedAt = DateTime.Now;

            db.SaveChanges();
            return true;
        }

        // https://tools.ietf.org/html/rfc5849#section-3.4.1.2
        private static string SignatureBaseStringUri(HttpRequest request)
        {
            var protocolString = request.IsHttps ? "https://" : "http://";
            var domainString = request.Host.Host.ToLower();
            var path = request.Path.Value;
            string portString = "";

            if (request.Host.Port != null)
            {
                switch (request.IsHttps)
                {
                    case true:
                        if (request.Host.Port != 443)
                        {
                            portString = $":{request.Host.Port}";
                        }
                        break;
                    case false:
                        if (request.Host.Port != 80)
                        {
                            portString = $":{request.Host.Port}";
                        }
                        break;
                }
            }

            return Uri.EscapeDataString($"{protocolString}{domainString}{portString}{path}");
        }

        // https://tools.ietf.org/html/rfc5849#section-3.4.1.3.2
        public static string NormalizeParams(List<KeyValuePair<string, string>> kvpParams)
        {
            IEnumerable<string> sortedParams =
              from p in kvpParams
              where p.Key != "oauth_signature"
              orderby p.Key ascending, p.Value ascending
              select p.Key + "=" + p.Value;

            return Uri.EscapeDataString(String.Join("&", sortedParams));
        }
        
        private static string GetUri(HttpRequest request)
        {
            var builder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.ToUriComponent(),
                Path = request.Path,
                Query = request.QueryString.ToUriComponent()
            };
            return builder.Uri.AbsolutePath;
        }

        // https://tools.ietf.org/html/rfc5849#section-3.4.2
        public static string GenerateHmac(string msg, string hashMethod, string secret)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes($"{Uri.EscapeDataString(secret)}&");
            byte[] msgBytes = Encoding.UTF8.GetBytes(msg);

            if (hashMethod == "HMAC-SHA1")
            {
                using (var sha1 = new HMACSHA1(keyBytes))
                {
                    var unescaped = Convert.ToBase64String(sha1.ComputeHash(msgBytes));
                    return Uri.EscapeDataString(unescaped);
                }
            }
            else
            {
                using (var sha256 = new HMACSHA256(keyBytes))
                {
                    return Uri.EscapeDataString(Convert.ToBase64String(sha256.ComputeHash(msgBytes)));
                }
            }
        }
    }
}
