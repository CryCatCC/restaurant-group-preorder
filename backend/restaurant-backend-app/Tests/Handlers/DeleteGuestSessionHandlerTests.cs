using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using APIHandler.Exceptions;
using APIHandler.Handlers.GuestSessions;
using APIHandler.Models.GuestSessions;
using APIHandler.Services.GuestSessions;
using Moq;

namespace APIHandler.Tests.Handlers;

public class DeleteGuestSessionHandlerTests
{
    private const string GroupReservationId = "group-1";
    private const string GuestSessionId = "session-1";
    private const string OwnerId = "owner-1";

    [Fact]
    public async Task HandleRequest_MissingSession_Returns404()
    {
        var service = new Mock<IGuestSessionService>();
        service
            .Setup(s => s.DeleteGuestSessionAsync(OwnerId, GroupReservationId, GuestSessionId))
            .ThrowsAsync(new GuestSessionNotFoundException("Guest session was not found."));
        var handler = new DeleteGuestSessionHandler(service.Object);

        var response = await handler.HandleRequest(CreateRequest(), CreateContext());

        Assert.Equal(404, response.StatusCode);
    }

    [Fact]
    public async Task HandleRequest_SessionFromAnotherGroup_Returns400()
    {
        var service = new Mock<IGuestSessionService>();
        service
            .Setup(s => s.DeleteGuestSessionAsync(OwnerId, GroupReservationId, GuestSessionId))
            .ThrowsAsync(new ValidationException("Guest session does not belong to this group reservation."));
        var handler = new DeleteGuestSessionHandler(service.Object);

        var response = await handler.HandleRequest(CreateRequest(), CreateContext());

        Assert.Equal(400, response.StatusCode);
    }

    private static APIGatewayProxyRequest CreateRequest() => new()
    {
        PathParameters = new Dictionary<string, string>
        {
            ["groupReservationId"] = GroupReservationId,
            ["guestSessionId"] = GuestSessionId
        },
        RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
        {
            Authorizer = new APIGatewayCustomAuthorizerContext
            {
                Claims = new Dictionary<string, string> { ["sub"] = OwnerId }
            }
        }
    };

    private static ILambdaContext CreateContext()
    {
        var logger = new Mock<ILambdaLogger>();
        var context = new Mock<ILambdaContext>();
        context.Setup(c => c.Logger).Returns(logger.Object);
        return context.Object;
    }
}