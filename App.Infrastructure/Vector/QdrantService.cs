using App.Application.Interfaces;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Vector
{
    public class QdrantService : IVectorService
    {
        private readonly QdrantClient _client;

        public QdrantService()
        {
            _client = new QdrantClient("127.0.0.1", 6334); // adjust host/port
        }

        public async Task StoreEmbeddingAsync(string id, string text)
        {
            // Dummy embedding (replace with HuggingFace API or sentence-transformers)
            var vector = new float[] { 0.12f, 0.54f, 0.88f };

            await _client.UpsertAsync("findings", new[]
            {
                new PointStruct
                {
                    Id = Guid.Parse(id),
                    Vectors = vector,
                    Payload = { ["text"] = text }
                }
            });
        }

        public async Task<string> SearchAsync(string query)
        {
            var vector = new float[] { 0.12f, 0.54f, 0.88f };
            var result = await _client.SearchAsync("findings", vector, limit: 1);
            return result.Count > 0 ? result[0].Payload["text"].StringValue : "No match";
        }
    }
}


