﻿using HitoBit.Net.Converters;
using HitoBit.Net.Enums;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HitoBit.Net.Objects.Models.Futures.AlgoOrders
{
    /// <summary>
    /// Algo orders
    /// </summary>
    public class HitoBitAlgoOrders
    {
        /// <summary>
        /// Total items
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// Orders
        /// </summary>
        public IEnumerable<HitoBitAlgoOrder> Orders { get; set; } = Array.Empty<HitoBitAlgoOrder>();
    }

    /// <summary>
    /// Algo order info
    /// </summary>
    public class HitoBitAlgoOrder
    {
        /// <summary>
        /// Algo id
        /// </summary>
        public long AlgoId { get; set; }
        /// <summary>
        /// Symbol
        /// </summary>
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Order side
        /// </summary>
        [JsonConverter(typeof(OrderSideConverter))]
        public OrderSide Side { get; set; }
        /// <summary>
        /// Position side
        /// </summary>
        [JsonConverter(typeof(PositionSideConverter))]
        public PositionSide? PositionSide { get; set; }
        /// <summary>
        /// Total quantity
        /// </summary>
        [JsonProperty("totalQty")]
        public decimal TotalQuantity { get; set; }
        /// <summary>
        /// Executed quantity
        /// </summary>
        [JsonProperty("executedQty")]
        public decimal ExecutedQuantity { get; set; }
        /// <summary>
        /// exceuted amount
        /// </summary>
        [JsonProperty("executedAmt")]
        public decimal ExecutedAmount { get; set; }
        /// <summary>
        /// Average price
        /// </summary>
        [JsonProperty("avgPrice")]
        public decimal AveragePrice { get; set; }
        /// <summary>
        /// Client algo id
        /// </summary>
        public string ClientAlgoId { get; set; } = string.Empty;
        /// <summary>
        /// Book time
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime BookTime { get; set; }
        /// <summary>
        /// End time
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string AlgoStatus { get; set; } = string.Empty;
        /// <summary>
        /// Algo type
        /// </summary>
        public string AlgoType { get; set; } = string.Empty;
        /// <summary>
        /// Urgency
        /// </summary>
        [JsonConverter(typeof(EnumConverter))]
        public OrderUrgency? Urgency { get; set; }
    }
}