namespace GSEditor.UI.Forms
{
  partial class MainForm
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
      this.msMain = new System.Windows.Forms.MenuStrip();
      this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
      this.menuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
      this.menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
      this.menuFileSeparator = new System.Windows.Forms.ToolStripSeparator();
      this.menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
      this.menuGame = new System.Windows.Forms.ToolStripMenuItem();
      this.menuGamePlay = new System.Windows.Forms.ToolStripMenuItem();
      this.menuGameSeparator = new System.Windows.Forms.ToolStripSeparator();
      this.menuGameSettings = new System.Windows.Forms.ToolStripMenuItem();
      this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
      this.menuHelpInfo = new System.Windows.Forms.ToolStripMenuItem();
      this.tsMain = new System.Windows.Forms.ToolStrip();
      this.toolOpen = new System.Windows.Forms.ToolStripButton();
      this.toolSave = new System.Windows.Forms.ToolStripButton();
      this.toolSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.toolPlay = new System.Windows.Forms.ToolStripButton();
      this.toolSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.toolExit = new System.Windows.Forms.ToolStripButton();
      this.tabMain = new System.Windows.Forms.TabControl();
      this.tabUnused = new System.Windows.Forms.TabPage();
      this.stMain = new System.Windows.Forms.StatusStrip();
      this.lbFilename = new System.Windows.Forms.ToolStripStatusLabel();
      this.msMain.SuspendLayout();
      this.tsMain.SuspendLayout();
      this.tabMain.SuspendLayout();
      this.stMain.SuspendLayout();
      this.SuspendLayout();
      // 
      // msMain
      // 
      this.msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuGame,
            this.menuHelp});
      this.msMain.Location = new System.Drawing.Point(0, 0);
      this.msMain.Name = "msMain";
      this.msMain.Padding = new System.Windows.Forms.Padding(0);
      this.msMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
      this.msMain.Size = new System.Drawing.Size(634, 24);
      this.msMain.TabIndex = 0;
      // 
      // menuFile
      // 
      this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFileOpen,
            this.menuFileSave,
            this.menuFileSeparator,
            this.menuFileExit});
      this.menuFile.Name = "menuFile";
      this.menuFile.Size = new System.Drawing.Size(57, 24);
      this.menuFile.Text = "파일(&F)";
      // 
      // menuFileOpen
      // 
      this.menuFileOpen.Image = global::GSEditor.Properties.Resources.if_folder;
      this.menuFileOpen.ImageTransparentColor = System.Drawing.Color.Fuchsia;
      this.menuFileOpen.Name = "menuFileOpen";
      this.menuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
      this.menuFileOpen.Size = new System.Drawing.Size(167, 22);
      this.menuFileOpen.Text = "열기(&O)...";
      this.menuFileOpen.Click += new System.EventHandler(this.OnMenuFileOpenClick);
      // 
      // menuFileSave
      // 
      this.menuFileSave.Enabled = false;
      this.menuFileSave.Image = global::GSEditor.Properties.Resources.if_save;
      this.menuFileSave.ImageTransparentColor = System.Drawing.Color.Fuchsia;
      this.menuFileSave.Name = "menuFileSave";
      this.menuFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
      this.menuFileSave.Size = new System.Drawing.Size(167, 22);
      this.menuFileSave.Text = "저장(&S)";
      this.menuFileSave.Click += new System.EventHandler(this.OnMenuFileSaveClick);
      // 
      // menuFileSeparator
      // 
      this.menuFileSeparator.Name = "menuFileSeparator";
      this.menuFileSeparator.Size = new System.Drawing.Size(164, 6);
      // 
      // menuFileExit
      // 
      this.menuFileExit.Image = global::GSEditor.Properties.Resources.if_exit;
      this.menuFileExit.ImageTransparentColor = System.Drawing.Color.Fuchsia;
      this.menuFileExit.Name = "menuFileExit";
      this.menuFileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
      this.menuFileExit.Size = new System.Drawing.Size(167, 22);
      this.menuFileExit.Text = "종료(&X)";
      this.menuFileExit.Click += new System.EventHandler(this.OnMenuFileExitClick);
      // 
      // menuGame
      // 
      this.menuGame.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuGamePlay,
            this.menuGameSeparator,
            this.menuGameSettings});
      this.menuGame.Name = "menuGame";
      this.menuGame.Size = new System.Drawing.Size(59, 24);
      this.menuGame.Text = "게임(&G)";
      // 
      // menuGamePlay
      // 
      this.menuGamePlay.Enabled = false;
      this.menuGamePlay.Image = global::GSEditor.Properties.Resources.if_play;
      this.menuGamePlay.ImageTransparentColor = System.Drawing.Color.Fuchsia;
      this.menuGamePlay.Name = "menuGamePlay";
      this.menuGamePlay.ShortcutKeys = System.Windows.Forms.Keys.F5;
      this.menuGamePlay.Size = new System.Drawing.Size(194, 22);
      this.menuGamePlay.Text = "테스트 플레이(&P)...";
      this.menuGamePlay.Click += new System.EventHandler(this.OnMenuGamePlayClick);
      // 
      // menuGameSeparator
      // 
      this.menuGameSeparator.Name = "menuGameSeparator";
      this.menuGameSeparator.Size = new System.Drawing.Size(191, 6);
      // 
      // menuGameSettings
      // 
      this.menuGameSettings.Image = global::GSEditor.Properties.Resources.if_settings;
      this.menuGameSettings.ImageTransparentColor = System.Drawing.Color.Fuchsia;
      this.menuGameSettings.Name = "menuGameSettings";
      this.menuGameSettings.Size = new System.Drawing.Size(194, 22);
      this.menuGameSettings.Text = "에뮬레이터 설정(&O)...";
      this.menuGameSettings.Click += new System.EventHandler(this.OnMenuGameSettingsClick);
      // 
      // menuHelp
      // 
      this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelpInfo});
      this.menuHelp.Name = "menuHelp";
      this.menuHelp.Size = new System.Drawing.Size(72, 24);
      this.menuHelp.Text = "도움말(&H)";
      // 
      // menuHelpInfo
      // 
      this.menuHelpInfo.Name = "menuHelpInfo";
      this.menuHelpInfo.Size = new System.Drawing.Size(182, 22);
      this.menuHelpInfo.Text = "GS 에디터 정보(&A)...";
      // 
      // tsMain
      // 
      this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolOpen,
            this.toolSave,
            this.toolSeparator1,
            this.toolPlay,
            this.toolSeparator2,
            this.toolExit});
      this.tsMain.Location = new System.Drawing.Point(0, 24);
      this.tsMain.Name = "tsMain";
      this.tsMain.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.tsMain.Size = new System.Drawing.Size(634, 25);
      this.tsMain.TabIndex = 1;
      // 
      // toolOpen
      // 
      this.toolOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolOpen.Image = global::GSEditor.Properties.Resources.if_folder;
      this.toolOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolOpen.Name = "toolOpen";
      this.toolOpen.Size = new System.Drawing.Size(23, 22);
      this.toolOpen.Text = "열기...";
      this.toolOpen.Click += new System.EventHandler(this.OnMenuFileOpenClick);
      // 
      // toolSave
      // 
      this.toolSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolSave.Enabled = false;
      this.toolSave.Image = global::GSEditor.Properties.Resources.if_save;
      this.toolSave.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolSave.Name = "toolSave";
      this.toolSave.Size = new System.Drawing.Size(23, 22);
      this.toolSave.Text = "저장";
      this.toolSave.Click += new System.EventHandler(this.OnMenuFileSaveClick);
      // 
      // toolSeparator1
      // 
      this.toolSeparator1.Name = "toolSeparator1";
      this.toolSeparator1.Size = new System.Drawing.Size(6, 25);
      // 
      // toolPlay
      // 
      this.toolPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolPlay.Enabled = false;
      this.toolPlay.Image = global::GSEditor.Properties.Resources.if_play;
      this.toolPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolPlay.Name = "toolPlay";
      this.toolPlay.Size = new System.Drawing.Size(23, 22);
      this.toolPlay.Text = "테스트 플레이...";
      this.toolPlay.Click += new System.EventHandler(this.OnMenuGamePlayClick);
      // 
      // toolSeparator2
      // 
      this.toolSeparator2.Name = "toolSeparator2";
      this.toolSeparator2.Size = new System.Drawing.Size(6, 25);
      // 
      // toolExit
      // 
      this.toolExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolExit.Image = global::GSEditor.Properties.Resources.if_exit;
      this.toolExit.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolExit.Name = "toolExit";
      this.toolExit.Size = new System.Drawing.Size(23, 22);
      this.toolExit.Text = "종료";
      this.toolExit.Click += new System.EventHandler(this.OnMenuFileExitClick);
      // 
      // tabMain
      // 
      this.tabMain.Controls.Add(this.tabUnused);
      this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabMain.Location = new System.Drawing.Point(0, 49);
      this.tabMain.Name = "tabMain";
      this.tabMain.SelectedIndex = 0;
      this.tabMain.Size = new System.Drawing.Size(634, 362);
      this.tabMain.TabIndex = 2;
      // 
      // tabUnused
      // 
      this.tabUnused.Location = new System.Drawing.Point(4, 24);
      this.tabUnused.Name = "tabUnused";
      this.tabUnused.Padding = new System.Windows.Forms.Padding(3);
      this.tabUnused.Size = new System.Drawing.Size(626, 334);
      this.tabUnused.TabIndex = 0;
      this.tabUnused.Text = "-";
      this.tabUnused.UseVisualStyleBackColor = true;
      // 
      // stMain
      // 
      this.stMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbFilename});
      this.stMain.Location = new System.Drawing.Point(0, 389);
      this.stMain.Name = "stMain";
      this.stMain.Size = new System.Drawing.Size(634, 22);
      this.stMain.TabIndex = 3;
      // 
      // lbFilename
      // 
      this.lbFilename.Name = "lbFilename";
      this.lbFilename.Size = new System.Drawing.Size(12, 17);
      this.lbFilename.Text = "-";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(634, 411);
      this.Controls.Add(this.stMain);
      this.Controls.Add(this.tabMain);
      this.Controls.Add(this.tsMain);
      this.Controls.Add(this.msMain);
      this.MainMenuStrip = this.msMain;
      this.Name = "MainForm";
      this.msMain.ResumeLayout(false);
      this.msMain.PerformLayout();
      this.tsMain.ResumeLayout(false);
      this.tsMain.PerformLayout();
      this.tabMain.ResumeLayout(false);
      this.stMain.ResumeLayout(false);
      this.stMain.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

        #endregion

        private MenuStrip msMain;
        private ToolStripMenuItem menuFile;
        private ToolStripMenuItem menuFileOpen;
        private ToolStripMenuItem menuFileSave;
        private ToolStripSeparator menuFileSeparator;
        private ToolStripMenuItem menuFileExit;
        private ToolStripMenuItem menuGame;
        private ToolStripMenuItem menuHelp;
        private ToolStripMenuItem menuGamePlay;
        private ToolStripSeparator menuGameSeparator;
        private ToolStripMenuItem menuGameSettings;
        private ToolStripMenuItem menuHelpInfo;
        private ToolStrip tsMain;
        private ToolStripButton toolOpen;
        private ToolStripButton toolSave;
        private ToolStripSeparator toolSeparator1;
        private ToolStripButton toolPlay;
        private ToolStripSeparator toolSeparator2;
        private ToolStripButton toolExit;
        private TabControl tabMain;
        private TabPage tabUnused;
        private StatusStrip stMain;
        private ToolStripStatusLabel lbFilename;
    }
}