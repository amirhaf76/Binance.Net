---
title: Rest API documentation
has_children: true
---
*[generated documentation]*  
### HitoBitRestClient  
*Client for accessing the HitoBit Rest API.*
  
***
*Coin futures API endpoints*  
**[IHitoBitRestClientCoinFuturesApi](CoinFuturesApi/IHitoBitRestClientCoinFuturesApi.html) CoinFuturesApi { get; }**  
***
*General API endpoints*  
**[IHitoBitRestClientGeneralApi](GeneralApi/IHitoBitRestClientGeneralApi.html) GeneralApi { get; }**  
***
*Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.*  
**void SetApiCredentials(ApiCredentials credentials);**  
***
*Spot API endpoints*  
**[IHitoBitRestClientSpotApi](SpotApi/IHitoBitRestClientSpotApi.html) SpotApi { get; }**  
***
*Usd futures API endpoints*  
**[IHitoBitRestClientUsdFuturesApi](UsdFuturesApi/IHitoBitRestClientUsdFuturesApi.html) UsdFuturesApi { get; }**  
