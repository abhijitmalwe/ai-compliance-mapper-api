using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Interfaces
{
    public interface IVectorService
    {
        Task StoreEmbeddingAsync(string id, string text);

        //Task<string> SearchAsync(string query);
    }
}
