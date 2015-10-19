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
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.Media;

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

        No_QuitMenu_ExitBPM, //noExitBPM
        No_QuitMenu_ExitSteam, //noExitSteam

        No_QuitMenu_Settings, //noSettings
        No_QuitMenu_LogOff, //noChangeUser

        No_QuitMenu_Shutdown, //noShutdown
        No_QuitMenu_Restart, //noRestart
        No_QuitMenu_Sleep, //noSleep
        Program_SendUsageStatistics,
        Program_EnableAutoUpdate,
        Program_FirstRunPassed_v0050,
        Program_DelayAutoUpdateAfterStart,

#if _DEVELOPEMENT_MODE_
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
        NoCommunityMainScreen,//NoCommunityMainScreen
        FMode, //FMode
        HomeScreenHideProfileName,
        NoHdmiInput, //noHdmiInput
        NoTurnOffCont, //noTurnOffCont
        NoZT, //noZT
        NoOffline, //noOffline 
        NoOnline, //noOnline        
#endif


    }

    class SteamBPMCustomizer
    {
        private FileSystemWatcher _fsw;
        private string _steamPath;
        private static bool _isSteamRunning = false;
        //DEPRECATED private BootStrapMonitor _bootstrapMonitor;
        private SteamBPMWindowHook _steamBpmHook;
        private BPM_Updater _autoUpdater;
        private const string STEAM_REGISTRY_KEY = @"Software\Valve\Steam";
        private const string STEAM_REGISTRY_INSTALLATION_PATH_VALUE_NAME = @"SteamPath";
        private const string STEAM_BPM_WINDOW_CLASS_NAME = "CUIEngineWin32";
        private Dictionary<string,FileData> _filesToWatch;
        //private List<SteamItemSettings> _settings;
        private static FormLog _formLog;
        private static formOptions _formOptions;
        private static SteamBPMCustomizer _bpmCustomizerWorker;
        private static NotifyIcon _trayIcon;
        private static DateTime _applicationStartTime;
        //private static bool _isRestarting;
        

        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ApplicationExit += OnApplicationExit;
            _bpmCustomizerWorker = null;
            //_isRestarting = false;
            _applicationStartTime = DateTime.Now;
            IsSteamRunning = false;
            _formLog = new FormLog();
            _formOptions = new formOptions(); 
            _trayIcon = new NotifyIcon();
            InitTrayIcon();



            Log(Properties.Resources.APPLICATION_LOGO);
            for (int i = 60; i > 0; i--)
            {
                if (!IsApplicationFirstInstance)
                {
                    Log("This is not single instance of application. Will wait some time for old app to close if this is self-update...");
                    Thread.Sleep(500);
                }
            }
            if (!IsApplicationFirstInstance)
            {
                Log(Properties.Resources.ERROR_ANOTHER_INSTANCE_ALREADY_RUNNING);
                Thread.Sleep(1000);
                _trayIcon.Visible = false;
                Environment.Exit(2);
            }

            if (BPM_Updater.CheckUpdate() == BPM_Updater_UpdateStatus.UPDATE_AVAILABLE)
            {
                _trayIcon.BalloonTipText = "Updating BPMCustomizer";
                _trayIcon.ShowBalloonTip(1000);
                Thread.Sleep(1000);
                BPM_Updater.PerformUpdate();
                Environment.Exit(3);
            }

            string steamPath = String.Empty;
            try
            {
                steamPath = GetSteamPath();
            } catch
            {
                Log("Unable to read STEAM path from registry");
                Thread.Sleep(1000);
                _trayIcon.Visible = false;
                Environment.Exit(1);
            }

           List<SteamItemSettings> settings = BPM_OptionsManager.GetSettings();
           if (!settings.Contains(SteamItemSettings.Program_FirstRunPassed_v0050))
           {
                BPM_OptionsManager.SetDefaults();
                settings = BPM_OptionsManager.GetSettings();
                _trayIcon.BalloonTipTitle = @"BPM Customizer";
                _trayIcon.BalloonTipText = @"BPM Customizer is in notification area. Right click it to show more.";
                _trayIcon.ShowBalloonTip(1000);
           }
           foreach (SteamItemSettings set in settings.ToArray())
           {
                BPM_Log.Log("Setting enabled: " + Enum.GetName(typeof(SteamItemSettings),set));
           }

            _bpmCustomizerWorker = new SteamBPMCustomizer(steamPath, settings);
            _bpmCustomizerWorker.Start();
            Application.Run();
        }

        public static void RestartApp()
        {
            Log("[!!!]Restarting App!");
            Process.Start(Assembly.GetEntryAssembly().Location);
            //_isRestarting = true;
            trayIconMenu_Exit_Click(null, null);
        }

        public static TimeSpan ApplicationUptime
        {
            get
            {
                return (DateTime.Now - _applicationStartTime);
            }
        }

        public static bool IsSteamClientRunning
        {
            get
            {
                return IsSteamRunning;
            }
        }

        public static Version ApplicationVersion
        {
            get
            {
                return Version.Parse(Application.ProductVersion);
            }
        }


        private static void OnApplicationExit(object sender, EventArgs e)
        {
            BPM_Log.Log("Preparing to exit...");
            _formLog.Close();
            _formOptions.Close();
            if (_bpmCustomizerWorker != null)
            {
                _bpmCustomizerWorker.Stop();
            } 
            BPM_Log.Log("Exiting");
            //Thread.Sleep(_isRestarting?0:5000);
            _trayIcon.Visible = false;
            Environment.Exit(0);
        }

        private static void InitTrayIcon()
        {

            _trayIcon = new NotifyIcon();
            ContextMenu trayIconMenu = new ContextMenu();

            MenuItem trayIconMenu_VersionInfo = new MenuItem();
            trayIconMenu_VersionInfo.Text = Application.ProductName + " (v" + Application.ProductVersion + ")";
            trayIconMenu_VersionInfo.Index = 3;
            trayIconMenu_VersionInfo.Enabled = false;
            
          //  trayIconMenu_VersionInfo.Click += new EventHandler(trayIconMenu_Log_Click);

            trayIconMenu.MenuItems.Add(trayIconMenu_VersionInfo);
            trayIconMenu.MenuItems.Add("Author's home page ...", trayIconMenu_AuthorsHomePage_Click);
            trayIconMenu.MenuItems.Add("-");
            //trayIconMenu.MenuItems.Add("Log ...", trayIconMenu_Log_Click);
            trayIconMenu.MenuItems.Add("Settings ...", trayIconMenu_Options_Click);
            trayIconMenu.MenuItems.Add("Exit BPM Customizer", trayIconMenu_Exit_Click);
            _trayIcon.Icon = Properties.Resources._48ico;
            _trayIcon.Text = Application.ProductName + " (v" + Application.ProductVersion + ")";
            _trayIcon.ContextMenu = trayIconMenu;
            _trayIcon.DoubleClick += new EventHandler(trayIconMenu_Options_Click);

            //_trayIcon.Click += new EventHandler(trayMenu_Click);
            _trayIcon.Visible = true;
        }

        private static void trayIconMenu_Exit_Click(object Sender, EventArgs e)
        {
            BPM_Log.Log("Exiting fired");
            //_trayIcon.ContextMenu = null;
            _formLog.Dispose();
            _formOptions.Dispose();
            Application.Exit();
            
            //Environment.Exit(0);
        }

        private static void trayIconMenu_AuthorsHomePage_Click(object Sender, EventArgs e)
        {
            //MessageBox.Show("HEHE");
            System.Diagnostics.Process.Start(@"http://fil.guru");
            //_formLog.Log(BPM_Log.GetLog());
        }

        private static void trayIconMenu_Log_Click(object Sender, EventArgs e)
        {
            //MessageBox.Show("HEHE");
            _formLog.Show();
            //_formLog.Log(BPM_Log.GetLog());
        }

        private static void trayIconMenu_Options_Click(object Sender, EventArgs e)
        {
            
            //MessageBox.Show("HEHE");
            _formOptions.Show();
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
            
            _steamPath = steamPath;
            //_settings = settings;
            Log("Steam running?" + IsSteamClientRunning);
            _steamBpmHook = new SteamBPMWindowHook(STEAM_BPM_WINDOW_CLASS_NAME);
            _steamBpmHook.StateChanged += new EventHandler<int>(OnSteamWindowStateChanged);
            _autoUpdater = new BPM_Updater();


            BPM_OptionsManager.SettingsChanged += OnSettingsChanged;
            //DEPRECATED _bootstrapMonitor = new BootStrapMonitor(Path.GetFullPath(Path.Combine(_steamPath, @"logs\bootstrap_log.txt")));
            //DEPRECATED _bootstrapMonitor.LineAdded += new EventHandler<string>(OnBoostrapLogChanged);
        }

        private void OnSettingsChanged(Object sender, EventArgs e)
        {
            //TODO
            UpdateAllFileData();
        }

        private static bool IsSteamRunning
        {
            get 
            {
                Log("[int]Checked is steam running: " + _isSteamRunning);
                return _isSteamRunning;
            }
            set
            {
                Log("[int]Set steam running state: " + value);
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

        private static void AddSettingParameter(ref List<SteamItemSettings> settingsList, SteamItemSettings setting )
        {
            if (!settingsList.Contains(setting))
            {
                settingsList.Add(setting);
            }
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
            List<string> _fileNames;
            _fileNames = _filesToWatch.Keys.ToList();
            foreach (string path in _fileNames)
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
            fileData.TransformedData = fileData.TransformFunction(originalFileData, BPM_OptionsManager.GetSettings().ToArray(),_steamPath);
            _filesToWatch[path] = fileData;
            Log(String.Format(Properties.Resources.REPORT_FILE_UPDATED,path));
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private void Start()
        {
            _filesToWatch = new Dictionary<string, FileData>(StringComparer.OrdinalIgnoreCase);

            AddFileToWatch(Path.GetFullPath(Path.Combine(_steamPath, Properties.Resources.SETTING_FILE_QUITENTRIES)), BPMT_QuitEntries.Transform);
            //AddFileToWatch(Path.GetFullPath(Path.Combine(_steamPath, Properties.Resources.SETTING_FILE_MAINMENU)), BPMT_MainMenu.Transform);

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
            _autoUpdater.Start();
            _fsw.EnableRaisingEvents = true;


        }

        private void Stop()
        {
            _fsw.EnableRaisingEvents = false;
            WriteAllFiles(false);
            _fsw.Dispose();
            _autoUpdater.Stop();
            _steamBpmHook.Stop();
            
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


        private static void Log(string message)
        {
            BPM_Log.Log(message);
        }
    }
}
