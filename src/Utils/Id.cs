namespace Utils;

public static class Id {
    private static int id = 0;

    public static int Generate()
    {
        return id++;
    }
}

