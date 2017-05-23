using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SilentRoomControllerv2
{
    public class Program
    {
         /*
         *                                             *
         *    -=+ [ SilentRoomController 2.0 ] +=-     *
         *        Made by Kees van Voorthuizen.        *
         *         Credits to Philips (c) Hue.         *
         *                                             *
         */
        
        public static string[] Args { get; set; }
        public static string BridgeIP { get; set; }
        public static string APIKey { get; set; }
        public static int LightID { get; set; }
        public static HueCommand Command { get; set; }
        public static string CommandArguments { get; set; }

        public static void Main(string[] args)
        {
            try
            {
                // Indicates if anything is left blank.
                bool isNull = false;

                // Stores all arguments into a public variable.
                Args = args;

                // Parses the arguments and store them into variables.
                #region Command-Line Arguments Parser
                if (Args[0] == "-ip")
                    BridgeIP = Args[1];
                else isNull = true;

                if (Args[2] == "-key")
                    APIKey = Args[3];
                else isNull = true;

                if (Args[4] == "-id")
                    LightID = int.Parse(Args[5]);
                else isNull = true;

                if (Args[6] == "-command")
                    Command = new HueCommand((Commands)int.Parse(Args[7]));
                else isNull = true;

                try
                {
                    if (Args[8] != null)
                        CommandArguments = Args[8];
                }
                catch (Exception) { }
                
                #endregion

                // Checks if the [isNull] variable is set to true/false.
                if (isNull != true)
                    Command.Execute(BridgeIP, APIKey, LightID, CommandArguments);
                else Utilities.PrintUsage();
            }
            catch (Exception) { Utilities.PrintUsage(); }
        }
    }

    public class HueCommand
    {
        public Commands SetCommand { get; set; }
        public HueCommand(Commands command)
        {
            SetCommand = command;
        }

        public void Execute(string ipAddress, string apiKey, int lightID, string arguments = null)
        {
            string command = "";
            string targetURI = "http://" +
                ipAddress + "/api/" +
                apiKey + "/lights/" +
                lightID + "/state";

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
                default:
                    break;
            }

            Utilities.SendPUTRequest(targetURI, command);
        }
    }

    public enum Commands
    {
        COMMAND_ON = 1,
        COMMAND_OFF = 2,
        COMMAND_TOGGLE = 3,
        COMMAND_SET_BRIGHTNESS = 4,
        COMMAND_SET_HUE = 5
    }
}
