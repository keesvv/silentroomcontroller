using System;
using System.Collections.Generic;
using NDesk.Options;

namespace SilentRoomController
{
    public class SilentRoomController
    {
         /*
         *                                             *
         *       -=+ [ SilentRoomController ] +=-      *
         *        Made by Kees van Voorthuizen.        *
         *         Credits to Philips (c) Hue.         *
         *                                             *
         */
        
        public static string[] Args { get; set; }
        public static List<int> LightIDs { get; set; }
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
            var options = new OptionSet()
            {
                {
                    "i|id=",
                    "The light ID(s) to select for controlling the lights.",
                    (int v) => LightIDs.Add(v)
                },

                {
                    "c|command=",
                    "the number of {TIMES} to repeat the greeting.\n" +
                    "this must be an integer.",
                    (int v) => Command = null
                },


                {
                    "v",
                    "increase debug message verbosity",
                    v => Console.WriteLine()
                },

                {
                    "h|help",
                    "show this message and exit",
                    v => Console.WriteLine()
                },
            };

            string[] stringIDs = Args[1].Split(',');
            List<int> IDs = new List<int>();
            foreach (var id in stringIDs)
            {
                int finalID = int.Parse(id);
                IDs.Add(finalID);
            }

            //LightIDs = IDs.ToArray();
            Command = new HueCommand((Commands)int.Parse(Args[3]));
            isNull = false;

            try
            {
                if (Args[4] != null)
                    CommandArguments = Args[4];
            }
            catch (Exception)
            {
            }
            #endregion

            // Checks if the anything is left blank.
            try
            {
                if (isNull != true)
                    foreach (var id in LightIDs)
                    {
                        bool quietMode = false;
                        if (Command.SetCommand == Commands.COMMAND_TOGGLE)
                            quietMode = true;
                        Command.Execute(HueCommand.BridgeIP, HueCommand.APIKey, id, CommandArguments, quietMode);
                    }
                else Utilities.PrintUsage();
            }
            catch (Exception)
            {
            }
        }
    }
}
