using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Microsoft.Win32;

namespace Steam_BPM_Customizer
{
    class BPM_OptionsManager
    {
        public static event EventHandler SettingsChanged;
        public static List<SteamItemSettings> GetSettings()
        {
            List<SteamItemSettings> settingsList = new List<SteamItemSettings>();
            foreach (String settingName in Enum.GetNames(typeof(SteamItemSettings)))
            {
                
                if (IsSettingEnabled(settingName))
                {
                    SteamItemSettings setting;
                    if (Enum.TryParse<SteamItemSettings>(settingName, out setting))
                    {
                        settingsList.Add(setting);
                    }
                }
            }
            return settingsList;
        }

        public static bool IsSettingEnabled(SteamItemSettings setting)
        {
            return IsSettingEnabled(Enum.GetName(typeof(SteamItemSettings),setting));
        }

        public static bool IsSettingEnabled(String settingName)
        {
            int value;
            try {
                value = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\BPMCustomizer", settingName, 0);
            }
            catch
            {
                BPM_Log.Log("Error parsing " + settingName);
                return false;
            }

            if (value == 0)
            {
               // Console.WriteLine("Setting " + settingName + " is disabled");
                return false;
            } else {
                SteamItemSettings setting;
                if (Enum.TryParse<SteamItemSettings>(settingName,out setting)) {
                    //BPM_Log.Log("Setting " + settingName + " is enabled");
                    return true;
                }
                else
                {
                    //Console.WriteLine("Setting " + settingName + " is not a member of enum");
                    return false;
                }
            }
        }

        public static void SetSetting(SteamItemSettings setting, bool newState)
        {
            bool currentState = IsSettingEnabled(setting);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\BPMCustomizer", Enum.GetName(typeof(SteamItemSettings), setting), newState ? 1:0);
            if (currentState != newState)
            {
                FireEventSettingsChanged();
            }
            
        }

        public static void SetDefaults()
        {
            foreach (SteamItemSettings setting in Enum.GetValues(typeof(SteamItemSettings)))
            {
                if (!IsSettingEnabled(setting))
                {
                    bool newSettingDefaultValue = false;
                    switch (setting)
                    {
                        case SteamItemSettings.No_QuitMenu_Sleep:
                        case SteamItemSettings.No_QuitMenu_LogOff:
                        case SteamItemSettings.No_QuitMenu_Shutdown:
                        case SteamItemSettings.No_QuitMenu_ExitBPM:
                        case SteamItemSettings.No_QuitMenu_Restart:
                        case SteamItemSettings.Program_EnableAutoUpdate:
                        case SteamItemSettings.Program_DelayAutoUpdateAfterStart:
                            newSettingDefaultValue = true;
                            break;
                    }
                    BPM_OptionsManager.SetSetting(setting, newSettingDefaultValue);
                }
            }

            BPM_OptionsManager.SetSetting(SteamItemSettings.Program_FirstRunPassed_v0050, true);
            BPM_Log.Log("Defaults set!");

        }
        private static void FireEventSettingsChanged()
        {
            EventHandler handler = SettingsChanged;
            if (null != handler)
            {
                handler(null, EventArgs.Empty);
            }

        }
    }

    
}
