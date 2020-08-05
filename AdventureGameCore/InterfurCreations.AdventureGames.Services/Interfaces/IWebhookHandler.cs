using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services
{
    public interface IWebhookHandler
    {
        Type ObjectType { get; }
        void Handle<T>(T obj, JObject jsonData);
        bool IsValid(object obj);
    }
}
