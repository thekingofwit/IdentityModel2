﻿//// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
//// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

//using FluentAssertions;
//using IdentityModel.Client;
//using Microsoft.AspNetCore.WebUtilities;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;
//using Xunit;

//namespace IdentityModel.UnitTests
//{
//    public class IntrospectionClientTests
//    {
//        const string Endpoint = "http://server/token";

//        [Fact]
//        public async Task Malformed_response_document_should_be_handled_correctly()
//        {
//            var document = "invalid";
//            var handler = new NetworkHandler(document, HttpStatusCode.OK);

//            var client = new IntrospectionClient(
//                Endpoint,
//                "client",
//                innerHttpMessageHandler: handler);

//            var response = await client.SendAsync(new IntrospectionRequest { Token = "token" });

//            response.IsError.Should().BeTrue();
//            response.ErrorType.Should().Be(ResponseErrorType.Exception);
//            response.Raw.Should().Be("invalid");
//            response.Exception.Should().NotBeNull();
//        }

//        [Fact]
//        public async Task Exception_should_be_handled_correctly()
//        {
//            var handler = new NetworkHandler(new Exception("exception"));

//            var client = new IntrospectionClient(
//                Endpoint,
//                "client",
//                innerHttpMessageHandler: handler);

//            var response = await client.SendAsync(new IntrospectionRequest { Token = "token" });

//            response.IsError.Should().BeTrue();
//            response.ErrorType.Should().Be(ResponseErrorType.Exception);
//            response.Error.Should().Be("exception");
//            response.Exception.Should().NotBeNull();
//        }

//        [Fact]
//        public async Task Http_error_should_be_handled_correctly()
//        {
//            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");

//            var client = new IntrospectionClient(
//                Endpoint,
//                "client",
//                innerHttpMessageHandler: handler);

//            var response = await client.SendAsync(new IntrospectionRequest { Token = "token" });

//            response.IsError.Should().BeTrue();
//            response.ErrorType.Should().Be(ResponseErrorType.Http);
//            response.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
//            response.Error.Should().Be("not found");
//        }

//        [Fact]
//        public async Task Legacy_protocol_response_should_be_handled_correctly()
//        {
//            var document = File.ReadAllText(FileName.Create("legacy_success_introspection_response.json"));
//            var handler = new NetworkHandler(document, HttpStatusCode.OK);

//            var client = new IntrospectionClient(
//                Endpoint,
//                "client",
//                innerHttpMessageHandler: handler);

//            var response = await client.SendAsync(new IntrospectionRequest { Token = "token" });

//            response.IsError.Should().BeFalse();
//            response.ErrorType.Should().Be(ResponseErrorType.None);
//            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
//            response.IsActive.Should().BeTrue();
//            response.Claims.Should().NotBeEmpty();

//            var audiences = response.Claims.Where(c => c.Type == "aud");
//            audiences.Count().Should().Be(2);
//            audiences.First().Value.Should().Be("https://idsvr4/resources");
//            audiences.Skip(1).First().Value.Should().Be("api1");

//            response.Claims.First(c => c.Type == "iss").Value.Should().Be("https://idsvr4");
//            response.Claims.First(c => c.Type == "nbf").Value.Should().Be("1475824871");
//            response.Claims.First(c => c.Type == "exp").Value.Should().Be("1475828471");
//            response.Claims.First(c => c.Type == "client_id").Value.Should().Be("client");
//            response.Claims.First(c => c.Type == "sub").Value.Should().Be("1");
//            response.Claims.First(c => c.Type == "auth_time").Value.Should().Be("1475824871");
//            response.Claims.First(c => c.Type == "idp").Value.Should().Be("local");
//            response.Claims.First(c => c.Type == "amr").Value.Should().Be("password");
//            response.Claims.First(c => c.Type == "active").Value.Should().Be("True");

//            var scopes = response.Claims.Where(c => c.Type == "scope");
//            scopes.Count().Should().Be(2);
//            scopes.First().Value.Should().Be("api1");
//            scopes.Skip(1).First().Value.Should().Be("api2");

//        }

//        [Fact]
//        public async Task Success_protocol_response_should_be_handled_correctly()
//        {
//            var document = File.ReadAllText(FileName.Create("success_introspection_response.json"));
//            var handler = new NetworkHandler(document, HttpStatusCode.OK);

//            var client = new IntrospectionClient(
//                Endpoint,
//                "client",
//                innerHttpMessageHandler: handler);

//            var response = await client.SendAsync(new IntrospectionRequest { Token = "token" });

//            response.IsError.Should().BeFalse();
//            response.ErrorType.Should().Be(ResponseErrorType.None);
//            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
//            response.IsActive.Should().BeTrue();
//            response.Claims.Should().NotBeEmpty();

//            var audiences = response.Claims.Where(c => c.Type == "aud");
//            audiences.Count().Should().Be(2);
//            audiences.First().Value.Should().Be("https://idsvr4/resources");
//            audiences.Skip(1).First().Value.Should().Be("api1");

//            response.Claims.First(c => c.Type == "iss").Value.Should().Be("https://idsvr4");
//            response.Claims.First(c => c.Type == "nbf").Value.Should().Be("1475824871");
//            response.Claims.First(c => c.Type == "exp").Value.Should().Be("1475828471");
//            response.Claims.First(c => c.Type == "client_id").Value.Should().Be("client");
//            response.Claims.First(c => c.Type == "sub").Value.Should().Be("1");
//            response.Claims.First(c => c.Type == "auth_time").Value.Should().Be("1475824871");
//            response.Claims.First(c => c.Type == "idp").Value.Should().Be("local");
//            response.Claims.First(c => c.Type == "amr").Value.Should().Be("password");
//            response.Claims.First(c => c.Type == "active").Value.Should().Be("True");

//            var scopes = response.Claims.Where(c => c.Type == "scope");
//            scopes.Count().Should().Be(2);
//            scopes.First().Value.Should().Be("api1");
//            scopes.Skip(1).First().Value.Should().Be("api2");
//        }

//        [Fact]
//        public async Task Additional_request_parameters_should_be_handled_correctly()
//        {
//            var document = File.ReadAllText(FileName.Create("success_introspection_response.json"));
//            var handler = new NetworkHandler(document, HttpStatusCode.OK);

//            var client = new IntrospectionClient(
//                Endpoint,
//                "client",
//                innerHttpMessageHandler: handler);

//            var additionalParams = new Dictionary<string, string>
//            {
//                { "scope", "scope1 scope2" },
//                { "foo", "bar" }
//            };

//            var response = await client.SendAsync(new IntrospectionRequest { Token = "token", Parameters = additionalParams });

//            // check request
//            var fields = QueryHelpers.ParseQuery(handler.Body);
//            fields.Count.Should().Be(4);

//            fields["client_id"].First().Should().Be("client");
//            fields["token"].First().Should().Be("token");
//            fields["scope"].First().Should().Be("scope1 scope2");
//            fields["foo"].First().Should().Be("bar");

//            // check response
//            response.IsError.Should().BeFalse();
//            response.ErrorType.Should().Be(ResponseErrorType.None);
//            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
//            response.IsActive.Should().BeTrue();
//            response.Claims.Should().NotBeEmpty();
//        }
//    }
//}