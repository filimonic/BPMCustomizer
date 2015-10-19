using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Reflection;
//using System.Windows.Forms;
using System.Threading;

namespace Steam_BPM_Customizer
{

    public enum BPM_Updater_UpdateStatus
    {
        UNKNOWN,
        UPDATE_AVAILABLE,
        UPDATE_ERROR_STEAM_RUNNING,
        UPDATE_ERROR_NOT_ENABLED,
        UPDATE_ERROR_NOT_YET,
        UPDATE_NOT_AVAILABLE,
        UPDATE_ERROR_INTERNAL,
        UPDATE_APPLIED_OK,
        UPDATE_APPLIED_ERROR
    }

    class BPM_Updater
    {
        private static string _updateXml = @"http://fil.guru/projects/BPMCustomizer/update.xml";
        private static int _updateCheckInterval = 30; //min
        private System.Threading.Timer _timer;
        private static DateTime _lastUpdateCheckTime = DateTime.MinValue;
        private static BPM_Updater_UpdateStatus _lastUpdateCheckStatus = BPM_Updater_UpdateStatus.UNKNOWN;
        

        public static BPM_Updater_UpdateStatus CheckUpdate(bool forceCheck = false)
        {
            String newVersionUrl;
            BPM_Updater_UpdateStatus result;
            result = CheckUpdate(out newVersionUrl, forceCheck);
            _lastUpdateCheckTime = DateTime.Now;
            _lastUpdateCheckStatus = result;
            newVersionUrl = null;
            return result;
        }

        private static BPM_Updater_UpdateStatus CheckUpdate(out string updatePath,bool forceCheck = false)
        {
            updatePath = string.Empty;
            string checkUrl = _updateXml;
            if (SteamBPMCustomizer.IsSteamClientRunning == true)
            {
                if (forceCheck)
                {
                    BPM_Log.Log("Will not check updates while Steam is running");
                }
                return BPM_Updater_UpdateStatus.UPDATE_ERROR_STEAM_RUNNING;
            }
            if (!BPM_OptionsManager.IsSettingEnabled(SteamItemSettings.Program_EnableAutoUpdate) && forceCheck == false)
            {
                return BPM_Updater_UpdateStatus.UPDATE_ERROR_NOT_ENABLED;
            }
            if ((SteamBPMCustomizer.ApplicationUptime <  TimeSpan.FromMinutes(5)) && (BPM_OptionsManager.IsSettingEnabled(SteamItemSettings.Program_DelayAutoUpdateAfterStart) ) && forceCheck == false)
            {
                BPM_Log.Log("Will not check updates on start");
                return BPM_Updater_UpdateStatus.UPDATE_ERROR_NOT_YET;
            }
            BPM_Log.Log("Checking for update... " );
            try {
                XmlDocument doc = new XmlDocument();
                doc.Load(checkUrl);
                XmlElement root = doc.DocumentElement;
                XmlNode latestVersionNode = root.SelectSingleNode(@"//latest_version[@updater_version=1]");
                Version latestVesrionNumber = Version.Parse(latestVersionNode.Attributes[@"version"].Value);
                string  latestVersionFileUrl = latestVersionNode.InnerText;
                BPM_Log.Log("Local  version is " + SteamBPMCustomizer.ApplicationVersion);
                BPM_Log.Log("Version on server is: " + latestVesrionNumber.ToString());
                BPM_Log.Log("URL of server version is: "+ latestVersionFileUrl);
                    
                if (latestVesrionNumber > SteamBPMCustomizer.ApplicationVersion)
                {
                    updatePath = latestVersionFileUrl.Trim();
                    BPM_Log.Log("Update is available! [" + updatePath + "]");
                    return BPM_Updater_UpdateStatus.UPDATE_AVAILABLE;
                } else
                {
                    BPM_Log.Log("Update is not available");
                    return BPM_Updater_UpdateStatus.UPDATE_NOT_AVAILABLE;
                }

            } catch (Exception e)
            {
                BPM_Log.Log("Internal update check error: " +  e.Message);
                return BPM_Updater_UpdateStatus.UPDATE_ERROR_INTERNAL;
            }
        }

        public static BPM_Updater_UpdateStatus PerformUpdate()
        {
            BPM_Log.Log("Starting uopdate...");
            string programPath = Assembly.GetEntryAssembly().Location;
            string tempProgramPath = programPath + @".update_process";
            string newProgramPath;
            string newVersionUrl;
            BPM_Updater_UpdateStatus checkResult;
            checkResult = CheckUpdate( out newVersionUrl, true);
            if (checkResult != BPM_Updater_UpdateStatus.UPDATE_AVAILABLE)
            {
                BPM_Log.Log("Problems while downloading update");
                return checkResult;
            }
            
            CleanupUpdateFile();
            if (DownloadNewExecutable(newVersionUrl, out newProgramPath))
            {
                if(!ApplyUpdate(tempProgramPath, newProgramPath))
                {
                    BPM_Log.Log("Unable to apply update.");
                    return BPM_Updater_UpdateStatus.UPDATE_APPLIED_ERROR;
                }
                BPM_Log.Log("Update applied");
                return BPM_Updater_UpdateStatus.UPDATE_APPLIED_OK;
            } else
            {
                BPM_Log.Log("Unable to download update.");
            }
            return BPM_Updater_UpdateStatus.UPDATE_APPLIED_ERROR;
        }
       
        private static bool ApplyUpdate(string localOldPath, string localNewPath)
        {
            BPM_Log.Log("Applying update...");
            string currentPath = Assembly.GetEntryAssembly().Location;
            try
            {
                BPM_Log.Log("Move original file to new location");
                File.Move(currentPath, localOldPath);
            } catch (Exception e)
            {
                BPM_Log.Log("Error renaming current file: "+ e.Message);
                return false;
            }
            try
            {
                BPM_Log.Log("Move new file to rigth location");
                File.Move(localNewPath, currentPath);
            } catch (Exception e)
            {
                BPM_Log.Log("Error moving temp file to old path: " + e.Message);
                try
                {
                    BPM_Log.Log("Roling back...");
                    File.Move(localOldPath, currentPath);
                    BPM_Log.Log("Rolled back!");
                } catch
                {
                    BPM_Log.Log("Unable to roll back");
                    //MessageBox.Show("Error while update! Please reinstall BPMCustomizer!");
                }
            }
            BPM_Log.Log("Restarting app...");
            SteamBPMCustomizer.RestartApp();
            return true;

            
        }
        private static bool DownloadNewExecutable(string url, out string newProgramPath)
        {
            BPM_Log.Log("Downloading update");
            newProgramPath = string.Empty;
            string tempFilePath = Path.GetTempFileName();
            WebClient wc = new WebClient();
            try
            {
                wc.DownloadFile(url, tempFilePath);
                BPM_Log.Log("File downloaded to [" + tempFilePath + "]");
            } catch (Exception e)
            {
                BPM_Log.Log("Error downloading file: " + e.Message);
                return false;
            }
            newProgramPath = tempFilePath;
            return true;
            
        }
        public static void CleanupUpdateFile()
        {
            string programPath = Assembly.GetEntryAssembly().Location;
            string tempProgramPath = programPath + @".update_process";
            if (File.Exists(tempProgramPath))
            {
                try
                {
                    File.Delete(tempProgramPath);
                    BPM_Log.Log("Pre-Update application file is deleted [" + tempProgramPath + "]");
                } catch (Exception e)
                {
                    BPM_Log.Log("Error cleanup old file: " + e.Message);
                }
            } else
            {
                BPM_Log.Log("No pre-update application file on path [ " + tempProgramPath + "]... OK!");
            }
            
        }

        public static string UpdateStatusToString(BPM_Updater_UpdateStatus s)
        {
            switch (s) {
                case BPM_Updater_UpdateStatus.UPDATE_AVAILABLE:
                    return "Update is available.";
                case BPM_Updater_UpdateStatus.UPDATE_NOT_AVAILABLE:
                    return "You have latest version. No updates yet.";
                case BPM_Updater_UpdateStatus.UPDATE_ERROR_STEAM_RUNNING:
                    return "Steam is running, can not perform update.";
                case BPM_Updater_UpdateStatus.UPDATE_ERROR_NOT_YET:
                    return "Startup pre-update timeout is not passed yet.";
                case BPM_Updater_UpdateStatus.UPDATE_ERROR_INTERNAL:
                    return "Internal update error.";
                case BPM_Updater_UpdateStatus.UPDATE_ERROR_NOT_ENABLED:
                    return "Updates are not enabled.";
                case BPM_Updater_UpdateStatus.UPDATE_APPLIED_OK:
                    return "Update applied.";
                case BPM_Updater_UpdateStatus.UPDATE_APPLIED_ERROR:
                    return "Error while performing update.";
                default:
                    return "Status unknown. Something wrong.";

            }
        }

        public static void RestartApp()
        {
            SteamBPMCustomizer.RestartApp();
        }

        public void Start()
        {
            double timerInterval_big = TimeSpan.FromMinutes(_updateCheckInterval).TotalMilliseconds;
            int timerInterval = 99999;
            if (timerInterval_big > (double)(int.MaxValue))
            {
                timerInterval = int.MaxValue;
            } else
            {
                timerInterval = (int)(timerInterval_big);
            }
            BPM_Log.Log("Update check interval is " + timerInterval / 1000 + " seconds") ;
            _timer = new System.Threading.Timer(new TimerCallback(OnTimer), null, timerInterval, timerInterval);
            BPM_Log.Log("Updater thread started");
        }

        private void OnTimer(object state)
        {
            if (BPM_OptionsManager.IsSettingEnabled(SteamItemSettings.Program_EnableAutoUpdate))
            {
                BPM_Log.Log("Auto update checking...");
                if (CheckUpdate() == BPM_Updater_UpdateStatus.UPDATE_AVAILABLE)
                {
                    PerformUpdate();
                }
            }
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            BPM_Log.Log("Updater thread is stopped");
        }

        public static BPM_Updater_UpdateStatus LastUpdateCheckStatus
        {
            get
            {
                return _lastUpdateCheckStatus;
            }
        }

        public static DateTime LastUpdateCheckTime
        {
            get
            {
                return _lastUpdateCheckTime;
            }
        }

    }
}
