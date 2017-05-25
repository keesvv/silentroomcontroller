using System;
using System.IO;
using System.Net;
using System.Text;

using static SilentRoomControllerv2.Serialization;
using static SilentRoomControllerv2.Program;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;

namespace SilentRoomControllerv2
{
    public class Utilities
    {
        public static void PrintUsage()
        {
            Console.Clear();
            PrintLights(BridgeIP, APIKey);

            string[] usage = new string[]
            {
                "-=+ [ SilentRoomController 2.0 ] +=-",
                "",
                "Usage:",
                "   SilentRoomController.exe -id <light_id> -command <command> [command_args]",
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

        public static void CheckSetup()
        {
            string setupWarning = "WARNING: You haven't set up SilentRoomController yet.\n" +
                "If you don't set up SilentRoomController, you won't be able to use SilentRoomController!\n" +
                "To set up SilentRoomController, type:\n" +
                "SilentRoomController.exe --setup";

            try
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SilentRoomController.conf"))
                {
                    try
                    {
                        StreamReader configReader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SilentRoomController.conf");
                        BridgeIP = configReader.ReadLine();
                        APIKey = configReader.ReadLine();
                        configReader.Close();
                    }
                    catch (Exception) { Console.WriteLine("Unable to parse the config file. Please run under Administrator privileges."); }
                    ParseArgs();
                }
                else
                {
                    try
                    {
                        if (Args[0] == "--setup")
                            Setup();
                    }
                    catch (Exception) { }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(setupWarning);
                    Console.ResetColor();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to read the config file. Please run under Administrator privileges.");
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

        public static string SendPOSTRequest(string targetURI, string command)
        {
            WebRequest request = WebRequest.Create(targetURI);
            request.Method = "POST";
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

        public static void Setup()
        {
            string ipAddress;
            string apiKey;

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();

            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("-=+ [ Setup ] +=-");

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.Write("\nBridge IP Address: ");
            ipAddress = Console.ReadLine();
            BridgeIP = ipAddress;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\nThe API key can be registered by typing /register ." +
                              "\nIf you already have an existing API key which you want" +
                              "\nto use, use that one instead.");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("API Key: ");
            var key = Console.ReadLine();
            if (key == "/register")
            {
                register:
                Console.WriteLine("\nPlease press the Link-button on the Bridge and press any key to continue...");
                Console.ReadLine();

                string rawKeyJSON = RegisterUser("SilentRoomController");

                JObject objResult;
                try
                {
                    JArray response = JArray.Parse(rawKeyJSON);
                    objResult = (JObject)response.First;
                }
                catch (Exception)
                {
                    Console.WriteLine("Error while generating key.");
                    objResult = null;
                }

                JToken objError;
                if(objResult.TryGetValue("error", out objError))
                {
                    if(objError["type"].Value<int>() == 101)
                    {
                        Console.WriteLine("Link button was not pressed.");
                        Thread.Sleep(1000);
                        goto register;
                    }
                    else
                    {
                        Console.WriteLine("An unknown error occured.");
                        Thread.Sleep(1000);
                        goto register;
                    }
                }

                var finalKey = objResult["success"]["username"].Value<string>();
                Console.WriteLine("Key generated: " + finalKey);
                apiKey = finalKey;
            }
            else apiKey = key;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nYou have now successfully set up SilentRoomController.\n");
            Console.WriteLine("Writing settings to file...");

            try
            {
                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                    @"\SilentRoomController.conf",
                    ipAddress + "\n" + apiKey);
            }
            catch (Exception) { Console.WriteLine("Write to file failed. You might need to restart the setup under Administrator privileges."); }

            Console.ForegroundColor = ConsoleColor.Red;

            int interval = 5;
            while (interval >= 0)
            {
                Console.WriteLine("Restarting SilentRoomController in [{0}] second(s)...", interval);
                interval--;
                Thread.Sleep(1000);
            }

            Console.ResetColor();
            Console.Clear();
            CheckSetup();
        }

        public static string RegisterUser(string appName)
        {
            string username = SendPOSTRequest(APIUri, "{\"devicetype\": \"" + appName + "\"}");
            return username;
        }

        public static Light GetLight(string ipAddress, string apiKey, int lightID)
        {
            string uri = BaseURI + "lights/" + lightID;

            string rawJSON = SendGETRequest(uri);
            Light light = JsonConvert.DeserializeObject<Light>(rawJSON);

            return light;
        }

        public static Light[] GetLights(string ipAddress, string apiKey)
        {
            string uri = "http://" + ipAddress + "/api/" + apiKey + "/lights/";
            List<Light> lights = new List<Light>();
            JToken token = JToken.Parse(SendGETRequest(uri));
            if(token.Type == JTokenType.Object)
            {
                JObject lightJSON = (JObject)token;
                foreach (var property in lightJSON.Properties())
                {
                    Light light = JsonConvert.DeserializeObject<Light>(property.Value.ToString());
                    light.ID = property.Name;
                    lights.Add(light);
                }
            }

            return lights.ToArray();
        }

        public static void ToggleLight(string ipAddress, string apiKey, int lightID)
        {
            try
            {
                Light light = GetLight(ipAddress, apiKey, lightID);
                if (light.State.Enabled == true)
                    new HueCommand(Commands.COMMAND_OFF).Execute(ipAddress, apiKey, lightID);
                else if (light.State.Enabled == false)
                    new HueCommand(Commands.COMMAND_ON).Execute(ipAddress, apiKey, lightID);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        public static void PrintLights(string ipAddress, string apiKey)
        {
            Console.WriteLine("Loading available lights...");
            Light[] lights = GetLights(ipAddress, apiKey);
            Console.Clear();

            Console.WriteLine("======== [ AVAILABLE LIGHTS ] ========");
            foreach (Light light in lights)
            {
                Console.WriteLine("[" + light.ID + "] " + light.Name);
            }
            Console.WriteLine("======== [ -=+=-=+=-=+=-=+= ] ========\n");
        }
    }
}
