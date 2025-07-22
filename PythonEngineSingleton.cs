using System.Diagnostics;
using Python.Runtime;

public sealed class PythonEngineSingleton
{
    public bool isReady = false;
    public PythonEngineSingleton()
    {    
        PythonEngine.Initialize();
        PythonEngine.BeginAllowThreads();
    }

    public async Task WarmUpAsync()
    {
        Console.WriteLine("Initializing python and its dependencies...");
        await Task.Run(() =>
        {
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(Environment.GetEnvironmentVariable("PythonScriptsFolder"));
                dynamic embeddingModule = Py.Import("embedding_service");
                embeddingModule.get_embedding("warmup"); // dummy call to warm up the model
            }
            isReady = true;
            Console.WriteLine("Python initialized and model warmed up successfully.");
        });
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
