using System;
using System.Collections.Generic;

namespace SilentRoomController
{
    public class SilentRoomController
    {
         /*
         *                                             *
         *    -=+ [ SilentRoomController 2.0 ] +=-     *
         *        Made by Kees van Voorthuizen.        *
         *         Credits to Philips (c) Hue.         *
         *                                             *
         */
        
        public static string[] Args { get; set; }
        public static int[] LightIDs { get; set; }
        public static HueCommand Command { get; set; }
        public static string CommandArguments { get; set; }

        public static string APIUri
        {
            get
            {
                return string.Format("http://{0}/api", HueCommand.BridgeIP);
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                // Stores all arguments into a public variable.
                Args = args;
            }
            catch (Exception)
            {
            }

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
                catch (Exception)
                {
                }
            }
            catch (Exception)
            {
                Utilities.PrintUsage();
            }
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
                        Command.Execute(HueCommand.BridgeIP, HueCommand.APIKey, id, CommandArguments, bypassMsg);
                    }
                else Utilities.PrintUsage();
            }
            catch (Exception)
            {
            }
        }
    }
}
