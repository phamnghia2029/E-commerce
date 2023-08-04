using Base.Functionals;

namespace Base.Utils;

public static class Asyncs
{
    public static T ApplyAsync<T>(Supplier<Task<T>> supplier)
    {
        return Task.Run(async () => await supplier()).Result;
    }
}