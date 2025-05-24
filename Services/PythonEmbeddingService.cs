using GeekStore.API.Service;
using Pgvector;
using Python.Runtime;

namespace GeekStore.API.Services
{
    public class PythonEmbeddingService : IEmbeddingService
    {
        public async Task<Vector> GenerateEmbeddingAsync(string text)
        {
            float[] embedding = GenerateEmbeddingFromPython(text);

            if (embedding.Count() != 384)
            {
                throw new Exception("Embedding size is not 384");
            }

            return new Vector(embedding);
        }

        private float[] GenerateEmbeddingFromPython(string text)
        {
            return PythonEngineSingleton.Instance.RunWithGIL(() =>
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(Environment.GetEnvironmentVariable("PythonScriptsFolder"));

                dynamic embeddingModule = Py.Import("embedding_service");
                dynamic result = embeddingModule.get_embedding(text);

                float[] floatArray = ((PyObject)result).As<float[]>();

                return floatArray;
            });
        }
    }
}
