﻿using HitoBit.Net.Clients;
using HitoBit.Net.Enums;
using HitoBit.Net.Interfaces.Clients;
using HitoBit.Net.Objects;
using HitoBit.Net.Objects.Internal;
using HitoBit.Net.Objects.Models.Spot;
using HitoBit.Net.Objects.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;
using HitoBit.Net.SymbolOrderBooks;
using HitoBit.Net.Interfaces;

namespace HitoBit.Net
{
    /// <summary>
    /// Helper methods for the HitoBit API
    /// </summary>
    public static class HitoBitHelpers
    {
        /// <summary>
        /// Get the used weight from the response headers
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static int? UsedWeight(this IEnumerable<KeyValuePair<string, IEnumerable<string>>>? headers)
        {
            if (headers == null)
                return null;

            var headerValues = headers.SingleOrDefault(s => s.Key.StartsWith("X-MBX-USED-WEIGHT-", StringComparison.InvariantCultureIgnoreCase)).Value;
            if (headerValues != null && int.TryParse(headerValues.First(), out var value))
                return value;
            return null;
        }

        /// <summary>
        /// Get the used weight from the response headers
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static int? UsedOrderCount(this IEnumerable<KeyValuePair<string, IEnumerable<string>>>? headers)
        {
            if (headers == null)
                return null;

            var headerValues = headers.SingleOrDefault(s => s.Key.StartsWith("X-MBX-ORDER-COUNT-", StringComparison.InvariantCultureIgnoreCase)).Value;
            if (headerValues != null && int.TryParse(headerValues.First(), out var value))
                return value;
            return null;
        }

        /// <summary>
        /// Clamp a quantity between a min and max quantity and floor to the closest step
        /// </summary>
        /// <param name="minQuantity"></param>
        /// <param name="maxQuantity"></param>
        /// <param name="stepSize"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static decimal ClampQuantity(decimal minQuantity, decimal maxQuantity, decimal stepSize, decimal quantity)
        {
            quantity = Math.Min(maxQuantity, quantity);
            quantity = Math.Max(minQuantity, quantity);
            if (stepSize == 0)
                return quantity;
            quantity -= quantity % stepSize;
            quantity = Floor(quantity);
            return quantity;
        }

        /// <summary>
        /// Clamp a price between a min and max price
        /// </summary>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public static decimal ClampPrice(decimal minPrice, decimal maxPrice, decimal price)
        {
            price = Math.Min(maxPrice, price);
            price = Math.Max(minPrice, price);
            return price;
        }

        /// <summary>
        /// Floor a price to the closest tick
        /// </summary>
        /// <param name="tickSize"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public static decimal FloorPrice(decimal tickSize, decimal price)
        {
            price -= price % tickSize;
            price = Floor(price);
            return price;
        }

        /// <summary>
        /// Floor
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static decimal Floor(decimal number)
        {
            return Math.Floor(number * 100000000) / 100000000;
        }

        /// <summary>
        /// Add the IHitoBitClient and IHitoBitSocketClient to the sevice collection so they can be injected
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="defaultRestOptionsDelegate">Set default options for the rest client</param>
        /// <param name="defaultSocketOptionsDelegate">Set default options for the socket client</param>
        /// <param name="socketClientLifeTime">The lifetime of the IHitoBitSocketClient for the service collection. Defaults to Singleton.</param>
        /// <returns></returns>
        public static IServiceCollection AddHitoBit(
            this IServiceCollection services,
            Action<HitoBitRestOptions>? defaultRestOptionsDelegate = null,
            Action<HitoBitSocketOptions>? defaultSocketOptionsDelegate = null,
            ServiceLifetime? socketClientLifeTime = null)
        {
            var restOptions = HitoBitRestOptions.Default.Copy();

            if (defaultRestOptionsDelegate != null)
            {
                defaultRestOptionsDelegate(restOptions);
                HitoBitRestClient.SetDefaultOptions(defaultRestOptionsDelegate);
            }

            if (defaultSocketOptionsDelegate != null)
                HitoBitSocketClient.SetDefaultOptions(defaultSocketOptionsDelegate);

            services.AddHttpClient<IHitoBitRestClient, HitoBitRestClient>(options =>
            {
                options.Timeout = restOptions.RequestTimeout;
            }).ConfigurePrimaryHttpMessageHandler(() => {
                var handler = new HttpClientHandler();
                if (restOptions.Proxy != null)
                {
                    handler.Proxy = new WebProxy
                    {
                        Address = new Uri($"{restOptions.Proxy.Host}:{restOptions.Proxy.Port}"),
                        Credentials = restOptions.Proxy.Password == null ? null : new NetworkCredential(restOptions.Proxy.Login, restOptions.Proxy.Password)
                    };
                }
                return handler;
            });

            services.AddSingleton<IHitoBitOrderBookFactory, HitoBitOrderBookFactory>();
            services.AddTransient<IHitoBitRestClient, HitoBitRestClient>();
            if (socketClientLifeTime == null)
                services.AddSingleton<IHitoBitSocketClient, HitoBitSocketClient>();
            else
                services.Add(new ServiceDescriptor(typeof(IHitoBitSocketClient), typeof(HitoBitSocketClient), socketClientLifeTime.Value));
            return services;
        }

        /// <summary>
        /// Validate the string is a valid HitoBit symbol.
        /// </summary>
        /// <param name="symbolString">string to validate</param> 
        public static void ValidateHitoBitSymbol(this string symbolString)
        {
            if (string.IsNullOrEmpty(symbolString))
                throw new ArgumentException("Symbol is not provided");

            if(!Regex.IsMatch(symbolString, "^([A-Z|a-z|0-9]{5,})$"))
                throw new ArgumentException($"{symbolString} is not a valid HitoBit symbol. Should be [BaseAsset][QuoteAsset], e.g. BTCUSDT");
        }

        internal static HitoBitTradeRuleResult ValidateTradeRules(ILogger logger, TradeRulesBehaviour tradeRulesBehaviour, HitoBitExchangeInfo exchangeInfo, string symbol, decimal? quantity, decimal? quoteQuantity, decimal? price, decimal? stopPrice, SpotOrderType? type)
        {
            var outputQuantity = quantity;
            var outputQuoteQuantity = quoteQuantity;
            var outputPrice = price;
            var outputStopPrice = stopPrice;

            var symbolData = exchangeInfo.Symbols.SingleOrDefault(s => string.Equals(s.Name, symbol, StringComparison.CurrentCultureIgnoreCase));
            if (symbolData == null)
                return HitoBitTradeRuleResult.CreateFailed($"Trade rules check failed: Symbol {symbol} not found");

            if (type != null)
            {
                if (!symbolData.OrderTypes.Contains(type.Value))
                {
                    return HitoBitTradeRuleResult.CreateFailed(
                        $"Trade rules check failed: {type} order type not allowed for {symbol}");
                }
            }

            if (symbolData.LotSizeFilter != null || symbolData.MarketLotSizeFilter != null && type == SpotOrderType.Market)
            {
                var minQty = symbolData.LotSizeFilter?.MinQuantity;
                var maxQty = symbolData.LotSizeFilter?.MaxQuantity;
                var stepSize = symbolData.LotSizeFilter?.StepSize;
                if (type == SpotOrderType.Market && symbolData.MarketLotSizeFilter != null)
                {
                    minQty = symbolData.MarketLotSizeFilter.MinQuantity;
                    if (symbolData.MarketLotSizeFilter.MaxQuantity != 0)
                        maxQty = symbolData.MarketLotSizeFilter.MaxQuantity;

                    if (symbolData.MarketLotSizeFilter.StepSize != 0)
                        stepSize = symbolData.MarketLotSizeFilter.StepSize;
                }

                if (minQty.HasValue && quantity.HasValue)
                {
                    outputQuantity = HitoBitHelpers.ClampQuantity(minQty.Value, maxQty!.Value, stepSize!.Value, quantity.Value);
                    if (outputQuantity != quantity.Value)
                    {
                        if (tradeRulesBehaviour == TradeRulesBehaviour.ThrowError)
                        {
                            return HitoBitTradeRuleResult.CreateFailed($"Trade rules check failed: LotSize filter failed. Original quantity: {quantity}, Closest allowed: {outputQuantity}");
                        }

                        logger.Log(LogLevel.Information, $"Quantity clamped from {quantity} to {outputQuantity} based on lot size filter");
                    }
                }
            }

            if (symbolData.MinNotionalFilter != null && outputQuoteQuantity != null)
            {
                if (quoteQuantity < symbolData.MinNotionalFilter.MinNotional)
                {
                    if (tradeRulesBehaviour == TradeRulesBehaviour.ThrowError)
                    {
                        return HitoBitTradeRuleResult.CreateFailed(
                            $"Trade rules check failed: MinNotional filter failed. Order value: {quoteQuantity}, minimal order value: {symbolData.MinNotionalFilter.MinNotional}");
                    }

                    outputQuoteQuantity = symbolData.MinNotionalFilter.MinNotional;
                    logger.Log(LogLevel.Information, $"QuoteQuantity adjusted from {quoteQuantity} to {outputQuoteQuantity} based on min notional filter");
                }
            }

            if (price == null)
                return HitoBitTradeRuleResult.CreatePassed(outputQuantity, outputQuoteQuantity, null, outputStopPrice);

            if (symbolData.PriceFilter != null)
            {
                if (symbolData.PriceFilter.MaxPrice != 0 && symbolData.PriceFilter.MinPrice != 0)
                {
                    outputPrice = HitoBitHelpers.ClampPrice(symbolData.PriceFilter.MinPrice, symbolData.PriceFilter.MaxPrice, price.Value);
                    if (outputPrice != price)
                    {
                        if (tradeRulesBehaviour == TradeRulesBehaviour.ThrowError)
                            return HitoBitTradeRuleResult.CreateFailed($"Trade rules check failed: Price filter max/min failed. Original price: {price}, Closest allowed: {outputPrice}");

                        logger.Log(LogLevel.Information, $"price clamped from {price} to {outputPrice} based on price filter");
                    }

                    if (stopPrice != null)
                    {
                        outputStopPrice = HitoBitHelpers.ClampPrice(symbolData.PriceFilter.MinPrice,
                            symbolData.PriceFilter.MaxPrice, stopPrice.Value);
                        if (outputStopPrice != stopPrice)
                        {
                            if (tradeRulesBehaviour == TradeRulesBehaviour.ThrowError)
                            {
                                return HitoBitTradeRuleResult.CreateFailed(
                                    $"Trade rules check failed: Stop price filter max/min failed. Original stop price: {stopPrice}, Closest allowed: {outputStopPrice}");
                            }

                            logger.Log(LogLevel.Information,
                                $"Stop price clamped from {stopPrice} to {outputStopPrice} based on price filter");
                        }
                    }
                }

                if (symbolData.PriceFilter.TickSize != 0)
                {
                    var beforePrice = outputPrice;
                    outputPrice = HitoBitHelpers.FloorPrice(symbolData.PriceFilter.TickSize, price.Value);
                    if (outputPrice != beforePrice)
                    {
                        if (tradeRulesBehaviour == TradeRulesBehaviour.ThrowError)
                            return HitoBitTradeRuleResult.CreateFailed($"Trade rules check failed: Price filter tick failed. Original price: {price}, Closest allowed: {outputPrice}");

                        logger.Log(LogLevel.Information, $"price floored from {beforePrice} to {outputPrice} based on price filter");
                    }

                    if (stopPrice != null)
                    {
                        var beforeStopPrice = outputStopPrice;
                        outputStopPrice = HitoBitHelpers.FloorPrice(symbolData.PriceFilter.TickSize, stopPrice.Value);
                        if (outputStopPrice != beforeStopPrice)
                        {
                            if (tradeRulesBehaviour == TradeRulesBehaviour.ThrowError)
                            {
                                return HitoBitTradeRuleResult.CreateFailed(
                                    $"Trade rules check failed: Stop price filter tick failed. Original stop price: {stopPrice}, Closest allowed: {outputStopPrice}");
                            }

                            logger.Log(LogLevel.Information,
                                $"Stop price floored from {beforeStopPrice} to {outputStopPrice} based on price filter");
                        }
                    }
                }
            }

            if (symbolData.MinNotionalFilter == null || quantity == null || outputPrice == null)
                return HitoBitTradeRuleResult.CreatePassed(outputQuantity, outputQuoteQuantity, outputPrice, outputStopPrice);

            var currentQuantity = outputQuantity ?? quantity.Value;
            var notional = currentQuantity * outputPrice.Value;
            if (notional < symbolData.MinNotionalFilter.MinNotional)
            {
                if (tradeRulesBehaviour == TradeRulesBehaviour.ThrowError)
                {
                    return HitoBitTradeRuleResult.CreateFailed(
                        $"Trade rules check failed: MinNotional filter failed. Order quantity: {notional}, minimal order quantity: {symbolData.MinNotionalFilter.MinNotional}");
                }

                if (symbolData.LotSizeFilter == null)
                    return HitoBitTradeRuleResult.CreateFailed("Trade rules check failed: MinNotional filter failed. Unable to auto comply because LotSizeFilter not present");

                var minQuantity = symbolData.MinNotionalFilter.MinNotional / outputPrice.Value;
                var stepSize = symbolData.LotSizeFilter!.StepSize;
                outputQuantity = HitoBitHelpers.Floor(minQuantity + (stepSize - minQuantity % stepSize));
                logger.Log(LogLevel.Information, $"Quantity clamped from {currentQuantity} to {outputQuantity} based on min notional filter");
            }

            return HitoBitTradeRuleResult.CreatePassed(outputQuantity, outputQuoteQuantity, outputPrice, outputStopPrice);
        }
    }
}