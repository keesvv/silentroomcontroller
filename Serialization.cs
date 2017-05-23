using Newtonsoft.Json;

namespace SilentRoomControllerv2
{
    public class Serialization
    {
        public class Light
        {
            [JsonProperty("state")]
            public State State { get; set; }
            [JsonProperty("type")]
            public string Type { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("modelid")]
            public string ModelID { get; set; }
            [JsonProperty("manufacturername")]
            public string Manufacturer { get; set; }
            [JsonProperty("uniqueid")]
            public string UID { get; set; }
            [JsonProperty("swversion")]
            public string SoftwareVersion { get; set; }
        }

        public class State
        {
            [JsonProperty("on")]
            public bool Enabled { get; set; }
            [JsonProperty("bri")]
            public int Brightness { get; set; }
            [JsonProperty("hue")]
            public int Hue { get; set; }
            [JsonProperty("sat")]
            public int Saturation { get; set; }
            [JsonProperty("effect")]
            public string Effect { get; set; }
            [JsonProperty("xy")]
            public float[] XYColor { get; set; }
            [JsonProperty("alert")]
            public string Alert { get; set; }
            [JsonProperty("colormode")]
            public string Colormode { get; set; }
            [JsonProperty("reachable")]
            public bool Reachable { get; set; }
        }
    }
}
