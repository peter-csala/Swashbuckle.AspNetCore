﻿using System.Xml.XPath;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
#if !NET10_0_OR_GREATER
using Swashbuckle.AspNetCore.TestSupport;
#endif

namespace Swashbuckle.AspNetCore.SwaggerGen.Test;

public class XmlCommentsParameterFilterTests
{
    [Fact]
    public void Apply_SetsDescriptionAndExample_FromActionParamTag()
    {
        var parameter = new OpenApiParameter { Schema = new OpenApiSchema { Type = JsonSchemaTypes.String } };
        var parameterInfo = typeof(FakeControllerWithXmlComments)
            .GetMethod(nameof(FakeControllerWithXmlComments.ActionWithParamTags))
            .GetParameters()[0];
        var apiParameterDescription = new ApiParameterDescription { };
        var filterContext = new ParameterFilterContext(apiParameterDescription, null, null, parameterInfo: parameterInfo);

        Subject().Apply(parameter, filterContext);

        Assert.Equal("Description for param1", parameter.Description);
        Assert.NotNull(parameter.Example);
        Assert.Equal("\"Example for \\\"param1\\\"\"", parameter.Example.ToJson());
    }

    [Fact]
    public void Apply_SetsDescriptionAndExample_FromUriTypeActionParamTag()
    {
        var parameter = new OpenApiParameter { Schema = new OpenApiSchema { Type = JsonSchemaTypes.String } };
        var parameterInfo = typeof(FakeControllerWithXmlComments)
            .GetMethod(nameof(FakeControllerWithXmlComments.ActionWithParamTags))
            .GetParameters()[1];
        var apiParameterDescription = new ApiParameterDescription { };
        var filterContext = new ParameterFilterContext(apiParameterDescription, null, null, parameterInfo: parameterInfo);

        Subject().Apply(parameter, filterContext);

        Assert.Equal("Description for param2", parameter.Description);
        Assert.NotNull(parameter.Example);
        Assert.Equal("\"http://test.com/?param1=1&param2=2\"", parameter.Example.ToJson());
    }

    [Fact]
    public void Apply_SetsDescriptionAndExample_FromUnderlyingGenericTypeActionParamTag()
    {
        var parameter = new OpenApiParameter { Schema = new OpenApiSchema { Type = JsonSchemaTypes.String } };
        var parameterInfo = typeof(FakeConstructedControllerWithXmlComments)
            .GetMethod(nameof(FakeConstructedControllerWithXmlComments.ActionWithParamTags))
            .GetParameters()[0];
        var apiParameterDescription = new ApiParameterDescription { };
        var filterContext = new ParameterFilterContext(apiParameterDescription, null, null, parameterInfo: parameterInfo);

        Subject().Apply(parameter, filterContext);

        Assert.Equal("Description for param1", parameter.Description);
        Assert.NotNull(parameter.Example);
        Assert.Equal("\"Example for \\\"param1\\\"\"", parameter.Example.ToJson());
    }

    [Fact]
    public void Apply_SetsDescriptionAndExample_FromPropertySummaryAndExampleTags()
    {
        var parameter = new OpenApiParameter { Schema = new OpenApiSchema { Type = JsonSchemaTypes.String, Description = "schema-level description" } };
        var propertyInfo = typeof(XmlAnnotatedType).GetProperty(nameof(XmlAnnotatedType.StringProperty));
        var apiParameterDescription = new ApiParameterDescription { };
        var filterContext = new ParameterFilterContext(apiParameterDescription, null, null, propertyInfo: propertyInfo);

        Subject().Apply(parameter, filterContext);

        Assert.Equal("Summary for StringProperty", parameter.Description);
        Assert.Null(parameter.Schema.Description);
        Assert.NotNull(parameter.Example);
        Assert.Equal("\"Example for StringProperty\"", parameter.Example.ToJson());
    }

    [Fact]
    public void Apply_SetsDescriptionAndExample_FromUriTypePropertySummaryAndExampleTags()
    {
        var parameter = new OpenApiParameter { Schema = new OpenApiSchema { Type = JsonSchemaTypes.String, Description = "schema-level description" } };
        var propertyInfo = typeof(XmlAnnotatedType).GetProperty(nameof(XmlAnnotatedType.StringPropertyWithUri));
        var apiParameterDescription = new ApiParameterDescription { };
        var filterContext = new ParameterFilterContext(apiParameterDescription, null, null, propertyInfo: propertyInfo);

        Subject().Apply(parameter, filterContext);

        Assert.Equal("Summary for StringPropertyWithUri", parameter.Description);
        Assert.Null(parameter.Schema.Description);
        Assert.NotNull(parameter.Example);
        Assert.Equal("\"https://test.com/a?b=1&c=2\"", parameter.Example.ToJson());
    }

    private static XmlCommentsParameterFilter Subject()
    {
        using var xml = File.OpenText(typeof(FakeControllerWithXmlComments).Assembly.GetName().Name + ".xml");
        var document = new XPathDocument(xml);
        var members = XmlCommentsDocumentHelper.CreateMemberDictionary(document);
        return new(members, new());
    }
}
