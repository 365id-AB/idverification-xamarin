using System;

namespace IdVerificationSampleApp.Models
{
    public class TransactionResultModel
    {
        public string Status { get; set; }

        public string Assessment { get; set; }

        public string TransactionId { get; set; }

        public string UserMessage { get; set; }
    }
}