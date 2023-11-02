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
        /// Executes a delay 
        /// </summary>
        /// <param name="count">number of delay portionsm, delay = count * <see cref="Duration"/> </param>
        /// <param name="ct"></param>
        /// <returns>true if delay does not exceed a <see cref="RequestTimeout"/>, otherwise false</returns>
        Task<bool> DelayAsync(int count = 1, CancellationToken ct = default);
        void Adjust(HttpStatusCode statusCode);        
    }

    public class RateLimiter : IRateLimiter
    {
        private int _exponentialBackoff = 0;
        private DateTime? _blockTill;        
        public int RequestTimeout { get; set; } = 20_000;
        public int Duration { get; set; } = 50;

        public async Task<bool> DelayAsync(int count = 1, CancellationToken ct = default)
        {
            Guard.MustBe.Positive(count);

            var blockDelay = GetBlockDelay();
            var delay = blockDelay > Duration * count ? blockDelay : Duration * count;

            if (delay > RequestTimeout)
                return false;

            await Task.Delay(delay, ct).ConfigureAwait(false);
            return true;
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
            for (var i = 0; i < _exponentialBackoff && i <=10; i++)
            {
                delay *= k;
            }

            return delay;
        }
    }
}