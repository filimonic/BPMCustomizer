using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using Microsoft.Win32;
using System.Xml;
using System.Security.Permissions;
using System.Resources;
using System.Diagnostics;

namespace Steam_BPM_Customizer
{
    struct FileData
    {
        public byte[] OriginalData;
        public byte[] TransformedData;
        public Func<byte[], SteamItemSettings[],string, byte[] > TransformFunction;
    }

    enum SteamItemSettings 
    {
        NoHdmiInput, //noHdmiInput
        NoExitBPM, //noExitBPM
        NoExitSteam, //noExitSteam
        NoTurnOffCont, //noTurnOffCont
        NoZT, //noZT
        NoSettingsQuitMenu, //noSettings
        NoChangeUser, //noChangeUser
        NoOffline, //noOffline 
        NoOnline, //noOnline
        NoShutdown, //noShutdown
        NoRestart, //noRestart
        NoSleep, //noSleep
        NoProfileIcon, //NoProfileIcon
        NoSettingsMainMenu, //NoSettingsMainMenu
        NoUpperExitMainMenu, //NoSettingsMainMenu
        NoBetaButtonMainMenu, //NoBetaButtonMainMenu
        NoParentalLockButtonMainMenu, //NoParentalLockButtonMainMenu
        AddQuitEntryMainMenu, //AddQuitEntryMainMenu
        AddSleepEntryMainMenu, //AddSleepEntryMainMenu
        AddShutdownEntryMainMenu, //AddShutdownEntryMainMenu
        AddRestartEntryMainMenu, //AddRestartEntryMainMenu
        AddUpperStoreMainMenu, //AddUperStoreMainMenu
        HideConsole, //hideConsole
        NoBrowserNavigation, //NoBrowserNavigation
        NoStoreButtonMainMenu, //NoStoreButtonMainMenu
        GoToLibraryOnStart,//GoToLibraryOnStart
        FMode, //FMode
    }

    class SteamBPMCustomizer
    {
        private FileSystemWatcher _fsw;
        private string _steamPath;
        private bool _isSteamRunning;
        //DEPRECATED private BootStrapMonitor _bootstrapMonitor;
        private SteamBPMWindowHook _steamBpmHook;
        private const string STEAM_REGISTRY_KEY = @"Software\Valve\Steam";
        private const string STEAM_REGISTRY_INSTALLATION_PATH_VALUE_NAME = @"SteamPath";
        private const string STEAM_BPM_WINDOW_CLASS_NAME = "CUIEngineWin32";
        private Dictionary<string,FileData> _filesToWatch;
        private List<SteamItemSettings> _settings;
        public static void Main(string[] args)
        {
            Log(Properties.Resources.APPLICATION_LOGO);
            if (!IsApplicationFirstInstance)
            {
                Log(Properties.Resources.ERROR_ANOTHER_INSTANCE_ALREADY_RUNNING);
                return;
            }
            string steamPath = GetSteamPath();
            if (args.Length <= 0) {
                ShowHelp();
                Environment.Exit(0);
                return;
            }
            List<SteamItemSettings> settings = ProcessCliArgs(args);
            if (settings.Contains(SteamItemSettings.HideConsole)) {
                ConsoleManager.HideConsole();
            }
            SteamBPMCustomizer bpmCustomizer = new SteamBPMCustomizer(steamPath, settings);
            bpmCustomizer.Start();
            while (true) {
                Thread.Sleep(Timeout.Infinite);
            }
            
        }

        private static bool IsApplicationFirstInstance
        {
            get
            {
                try 
                {
                    if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
                    {
                        return false;
                    } else {
                        return true;
                    }
                } 
                catch (Exception e)
                {
                    Log(String.Format(Properties.Resources.ERROR_UNABLE_TO_DETECT_SINGLE_PROCESS_CHECK,e.Message));
                    return false;
                }
            }
        }

        public SteamBPMCustomizer(string steamPath, List<SteamItemSettings> settings)
        {
            IsSteamRunning = false;
            _steamPath = steamPath;
            _settings = settings;
            _steamBpmHook = new SteamBPMWindowHook(STEAM_BPM_WINDOW_CLASS_NAME);
            _steamBpmHook.StateChanged += new EventHandler<int>(OnSteamWindowStateChanged);
            //DEPRECATED _bootstrapMonitor = new BootStrapMonitor(Path.GetFullPath(Path.Combine(_steamPath, @"logs\bootstrap_log.txt")));
            //DEPRECATED _bootstrapMonitor.LineAdded += new EventHandler<string>(OnBoostrapLogChanged);
            
        }

        private bool IsSteamRunning
        {
            get 
            {
                return _isSteamRunning;
            }
            set
            {
                _isSteamRunning = value;
            }
            
        }

        private bool IsFileWatched(string path)
        {
            string correctPath = Path.GetFullPath(path);
            if (_filesToWatch.ContainsKey(path))
            {
                return true;
            }
            return false;
        }

        private static void CompleteSettings(ref List<SteamItemSettings> settingsList)
        {
            if (settingsList.Contains(SteamItemSettings.GoToLibraryOnStart))
            {
                AddSettingParameter(ref settingsList, SteamItemSettings.NoStoreButtonMainMenu);
                AddSettingParameter(ref settingsList, SteamItemSettings.AddUpperStoreMainMenu);
            }
            if (settingsList.Contains(SteamItemSettings.FMode))
            {
                AddSettingParameter(ref settingsList, SteamItemSettings.NoStoreButtonMainMenu);
                AddSettingParameter(ref settingsList, SteamItemSettings.AddQuitEntryMainMenu);
                AddSettingParameter(ref settingsList, SteamItemSettings.AddUpperStoreMainMenu);
                AddSettingParameter(ref settingsList, SteamItemSettings.NoBetaButtonMainMenu);
                AddSettingParameter(ref settingsList, SteamItemSettings.NoSettingsQuitMenu);
                AddSettingParameter(ref settingsList, SteamItemSettings.NoProfileIcon);
            }
        }

        private static void AddSettingParameter(ref List<SteamItemSettings> settingsList, SteamItemSettings setting )
        {
            if (!settingsList.Contains(setting))
            {
                settingsList.Add(setting);
            }
        }

        private static List<SteamItemSettings> ProcessCliArgs(string[] args)
        {
            List<SteamItemSettings> cliSettings;
            cliSettings = new List<SteamItemSettings>();

            for (int i = 0; i < args.Length; i++)
            {
                SteamItemSettings itemSettingName;
                if (Enum.TryParse<SteamItemSettings>(args[i], true, out itemSettingName))
                {
                    AddSettingParameter(ref cliSettings, itemSettingName);
                    //Log(String.Format(Properties.Resources.REPORT_CMDLINE_PARAMETER_ACCEPTED, itemSettingName.ToString("G")));
                }
                else
                {
                    Log(String.Format(Properties.Resources.REPORT_CMDLINE_PARAMETER_UNKNOWN, args[i]));
                    ShowHelp();
                    Environment.Exit(0);
                    return null;
                }
            }
            CompleteSettings(ref cliSettings);
            return cliSettings;
        }

        private static string GetSteamPath()
        {
            RegistryKey steamRegistryKey = Registry.CurrentUser.OpenSubKey(STEAM_REGISTRY_KEY, false);
            if (steamRegistryKey == null)
            {
                throw new Exception(Properties.Resources.ERROR_REGISTRY_OPEN_STEAM_REGISTY_KEY);
            }
            string steamPath = (string)steamRegistryKey.GetValue(STEAM_REGISTRY_INSTALLATION_PATH_VALUE_NAME);
            if (steamPath == null)
            {
                throw new Exception(Properties.Resources.ERROR_REGISTRY_READ_STEAM_PATH);
            }
            steamPath = Path.GetFullPath(steamPath);
            Log(string.Format(Properties.Resources.REPORT_STEAM_PATH, steamPath));

            steamRegistryKey.Close();
            return steamPath;
        }


        private void AddFileToWatch(string path, Func<byte[],SteamItemSettings[],string,byte[]> transformFunction)
        {

            if (IsFileWatched(path))
            {
                return;
            }
            Byte[] originalFileData;
            if (File.Exists(path))
            {
                try
                {
                    originalFileData = File.ReadAllBytes(path); //Check for reading
                    File.WriteAllBytes(path, originalFileData); //Chech for writing
                }
                catch (Exception e)
                {
                    Log(string.Format(Properties.Resources.ERROR_FILE_ADDING_TO_WATCH_IOERROR, path, e.Message));
                    return;
                }
                //TODO
                FileData fileData = new FileData();
                fileData.TransformFunction = transformFunction;
                _filesToWatch.Add(path,fileData);
                UpdateFileData(path);
                Log(String.Format(Properties.Resources.REPORT_FILE_ADDED_TO_WATCH, path));
            } 
            else 
            {
                throw new Exception(string.Format(Properties.Resources.ERROR_FILE_ADDING_TO_WATCH_NOTEXIST, path));
            }
        }

        private void UpdateAllFileData()
        {
            Log(Properties.Resources.REPORT_UPDATEALL_STARTED);
            foreach (string path in _filesToWatch.Keys)
            {
                UpdateFileData(path);
            }
            Log(Properties.Resources.REPORT_UPDATEALL_FINISHED);
        }

        private void UpdateFileData(string path)
        {
            if (!IsFileWatched(path)) 
            {
                Log(String.Format(Properties.Resources.REPORT_FILE_IS_NOT_WATCHED, path));
                return;
            }
            byte[] originalFileData = File.ReadAllBytes(path);
            FileData fileData;
            fileData = _filesToWatch[path];
            fileData.OriginalData = originalFileData;
            fileData.TransformedData = fileData.TransformFunction(originalFileData, _settings.ToArray(),_steamPath);
            _filesToWatch[path] = fileData;
            Log(String.Format(Properties.Resources.REPORT_FILE_UPDATED,path));
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private void Start()
        {
            _filesToWatch = new Dictionary<string, FileData>(StringComparer.OrdinalIgnoreCase);

            foreach (SteamItemSettings settingsItem in _settings)
            {
                switch (settingsItem)
                {
                    case SteamItemSettings.NoChangeUser :
                    case SteamItemSettings.NoExitBPM :
                    case SteamItemSettings.NoExitSteam:
                    case SteamItemSettings.NoHdmiInput:
                    case SteamItemSettings.NoOffline:
                    case SteamItemSettings.NoZT:
                    case SteamItemSettings.NoRestart:
                    case SteamItemSettings.NoSettingsQuitMenu:
                    case SteamItemSettings.NoShutdown:
                    case SteamItemSettings.NoSleep:
                    case SteamItemSettings.NoTurnOffCont:
                        AddFileToWatch(Path.GetFullPath(Path.Combine(_steamPath, Properties.Resources.SETTING_FILE_QUITENTRIES)), BPMT_QuitEntries.Transform);
                        break;
                    case SteamItemSettings.NoOnline:
                        AddFileToWatch(Path.GetFullPath(Path.Combine(_steamPath, Properties.Resources.SETTING_FILE_QUITENTRIES)), BPMT_QuitEntries.Transform);
                        AddFileToWatch(Path.GetFullPath(Path.Combine(_steamPath, Properties.Resources.SETTING_FILE_MAINMENU)), BPMT_MainMenu.Transform);
                        break;
                    case SteamItemSettings.NoParentalLockButtonMainMenu:
                    case SteamItemSettings.NoProfileIcon:
                    case SteamItemSettings.NoSettingsMainMenu:
                    case SteamItemSettings.AddQuitEntryMainMenu:
                    case SteamItemSettings.AddSleepEntryMainMenu:
                    case SteamItemSettings.AddShutdownEntryMainMenu:
                    case SteamItemSettings.AddRestartEntryMainMenu:
                    case SteamItemSettings.AddUpperStoreMainMenu:
                    case SteamItemSettings.GoToLibraryOnStart:
                    case SteamItemSettings.NoStoreButtonMainMenu:
                        AddFileToWatch(Path.GetFullPath(Path.Combine(_steamPath, Properties.Resources.SETTING_FILE_MAINMENU)), BPMT_MainMenu.Transform);
                        break;
                    case SteamItemSettings.NoBrowserNavigation: //This is separate because many will be added
                        AddFileToWatch(Path.GetFullPath(Path.Combine(_steamPath, Properties.Resources.SETTING_FILE_MAINMENU)), BPMT_MainMenu.Transform);
                        break;
                }
            }


            _fsw = new FileSystemWatcher();
            _fsw.Path = _steamPath;
            _fsw.IncludeSubdirectories = true;
            _fsw.NotifyFilter = NotifyFilters.LastWrite;
            _fsw.Changed += new FileSystemEventHandler(OnFileChanged);
            _fsw.Created += new FileSystemEventHandler(OnFileChanged);
            _fsw.Deleted += new FileSystemEventHandler(OnFileChanged);
            _fsw.Renamed += new RenamedEventHandler(OnFileRenamed);
            Log(Properties.Resources.REPORT_WATCHER_STARTED);
            //DEPRECATED _bootstrapMonitor.Start();
            _steamBpmHook.Start();
            _fsw.EnableRaisingEvents = true;


        }

        private void WriteAllFiles(bool transformedState = true)
        {
            foreach (string filePath in _filesToWatch.Keys )
            {
                if (transformedState)
                {
                    ReturnFileToTransformedState(filePath);
                }
                else
                {
                    ReturnFileToOriginalState(filePath);
                }
            }
        }

        private void OnSteamWindowStateChanged(object source, int changeType)
        {
            if (changeType == SteamBPMWindowHook.CREATED)
            {
                Log(Properties.Resources.REPORT_STEAM_WINDOW_DETECTED_CREATED);
                IsSteamRunning = true;
                WriteAllFiles(true);
            }
            else if (changeType == SteamBPMWindowHook.DESTROYED)
            {
                Log(Properties.Resources.REPORT_STEAM_WINDOW_DETECTED_DESTROYED);
                IsSteamRunning = false;
                WriteAllFiles(false);
            }
            else
            {
                Log(String.Format(Properties.Resources.ERROR_STEAM_WINDOW_DETECTED_UNKNOWN_STATE, changeType));
            }
        }

        /* DEPRECATED
        private void OnBoostrapLogChanged(object source, string lastLine)
        {
            //Log("Bootstrap change detected");
            if (lastLine.Contains("] Startup - "))
            {
                Log("Steam startup detected");
            } 
            else if (lastLine.Contains("] Verification complete")) 
            {
                Log("Steam verification end detected");
                _isVerificationCompleted = true;
                WriteAllFiles(true);

            } 
            else if (lastLine.Contains("] Shutdown")) 
            {
                Log("Steam shutdown detected");
                WriteAllFiles(false);
                _isVerificationCompleted = false;
            }
            else
            {
             //   Log(string.Format("Other bootstrap event detected: {0}", lastLine));
            }


        }
        */
        private static void OnFileRenamed(object source, RenamedEventArgs e)
        {
            
            Log(string.Format(Properties.Resources.REPORT_FILE_DETECTED_RENAMED, e.OldFullPath, e.FullPath));
        }

        private void WriteFileData(string path, byte[] data)
        {

            Log(String.Format(Properties.Resources.REPORT_FILE_REWRITE_BEGIN, path));
            
            try
            {
                Log(Properties.Resources.REPORT_STOP_FILE_WATCHER);
                _fsw.EnableRaisingEvents = false;
                Thread.Sleep(100);
                Log(String.Format(Properties.Resources.REPORT_LOCK_FILE, path));
                FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);


                Log(String.Format(Properties.Resources.REPORT_FILE_WRTE_DATA, data.Length));
                fileStream.Write(data, 0, data.Length);
                fileStream.Flush();
                fileStream.Close();
                Log(Properties.Resources.REPORT_RESTORE_FILE_WATCHER);
                _fsw.EnableRaisingEvents = true;
                Log(String.Format(Properties.Resources.REPORT_FILE_WRTE_DATA_COMPLETE, path));
            }
            catch (Exception e)
            {
                Log(String.Format(Properties.Resources.ERROR_FILE_WRITE_FAILURE, path, e.InnerException.Message));
            }
        }

        private void ReturnFileToOriginalState(string path)
        {
            
            if (IsFileWatched(path))
            {
                Log(string.Format(Properties.Resources.REPORT_FILE_WRITE_ORIGINAL_STATE, path));
                WriteFileData(path, _filesToWatch[path].OriginalData);
            }
            else
            {
                Log(String.Format(Properties.Resources.REPORT_FILE_IS_NOT_WATCHED, path));
            }
        }

        private void ReturnFileToTransformedState(string path)
        {
            Log(string.Format(Properties.Resources.REPORT_FILE_WRITE_MODIFIED_STATE, path));
            if (IsFileWatched(path))
            {
                WriteFileData(path, _filesToWatch[path].TransformedData);
            }
            else
            {
                Log(String.Format(Properties.Resources.REPORT_FILE_IS_NOT_WATCHED, path));
            }
        }

        private void OnFileChanged(object source, FileSystemEventArgs e)
        {
            if (!IsFileWatched(e.FullPath))
            {
                return;
            }
            string correctPath = Path.GetFullPath(e.FullPath);
            Log(String.Format(Properties.Resources.REPORT_FILE_DETECTED_CHANGED, correctPath, e.ChangeType));
            if (!IsSteamRunning)
            {
                Log(String.Format(Properties.Resources.REPORT_FILE_UPDATING_ORIGINAL_DATA, correctPath));
                UpdateFileData(Path.GetFullPath(e.FullPath));
                return;
            }
            else
            {

                Log(String.Format(Properties.Resources.REPORT_APPLYING_TRANSFORMATION, correctPath));
                ReturnFileToTransformedState(e.FullPath);
            }
            
        }
        private void Loop()
        {

        }

        private static void Log(string message)
        {
            Console.WriteLine(message);
        }

        private static void ShowHelp()
        {
            Log(Properties.Resources.HELP_HEADER);
            foreach (SteamItemSettings setting in Enum.GetValues(typeof(SteamItemSettings)))
            {
                Log(string.Format(Properties.Resources.HELP_PARAMETER,setting.ToString("G") ,GetSettingDescriptionText(setting)));
            }
            Log(Properties.Resources.HELP_CMDLINE_OPTIONS_DESCRIPTION);
            Log(Properties.Resources.HELP_CMDLINE_OPTIONS_FOOTER);
        }

        private static string GetSettingDescriptionText(SteamItemSettings setting)
        {
            switch (setting)
            {
                case SteamItemSettings.NoBetaButtonMainMenu:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoBetaButtonMainMenu;
                case SteamItemSettings.NoBrowserNavigation:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoBrowserNavigation;
                case SteamItemSettings.NoChangeUser:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoChangeUser;
                case SteamItemSettings.NoExitBPM:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoExitBPM;
                case SteamItemSettings.NoExitSteam:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoExitSteam;
                case SteamItemSettings.NoHdmiInput:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoHdmiInput;
                case SteamItemSettings.NoOffline:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoOffline;
                case SteamItemSettings.NoOnline:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoOnline;
                case SteamItemSettings.NoParentalLockButtonMainMenu:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoParentalLockButtonMainMenu;
                case SteamItemSettings.NoProfileIcon:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoProfileIcon;
                case SteamItemSettings.NoRestart:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoRestart;
                case SteamItemSettings.NoSettingsMainMenu:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoSettingsMainMenu;
                case SteamItemSettings.NoSettingsQuitMenu:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoSettingsQuitMenu;
                case SteamItemSettings.NoShutdown:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoShutdown;
                case SteamItemSettings.NoSleep:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoSleep;
                case SteamItemSettings.NoTurnOffCont:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoTurnOffCont;
                case SteamItemSettings.NoUpperExitMainMenu:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoUpperExitMainMenu;
                case SteamItemSettings.NoZT:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoZT;
                case SteamItemSettings.AddQuitEntryMainMenu:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_AddQuitEntryMainMenu;
                case SteamItemSettings.AddRestartEntryMainMenu:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_AddRestartEntryMainMenu;
                case SteamItemSettings.AddShutdownEntryMainMenu:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_AddShutdownEntryMainMenu;
                case SteamItemSettings.AddSleepEntryMainMenu:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_AddSleepEntryMainMenu;
                case SteamItemSettings.AddUpperStoreMainMenu:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_AddUpperStoreMainMenu;
                case SteamItemSettings.NoStoreButtonMainMenu:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NoStoreButtonMainMenu;
                case SteamItemSettings.HideConsole:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_HideConsole;
                case SteamItemSettings.FMode:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_FMode;
                case SteamItemSettings.GoToLibraryOnStart:
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_GoToLibraryOnStart;

                default :
                    return Properties.Resources.HELP_PARAMETER_DESCRIPTION_NODESCRIPTION;
            }
        }
    }
}
