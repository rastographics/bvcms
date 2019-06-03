using System;

namespace BvcmsBatch.PushPay
{
    /// <summary>
    /// Push pay documentation explains their throttling mechanics. Can be expanded later
    /// as we monitor usage metrics.
    /// 
    /// Class is a singleton intended to be called through .Instance
    /// </summary>
    public sealed class PushPayRateLimiter
    {
        private int _delay;
        private int _retryCount;

        public static PushPayRateLimiter Instance { get; } = new PushPayRateLimiter();

        public bool ShouldStop => _retryCount >= 7;
        public int CurrentDelay => _delay;

        static PushPayRateLimiter()
        {
        }

        private PushPayRateLimiter()
        {
        }

        /// <summary>
        /// Adds the interval specified in the retry-after header from pushpay to the 
        /// current delay.
        /// </summary>
        /// <param name="seconds">Value of the Response.Headers[Retry-After] as an int</param>
        public void AddDelay(int seconds)
        {
            _delay = seconds + GetBackoffDelay(_retryCount);
            _retryCount++;
        }

        /// <summary>
        /// Calculates an exponential delay based on the number of consecutive retries
        /// 0 = 0 ... 2^(n-1)
        /// </summary>
        /// <param name="retries"></param>
        /// <returns></returns>
        private int GetBackoffDelay(int retryCount)
        {
            if (retryCount == 0)
            {
                return 0;
            }

            var wait = Math.Pow(2.0, Convert.ToDouble(retryCount - 1));

            return Convert.ToInt32(wait);
        }

        /// <summary>
        /// If pushpay didn't respond with a wait, call this to reset the state
        /// </summary>
        public void ResetDelay()
        {
            _delay = 0;
            _retryCount = 0;
        }
    }
}
