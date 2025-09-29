using App.Application.Dto;
using App.Application.Interfaces;
using Microsoft.Extensions.Options;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace App.Infrastructure.Vector
{
    public class QdrantService : IVectorService
    {       
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly string _collection;


        public QdrantService(HttpClient httpClient, IOptions<QdrantConfig> config)
        {
            _http = httpClient;
            _baseUrl = config.Value.BaseUrl;
            _collection = config.Value.Collection;
        }

        // Simple upsert using Qdrant REST. Expects Qdrant running with collection created (or auto-create)
        public Task StoreAsync(string id, string text)
        {
            //var endpoint = $"{_baseUrl}/collections/{_collection}/points?wait=true";
            //// A minimal embedding placeholder; replace with real embeddings
            //var vector = new float[] { 0.1f, 0.2f, 0.3f };

            //var payload = new
            //{
            //    points = new object[] {
            //        new {
            //            id = id,
            //            vector = vector,
            //            payload = new { text = text }
            //        }
            //    }
            //};

            //var resp = await _http.PostAsJsonAsync(endpoint, payload);
            //resp.EnsureSuccessStatusCode();
            Console.WriteLine($"Simulated storing vector for: {id}");
            return Task.CompletedTask;
        }
    }
}


