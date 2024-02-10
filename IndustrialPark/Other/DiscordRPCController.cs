using DiscordRPC;
using DiscordRPC.Logging;
using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark
{
    public static class DiscordRPCController
    {
        public static DiscordRpcClient client;

        public static System.Windows.Forms.Timer timer1;

        public static List<ArchiveEditor> archiveEditors = Program.MainForm.archiveEditors;

        public static string currentArchiveEditor = string.Empty;

        public static bool alreadyIdling = false;

        public static Timestamps currentTimestamp = null;

        internal static void ToggleDiscordRichPresence(bool value)
        {
            if (value)
            {
                try
                {
                    Initialize();
                }
                catch { }
            }
            else
            {
                try
                {
                    if (client != null)
                        client.Dispose();
                }
                catch { }
                if (timer1 != null)
                    timer1.Enabled = false;
            }
        }

        public static void Initialize()
        {
            timer1 = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            timer1.Tick += timer1_Tick;

            // this application ID is assigned to Suprnova#0001 on Discord
            // it can be replaced with any other application ID as long as 
            // the name is "Industrial Park" and the image of the logo has 
            // the name of "icon"
            client = new DiscordRpcClient("734104261876514836")
            {
                Logger = new ConsoleLogger() { Level = LogLevel.Warning }
            };
            client.Initialize();
            timer1.Start();
            SetPresence(idling: true);
        }

        public static void SetPresence(ArchiveEditor archiveEditor = null, bool idling = false)
        {
            try
            {
                string details = "Idling";
                string state, largeImageKey, largeImageText, game;
                state = largeImageKey = largeImageText = game = string.Empty;
                string smallImageKey = "icon";
                string smallImageText = $"Industrial Park {new IPversion().version}";

                if (!idling)
                {
                    alreadyIdling = false;
                    if (archiveEditor == null)
                        // .SingleOrDefault() works best here but it's not worth throwing an
                        // exception just to update the presence.
                        archiveEditor = archiveEditors.Where(x => x.ContainsFocus).FirstOrDefault();

                    // no point updating if it wasn't changed
                    if (currentArchiveEditor == archiveEditor.Text)
                        return;
                    // additionally checking to maintain timestamp if in same level but different file
                    if (currentArchiveEditor.Substring(0, currentArchiveEditor.Length > 4 ? 4 : currentArchiveEditor.Length) != archiveEditor.Text.Substring(0, archiveEditor.Text.Length > 4 ? 4 : archiveEditor.Text.Length))
                    {
                        currentTimestamp = Timestamps.Now;
                    }
                    currentArchiveEditor = archiveEditor.Text;

                    game = GameHandler(archiveEditor);

                    switch (game)
                    {
                        case "Scooby":
                            largeImageText = "Scooby-Doo: Night of 100 Frights";
                            break;
                        case "BFBB":
                            largeImageText = "SpongeBob SquarePants: Battle for Bikini Bottom";
                            break;
                        case "TSSM":
                            largeImageText = "The SpongeBob SquarePants Movie Game";
                            break;
                        case "Incredibles":
                            largeImageText = "The Incredibles";
                            break;
                        case "ROTU":
                            largeImageText = "The Incredibles: Rise of the Underminer";
                            break;
                        case "Ratatouille":
                            largeImageText = "Ratatouille (Prototype)";
                            break;
                        case "Unknown":
                        default:
                            largeImageKey = smallImageKey;
                            largeImageText = smallImageText;
                            smallImageKey = "";
                            smallImageText = "";
                            break;
                    }

                    // uses "a" or "an" depending on the game's first char.
                    // (I = Incredibles, U = Unknown, RO = ROTU)
                    // If no match, it's OK to prefix with "a".
                    details = $"Editing {("IU".IndexOf(game.ElementAt(0)) >= 0 || game.StartsWith("RO") ? $"an {game}" : $"a {game}")} level";

                    state = $"{archiveEditor.Text}{(archiveEditor.archive.platform != Platform.Unknown ? $" - {archiveEditor.archive.platform}" : "")}";
                }
                else
                {
                    if (alreadyIdling)
                        return;
                    currentArchiveEditor = string.Empty;
                    alreadyIdling = true;
                    currentTimestamp = Timestamps.Now;
                    largeImageKey = smallImageKey;
                    largeImageText = smallImageText;
                    smallImageKey = "";
                    smallImageText = "";
                }

                client.SetPresence(new RichPresence()
                {
                    Details = details,
                    State = state,
                    Timestamps = currentTimestamp,
                    Assets = new Assets()
                    {
                        LargeImageKey = (largeImageKey == "icon" ? largeImageKey : game.ToLower()),
                        LargeImageText = largeImageText,
                        SmallImageKey = smallImageKey,
                        SmallImageText = smallImageText
                    }
                });
            }
            catch
            {

            }
        }

        private static string GameHandler(ArchiveEditor archiveEditor)
        {
            Game game = archiveEditor.archive.game;
            switch (game)
            {
                case Game.Scooby:
                case Game.BFBB:
                case Game.ROTU:
                    return game.ToString();
                case Game.RatProto:
                    return "Ratatouille";
                default:
                    switch (ArchiveEditor.GetGameFromGameConfigIni(archiveEditor.GetCurrentlyOpenFileName()))
                    {
                        case 3:
                            return "TSSM";
                        case 4:
                            return "Incredibles";
                        default:
                            return "Unknown";
                    }
            }
        }

        public static void timer1_Tick(object sender, EventArgs e)
        {
            // timer that elapses every second to check for active window updates
            // and adjust presence if needed
            timer1.Stop();
            GetActiveWindow();
            timer1.Start();
        }

        public static void GetActiveWindow()
        {
            var activeEditor = archiveEditors.Where(x => x.ContainsFocus).FirstOrDefault();
            var activeEditorName = activeEditor?.Text;
            if (activeEditorName != null)
            {
                // ensures that it only updates to archives
                if (activeEditorName.EndsWith(".hip", StringComparison.InvariantCultureIgnoreCase) || activeEditorName.EndsWith(".hop", StringComparison.InvariantCultureIgnoreCase))
                {
                    SetPresence(activeEditor);
                }
            }
            else if (archiveEditors.Count == 0)
                SetPresence(idling: true);
        }
    }
}
