namespace Chirp.Core.Utils;

public readonly struct Either<L, R> {
    private readonly L? left;
    private readonly R? right;

    internal Either(L value) {
        left = value;
    }

    internal Either(R value) {
        right = value;
    }

    public L Left() 
    {
        if(left == null)
            throw new NullReferenceException("Called Left() on Either.Right");
        else return left;
    }

    public R Right() {
        if(right == null)
            throw new NullReferenceException("Called Right() on Either.Left");
        else return right;
    }

    public bool IsLeft()
    {
        return left != null;
    }

    public bool IsRight()
    {
        return right != null;
    }
}

public static class Either {
    public static Either<L, R> Left<L, R>(L value)
    {
        return new Either<L, R>(value);
    }
    public static Either<L, R> Right<L, R>(R value)
    {
        return new Either<L, R>(value);
    }
}