public struct Option<T>
{
    private T _value;
    private bool _isSome;

    public static Option<T> Some(T value) => new()
    {
        _value = value,
        _isSome = true
    };

    public static Option<T> None() => new()
    {
        _isSome = false
    };

    public readonly T Value
    {
        get => _value;
    }

    public readonly bool IsSome
    {
        get => _isSome;
    }
}