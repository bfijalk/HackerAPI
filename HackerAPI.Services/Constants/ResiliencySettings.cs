using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerAPI.Services.Constants
{
    public class ResiliencySettings
    {
        public int MaxRetryAttempts { get; set; }
        public int RetryBaseDelaySeconds { get; set; }
        public int RateLimitRequests { get; set; }
        public int RateLimitTimeWindowSeconds { get; set; }
        public int RateLimitBurst { get; set; }
    }
}
