using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AVS.CoreLib.UnitTesting.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class FluentAssertionsExtensions
    {
        public static async Task<T> BeOkResult<T>(this ObjectAssertions actualValue)
        {
            if (actualValue.Subject is HttpResponseMessage responseMessage)
            {
                responseMessage.EnsureSuccessStatusCode();
                var content = await responseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }

            actualValue.BeOfType<OkObjectResult>();
            var res = (OkObjectResult)actualValue.Subject;
            res.Value.Should().BeOfType<T>();
            return (T)res.Value;
        }

        public static async Task<T> BeOkResult<T>(this ObjectAssertions actualValue, Action<T> assert) where T : class
        {
            if (actualValue.Subject is HttpResponseMessage responseMessage)
            {
                responseMessage.EnsureSuccessStatusCode();
                var content = await responseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }

            actualValue.BeOfType<OkObjectResult>();
            var res = (OkObjectResult)actualValue.Subject;
            res.Value.Should().BeOfType<T>();
            T obj = (T)res.Value;

            if (assert != null)
            {
                obj.Should().NotBeNull();
                assert(obj);
            }

            return obj;
        }

        public static async Task BeOkResult(this ObjectAssertions actualValue)
        {
            string message = string.Empty;
            if (actualValue.Subject is HttpResponseMessage responseMessage)
            {
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }

                message = await responseMessage.Content.ReadAsStringAsync();
            }

            actualValue.BeOfType<OkResult>(message);
        }

        public static async Task<string> BeInternalServerError(this ObjectAssertions actualValue, string expectedContent = null)
        {
            actualValue.Subject.Should().BeOfType<HttpResponseMessage>();
            var message = (HttpResponseMessage)actualValue.Subject;
            var content = await message.Content.ReadAsStringAsync();
            message.StatusCode.Should().Be(HttpStatusCode.InternalServerError, content);

            if (expectedContent != null)
            {
                content.Should().Contain(expectedContent);
            }

            return content;
        }

        public static async Task BeNoContent(this ObjectAssertions actualValue)
        {
            actualValue.Subject.Should().BeOfType<HttpResponseMessage>();
            var message = (HttpResponseMessage)actualValue.Subject;
            var content = await message.Content.ReadAsStringAsync();
            message.StatusCode.Should().Be(HttpStatusCode.NoContent, content);
        }

        public static void BeUnauthorized(this ObjectAssertions actualValue)
        {
            actualValue.Subject.Should().BeOfType<HttpResponseMessage>();
            var message = (HttpResponseMessage)actualValue.Subject;
            message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        public static async Task<string> BeNotFound(this ObjectAssertions actualValue)
        {
            actualValue.Subject.Should().BeOfType<HttpResponseMessage>();
            var message = (HttpResponseMessage)actualValue.Subject;
            message.StatusCode.Should().Be(HttpStatusCode.NotFound);
            return await message.Content.ReadAsStringAsync();
        }

        public static async Task<string> BeBadRequest(this ObjectAssertions actualValue, string expectedContent = null)
        {
            actualValue.Subject.Should().BeOfType<HttpResponseMessage>();
            var message = (HttpResponseMessage)actualValue.Subject;
            var content = await message.Content.ReadAsStringAsync();
            message.StatusCode.Should().Be(HttpStatusCode.BadRequest, content);

            if (expectedContent != null)
            {
                content.Should().Contain(expectedContent);
            }

            return content;
        }

        public static async Task<string> BeFailedDependency(this ObjectAssertions actualValue, string expectedContent = null)
        {
            actualValue.Subject.Should().BeOfType<HttpResponseMessage>();
            var message = (HttpResponseMessage)actualValue.Subject;
            var content = await message.Content.ReadAsStringAsync();
            message.StatusCode.Should().Be(HttpStatusCode.FailedDependency, content);

            if (expectedContent != null)
            {
                content.Should().Contain(expectedContent);
            }

            return content;
        }

        public static void Match<T>(this GenericCollectionAssertions<T> actualValue, IList<T> matchValue, Action<T, T> compare)
        {
            actualValue.NotBeNull();
            actualValue.NotBeEmpty();
            actualValue.HaveCount(matchValue.Count);
            if (matchValue.Count > 0 && compare != null)
            {
                var i = 0;
                foreach (var x in actualValue.Subject)
                {
                    compare.Invoke(x, matchValue[i++]);
                }
            }
        }
    }
}