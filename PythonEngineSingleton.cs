using Python.Runtime;

public sealed class PythonEngineSingleton
{
    private static readonly Lazy<PythonEngineSingleton> lazy =
        new(() => new PythonEngineSingleton());

    public static PythonEngineSingleton Instance => lazy.Value; // Readonly property

    private PythonEngineSingleton()
    {
        PythonEngine.Initialize();
        PythonEngine.BeginAllowThreads();
    }

    public T RunWithGIL<T>(Func<T> func)
    {
        using (Py.GIL())
        {
            return func();
        }
    }

    public void Shutdown()
    {
        PythonEngine.Shutdown();
    }
}
