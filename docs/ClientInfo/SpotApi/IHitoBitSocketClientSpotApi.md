---
title: IHitoBitSocketClientSpotApi
has_children: true
parent: Socket API documentation
---
*[generated documentation]*  
`HitoBitSocketClient > SpotApi`  
*Spot API socket subscriptions and requests*
  
***
*Account streams and queries*  
**[IHitoBitSocketClientSpotApiAccount](IHitoBitSocketClientSpotApiAccount.html) Account { get; }**  
***
*Exchange data streams and queries*  
**[IHitoBitSocketClientSpotApiExchangeData](IHitoBitSocketClientSpotApiExchangeData.html) ExchangeData { get; }**  
***
*Set the API credentials for this API*  
**void SetApiCredentials<T>(T credentials) where T : ApiCredentials;**  
***
*Factory for websockets*  
**IWebsocketFactory SocketFactory { get; set; }**  
***
*Trading data and queries*  
**[IHitoBitSocketClientSpotApiTrading](IHitoBitSocketClientSpotApiTrading.html) Trading { get; }**  
