using System;
using System.Collections.Generic;
using System.Text;

namespace Dmart_SlotCheck
{

    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Slots
    {
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("availableSlots")]
        public AvailableSlots AvailableSlots { get; set; }
    }

    public partial class AvailableSlots
    {
        [JsonProperty("resourcePoolkey")]
        public string ResourcePoolkey { get; set; }

        [JsonProperty("slotList")]
        public SlotList[] SlotList { get; set; }
    }

    public partial class SlotList
    {
        [JsonProperty("dayOfTheWeek")]
        public string DayOfTheWeek { get; set; }

        [JsonProperty("endTime")]
        public string EndTime { get; set; }

        [JsonProperty("todayOrTomorrow")]
        public string TodayOrTomorrow { get; set; }

        [JsonProperty("availability")]
        public int Availability { get; set; }

        [JsonProperty("slotDescription")]
        public string SlotDescription { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("startTime")]
        public string StartTime { get; set; }
    }
}
