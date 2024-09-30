#nullable enable
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.REST.Clients
{
    public interface IRateLimiter
    {
        /// <summary>
        /// in milliseconds
        /// </summary>
        int RequestTimeout { get; set; }
        /// <summary>
        /// 1 delay in milliseconds
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// min delay between requests in milliseconds
        /// </summary>
        int MinDelay { get; set; }
        Task Execute(string httpClientName, int count = 1, CancellationToken ct = default);
        void Adjust(HttpStatusCode statusCode);
    }

    public class RateLimiter : IRateLimiter
    {
        private int _exponentialBackoff = 0;
        private DateTime? _blockTill;
        /// <summary>
        /// timeout in milliseconds
        /// </summary>
        public int RequestTimeout { get; set; } = 20_000;

        /// <summary>
        /// 1 portion of delay in milliseconds
        /// </summary>
        public int Duration { get; set; } = 50;

        /// <summary>
        /// Min delay between requests in milliseconds
        /// (Prevents from being blocked when sending multiple parallel requests API provider might set rate limits,
        /// e.g. Binance returns access denied when detects too many requests within a small timespan)
        /// </summary>
        public int MinDelay { get; set; } = 25;

        /// <summary>
        /// Executes a request delay 
        /// </summary>
        /// <param name="httpClientName">Http Client Name</param>
        /// <param name="count">number of delay portions, delay = count * <see cref="Duration"/> </param>
        /// <param name="ct"></param>
        public async Task Execute(string httpClientName, int count = 1, CancellationToken ct = default)
        {
            Guard.MustBe.Positive(count);

            var blockDelay = GetBlockDelay();
            var delay = blockDelay > Duration * count ? blockDelay : Duration * count;

            if (delay > RequestTimeout)
                throw new RateLimitExceededException($"{httpClientName}: Rate limit exceeded a timeout ({RequestTimeout} ms)", httpClientName);

            if (MinDelay > 0 && delay < MinDelay)
                delay = MinDelay;

            await Task.Delay(delay, ct).ConfigureAwait(false);
        }

        public void Adjust(HttpStatusCode statusCode)
        {
            if (statusCode == HttpStatusCode.TooManyRequests)
            {
                //set block at once
                _blockTill = DateTime.Now.AddSeconds(10);

                //calculate back off delay
                _exponentialBackoff++;
                var delay = CalcBackoffDelay();
                _blockTill = DateTime.Now.AddMilliseconds(delay);
                return;
            }

            if ((int)statusCode == 418)
            {
                _exponentialBackoff = 9;
                _blockTill = DateTime.Now.AddMinutes(2);
                return;
            }

            _exponentialBackoff = 0;
        }

        private int GetBlockDelay()
        {
            if (!_blockTill.HasValue)
                return 0;

            if (_blockTill.Value <= DateTime.Now)
            {
                _blockTill = null;
                _exponentialBackoff = 0;
                return 0;
            }

            var delay = (int)(DateTime.Now - _blockTill.Value).TotalMilliseconds;
            return delay;
        }

        private int CalcBackoffDelay()
        {
            if (_exponentialBackoff == 0)
                return 0;

            var k = 2;
            var delay = 1000;
            for (var i = 0; i < _exponentialBackoff && i <= 10; i++)
            {
                delay *= k;
            }

            return delay;
        }
    }
}