namespace Utils;

// Haskell shitposting, ignore this

public interface Functor<A>
{
    // class Functor f where
    // fmap :: (a -> b) -> f a -> f b
    public Functor<B> Map<B>(Func<A, B> f, Functor<A> fa);

    // Equiv to <$ 
    // (<$) :: a -> f b -> f a
    public Functor<A> Transform<B>(A a, Functor<B> fb);

}

public interface Applicative<A> : Functor<A>
{
    // class Functor f => Applicative f where
    // pure :: a -> f a
    public Applicative<A> Pure(A a);

    // (<*>) :: f (a -> b) -> f a -> f b
    public Applicative<B> Application<B>(Applicative<Func<A, B>> f, Applicative<A> g);

    // (*>) :: f a -> f b -> f b
    public Applicative<B> SwapAndMapRight<B>( Applicative<A> fa, Applicative<B> fb);

    // (<*) :: f a -> f b -> f a
    public Applicative<B> SwapAndMapLeft<B>( Applicative<A> fa, Applicative<B> fb);

    // GHC.Base.liftA2 :: (a -> b -> c) -> f a -> f b -> f c
    public Applicative<B> Lift<B, C>(Func<A, Func<B, C>> a, Applicative<A> fa, Applicative<B> fb);

}

