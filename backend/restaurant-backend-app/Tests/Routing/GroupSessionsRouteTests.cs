using APIHandler.Constants;
using APIHandler.Handlers;
using APIHandler.Records;
using APIHandler.Routing;
using Moq;

namespace APIHandler.Tests.Routing;

public class GroupSessionsRouteTests
{
    [Fact]
    public void Match_GroupSessionsGetRoute_ExtractsGroupReservationId()
    {
        var route = new RouteHandler(
            new Route(HttpMethods.GET, "/guest/groups/{groupReservationId}/sessions"),
            Mock.Of<IApiRequestHandler>());

        var pathParameters = route.Match("/guest/groups/group-1/sessions");

        Assert.NotNull(pathParameters);
        Assert.Equal("group-1", pathParameters["groupReservationId"]);
    }
}