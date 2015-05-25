using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Steam_BPM_Customizer
{
    class BPMT_QuitEntries 
    {

        public static byte[] Transform(byte[] dataIn, SteamItemSettings[] settingList, string steamPath) {
            //Translate data to XML:
            Log(Properties.Resources.REPORT_TRANSFORMER_ALL_TRANSFORMING_BEGIN);
            XmlDocument xmlData;
            try 
            {
                xmlData = new XmlDocument();
                xmlData.PreserveWhitespace = true;
                string xmlText = Encoding.UTF8.GetString(dataIn);
                xmlData.LoadXml(xmlText);
            }
            catch (Exception e)
            {
                Log(String.Format(Properties.Resources.ERROR_TRANSFORMER_ALL_ERROR_XML,e.Message));
                return null;
            }
            foreach (SteamItemSettings setting in settingList)
            {
                //Log(String.Format("Processing setting {0}",setting.ToString("G")));
                bool processingResult = DisableEntry(xmlData, setting);
                if (processingResult) {
                    //Log("OK");
                } else {
                    //Log(String.Format(Properties.Resources.REPORT_TRANSFORMER_ALL_UNABLE_TO_PROCESS_SETTING,setting.ToString("G")));
                    //Log("Err");
                }
            }
            Log(Properties.Resources.REPORT_TRANSFORMER_ALL_TRANSFORMING_END);
            return Encoding.UTF8.GetBytes(xmlData.OuterXml);
        }
        
        private static bool DisableEntry(XmlDocument document, SteamItemSettings entryType) 
        {
            string entryOnActivateSubstring = GetEntryMark(entryType);
            if (entryOnActivateSubstring == null) 
            {
              //  Log("No such entry type in this transformer!");
                return false;
            }
            //Log(String.Format("Looking for node with [{0}]", entryOnActivateSubstring));
            XmlNode buttonNode = document.SelectSingleNode (@"//Button[contains(@onactivate,'" + entryOnActivateSubstring + "')]");
            if (buttonNode == null)
            {
                //Log("No such node found");
                return false;
            }
            XmlAttribute styleAttribute = document.CreateAttribute("style");
            styleAttribute.Value = "visibility: collapse;"; 
            buttonNode.Attributes.Append(styleAttribute);
            Log(String.Format(Properties.Resources.REPORT_TRANSFORMER_QUITENTRIES_NODE_DISABLED, entryType.ToString("G")));
            return true;
        }

        private static string GetEntryMark(SteamItemSettings entryType)
        {
            switch (entryType)
            {
                case SteamItemSettings.NoChangeUser :
                    return "ChangeUser()";
                case SteamItemSettings.NoExitBPM :
                    return "QuitApp()";
                case SteamItemSettings.NoExitSteam :
                    return "ExitSteam()";
                case SteamItemSettings.NoHdmiInput :
                    return "SwitchToHDMIInput()";
                case SteamItemSettings.NoOffline :
                    return "GoOffline()";
                case SteamItemSettings.NoOnline :
                    return "GoOnline()";
                case SteamItemSettings.NoZT :
                    return "ZeroTracker()";
                case SteamItemSettings.NoRestart :
                    return "RestartMachine()";
                case SteamItemSettings.NoSettingsQuitMenu :
                    return "ShowSettingsFromQuitEntries()";
                case SteamItemSettings.NoShutdown:
                    return "ShutdownMachine()";
                case SteamItemSettings.NoSleep:
                    return "SuspendMachine()";
                case SteamItemSettings.NoTurnOffCont:
                    return "TurnOffActiveController()";
                default :
                    return null;
            }
        }

        private static void Log (string message) {
            Console.WriteLine(Properties.Resources.REPORT_TRANSFORMER_QUITENTRIES_LOGPREFIX + message);
        }
    }
}
