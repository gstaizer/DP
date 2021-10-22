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

        public IndexModel(ILogger<IndexModel> logger, IStorage storage, IMessageBroker messageBroker)
        {
            _logger = logger;
            _storage = storage;
            _messageBroker = messageBroker;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();
            
            string similarity = "0";

            if(text == null)
            {
                text = "";
            }
            else
            {
                similarity = GetSimilarity(text).ToString();
            }

            PublishSimilarityEvent(id, similarity);

            _storage.Store(Constants.TextKey + id, text);

            _messageBroker.Send("valuator.processing.rank", id);
            //Console.WriteLine("SIM - " + GetSimilarity(text).ToString());
            _storage.Store(Constants.SimilarityKey + id, similarity);

            return Redirect($"summary?id={id}");
        }

        private double GetSimilarity(string text)
        {
            return _storage.IsValueExist(text) ? 1 : 0;
        }

        private void PublishSimilarityEvent(string id, string similarity)
        {
            EventContainer eventData = new EventContainer { Name = "valuator.similarity_calculated", Id = id, Value = similarity };
            _messageBroker.Send("events", JsonSerializer.Serialize(eventData));
        }
    }
}