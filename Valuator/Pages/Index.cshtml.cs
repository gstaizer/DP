using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Valuator;

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

            _messageBroker.Send("valuator.processing.rank", id);

            if(text == null)
            {
                text = "";
            }
            else
            {
                similarity = GetSimilarity(text).ToString();
            }

            string similarityKey = "SIMILARITY-" + id;
            _storage.Store(similarityKey, similarity);

            string textKey = "TEXT-" + id;
            _storage.Store(textKey, text);

            return Redirect($"summary?id={id}");
        }

        private double GetSimilarity(string text)
        {
            return _storage.IsValueExist(text) ? 1 : 0;
        }
    }
}