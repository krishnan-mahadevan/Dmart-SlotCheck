using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Dmart_SlotCheck
{
    public static class SlotCheck
    {
        [FunctionName("SlotCheck")]
        public static void Run([TimerTrigger("%JobTriggerInterval%", RunOnStartup = true)]TimerInfo myTimer, ExecutionContext context, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function started at: {DateTime.Now}");

            var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true) // <- This gives you access to your application settings in your local development environment
                    .AddEnvironmentVariables() // <- This is what actually gets you the application settings in Azure
                    .Build();

            var service = new SlotService(log, config);
            service.CheckSlotsAsync().ConfigureAwait(true).GetAwaiter();

            log.LogInformation($"C# Timer trigger function completed at: {DateTime.Now}");
        }
    }

    public class SlotService
    {
        private readonly ILogger _log;
        private readonly IConfiguration _config;
        public SlotService(ILogger log, IConfiguration config)
        {
            _log = log;
            _config = config;
        }

        public async Task CheckSlotsAsync()
        {
            var slots = await GetSlotsAsync();
            if (slots != null)
            {
                var availablilities = slots.AvailableSlots.SlotList.Where(a => a.Availability != 0).ToList();

                if (availablilities.Any())
                {
                    SendNotification(JsonConvert.SerializeObject(availablilities));
                }
                else
                {
                    _log.LogInformation("No slot available at the store yet!!!");
                }
            }
        }

        private async Task<Slots> GetSlotsAsync()
        {
            try
            { 
                var order = new Order { AddressId = 5212162, OrderId = 12982039, ShipModeId = 11107 };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://digital.dmart.in/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Referrer", "https://dmart.in/delivery?currentAction=Delivery");
                    client.DefaultRequestHeaders.Add("dm_token", _config["Dmart-Token"]);
                    client.DefaultRequestHeaders.Add("storeId", _config["Dmart-StoreId"]);

                    var json = JsonConvert.SerializeObject(order);
                    using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        using (var response = await client.PostAsync("/api/v2/secure/slots/", stringContent))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var data = await response.Content.ReadAsStringAsync();
                                return JsonConvert.DeserializeObject<Slots>(data);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"{ex.Message}\r\n{ ex.InnerException }");
            }
            return null;
        }

        private void SendNotification(string content)
        {
            try
            {
                var accountSid = _config["Twilio-AccountId"];
                var authToken = _config["Twilio-AuthToken"];
                var numberValues = _config["Notification-Numbers"];
                var numbers = numberValues.Split(',');

                TwilioClient.Init(accountSid, authToken);
                foreach (var number in numbers)
                { 
                    var message = MessageResource.Create(
                                    from: new PhoneNumber("whatsapp:+14155238886"),
                                    body: content,
                                    to: new PhoneNumber($"whatsapp:{number}")
                                );
                    _log.LogInformation(message.Body);
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"Error while sending WhtsApp message {ex.Message}\r\n{ex.StackTrace }");
            }
        }
    }
}
