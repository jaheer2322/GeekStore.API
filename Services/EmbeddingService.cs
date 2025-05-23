using System.Globalization;
using GeekStore.API.Service;
using Pgvector;
using Python.Runtime;

namespace GeekStore.API.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        public async Task<Vector> GenerateEmbeddingAsync(string text)
        {
            return PythonEngineSingleton.Instance.RunWithGIL(() =>
            {
                dynamic sys = Py.Import("sys");
                string pythonScriptsFolder = @"C:\ASPDOTNET\GeekStore\GeekStore.API\Python";
                sys.path.append(pythonScriptsFolder);

                dynamic embeddingModule = Py.Import("embedding_service");
                dynamic result = embeddingModule.get_embedding(text);

                float[] floatArray = ((PyObject)result).As<float[]>();
                
                if(floatArray.Count() != 384)
                {
                    throw new Exception("Embedding size is not 384");
                }

                return new Vector(floatArray);
            });
        }
    }
}
