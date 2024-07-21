using ConnectFour;
using ConnectFour.Api;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<GamesStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/games", ([FromServices] GamesStore gameStore) =>
    {
        var initialization = Game.ExecuteCommand(GameState.Zero, new InitializeGame(6, 7));
        return initialization.Match(
            success =>
            {
                var gameInitialized = success.Value.OfType<GameInitialized>().Single();
                gameStore.Put(gameInitialized.Id, success.Value);
                return Results.Created("/games", new { id = gameInitialized.Id });
            },
            errors => Results.BadRequest(errors));
    })
    .WithName("InitializeGame")
    .WithOpenApi();

app.MapPost("/games/{gameId}/turns", (
        [FromRoute] string gameId,
        [FromBody] TakeTurn takeTurn,
        [FromServices] GamesStore gameStore) =>
    {
        var game = gameStore.RehydrateState(gameId);
        if (game is GameState gameState)
        {
            var takeTurnResult = Game.ExecuteCommand(gameState, takeTurn);
            return takeTurnResult.Match(
                success =>
                {
                    gameStore.Put(gameId, success.Value);
                    return success.Value.OfType<TurnTook>().SingleOrDefault() is not null ?
                        Results.Accepted() : Results.UnprocessableEntity("Try_Again");
                },
                errors => Results.UnprocessableEntity(errors));
        }

        return Results.NotFound();
    })
    .WithName("TakeTurn")
    .WithOpenApi();

app.Run();

public partial class Program { }