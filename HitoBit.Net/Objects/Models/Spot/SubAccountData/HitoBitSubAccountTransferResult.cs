﻿using Newtonsoft.Json;

namespace HitoBit.Net.Objects.Models.Spot.SubAccountData
{
    /// <summary>
    /// Sub account transfer result
    /// </summary>
    public class HitoBitSubAccountTransferResult
    {
        /// <summary>
        /// Whether the transfer was successful
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// The transaction id of the transfer
        /// </summary>
        [JsonProperty("txnId")]
        public string? TransactionId { get; set; }
    }
}