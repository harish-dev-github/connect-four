using System.Collections.Immutable;

namespace ConnectFour;

public interface IValidated<T>
{
    TResult Match<TResult>(Func<Valid<T>, TResult> onValid, Func<ImmutableArray<string>, TResult> onInvalid);
}

public readonly record struct Valid<T>(T Value) : IValidated<T>
{
    public TResult Match<TResult>(Func<Valid<T>, TResult> onValid, Func<ImmutableArray<string>, TResult> onInvalid) =>
        onValid(this);
}

public readonly record struct Invalid<T>(ImmutableArray<string> Errors) : IValidated<T>
{
    public TResult Match<TResult>(Func<Valid<T>, TResult> onValid, Func<ImmutableArray<string>, TResult> onInvalid) =>
        onInvalid(Errors);
}