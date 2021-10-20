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

        public IndexModel(ILogger<IndexModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();
            
            string rank = "0";
            string similarity = "0";

            if(text == null)
            {
                text = "";
            }
            else
            {
                rank = GetRank(text);
                similarity = GetSimilarity(text).ToString();
            }

            string rankKey = "RANK-" + id;
            _storage.Store(rankKey, rank);

            string similarityKey = "SIMILARITY-" + id;
            _storage.Store(similarityKey, similarity);

            string textKey = "TEXT-" + id;
            _storage.Store(textKey, text);

            return Redirect($"summary?id={id}");
        }

        private string GetRank(string text)
        {
            return ((double)text.Count(ch => !char.IsLetter(ch)) / (double)text.Length).ToString("0.##");
        }

        private double GetSimilarity(string text)
        {
            return _storage.IsValueExist(text) ? 1 : 0;
        }
    }
}