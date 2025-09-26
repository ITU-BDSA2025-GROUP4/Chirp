namespace Utils;

public struct Optional<T> {
    private T? value;
    public bool HasValue {get; private set;}

    public Optional(T value) {
        this.value = value;
        HasValue = true;
    }

    public Optional() {
        HasValue = false;
    }

    public T ValueOrDefault(T def) 
    {
        if(HasValue) return value;
        else return def;
    }

    public T Value() {
        if(!HasValue) throw new NullReferenceException("Called value when optional's internal value is null");
        else return value;
    }
}
