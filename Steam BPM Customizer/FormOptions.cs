using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steam_BPM_Customizer
{
    public partial class formOptions : Form
    {
        private bool _isLoadingSettings = false;
        public formOptions()
        {
            InitializeComponent();
        }

        private void processSettings()
        {
            if (_isLoadingSettings)
            {
                return;
            }
            BPM_OptionsManager.SetSetting(SteamItemSettings.No_QuitMenu_Sleep, cb_DisableQuitMenuSleep.Checked);
            BPM_OptionsManager.SetSetting(SteamItemSettings.No_QuitMenu_LogOff, cb_DisableQuitMenuLogOff.Checked);
            BPM_OptionsManager.SetSetting(SteamItemSettings.No_QuitMenu_Shutdown, cb_DisableQuitMenuShutdown.Checked);
            BPM_OptionsManager.SetSetting(SteamItemSettings.No_QuitMenu_ExitBPM, cb_DisableQuitToDesktop.Checked);
            BPM_OptionsManager.SetSetting(SteamItemSettings.No_QuitMenu_Restart, cb_DisableQuitMenuRestart.Checked);
            BPM_OptionsManager.SetSetting(SteamItemSettings.No_QuitMenu_ExitSteam, cb_DisableQuitMenuExitSteam.Checked);
            BPM_OptionsManager.SetSetting(SteamItemSettings.Program_SendUsageStatistics, cb_InternetSendUsageStatistics.Checked);
            BPM_OptionsManager.SetSetting(SteamItemSettings.Program_EnableAutoUpdate, cb_InternetAutoUpdate.Checked);
            BPM_OptionsManager.SetSetting(SteamItemSettings.Program_DelayAutoUpdateAfterStart, cb_InternetAutoUpdateDelayOnStart.Checked);
#if _DEVELOPEMENT_MODE_
            BPM_OptionsManager.SetSetting(SteamItemSettings.NoProfileIcon, cb_HomeScreenHideProfileIcon.Checked);
            BPM_OptionsManager.SetSetting(SteamItemSettings.HomeScreenHideProfileName, cb_HomeScreenHideProfileName.Checked);
#endif
            BPM_OptionsManager.SetSetting(SteamItemSettings.Program_FirstRunPassed_v0050, true);


            BPM_Log.Log("Settings changed");
        }

        private void cb_any_CheckedChanged(object sender, EventArgs e)
        {
            processSettings();
        }

        private void formOptions_Shown(object sender, EventArgs e)
        {
            _isLoadingSettings = true;
            foreach(SteamItemSettings setting in Enum.GetValues(typeof(SteamItemSettings))) {
                if (BPM_OptionsManager.IsSettingEnabled(setting))
                {
                    switch (setting)
                    {
                        case SteamItemSettings.No_QuitMenu_Sleep:
                            cb_DisableQuitMenuSleep.Checked = true;
                            break;
                        case SteamItemSettings.No_QuitMenu_LogOff:
                            cb_DisableQuitMenuLogOff.Checked = true;
                            break;
                        case SteamItemSettings.No_QuitMenu_Shutdown:
                            cb_DisableQuitMenuShutdown.Checked = true;
                            break;
                        case SteamItemSettings.No_QuitMenu_ExitBPM:
                            cb_DisableQuitToDesktop.Checked = true;
                            break;
                        case SteamItemSettings.No_QuitMenu_Restart:
                            cb_DisableQuitMenuRestart.Checked = true;
                            break;
                        case SteamItemSettings.No_QuitMenu_ExitSteam:
                            cb_DisableQuitMenuExitSteam.Checked = true;
                            break;
                        case SteamItemSettings.Program_SendUsageStatistics:
                            cb_InternetSendUsageStatistics.Checked = true;
                            break;
                        case SteamItemSettings.Program_EnableAutoUpdate:
                            cb_InternetAutoUpdate.Checked = true;
                            break;
                        case SteamItemSettings.Program_DelayAutoUpdateAfterStart:
                            cb_InternetAutoUpdateDelayOnStart.Checked = true;
                            break;
#if _DEVELOPEMENT_MODE_
                        case SteamItemSettings.HomeScreenHideProfileName:
                            cb_HomeScreenHideProfileName.Checked = true;
                            break;
                        case SteamItemSettings.NoProfileIcon:
                            cb_HomeScreenHideProfileIcon.Checked = true;
                            break;
#endif
                    }
                }
            }
            _isLoadingSettings = false;
        }

        private void formOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void hl_InternetAutoUpdateShortTimeout_MouseClick(object sender, MouseEventArgs e)
        {
            this.BeginInvoke((Action)(() => MessageBox.Show("If you put BPMCustomizer in startup, and Steam in startup, during update, BPMCustomizer can miss Steam goes to Big Picture Mode.")));
        }

        private void btm_UpdateNow_Click(object sender, EventArgs e)
        {
            BPM_Updater_UpdateStatus update_check_result = BPM_Updater.CheckUpdate(true);
            BPM_Updater_UpdateStatus update_result = BPM_Updater_UpdateStatus.UNKNOWN;
            if (update_check_result == BPM_Updater_UpdateStatus.UPDATE_AVAILABLE)
            {
                update_result = BPM_Updater.PerformUpdate();
            }
            lbl_UpdateCheckResult.Text = BPM_Updater.UpdateStatusToString(update_result == BPM_Updater_UpdateStatus.UNKNOWN? update_check_result : update_result);
        }

        private void btn_RestartApp_Click(object sender, EventArgs e)
        {
            SteamBPMCustomizer.RestartApp();
        }

        private void timer_renewUpdateCheck_Tick(object sender, EventArgs e)
        {
            DateTime lastUpdateCheckTime = BPM_Updater.LastUpdateCheckTime;
            lbl_lastUpdateCheckTime.Text = (lastUpdateCheckTime == DateTime.MinValue ? "Never" : lastUpdateCheckTime.ToString());
            lbl_UpdateCheckResult.Text = BPM_Updater.UpdateStatusToString(BPM_Updater.LastUpdateCheckStatus); 
        }

        private void btn_ExitApp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Exit through tray icon menu, please");
        }
    }
}
