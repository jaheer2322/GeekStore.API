using System.Diagnostics;
using Python.Runtime;

public sealed class PythonEngineSingleton
{
    private static readonly PythonEngineSingleton instance = new PythonEngineSingleton();
    public static PythonEngineSingleton Instance => instance;

    private PythonEngineSingleton()
    {
        Console.WriteLine("Initializing Python and its dependencies... This can take upto a minute");

        var stopwatch = Stopwatch.StartNew();

        PythonEngine.Initialize();
        PythonEngine.BeginAllowThreads();

        RunWithGIL(() =>
        {
            dynamic sys = Py.Import("sys");
            sys.path.append(Environment.GetEnvironmentVariable("PythonScriptsFolder"));
            dynamic embeddingModule = Py.Import("embedding_service");
            embeddingModule.get_embedding("warmup"); // dummy call
        });

        stopwatch.Stop();
        Console.WriteLine($"Python initialized and model warmed up in {stopwatch.ElapsedMilliseconds} ms");
    }

    public T RunWithGIL<T>(Func<T> func)
    {
        using (Py.GIL())
        {
            return func();
        }
    }

    public void RunWithGIL(Action action)
    {
        using (Py.GIL())
        {
            action();
        }
    }

    public void Shutdown()
    {
        PythonEngine.Shutdown();
    }
}
