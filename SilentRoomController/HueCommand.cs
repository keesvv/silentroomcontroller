using System;

namespace SilentRoomController
{
    public class HueCommand
    {
        public Commands SetCommand { get; set; }
        public HueCommand(Commands command)
        {
            SetCommand = command;
        }

        public static string BridgeIP { get; set; }
        public static string APIKey { get; set; }
        
        public static string BaseURI
        {
            get
            {
                return string.Format("http://{0}/api/{1}/", BridgeIP, APIKey);
            }
        }

        public void Execute(string ipAddress, string apiKey, int lightID, string arguments = null, bool bypassMessage = false)
        {
            string command = "";
            string targetURI = BaseURI + "lights/" + lightID + "/state";

            switch (SetCommand)
            {
                case Commands.COMMAND_ON:
                    command = "{\"on\":true}";
                    break;
                case Commands.COMMAND_OFF:
                    command = "{\"on\":false}";
                    break;
                case Commands.COMMAND_TOGGLE:
                    Utilities.ToggleLight(ipAddress, apiKey, lightID);
                    break;
                case Commands.COMMAND_SET_BRIGHTNESS:
                    command = "{\"bri\":" + arguments + "}";
                    break;
                case Commands.COMMAND_SET_HUE:
                    command = "{\"hue\":" + arguments + "}";
                    break;
                case Commands.COMMAND_SET_SATURATION:
                    command = "{\"sat\":" + arguments + "}";
                    break;
                case Commands.COMMAND_ENABLE_COLORLOOP:
                    command = "{\"effect\":\"colorloop\"}";
                    break;
                case Commands.COMMAND_DISABLE_COLORLOOP:
                    command = "{\"effect\":\"none\"}";
                    break;
                default:
                    break;
            }

            bool success = false;
            try
            {
                Utilities.SendPUTRequest(targetURI, command);
                success = true;
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unable to execute command " + SetCommand.ToString() + ".");
                Console.ResetColor();
                success = false;
            }
            finally
            {
                if (success && bypassMessage != true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("The command " + SetCommand.ToString() + " executed successfully at light ID " + lightID + ".");
                    Console.ResetColor();
                }
            }
        }
    }
}
