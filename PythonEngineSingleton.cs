using System.Diagnostics;
using Python.Runtime;

public sealed class PythonEngineSingleton
{
    private bool _isReady = false;
    public bool IsReady => _isReady;
    public PythonEngineSingleton()
    {    
        Console.WriteLine("Initializing Python engine...");
        PythonEngine.Initialize();
        PythonEngine.BeginAllowThreads();
        Console.WriteLine("Python engine initialized successfully.");
    }

    public async Task WarmUpAsync()
    {
        Console.WriteLine("Initializing sentence transformer model...");
        await Task.Run(() =>
        {
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(Environment.GetEnvironmentVariable("PythonScriptsFolder"));
                dynamic embeddingModule = Py.Import("embedding_service");
                embeddingModule.get_embedding("warmup"); // dummy call to warm up the model
            }
        });
        Console.WriteLine("Sentence transformer model warmed up successfully.");
        _isReady = true;
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
