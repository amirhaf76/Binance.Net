---
title: IHitoBitSocketClientSpotApiAccount
has_children: false
parent: IHitoBitSocketClientSpotApi
grand_parent: Socket API documentation
---
*[generated documentation]*  
`HitoBitSocketClient > SpotApi > Account`  
*HitoBit Spot Account socket requests and subscriptions*
  

***

## GetAccountInfoAsync  

[https://HitoBit-docs.github.io/apidocs/websocket_api/en/#account-information-user_data](https://HitoBit-docs.github.io/apidocs/websocket_api/en/#account-information-user_data)  
<p>

*Gets account information, including balances*  

```csharp  
var client = new HitoBitSocketClient();  
var result = await client.SpotApi.Account.GetAccountInfoAsync();  
```  

```csharp  
Task<CallResult<HitoBitResponse<HitoBitAccountInfo>>> GetAccountInfoAsync(IEnumerable<string>? symbols = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ symbols||

</p>

***

## GetOrderRateLimitsAsync  

[https://HitoBit-docs.github.io/apidocs/websocket_api/en/#account-order-rate-limits-user_data](https://HitoBit-docs.github.io/apidocs/websocket_api/en/#account-order-rate-limits-user_data)  
<p>

*Get order rate limit status*  

```csharp  
var client = new HitoBitSocketClient();  
var result = await client.SpotApi.Account.GetOrderRateLimitsAsync();  
```  

```csharp  
Task<CallResult<HitoBitResponse<IEnumerable<HitoBitCurrentRateLimit>>>> GetOrderRateLimitsAsync(IEnumerable<string>? symbols = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ symbols||

</p>

***

## KeepAliveUserStreamAsync  

[https://HitoBit-docs.github.io/apidocs/websocket_api/en/#ping-user-data-stream-user_stream](https://HitoBit-docs.github.io/apidocs/websocket_api/en/#ping-user-data-stream-user_stream)  
<p>

*Sends a keep alive for the current user stream listen key to keep the stream from closing. Stream auto closes after 60 minutes if no keep alive is send. 30 minute interval for keep alive is recommended.*  

```csharp  
var client = new HitoBitSocketClient();  
var result = await client.SpotApi.Account.KeepAliveUserStreamAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<HitoBitResponse<object>>> KeepAliveUserStreamAsync(string listenKey);  
```  

|Parameter|Description|
|---|---|
|listenKey||

</p>

***

## StartUserStreamAsync  

[https://HitoBit-docs.github.io/apidocs/websocket_api/en/#start-user-data-stream-user_stream](https://HitoBit-docs.github.io/apidocs/websocket_api/en/#start-user-data-stream-user_stream)  
<p>

*Starts a user stream by requesting a listen key. This listen key can be used in a subsequent request to SubscribeToUserDataUpdates. The stream will close after 60 minutes unless a keep alive is send.*  

```csharp  
var client = new HitoBitSocketClient();  
var result = await client.SpotApi.Account.StartUserStreamAsync();  
```  

```csharp  
Task<CallResult<HitoBitResponse<string>>> StartUserStreamAsync();  
```  

|Parameter|Description|
|---|---|

</p>

***

## StopUserStreamAsync  

[https://HitoBit-docs.github.io/apidocs/websocket_api/en/#stop-user-data-stream-user_stream](https://HitoBit-docs.github.io/apidocs/websocket_api/en/#stop-user-data-stream-user_stream)  
<p>

*Stops the current user stream*  

```csharp  
var client = new HitoBitSocketClient();  
var result = await client.SpotApi.Account.StopUserStreamAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<HitoBitResponse<object>>> StopUserStreamAsync(string listenKey);  
```  

|Parameter|Description|
|---|---|
|listenKey||

</p>

***

## SubscribeToUserDataUpdatesAsync  

[https://HitoBit-docs.github.io/apidocs/spot/en/#user-data-streams](https://HitoBit-docs.github.io/apidocs/spot/en/#user-data-streams)  
<p>

*Subscribes to the account update stream. Prior to using this, the HitoBitClient.Spot.UserStreams.StartUserStream method should be called.*  

```csharp  
var client = new HitoBitSocketClient();  
var result = await client.SpotApi.Account.SubscribeToUserDataUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToUserDataUpdatesAsync(string listenKey, Action<DataEvent<HitoBitStreamOrderUpdate>>? onOrderUpdateMessage, Action<DataEvent<HitoBitStreamOrderList>>? onOcoOrderUpdateMessage, Action<DataEvent<HitoBitStreamPositionsUpdate>>? onAccountPositionMessage, Action<DataEvent<HitoBitStreamBalanceUpdate>>? onAccountBalanceUpdate, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|listenKey|Listen key retrieved by the StartUserStream method|
|onOrderUpdateMessage|The event handler for whenever an order status update is received|
|onOcoOrderUpdateMessage|The event handler for whenever an oco order status update is received|
|onAccountPositionMessage|The event handler for whenever an account position update is received. Account position updates are a list of changed funds|
|onAccountBalanceUpdate|The event handler for whenever a deposit or withdrawal has been processed and the account balance has changed|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>
