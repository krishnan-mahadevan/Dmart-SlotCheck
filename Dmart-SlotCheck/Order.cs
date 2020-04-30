using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dmart_SlotCheck
{
    public partial class Order
    {
        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        [JsonProperty("addressId")]
        public long AddressId { get; set; }

        [JsonProperty("shipModeId")]
        public long ShipModeId { get; set; }
    }
}
