﻿using Microsoft.AspNetCore.Mvc.Testing;
using Swashbuckle.AspNetCore.ApiTesting.Xunit;
using Xunit;

namespace TestFirst.IntegrationTests;

public class GetProductsTests(ApiTestRunner apiTestRunner, WebApplicationFactory<Startup> webApplicationFactory)
    : ApiTestFixture<Startup>(apiTestRunner, webApplicationFactory, "v1-imported")
{
    [Fact]
    public async Task GetProducts_Returns200_IfRequiredParametersProvided()
    {
        await TestAsync(
            "GetProducts",
            "200",
            new HttpRequestMessage
            {
                RequestUri = new Uri("/api/products?pageNo=1", UriKind.Relative),
                Method = HttpMethod.Get
            }
        );
    }

    [Fact]
    public async Task GetProducts_Returns400_IfRequiredParametersMissing()
    {
        await TestAsync(
            "GetProducts",
            "400",
            new HttpRequestMessage
            {
                RequestUri = new Uri("/api/products", UriKind.Relative),
                Method = HttpMethod.Get
            }
        );
    }
}
