using System.Collections.Immutable;

namespace ConnectFour.Api;

public class GamesStore
{
    private ImmutableDictionary<string, ImmutableArray<IGameEvent>> _games =
        ImmutableDictionary.Create<string, ImmutableArray<IGameEvent>>();

    internal GameState? RehydrateState(string id) =>
        _games.TryGetValue(id, out var events) ? events.Aggregate(GameState.Zero, Game.ApplyEvent) : default;

    internal void Put(string id, ImmutableArray<IGameEvent> events)
    {
        ImmutableInterlocked.AddOrUpdate(
            ref _games,
            id,
            _ => events,
            (_, oldValue) => oldValue.AddRange(events));
    }
}