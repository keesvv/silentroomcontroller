using System;
using System.Collections.Generic;
using static SilentRoomControllerv2.Program;

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
        public static int[] LightIDs { get; set; }
        public static HueCommand Command { get; set; }
        public static string CommandArguments { get; set; }

        public static string BaseURI
        {
            get
            {
                return string.Format("http://{0}/api/{1}/", BridgeIP, APIKey);
            }
        }
        public static string APIUri
        {
            get
            {
                return string.Format("http://{0}/api", BridgeIP);
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                // Stores all arguments into a public variable.
                Args = args;
            }
            catch (Exception) { }

            // Checks if a previous setup was successful.
            Utilities.CheckSetup();
        }

        public static void ParseArgs()
        {

            // Indicates if anything is left blank.
            bool isNull = false;

            // Parses the arguments and store them into variables.
            #region Command-Line Arguments Parser
            try
            {
                if (Args[0] == "-id")
                {
                    string[] stringIDs = Args[1].Split(',');
                    List<int> IDs = new List<int>();
                    foreach (var id in stringIDs)
                    {
                        int finalID = int.Parse(id);
                        IDs.Add(finalID);
                    }

                    LightIDs = IDs.ToArray();
                }
                else isNull = true;

                if (Args[2] == "-command")
                    Command = new HueCommand((Commands)int.Parse(Args[3]));
                else isNull = true;

                try
                {
                    if (Args[4] != null)
                        CommandArguments = Args[4];
                }
                catch (Exception) { }
            }
            catch (Exception) { Utilities.PrintUsage(); }

            #endregion

            // Checks if the anything is left blank.
            try
            {
                if (isNull != true)
                    foreach (var id in LightIDs)
                    {
                        bool bypassMsg = false;
                        if (Command.SetCommand == Commands.COMMAND_TOGGLE)
                            bypassMsg = true;
                        Command.Execute(BridgeIP, APIKey, id, CommandArguments, bypassMsg);
                    }
                else Utilities.PrintUsage();
            }
            catch (Exception) { }
        }
    }

    public class HueCommand
    {
        public Commands SetCommand { get; set; }
        public HueCommand(Commands command)
        {
            SetCommand = command;
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

    public enum Commands
    {
        COMMAND_ON = 1,
        COMMAND_OFF = 2,
        COMMAND_TOGGLE = 3,
        COMMAND_SET_BRIGHTNESS = 4,
        COMMAND_SET_HUE = 5,
        COMMAND_SET_SATURATION = 6,
        COMMAND_ENABLE_COLORLOOP = 7,
        COMMAND_DISABLE_COLORLOOP = 8
    }
}
