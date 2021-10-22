using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Lib;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;
        private readonly IMessageBroker _messageBroker;
        public string[] Countries { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IStorage storage, IMessageBroker messageBroker)
        {
            _logger = logger;
            _storage = storage;
            _messageBroker = messageBroker;
            Countries = Constants.Countries;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string text, string country)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();

            string shardId = GetShardIdByCountry(country);
            _logger.LogDebug($"LOOKUP: {id}, {shardId}");
            _storage.Store(Constants.ShardKey + id, shardId);
            
            string similarity = "0";

            if(text == null)
            {
                text = "";
            }
            else
            {
            _storage.Store(Constants.ShardKey + id, shardId);
                similarity = GetSimilarity(text).ToString();
            }

            PublishSimilarityEvent(id, similarity);

            _storage.Store(shardId, Constants.TextKey + id, text);
            _messageBroker.Send("valuator.processing.rank", id);
            _storage.Store(shardId, Constants.SimilarityKey + id, similarity);
            _storage.StoreToSet(shardId, Constants.TextSetKey, text);

            return Redirect($"summary?id={id}");
        }

        private string GetShardIdByCountry(string country)
        {
            switch (country)
            {
                case "Russia":
                    return Constants.SHARD_RUS;
                case "France":
                case "Germany":
                    return Constants.SHARD_EU;
                case "USA":
                case "India":
                    return Constants.SHARD_OTHER;
            }         
            return "";
        }

        private double GetSimilarity(string text)
        {
            return _storage.IsValueExist(Constants.TextSetKey, text) ? 1 : 0;
        }

        private void PublishSimilarityEvent(string id, string similarity)
        {
            EventContainer eventData = new EventContainer { Name = "valuator.similarity_calculated", Id = id, Value = similarity };
            _messageBroker.Send("events", JsonSerializer.Serialize(eventData));
        }
    }
}