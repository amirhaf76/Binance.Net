﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HitoBit.Net.Converters;
using HitoBit.Net.Enums;
using HitoBit.Net.Interfaces.Clients.GeneralApi;
using HitoBit.Net.Objects.Models;
using HitoBit.Net.Objects.Models.Spot.Loans;
using CryptoExchange.Net;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;

namespace HitoBit.Net.Clients.GeneralApi
{
    /// <inheritdoc />
    public class HitoBitRestClientGeneralApiLoans : IHitoBitRestClientGeneralApiLoans
    {
        // Crypto loans
        private const string incomingEndpoint = "loan/income";
        private const string borrowEndpoint = "loan/borrow";
        private const string borrowHistoryEndpoint = "loan/borrow/history";
        private const string openBorrowOrdersEndpoint = "loan/ongoing/orders";
        private const string repayEndpoint = "loan/repay";
        private const string repayHistoryEndpoint = "loan/repay/history";
        private const string adjustLtvEndpoint = "loan/adjust/ltv";
        private const string adjustLtvHistoryEndpoint = "loan/ltv/adjustment/history";

        private readonly HitoBitRestClientGeneralApi _baseClient;

        internal HitoBitRestClientGeneralApiLoans(HitoBitRestClientGeneralApi baseClient)
        {
            _baseClient = baseClient;
        }

        #region Get Income History
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<HitoBitCryptoLoanIncome>>> GetIncomeHistoryAsync(string asset, LoanIncomeType? type = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "asset", asset }
            };
            parameters.AddOptionalParameter("type", type.HasValue ? JsonConvert.SerializeObject(type.Value, new LoanIncomeTypeConverter(false)) : null);
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("startTime", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("endTime", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<HitoBitCryptoLoanIncome>>(_baseClient.GetUrl(incomingEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true, weight: 6000).ConfigureAwait(false);
        }
        #endregion

        #region Borrow
        /// <inheritdoc />
        public async Task<WebCallResult<HitoBitCryptoLoanBorrow>> BorrowAsync(string loanAsset, string collateralAsset, int loanTerm, decimal? loanQuantity = null, decimal? collateralQuantity = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "loanCoin", loanAsset },
                { "collateralCoin", collateralAsset },
                { "loanTerm", loanTerm },
            };
            parameters.AddOptionalParameter("loanAmount", loanQuantity?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("collateralAmount", collateralQuantity?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<HitoBitCryptoLoanBorrow>(_baseClient.GetUrl(borrowEndpoint, "sapi", "1"), HttpMethod.Post, ct, parameters, true, weight: 6000).ConfigureAwait(false);
        }
        #endregion

        #region Get Borrow History
        /// <inheritdoc />
        public async Task<WebCallResult<HitoBitQueryRecords<HitoBitCryptoLoanBorrowRecord>>> GetBorrowHistoryAsync(long? orderId = null, string? loanAsset = null, string? collateralAsset = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? limit = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("orderId", orderId);
            parameters.AddOptionalParameter("loanAsset", loanAsset);
            parameters.AddOptionalParameter("collateralAsset", collateralAsset);
            parameters.AddOptionalParameter("current", page?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("startTime", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("endTime", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<HitoBitQueryRecords<HitoBitCryptoLoanBorrowRecord>>(_baseClient.GetUrl(borrowHistoryEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true, weight: 400).ConfigureAwait(false);
        }
        #endregion

        #region Get Open Borrow Orders
        /// <inheritdoc />
        public async Task<WebCallResult<HitoBitQueryRecords<HitoBitCryptoLoanOpenBorrowOrder>>> GetOpenBorrowOrdersAsync(long? orderId = null, string? loanAsset = null, string? collateralAsset = null, int? page = null, int? limit = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("orderId", orderId);
            parameters.AddOptionalParameter("loanAsset", loanAsset);
            parameters.AddOptionalParameter("collateralAsset", collateralAsset);
            parameters.AddOptionalParameter("current", page?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<HitoBitQueryRecords<HitoBitCryptoLoanOpenBorrowOrder>>(_baseClient.GetUrl(openBorrowOrdersEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true, weight: 400).ConfigureAwait(false);
        }
        #endregion

        #region Repay
        /// <inheritdoc />
        public async Task<WebCallResult<HitoBitCryptoLoanRepay>> RepayAsync(long orderId, decimal quantity, bool? repayWithBorrowedAsset = null, bool? collateralReturn = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "orderId", orderId },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("type", repayWithBorrowedAsset == null ? null : repayWithBorrowedAsset.Value ? "1" : "2");
            parameters.AddOptionalParameter("collateralReturn", collateralReturn);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<HitoBitCryptoLoanRepay>(_baseClient.GetUrl(repayEndpoint, "sapi", "1"), HttpMethod.Post, ct, parameters, true, weight: 6000).ConfigureAwait(false);
        }
        #endregion

        #region Get Repay History
        /// <inheritdoc />
        public async Task<WebCallResult<HitoBitQueryRecords<HitoBitCryptoLoanRepayRecord>>> GetRepayHistoryAsync(long? orderId = null, string? loanAsset = null, string? collateralAsset = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? limit = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("orderId", orderId);
            parameters.AddOptionalParameter("loanAsset", loanAsset);
            parameters.AddOptionalParameter("collateralAsset", collateralAsset);
            parameters.AddOptionalParameter("current", page?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("startTime", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("endTime", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<HitoBitQueryRecords<HitoBitCryptoLoanRepayRecord>>(_baseClient.GetUrl(repayHistoryEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true, weight: 400).ConfigureAwait(false);
        }
        #endregion

        #region Adjust LTV
        /// <inheritdoc />
        public async Task<WebCallResult<HitoBitCryptoLoanLtvAdjust>> AdjustLTVAsync(long orderId, decimal quantity, bool addOrRmove, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "orderId", orderId },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) },
                { "direction", addOrRmove ? "ADDITIONAL" : "REDUCED" }
            };
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<HitoBitCryptoLoanLtvAdjust>(_baseClient.GetUrl(adjustLtvEndpoint, "sapi", "1"), HttpMethod.Post, ct, parameters, true, weight: 6000).ConfigureAwait(false);
        }
        #endregion

        #region Get Repay History
        /// <inheritdoc />
        public async Task<WebCallResult<HitoBitQueryRecords<HitoBitCryptoLoanLtvAdjustRecord>>> GetLtvAdjustHistoryAsync(long? orderId = null, string? loanAsset = null, string? collateralAsset = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? limit = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("orderId", orderId);
            parameters.AddOptionalParameter("loanAsset", loanAsset);
            parameters.AddOptionalParameter("collateralAsset", collateralAsset);
            parameters.AddOptionalParameter("current", page?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("startTime", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("endTime", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<HitoBitQueryRecords<HitoBitCryptoLoanLtvAdjustRecord>>(_baseClient.GetUrl(adjustLtvHistoryEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true, weight: 400).ConfigureAwait(false);
        }
        #endregion
    }
}