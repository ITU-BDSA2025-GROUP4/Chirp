namespace Chirp.Core.Utils;

public readonly struct Optional<T> {
    private readonly T value;
    public readonly bool HasValue;

    public Optional(T value) {
        this.value = value;
        HasValue = true;
    }

    public T ValueOrDefault(T def) 
    {
        if(HasValue) return value;
        else return def;
    }

    public T Value() {
        if(!HasValue) throw new NullReferenceException("Called Value() on empty Optional");
        else return value;
    }
}

public static class Optional {
    public static Optional<T> Of<T>(T value)
    {
        return new Optional<T>(value);
    }
    public static Optional<T> Empty<T>()
    { 
        return new Optional<T>();
    }
}