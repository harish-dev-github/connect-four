using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ConnectFour.Tests;

public class ApiTests(WebApplicationFactory<Program> application) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task IntiializationReturnsId()
    {
        var client = application.CreateClient();
        var id = await CreateGame(client);
        id.Should().NotBeNull();
        id.Should().NotBe("");
    }
    
    [Fact]
    public async Task TakeTurnWorks()
    {
        var client = application.CreateClient();
        var id = await CreateGame(client);
        var response = await client.PostAsJsonAsync($"/games/{id}/turns", new TakeTurn(
            Player.One,
            new Grid.Position(0, 0)));
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
    
    [Fact]
    public async Task ValidationWorks()
    {
        var client = application.CreateClient();
        var id = await CreateGame(client);
        var response = await client.PostAsJsonAsync($"/games/{id}/turns", new TakeTurn(
            Player.None,
            new Grid.Position(0, 0)));
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    private async Task<string> CreateGame(HttpClient client)
    {
        var response = await client.PostAsync("/games", default);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<JsonNode>();
        return result!["id"]!.GetValue<string>();
    }
}