using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Lib;

namespace Valuator.Pages
{
    public class SummaryModel : PageModel
    {
        private readonly ILogger<SummaryModel> _logger;
        private IStorage _storage;

        public SummaryModel(ILogger<SummaryModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public double Rank { get; set; }
        public double Similarity { get; set; }
        public bool IsRankEmpty { get; set; }

        public void OnGet(string id)
        {
            _logger.LogDebug(id);

            //TODO: проинициализировать свойства Rank и Similarity сохранёнными в БД значениями
            string shardId = _storage.GetShardId(id);

            string rank = _storage.Load(shardId, Constants.RankKey + id);
            IsRankEmpty = rank == null;
            Rank = Convert.ToDouble(rank);
            Similarity = Convert.ToDouble(_storage.Load(shardId, Constants.SimilarityKey + id));
        }
    }
}
