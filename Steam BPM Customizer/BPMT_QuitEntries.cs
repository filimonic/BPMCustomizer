using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Steam_BPM_Customizer
{
    class BPMT_QuitEntries 
    {

        public static byte[] Transform(byte[] dataIn, SteamItemSettings[] _unused, string steamPath) {
            //Translate data to XML:
            SteamItemSettings[] settingList = BPM_OptionsManager.GetSettings().ToArray();
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
            Log(String.Format("Looking for node with [{0}]", entryOnActivateSubstring));
            XmlNode buttonNode = document.SelectSingleNode (@"//Button[contains(@onactivate,'" + entryOnActivateSubstring + "')]");
            if (buttonNode == null)
            {
                Log("No such node found");
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
                case SteamItemSettings.No_QuitMenu_LogOff :
                    return "ChangeUser()";
                case SteamItemSettings.No_QuitMenu_ExitBPM :
                    return "QuitApp()";
                case SteamItemSettings.No_QuitMenu_ExitSteam :
                    return "ExitSteam()";
                case SteamItemSettings.No_QuitMenu_Restart:
                    return "RestartMachine()";
                case SteamItemSettings.No_QuitMenu_Settings:
                    return "ShowSettingsFromQuitEntries()";
                case SteamItemSettings.No_QuitMenu_Shutdown:
                    return "ShutdownMachine()";
                case SteamItemSettings.No_QuitMenu_Sleep:
                    return "SuspendMachine()";
                case SteamItemSettings.No_QuitMenu_MinimizeBigBicture:
                    return "MinimizeApp()";
#if _DEVELOPEMENT_MODE_
                case SteamItemSettings.NoHdmiInput :
                    return "SwitchToHDMIInput()";
                case SteamItemSettings.NoOffline :
                    return "GoOffline()";
                case SteamItemSettings.NoOnline :
                    return "GoOnline()";
                case SteamItemSettings.NoZT :
                    return "ZeroTracker()";
                case SteamItemSettings.NoTurnOffCont:
                    return "TurnOffActiveController()";
#endif
                default :
                    return null;
            }
        }

        private static void Log (string message) {
            BPM_Log.Log(Properties.Resources.REPORT_TRANSFORMER_QUITENTRIES_LOGPREFIX + message);
        }
    }
}
