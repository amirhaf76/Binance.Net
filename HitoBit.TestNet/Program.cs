using HitoBit.Net.Clients;
using HitoBit.Net.Interfaces;
using HitoBit.Net.Objects.Models.Spot.Socket;
using CryptoExchange.Net.Sockets;

namespace HitoBit.TestNet
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var h = new HttpClient();
            var client = new HitoBitRestClient(h, null, c =>
            {
                c.Environment = HitoBit.Net.HitoBitEnvironment.Live;
                c.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("YHjEwBDqNbjO414F6LXol0OjpSYkWh9esNgzk9NeEINw9KS6Ez4Z4rfG0ABs65JT", "rvpbP4xkfXs31A4LUDWsK0RxIJGAXjBkuDoM0PvAHch5e1ITmi4wM49ZiHl0gg4g");
            });


            var socketClient = new HitoBitSocketClient(c =>
            {
                c.Environment = HitoBit.Net.HitoBitEnvironment.Live;
                c.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("YHjEwBDqNbjO414F6LXol0OjpSYkWh9esNgzk9NeEINw9KS6Ez4Z4rfG0ABs65JT", "rvpbP4xkfXs31A4LUDWsK0RxIJGAXjBkuDoM0PvAHch5e1ITmi4wM49ZiHl0gg4g");
            });

            var listenKeyData = await client.SpotApi.Account.StartUserStreamAsync();
            var yyy = await socketClient.SpotApi.Account.SubscribeToUserDataUpdatesAsync(listenKeyData.Data, OnOrderUpdated, null, OnAccountBalanceUpdate, OnBalanceUpdated);
            if (yyy.Success)
            {

            }

            Console.ReadLine();
        }

        private static void OnBalanceUpdated(DataEvent<HitoBitStreamBalanceUpdate> obj)
        {
        }

        private static void OnAccountBalanceUpdate(DataEvent<HitoBitStreamPositionsUpdate> obj)
        {
        }

        private static void OnOrderUpdated(DataEvent<HitoBitStreamOrderUpdate> obj)
        {
        }

        private static void OnMiniTicker(DataEvent<IEnumerable<IHitoBitMiniTick>> obj)
        {
        }
    }
}