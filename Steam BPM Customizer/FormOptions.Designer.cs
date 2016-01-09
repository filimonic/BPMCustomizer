namespace Steam_BPM_Customizer
{
    partial class formOptions
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formOptions));
            this.cb_DisableQuitMenuSleep = new System.Windows.Forms.CheckBox();
            this.cb_DisableQuitMenuLogOff = new System.Windows.Forms.CheckBox();
            this.cb_DisableQuitMenuShutdown = new System.Windows.Forms.CheckBox();
            this.cb_DisableQuitToDesktop = new System.Windows.Forms.CheckBox();
            this.cb_DisableQuitMenuRestart = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cb_DisableQuitMenuExitSteam = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.hl_InternetAutoUpdateShortTimeout = new System.Windows.Forms.LinkLabel();
            this.cb_InternetAutoUpdateDelayOnStart = new System.Windows.Forms.CheckBox();
            this.cb_InternetSendUsageStatistics = new System.Windows.Forms.CheckBox();
            this.cb_InternetAutoUpdate = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cb_HomeScreenHideProfileName = new System.Windows.Forms.CheckBox();
            this.cb_HomeScreenHideProfileIcon = new System.Windows.Forms.CheckBox();
            this.btn_UpdateNow = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btn_ExitApp = new System.Windows.Forms.Button();
            this.btn_RestartApp = new System.Windows.Forms.Button();
            this.lbl_lastUpdateCheckTime = new System.Windows.Forms.Label();
            this.lbl_UpdateCheckResult = new System.Windows.Forms.Label();
            this.timer_renewUpdateCheck = new System.Windows.Forms.Timer(this.components);
            this.cb_DisableMinimizeBigPicture = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // cb_DisableQuitMenuSleep
            // 
            this.cb_DisableQuitMenuSleep.AutoSize = true;
            this.cb_DisableQuitMenuSleep.Location = new System.Drawing.Point(7, 45);
            this.cb_DisableQuitMenuSleep.Name = "cb_DisableQuitMenuSleep";
            this.cb_DisableQuitMenuSleep.Size = new System.Drawing.Size(117, 17);
            this.cb_DisableQuitMenuSleep.TabIndex = 0;
            this.cb_DisableQuitMenuSleep.Text = "Disable \'Sleep\' item";
            this.cb_DisableQuitMenuSleep.UseVisualStyleBackColor = true;
            this.cb_DisableQuitMenuSleep.CheckedChanged += new System.EventHandler(this.cb_any_CheckedChanged);
            // 
            // cb_DisableQuitMenuLogOff
            // 
            this.cb_DisableQuitMenuLogOff.AutoSize = true;
            this.cb_DisableQuitMenuLogOff.Location = new System.Drawing.Point(7, 68);
            this.cb_DisableQuitMenuLogOff.Name = "cb_DisableQuitMenuLogOff";
            this.cb_DisableQuitMenuLogOff.Size = new System.Drawing.Size(125, 17);
            this.cb_DisableQuitMenuLogOff.TabIndex = 1;
            this.cb_DisableQuitMenuLogOff.Text = "Disable \'Log Off\' item";
            this.cb_DisableQuitMenuLogOff.UseVisualStyleBackColor = true;
            this.cb_DisableQuitMenuLogOff.CheckedChanged += new System.EventHandler(this.cb_any_CheckedChanged);
            // 
            // cb_DisableQuitMenuShutdown
            // 
            this.cb_DisableQuitMenuShutdown.AutoSize = true;
            this.cb_DisableQuitMenuShutdown.Location = new System.Drawing.Point(7, 91);
            this.cb_DisableQuitMenuShutdown.Name = "cb_DisableQuitMenuShutdown";
            this.cb_DisableQuitMenuShutdown.Size = new System.Drawing.Size(138, 17);
            this.cb_DisableQuitMenuShutdown.TabIndex = 1;
            this.cb_DisableQuitMenuShutdown.Text = "Disable \'Shutdown\' item";
            this.cb_DisableQuitMenuShutdown.UseVisualStyleBackColor = true;
            this.cb_DisableQuitMenuShutdown.CheckedChanged += new System.EventHandler(this.cb_any_CheckedChanged);
            // 
            // cb_DisableQuitToDesktop
            // 
            this.cb_DisableQuitToDesktop.AutoSize = true;
            this.cb_DisableQuitToDesktop.Location = new System.Drawing.Point(7, 137);
            this.cb_DisableQuitToDesktop.Name = "cb_DisableQuitToDesktop";
            this.cb_DisableQuitToDesktop.Size = new System.Drawing.Size(162, 17);
            this.cb_DisableQuitToDesktop.TabIndex = 1;
            this.cb_DisableQuitToDesktop.Text = "Disable \'Quit to desktop\' item";
            this.cb_DisableQuitToDesktop.UseVisualStyleBackColor = true;
            this.cb_DisableQuitToDesktop.CheckedChanged += new System.EventHandler(this.cb_any_CheckedChanged);
            // 
            // cb_DisableQuitMenuRestart
            // 
            this.cb_DisableQuitMenuRestart.AutoSize = true;
            this.cb_DisableQuitMenuRestart.Location = new System.Drawing.Point(7, 114);
            this.cb_DisableQuitMenuRestart.Name = "cb_DisableQuitMenuRestart";
            this.cb_DisableQuitMenuRestart.Size = new System.Drawing.Size(127, 17);
            this.cb_DisableQuitMenuRestart.TabIndex = 1;
            this.cb_DisableQuitMenuRestart.Text = "Disable \'Restart\" item";
            this.cb_DisableQuitMenuRestart.UseVisualStyleBackColor = true;
            this.cb_DisableQuitMenuRestart.CheckedChanged += new System.EventHandler(this.cb_any_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cb_DisableMinimizeBigPicture);
            this.groupBox1.Controls.Add(this.cb_DisableQuitToDesktop);
            this.groupBox1.Controls.Add(this.cb_DisableQuitMenuExitSteam);
            this.groupBox1.Controls.Add(this.cb_DisableQuitMenuSleep);
            this.groupBox1.Controls.Add(this.cb_DisableQuitMenuLogOff);
            this.groupBox1.Controls.Add(this.cb_DisableQuitMenuShutdown);
            this.groupBox1.Controls.Add(this.cb_DisableQuitMenuRestart);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(273, 266);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Quit menu";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.DarkRed;
            this.label1.Location = new System.Drawing.Point(6, 250);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(248, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "* If you enable all, you will not be able to exit Steam";
            // 
            // cb_DisableQuitMenuExitSteam
            // 
            this.cb_DisableQuitMenuExitSteam.AutoSize = true;
            this.cb_DisableQuitMenuExitSteam.Location = new System.Drawing.Point(6, 22);
            this.cb_DisableQuitMenuExitSteam.Name = "cb_DisableQuitMenuExitSteam";
            this.cb_DisableQuitMenuExitSteam.Size = new System.Drawing.Size(140, 17);
            this.cb_DisableQuitMenuExitSteam.TabIndex = 0;
            this.cb_DisableQuitMenuExitSteam.Text = "Disable \'Exit Steam\' item";
            this.cb_DisableQuitMenuExitSteam.UseVisualStyleBackColor = true;
            this.cb_DisableQuitMenuExitSteam.CheckedChanged += new System.EventHandler(this.cb_any_CheckedChanged);
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(12, 424);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(273, 46);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "Options are saved when you click elements\r\nHowever, they will not be applied unti" +
    "l you restart Steam";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.hl_InternetAutoUpdateShortTimeout);
            this.groupBox2.Controls.Add(this.cb_InternetAutoUpdateDelayOnStart);
            this.groupBox2.Controls.Add(this.cb_InternetSendUsageStatistics);
            this.groupBox2.Controls.Add(this.cb_InternetAutoUpdate);
            this.groupBox2.Location = new System.Drawing.Point(12, 284);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(273, 134);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Internet communictaions";
            // 
            // hl_InternetAutoUpdateShortTimeout
            // 
            this.hl_InternetAutoUpdateShortTimeout.AutoSize = true;
            this.hl_InternetAutoUpdateShortTimeout.Location = new System.Drawing.Point(248, 62);
            this.hl_InternetAutoUpdateShortTimeout.Name = "hl_InternetAutoUpdateShortTimeout";
            this.hl_InternetAutoUpdateShortTimeout.Size = new System.Drawing.Size(19, 13);
            this.hl_InternetAutoUpdateShortTimeout.TabIndex = 2;
            this.hl_InternetAutoUpdateShortTimeout.TabStop = true;
            this.hl_InternetAutoUpdateShortTimeout.Text = "(?)";
            this.hl_InternetAutoUpdateShortTimeout.MouseClick += new System.Windows.Forms.MouseEventHandler(this.hl_InternetAutoUpdateShortTimeout_MouseClick);
            // 
            // cb_InternetAutoUpdateDelayOnStart
            // 
            this.cb_InternetAutoUpdateDelayOnStart.AutoSize = true;
            this.cb_InternetAutoUpdateDelayOnStart.Location = new System.Drawing.Point(6, 58);
            this.cb_InternetAutoUpdateDelayOnStart.Name = "cb_InternetAutoUpdateDelayOnStart";
            this.cb_InternetAutoUpdateDelayOnStart.Size = new System.Drawing.Size(239, 17);
            this.cb_InternetAutoUpdateDelayOnStart.TabIndex = 1;
            this.cb_InternetAutoUpdateDelayOnStart.Text = "Wait for 5 minutes after start to check update";
            this.cb_InternetAutoUpdateDelayOnStart.UseVisualStyleBackColor = true;
            this.cb_InternetAutoUpdateDelayOnStart.CheckedChanged += new System.EventHandler(this.cb_any_CheckedChanged);
            // 
            // cb_InternetSendUsageStatistics
            // 
            this.cb_InternetSendUsageStatistics.AutoSize = true;
            this.cb_InternetSendUsageStatistics.Enabled = false;
            this.cb_InternetSendUsageStatistics.Location = new System.Drawing.Point(6, 39);
            this.cb_InternetSendUsageStatistics.Name = "cb_InternetSendUsageStatistics";
            this.cb_InternetSendUsageStatistics.Size = new System.Drawing.Size(126, 17);
            this.cb_InternetSendUsageStatistics.TabIndex = 1;
            this.cb_InternetSendUsageStatistics.Text = "Send usage statistics";
            this.cb_InternetSendUsageStatistics.UseVisualStyleBackColor = true;
            this.cb_InternetSendUsageStatistics.CheckedChanged += new System.EventHandler(this.cb_any_CheckedChanged);
            // 
            // cb_InternetAutoUpdate
            // 
            this.cb_InternetAutoUpdate.AutoSize = true;
            this.cb_InternetAutoUpdate.Location = new System.Drawing.Point(6, 19);
            this.cb_InternetAutoUpdate.Name = "cb_InternetAutoUpdate";
            this.cb_InternetAutoUpdate.Size = new System.Drawing.Size(125, 17);
            this.cb_InternetAutoUpdate.TabIndex = 0;
            this.cb_InternetAutoUpdate.Text = "Auto update program";
            this.cb_InternetAutoUpdate.UseVisualStyleBackColor = true;
            this.cb_InternetAutoUpdate.CheckedChanged += new System.EventHandler(this.cb_any_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cb_HomeScreenHideProfileName);
            this.groupBox3.Controls.Add(this.cb_HomeScreenHideProfileIcon);
            this.groupBox3.Location = new System.Drawing.Point(291, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(250, 185);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Home Screen";
            // 
            // cb_HomeScreenHideProfileName
            // 
            this.cb_HomeScreenHideProfileName.AutoSize = true;
            this.cb_HomeScreenHideProfileName.Enabled = false;
            this.cb_HomeScreenHideProfileName.Location = new System.Drawing.Point(7, 45);
            this.cb_HomeScreenHideProfileName.Name = "cb_HomeScreenHideProfileName";
            this.cb_HomeScreenHideProfileName.Size = new System.Drawing.Size(185, 17);
            this.cb_HomeScreenHideProfileName.TabIndex = 0;
            this.cb_HomeScreenHideProfileName.Text = "cb_HomeScreenHideProfileName";
            this.cb_HomeScreenHideProfileName.UseVisualStyleBackColor = true;
            this.cb_HomeScreenHideProfileName.CheckedChanged += new System.EventHandler(this.cb_any_CheckedChanged);
            // 
            // cb_HomeScreenHideProfileIcon
            // 
            this.cb_HomeScreenHideProfileIcon.AutoSize = true;
            this.cb_HomeScreenHideProfileIcon.Enabled = false;
            this.cb_HomeScreenHideProfileIcon.Location = new System.Drawing.Point(7, 20);
            this.cb_HomeScreenHideProfileIcon.Name = "cb_HomeScreenHideProfileIcon";
            this.cb_HomeScreenHideProfileIcon.Size = new System.Drawing.Size(178, 17);
            this.cb_HomeScreenHideProfileIcon.TabIndex = 0;
            this.cb_HomeScreenHideProfileIcon.Text = "cb_HomeScreenHideProfileIcon";
            this.cb_HomeScreenHideProfileIcon.UseVisualStyleBackColor = true;
            this.cb_HomeScreenHideProfileIcon.CheckedChanged += new System.EventHandler(this.cb_any_CheckedChanged);
            // 
            // btn_UpdateNow
            // 
            this.btn_UpdateNow.Location = new System.Drawing.Point(7, 19);
            this.btn_UpdateNow.Name = "btn_UpdateNow";
            this.btn_UpdateNow.Size = new System.Drawing.Size(237, 23);
            this.btn_UpdateNow.TabIndex = 3;
            this.btn_UpdateNow.Text = "Update now";
            this.btn_UpdateNow.UseVisualStyleBackColor = true;
            this.btn_UpdateNow.Click += new System.EventHandler(this.btm_UpdateNow_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btn_ExitApp);
            this.groupBox4.Controls.Add(this.btn_RestartApp);
            this.groupBox4.Controls.Add(this.lbl_lastUpdateCheckTime);
            this.groupBox4.Controls.Add(this.lbl_UpdateCheckResult);
            this.groupBox4.Controls.Add(this.btn_UpdateNow);
            this.groupBox4.Location = new System.Drawing.Point(291, 203);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(250, 134);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Maual update";
            // 
            // btn_ExitApp
            // 
            this.btn_ExitApp.Location = new System.Drawing.Point(136, 105);
            this.btn_ExitApp.Name = "btn_ExitApp";
            this.btn_ExitApp.Size = new System.Drawing.Size(108, 23);
            this.btn_ExitApp.TabIndex = 5;
            this.btn_ExitApp.Text = "Exit Customizer";
            this.btn_ExitApp.UseVisualStyleBackColor = true;
            this.btn_ExitApp.Click += new System.EventHandler(this.btn_ExitApp_Click);
            // 
            // btn_RestartApp
            // 
            this.btn_RestartApp.Location = new System.Drawing.Point(9, 105);
            this.btn_RestartApp.Name = "btn_RestartApp";
            this.btn_RestartApp.Size = new System.Drawing.Size(108, 23);
            this.btn_RestartApp.TabIndex = 5;
            this.btn_RestartApp.Text = "Restart Customizer";
            this.btn_RestartApp.UseVisualStyleBackColor = true;
            this.btn_RestartApp.Click += new System.EventHandler(this.btn_RestartApp_Click);
            // 
            // lbl_lastUpdateCheckTime
            // 
            this.lbl_lastUpdateCheckTime.AutoSize = true;
            this.lbl_lastUpdateCheckTime.Location = new System.Drawing.Point(6, 49);
            this.lbl_lastUpdateCheckTime.Name = "lbl_lastUpdateCheckTime";
            this.lbl_lastUpdateCheckTime.Size = new System.Drawing.Size(28, 13);
            this.lbl_lastUpdateCheckTime.TabIndex = 4;
            this.lbl_lastUpdateCheckTime.Text = "###";
            // 
            // lbl_UpdateCheckResult
            // 
            this.lbl_UpdateCheckResult.AutoSize = true;
            this.lbl_UpdateCheckResult.Location = new System.Drawing.Point(6, 62);
            this.lbl_UpdateCheckResult.Name = "lbl_UpdateCheckResult";
            this.lbl_UpdateCheckResult.Size = new System.Drawing.Size(28, 13);
            this.lbl_UpdateCheckResult.TabIndex = 4;
            this.lbl_UpdateCheckResult.Text = "###";
            // 
            // timer_renewUpdateCheck
            // 
            this.timer_renewUpdateCheck.Enabled = true;
            this.timer_renewUpdateCheck.Interval = 1000;
            this.timer_renewUpdateCheck.Tick += new System.EventHandler(this.timer_renewUpdateCheck_Tick);
            // 
            // cb_DisableMinimizeBigPicture
            // 
            this.cb_DisableMinimizeBigPicture.AutoSize = true;
            this.cb_DisableMinimizeBigPicture.Location = new System.Drawing.Point(6, 160);
            this.cb_DisableMinimizeBigPicture.Name = "cb_DisableMinimizeBigPicture";
            this.cb_DisableMinimizeBigPicture.Size = new System.Drawing.Size(184, 17);
            this.cb_DisableMinimizeBigPicture.TabIndex = 1;
            this.cb_DisableMinimizeBigPicture.Text = "Disable \'Minimize Big Picture\' item";
            this.cb_DisableMinimizeBigPicture.UseVisualStyleBackColor = true;
            this.cb_DisableMinimizeBigPicture.CheckedChanged += new System.EventHandler(this.cb_any_CheckedChanged);
            // 
            // formOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 481);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formOptions";
            this.ShowIcon = false;
            this.Text = "BPM Customizer Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formOptions_FormClosing);
            this.Shown += new System.EventHandler(this.formOptions_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cb_DisableQuitMenuSleep;
        private System.Windows.Forms.CheckBox cb_DisableQuitMenuLogOff;
        private System.Windows.Forms.CheckBox cb_DisableQuitMenuShutdown;
        private System.Windows.Forms.CheckBox cb_DisableQuitToDesktop;
        private System.Windows.Forms.CheckBox cb_DisableQuitMenuRestart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cb_DisableQuitMenuExitSteam;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cb_InternetSendUsageStatistics;
        private System.Windows.Forms.CheckBox cb_InternetAutoUpdate;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cb_HomeScreenHideProfileIcon;
        private System.Windows.Forms.CheckBox cb_HomeScreenHideProfileName;
        private System.Windows.Forms.CheckBox cb_InternetAutoUpdateDelayOnStart;
        private System.Windows.Forms.LinkLabel hl_InternetAutoUpdateShortTimeout;
        private System.Windows.Forms.Button btn_UpdateNow;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lbl_UpdateCheckResult;
        private System.Windows.Forms.Button btn_RestartApp;
        private System.Windows.Forms.Label lbl_lastUpdateCheckTime;
        private System.Windows.Forms.Timer timer_renewUpdateCheck;
        private System.Windows.Forms.Button btn_ExitApp;
        private System.Windows.Forms.CheckBox cb_DisableMinimizeBigPicture;
    }
}