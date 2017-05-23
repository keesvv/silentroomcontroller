using System;
using System.IO;
using System.Net;
using System.Text;

using static SilentRoomControllerv2.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SilentRoomControllerv2
{
    public class Utilities
    {
        public static void PrintUsage()
        {
            string[] usage = new string[]
            {
                "-=+ [ SilentRoomController 2.0 ] +=-",
                "",
                "Usage:",
                "   SilentRoomController.exe -ip <ip_address> -key <api_key> -id <light_id> -command <command> [command_args]",
                "",
                "Available Commands:",
                "   - [1] COMMAND_ON: Turns on the specified light.",
                "   - [2] COMMAND_OFF: Turns of the specified light.",
                "   - [3] COMMAND_TOGGLE: Toggles the specified light.",
                "   - [4] COMMAND_SET_BRIGHTNESS: (*) Sets the brightness of the specified light.",
                "   - [5] COMMAND_SET_HUE: (*) Sets the Hue color value of the specified light.",
                "",
                "The commands marked with '(*)' need to have a parameter in the [command_args] parameter."
            };

            foreach (string line in usage)
            {
                Console.WriteLine(line);
            }
        }

        public static string SendPUTRequest(string targetURI, string command)
        {
            WebRequest request = WebRequest.Create(targetURI);
            request.Method = "PUT";
            string postData = command;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;

            var dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);
            string readerString = reader.ReadToEnd();

            response.Close();
            reader.Close();

            return readerString;
        }

        public static string SendGETRequest(string targetURI)
        {
            WebRequest request = WebRequest.Create(targetURI);
            request.Method = "GET";
            request.ContentType = "application/json";

            WebResponse response = request.GetResponse();
            var dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);
            string readerString = reader.ReadToEnd();

            response.Close();
            reader.Close();

            return readerString;
        }

        public static Light GetLightState(string ipAddress, string apiKey, int lightID)
        {
            string uri = "http://" +
                ipAddress + "/api/" +
                apiKey + "/lights/" +
                lightID;

            string rawJSON = SendGETRequest(uri);
            Light light = JsonConvert.DeserializeObject<Light>(rawJSON);

            return light;
        }

        public static void ToggleLight(string ipAddress, string apiKey, int lightID)
        {
            try
            {
                Light light = GetLightState(ipAddress, apiKey, lightID);
                if (light.State.Enabled == true)
                    new HueCommand(Commands.COMMAND_OFF).Execute(ipAddress, apiKey, lightID);
                else if (light.State.Enabled == false)
                    new HueCommand(Commands.COMMAND_ON).Execute(ipAddress, apiKey, lightID);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}
