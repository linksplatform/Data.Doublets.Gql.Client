using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Platform.Data.Doublets.Gql.Client;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Xunit;
using TLinkAddress = System.UInt64;

namespace Platform.Data.Doublets.Gql.Tests;

public class ClientTests
{
    private readonly LinksConstants<TLinkAddress> _constants;
    private readonly LinksGqlAdapter _linksGqlAdapter;
    public readonly Uri EndPoint = new Uri("");
    public string TempFilePath = new IO.TemporaryFile();

    public ClientTests()
    {
        _constants = new LinksConstants<TLinkAddress>(true);
        var graphQlClient = new GraphQLHttpClient(EndPoint, new NewtonsoftJsonSerializer());
        _linksGqlAdapter = new LinksGqlAdapter(graphQlClient, _constants);
    }


    private void TestCud()
    {
        TLinkAddress linksAmount = 5;
        // Create
        for (TLinkAddress i = 1; i <= linksAmount; i++)
        {
            TLinkAddress one = 1;
            // Create
            var createdLink = _linksGqlAdapter.CreateAndUpdate(one, i);
            // Count
            Assert.Equal(i, createdLink);
            Assert.Equal(i, _linksGqlAdapter.Count());
            var allLinks = new List<Link<TLinkAddress>>();
            _linksGqlAdapter.Each(link =>
            {
                allLinks.Add(new Link<TLinkAddress>(link));
                return _constants.Continue;
            });
            Assert.Equal(i, _linksGqlAdapter.Count());
        }
    }

    [Fact]
    public void CudTest()
    {
        TestCud();
    }

    [Fact]
    public void EachTest()
    {
        TestCud();
        var count = _linksGqlAdapter.Count();
        TLinkAddress eachIterations = 0;
        _linksGqlAdapter.Each(link =>
        {
            Assert.Equal(++eachIterations, _linksGqlAdapter.GetTarget(link));
            return _linksGqlAdapter.Constants.Continue;
        }, new Link<TLinkAddress>(_constants.Any, _constants.Any, _constants.Any));
        Assert.Equal(count, eachIterations);
    }
}
