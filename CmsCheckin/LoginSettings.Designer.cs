namespace CmsCheckin
{
	partial class LoginSettings
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
			if (disposing && (components != null)) {
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
            this.PrintKiosks = new System.Windows.Forms.TextBox();
            this.KiosksToPrintForLabel = new System.Windows.Forms.Label();
            this.PrintMode = new System.Windows.Forms.ComboBox();
            this.Printer = new System.Windows.Forms.ComboBox();
            this.PrinterLabel = new System.Windows.Forms.Label();
            this.DisableLocationLabels = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BuildingAccessMode = new System.Windows.Forms.CheckBox();
            this.Building = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.PrintTest = new System.Windows.Forms.Button();
            this.FormatLabel = new System.Windows.Forms.Label();
            this.LabelFormat = new System.Windows.Forms.TextBox();
            this.LoadLabelList = new System.Windows.Forms.Button();
            this.LabelList = new System.Windows.Forms.ComboBox();
            this.NameLabel = new System.Windows.Forms.Label();
            this.SaveLabel = new System.Windows.Forms.Button();
            this.LabelPrinterSize = new System.Windows.Forms.Label();
            this.PageWidthLabel = new System.Windows.Forms.Label();
            this.PrinterWidth = new System.Windows.Forms.TextBox();
            this.PageHeightLabel = new System.Windows.Forms.Label();
            this.PrinterHeight = new System.Windows.Forms.TextBox();
            this.SizeFromPrinter = new System.Windows.Forms.Button();
            this.AdvancedPageSize = new System.Windows.Forms.CheckBox();
            this.AdminPIN = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.AdminPINTimeout = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.ExtraBlankLabel = new System.Windows.Forms.CheckBox();
            this.AskChurchName = new System.Windows.Forms.CheckBox();
            this.AskChurch = new System.Windows.Forms.CheckBox();
            this.AskGrade = new System.Windows.Forms.CheckBox();
            this.AskFriend = new System.Windows.Forms.CheckBox();
            this.SecurityLabelPerChild = new System.Windows.Forms.CheckBox();
            this.KioskName = new System.Windows.Forms.TextBox();
            this.KioskNameLabel = new System.Windows.Forms.Label();
            this.LateMinutes = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.EarlyHours = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.DayOfWeekCombo = new System.Windows.Forms.ComboBox();
            this.CampusCombo = new System.Windows.Forms.ComboBox();
            this.askForOptioonsGroup = new System.Windows.Forms.GroupBox();
            this.otherOptionsGroup = new System.Windows.Forms.GroupBox();
            this.FullScreen = new System.Windows.Forms.CheckBox();
            this.HideCursor = new System.Windows.Forms.CheckBox();
            this.EnableTimer = new System.Windows.Forms.CheckBox();
            this.DisableJoin = new System.Windows.Forms.CheckBox();
            this.adminOptionsGroup = new System.Windows.Forms.GroupBox();
            this.buildingOptionsGroup = new System.Windows.Forms.GroupBox();
            this.printerOptionsGroup = new System.Windows.Forms.GroupBox();
            this.advancedPrinterOptionsGroup = new System.Windows.Forms.GroupBox();
            this.labelOptionsGroup = new System.Windows.Forms.GroupBox();
            this.UseOldDatamaxFormat = new System.Windows.Forms.CheckBox();
            this.mainOptionsGroup = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.SettingsCombo = new System.Windows.Forms.ComboBox();
            this.askForOptioonsGroup.SuspendLayout();
            this.otherOptionsGroup.SuspendLayout();
            this.adminOptionsGroup.SuspendLayout();
            this.buildingOptionsGroup.SuspendLayout();
            this.printerOptionsGroup.SuspendLayout();
            this.advancedPrinterOptionsGroup.SuspendLayout();
            this.labelOptionsGroup.SuspendLayout();
            this.mainOptionsGroup.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // PrintKiosks
            // 
            this.PrintKiosks.Enabled = false;
            this.PrintKiosks.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PrintKiosks.Location = new System.Drawing.Point(14, 95);
            this.PrintKiosks.Name = "PrintKiosks";
            this.PrintKiosks.Size = new System.Drawing.Size(230, 22);
            this.PrintKiosks.TabIndex = 7;
            // 
            // KiosksToPrintForLabel
            // 
            this.KiosksToPrintForLabel.AutoSize = true;
            this.KiosksToPrintForLabel.Enabled = false;
            this.KiosksToPrintForLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KiosksToPrintForLabel.Location = new System.Drawing.Point(11, 76);
            this.KiosksToPrintForLabel.Name = "KiosksToPrintForLabel";
            this.KiosksToPrintForLabel.Size = new System.Drawing.Size(123, 16);
            this.KiosksToPrintForLabel.TabIndex = 118;
            this.KiosksToPrintForLabel.Text = "Kiosks To Print For:";
            this.KiosksToPrintForLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PrintMode
            // 
            this.PrintMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PrintMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PrintMode.FormattingEnabled = true;
            this.PrintMode.Items.AddRange(new object[] {
            "Print To Printer",
            "Print To Server",
            "Print From Server",
            "Cloud Printing",
            "No Printer"});
            this.PrintMode.Location = new System.Drawing.Point(14, 43);
            this.PrintMode.Name = "PrintMode";
            this.PrintMode.Size = new System.Drawing.Size(230, 24);
            this.PrintMode.TabIndex = 6;
            this.PrintMode.SelectedIndexChanged += new System.EventHandler(this.onPrintModeChanged);
            // 
            // Printer
            // 
            this.Printer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Printer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Printer.FormattingEnabled = true;
            this.Printer.Location = new System.Drawing.Point(14, 147);
            this.Printer.Name = "Printer";
            this.Printer.Size = new System.Drawing.Size(230, 24);
            this.Printer.TabIndex = 8;
            this.Printer.SelectedIndexChanged += new System.EventHandler(this.onPrinterChanged);
            // 
            // PrinterLabel
            // 
            this.PrinterLabel.AutoSize = true;
            this.PrinterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PrinterLabel.Location = new System.Drawing.Point(11, 128);
            this.PrinterLabel.Name = "PrinterLabel";
            this.PrinterLabel.Size = new System.Drawing.Size(49, 16);
            this.PrinterLabel.TabIndex = 120;
            this.PrinterLabel.Text = "Printer:";
            this.PrinterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DisableLocationLabels
            // 
            this.DisableLocationLabels.AutoSize = true;
            this.DisableLocationLabels.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DisableLocationLabels.Location = new System.Drawing.Point(16, 21);
            this.DisableLocationLabels.Name = "DisableLocationLabels";
            this.DisableLocationLabels.Size = new System.Drawing.Size(172, 20);
            this.DisableLocationLabels.TabIndex = 14;
            this.DisableLocationLabels.Tag = "";
            this.DisableLocationLabels.Text = "Disable Location Labels";
            this.DisableLocationLabels.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(11, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 16);
            this.label2.TabIndex = 123;
            this.label2.Text = "Print Mode:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BuildingAccessMode
            // 
            this.BuildingAccessMode.AutoSize = true;
            this.BuildingAccessMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BuildingAccessMode.Location = new System.Drawing.Point(16, 21);
            this.BuildingAccessMode.Name = "BuildingAccessMode";
            this.BuildingAccessMode.Size = new System.Drawing.Size(113, 20);
            this.BuildingAccessMode.TabIndex = 17;
            this.BuildingAccessMode.Text = "Building Mode";
            this.BuildingAccessMode.UseVisualStyleBackColor = true;
            this.BuildingAccessMode.CheckedChanged += new System.EventHandler(this.onBuildingModeChanged);
            // 
            // Building
            // 
            this.Building.Enabled = false;
            this.Building.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Building.Location = new System.Drawing.Point(74, 47);
            this.Building.Name = "Building";
            this.Building.Size = new System.Drawing.Size(150, 22);
            this.Building.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 16);
            this.label3.TabIndex = 133;
            this.label3.Text = "Building:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PrintTest
            // 
            this.PrintTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PrintTest.Location = new System.Drawing.Point(867, 363);
            this.PrintTest.Name = "PrintTest";
            this.PrintTest.Size = new System.Drawing.Size(62, 23);
            this.PrintTest.TabIndex = 8;
            this.PrintTest.TabStop = false;
            this.PrintTest.Text = "Test";
            this.PrintTest.UseVisualStyleBackColor = true;
            this.PrintTest.Click += new System.EventHandler(this.onPrintTestClick);
            // 
            // FormatLabel
            // 
            this.FormatLabel.AutoSize = true;
            this.FormatLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormatLabel.Location = new System.Drawing.Point(15, 367);
            this.FormatLabel.Name = "FormatLabel";
            this.FormatLabel.Size = new System.Drawing.Size(149, 16);
            this.FormatLabel.TabIndex = 135;
            this.FormatLabel.Text = "Test Label Print Format:";
            // 
            // LabelFormat
            // 
            this.LabelFormat.AcceptsReturn = true;
            this.LabelFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelFormat.Location = new System.Drawing.Point(12, 392);
            this.LabelFormat.Multiline = true;
            this.LabelFormat.Name = "LabelFormat";
            this.LabelFormat.Size = new System.Drawing.Size(920, 206);
            this.LabelFormat.TabIndex = 136;
            this.LabelFormat.TabStop = false;
            // 
            // LoadLabelList
            // 
            this.LoadLabelList.Location = new System.Drawing.Point(703, 363);
            this.LoadLabelList.Name = "LoadLabelList";
            this.LoadLabelList.Size = new System.Drawing.Size(73, 23);
            this.LoadLabelList.TabIndex = 138;
            this.LoadLabelList.TabStop = false;
            this.LoadLabelList.Text = "Load List";
            this.LoadLabelList.UseVisualStyleBackColor = true;
            this.LoadLabelList.Click += new System.EventHandler(this.onLoadLabelsClick);
            // 
            // LabelList
            // 
            this.LabelList.FormattingEnabled = true;
            this.LabelList.Location = new System.Drawing.Point(544, 364);
            this.LabelList.MaxDropDownItems = 16;
            this.LabelList.Name = "LabelList";
            this.LabelList.Size = new System.Drawing.Size(143, 24);
            this.LabelList.TabIndex = 139;
            this.LabelList.TabStop = false;
            this.LabelList.SelectedIndexChanged += new System.EventHandler(this.onLabelListChanged);
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(493, 368);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(48, 16);
            this.NameLabel.TabIndex = 142;
            this.NameLabel.Text = "Name:";
            // 
            // SaveLabel
            // 
            this.SaveLabel.Location = new System.Drawing.Point(785, 363);
            this.SaveLabel.Name = "SaveLabel";
            this.SaveLabel.Size = new System.Drawing.Size(75, 23);
            this.SaveLabel.TabIndex = 143;
            this.SaveLabel.TabStop = false;
            this.SaveLabel.Text = "Save";
            this.SaveLabel.UseVisualStyleBackColor = true;
            this.SaveLabel.Click += new System.EventHandler(this.onSaveLabelClick);
            // 
            // LabelPrinterSize
            // 
            this.LabelPrinterSize.AutoSize = true;
            this.LabelPrinterSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelPrinterSize.Location = new System.Drawing.Point(164, 367);
            this.LabelPrinterSize.Name = "LabelPrinterSize";
            this.LabelPrinterSize.Size = new System.Drawing.Size(108, 16);
            this.LabelPrinterSize.TabIndex = 144;
            this.LabelPrinterSize.Text = "Printer Size Here";
            // 
            // PageWidthLabel
            // 
            this.PageWidthLabel.AutoSize = true;
            this.PageWidthLabel.Enabled = false;
            this.PageWidthLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PageWidthLabel.Location = new System.Drawing.Point(13, 50);
            this.PageWidthLabel.Name = "PageWidthLabel";
            this.PageWidthLabel.Size = new System.Drawing.Size(24, 16);
            this.PageWidthLabel.TabIndex = 145;
            this.PageWidthLabel.Text = "W:";
            this.PageWidthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PrinterWidth
            // 
            this.PrinterWidth.Enabled = false;
            this.PrinterWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PrinterWidth.Location = new System.Drawing.Point(38, 47);
            this.PrinterWidth.Name = "PrinterWidth";
            this.PrinterWidth.Size = new System.Drawing.Size(45, 22);
            this.PrinterWidth.TabIndex = 9;
            this.PrinterWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.PrinterWidth.TextChanged += new System.EventHandler(this.limitNumbersOnly);
            // 
            // PageHeightLabel
            // 
            this.PageHeightLabel.AutoSize = true;
            this.PageHeightLabel.Enabled = false;
            this.PageHeightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PageHeightLabel.Location = new System.Drawing.Point(89, 50);
            this.PageHeightLabel.Name = "PageHeightLabel";
            this.PageHeightLabel.Size = new System.Drawing.Size(21, 16);
            this.PageHeightLabel.TabIndex = 147;
            this.PageHeightLabel.Text = "H:";
            this.PageHeightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PrinterHeight
            // 
            this.PrinterHeight.Enabled = false;
            this.PrinterHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PrinterHeight.Location = new System.Drawing.Point(111, 47);
            this.PrinterHeight.Name = "PrinterHeight";
            this.PrinterHeight.Size = new System.Drawing.Size(45, 22);
            this.PrinterHeight.TabIndex = 10;
            this.PrinterHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.PrinterHeight.TextChanged += new System.EventHandler(this.limitNumbersOnly);
            // 
            // SizeFromPrinter
            // 
            this.SizeFromPrinter.Enabled = false;
            this.SizeFromPrinter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SizeFromPrinter.Location = new System.Drawing.Point(172, 43);
            this.SizeFromPrinter.Name = "SizeFromPrinter";
            this.SizeFromPrinter.Size = new System.Drawing.Size(72, 26);
            this.SizeFromPrinter.TabIndex = 11;
            this.SizeFromPrinter.Text = "From Printer";
            this.SizeFromPrinter.UseVisualStyleBackColor = true;
            this.SizeFromPrinter.Click += new System.EventHandler(this.onFromPrinterClick);
            // 
            // AdvancedPageSize
            // 
            this.AdvancedPageSize.AutoSize = true;
            this.AdvancedPageSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AdvancedPageSize.Location = new System.Drawing.Point(16, 21);
            this.AdvancedPageSize.Name = "AdvancedPageSize";
            this.AdvancedPageSize.Size = new System.Drawing.Size(182, 20);
            this.AdvancedPageSize.TabIndex = 16;
            this.AdvancedPageSize.Text = "Use Advanced Page Size";
            this.AdvancedPageSize.UseVisualStyleBackColor = true;
            this.AdvancedPageSize.CheckedChanged += new System.EventHandler(this.onAdvancedPageSizeChanged);
            // 
            // AdminPIN
            // 
            this.AdminPIN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AdminPIN.Location = new System.Drawing.Point(9, 42);
            this.AdminPIN.Name = "AdminPIN";
            this.AdminPIN.Size = new System.Drawing.Size(148, 22);
            this.AdminPIN.TabIndex = 4;
            this.AdminPIN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.AdminPIN.TextChanged += new System.EventHandler(this.limitNumbersOnly);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(6, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(74, 16);
            this.label9.TabIndex = 154;
            this.label9.Text = "Admin PIN:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // AdminPINTimeout
            // 
            this.AdminPINTimeout.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AdminPINTimeout.Location = new System.Drawing.Point(7, 93);
            this.AdminPINTimeout.Name = "AdminPINTimeout";
            this.AdminPINTimeout.Size = new System.Drawing.Size(66, 22);
            this.AdminPINTimeout.TabIndex = 5;
            this.AdminPINTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.AdminPINTimeout.TextChanged += new System.EventHandler(this.limitNumbersOnly);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(6, 74);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(85, 16);
            this.label11.TabIndex = 156;
            this.label11.Text = "PIN Timeout:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ExtraBlankLabel
            // 
            this.ExtraBlankLabel.AutoSize = true;
            this.ExtraBlankLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExtraBlankLabel.Location = new System.Drawing.Point(16, 52);
            this.ExtraBlankLabel.Name = "ExtraBlankLabel";
            this.ExtraBlankLabel.Size = new System.Drawing.Size(160, 20);
            this.ExtraBlankLabel.TabIndex = 157;
            this.ExtraBlankLabel.Tag = "";
            this.ExtraBlankLabel.Text = "Print Extra Blank Label";
            this.ExtraBlankLabel.UseVisualStyleBackColor = true;
            // 
            // AskChurchName
            // 
            this.AskChurchName.AutoSize = true;
            this.AskChurchName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AskChurchName.Location = new System.Drawing.Point(99, 52);
            this.AskChurchName.Name = "AskChurchName";
            this.AskChurchName.Size = new System.Drawing.Size(108, 20);
            this.AskChurchName.TabIndex = 160;
            this.AskChurchName.Text = "Church Name";
            this.AskChurchName.UseVisualStyleBackColor = true;
            // 
            // AskChurch
            // 
            this.AskChurch.AutoSize = true;
            this.AskChurch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AskChurch.Location = new System.Drawing.Point(16, 52);
            this.AskChurch.Name = "AskChurch";
            this.AskChurch.Size = new System.Drawing.Size(68, 20);
            this.AskChurch.TabIndex = 161;
            this.AskChurch.Text = "Church";
            this.AskChurch.UseVisualStyleBackColor = true;
            // 
            // AskGrade
            // 
            this.AskGrade.AutoSize = true;
            this.AskGrade.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AskGrade.Location = new System.Drawing.Point(16, 21);
            this.AskGrade.Name = "AskGrade";
            this.AskGrade.Size = new System.Drawing.Size(65, 20);
            this.AskGrade.TabIndex = 159;
            this.AskGrade.Text = "Grade";
            this.AskGrade.UseVisualStyleBackColor = true;
            // 
            // AskFriend
            // 
            this.AskFriend.AutoSize = true;
            this.AskFriend.Checked = true;
            this.AskFriend.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AskFriend.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AskFriend.Location = new System.Drawing.Point(99, 21);
            this.AskFriend.Name = "AskFriend";
            this.AskFriend.Size = new System.Drawing.Size(137, 20);
            this.AskFriend.TabIndex = 158;
            this.AskFriend.Text = "Emergency Friend";
            this.AskFriend.UseVisualStyleBackColor = true;
            // 
            // SecurityLabelPerChild
            // 
            this.SecurityLabelPerChild.AutoSize = true;
            this.SecurityLabelPerChild.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SecurityLabelPerChild.Location = new System.Drawing.Point(16, 83);
            this.SecurityLabelPerChild.Name = "SecurityLabelPerChild";
            this.SecurityLabelPerChild.Size = new System.Drawing.Size(221, 20);
            this.SecurityLabelPerChild.TabIndex = 175;
            this.SecurityLabelPerChild.Tag = "";
            this.SecurityLabelPerChild.Text = "Security Label Per Child/Meeting";
            this.SecurityLabelPerChild.UseVisualStyleBackColor = true;
            // 
            // KioskName
            // 
            this.KioskName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KioskName.Location = new System.Drawing.Point(11, 199);
            this.KioskName.Name = "KioskName";
            this.KioskName.Size = new System.Drawing.Size(233, 22);
            this.KioskName.TabIndex = 173;
            // 
            // KioskNameLabel
            // 
            this.KioskNameLabel.AutoSize = true;
            this.KioskNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KioskNameLabel.Location = new System.Drawing.Point(11, 180);
            this.KioskNameLabel.Name = "KioskNameLabel";
            this.KioskNameLabel.Size = new System.Drawing.Size(84, 16);
            this.KioskNameLabel.TabIndex = 172;
            this.KioskNameLabel.Text = "Kiosk Name:";
            this.KioskNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LateMinutes
            // 
            this.LateMinutes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LateMinutes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LateMinutes.FormattingEnabled = true;
            this.LateMinutes.Items.AddRange(new object[] {
            "30",
            "45",
            "60",
            "90",
            "120",
            "180",
            "240",
            "1440"});
            this.LateMinutes.Location = new System.Drawing.Point(14, 199);
            this.LateMinutes.Name = "LateMinutes";
            this.LateMinutes.Size = new System.Drawing.Size(210, 24);
            this.LateMinutes.TabIndex = 169;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(11, 180);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(140, 16);
            this.label7.TabIndex = 168;
            this.label7.Text = "Late Check In Minutes:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // EarlyHours
            // 
            this.EarlyHours.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EarlyHours.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EarlyHours.FormattingEnabled = true;
            this.EarlyHours.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "24"});
            this.EarlyHours.Location = new System.Drawing.Point(14, 147);
            this.EarlyHours.Name = "EarlyHours";
            this.EarlyHours.Size = new System.Drawing.Size(210, 24);
            this.EarlyHours.TabIndex = 167;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(11, 128);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(135, 16);
            this.label8.TabIndex = 166;
            this.label8.Text = "Early Check In Hours:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(11, 76);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(91, 16);
            this.label12.TabIndex = 164;
            this.label12.Text = "Day Of Week:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(11, 24);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(61, 16);
            this.label13.TabIndex = 162;
            this.label13.Text = "Campus:";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DayOfWeekCombo
            // 
            this.DayOfWeekCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DayOfWeekCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DayOfWeekCombo.FormattingEnabled = true;
            this.DayOfWeekCombo.Location = new System.Drawing.Point(14, 95);
            this.DayOfWeekCombo.Name = "DayOfWeekCombo";
            this.DayOfWeekCombo.Size = new System.Drawing.Size(210, 24);
            this.DayOfWeekCombo.TabIndex = 165;
            // 
            // CampusCombo
            // 
            this.CampusCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CampusCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CampusCombo.FormattingEnabled = true;
            this.CampusCombo.Location = new System.Drawing.Point(14, 43);
            this.CampusCombo.Name = "CampusCombo";
            this.CampusCombo.Size = new System.Drawing.Size(210, 24);
            this.CampusCombo.TabIndex = 163;
            // 
            // askForOptioonsGroup
            // 
            this.askForOptioonsGroup.Controls.Add(this.AskChurch);
            this.askForOptioonsGroup.Controls.Add(this.AskGrade);
            this.askForOptioonsGroup.Controls.Add(this.AskChurchName);
            this.askForOptioonsGroup.Controls.Add(this.AskFriend);
            this.askForOptioonsGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.askForOptioonsGroup.Location = new System.Drawing.Point(518, 164);
            this.askForOptioonsGroup.Name = "askForOptioonsGroup";
            this.askForOptioonsGroup.Size = new System.Drawing.Size(242, 87);
            this.askForOptioonsGroup.TabIndex = 176;
            this.askForOptioonsGroup.TabStop = false;
            this.askForOptioonsGroup.Text = "Ask For Options";
            // 
            // otherOptionsGroup
            // 
            this.otherOptionsGroup.Controls.Add(this.FullScreen);
            this.otherOptionsGroup.Controls.Add(this.HideCursor);
            this.otherOptionsGroup.Controls.Add(this.EnableTimer);
            this.otherOptionsGroup.Controls.Add(this.DisableJoin);
            this.otherOptionsGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.otherOptionsGroup.Location = new System.Drawing.Point(518, 257);
            this.otherOptionsGroup.Name = "otherOptionsGroup";
            this.otherOptionsGroup.Size = new System.Drawing.Size(242, 87);
            this.otherOptionsGroup.TabIndex = 177;
            this.otherOptionsGroup.TabStop = false;
            this.otherOptionsGroup.Text = "Other Options";
            // 
            // FullScreen
            // 
            this.FullScreen.AutoSize = true;
            this.FullScreen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FullScreen.Location = new System.Drawing.Point(16, 21);
            this.FullScreen.Name = "FullScreen";
            this.FullScreen.Size = new System.Drawing.Size(94, 20);
            this.FullScreen.TabIndex = 13;
            this.FullScreen.Text = "Full Screen";
            this.FullScreen.UseVisualStyleBackColor = true;
            // 
            // HideCursor
            // 
            this.HideCursor.AutoSize = true;
            this.HideCursor.Checked = true;
            this.HideCursor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.HideCursor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideCursor.Location = new System.Drawing.Point(16, 52);
            this.HideCursor.Name = "HideCursor";
            this.HideCursor.Size = new System.Drawing.Size(98, 20);
            this.HideCursor.TabIndex = 171;
            this.HideCursor.Text = "Hide Cursor";
            this.HideCursor.UseVisualStyleBackColor = true;
            // 
            // EnableTimer
            // 
            this.EnableTimer.AutoSize = true;
            this.EnableTimer.Checked = true;
            this.EnableTimer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.EnableTimer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnableTimer.Location = new System.Drawing.Point(128, 21);
            this.EnableTimer.Name = "EnableTimer";
            this.EnableTimer.Size = new System.Drawing.Size(108, 20);
            this.EnableTimer.TabIndex = 170;
            this.EnableTimer.Text = "Enable Timer";
            this.EnableTimer.UseVisualStyleBackColor = true;
            // 
            // DisableJoin
            // 
            this.DisableJoin.AutoSize = true;
            this.DisableJoin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DisableJoin.Location = new System.Drawing.Point(128, 52);
            this.DisableJoin.Name = "DisableJoin";
            this.DisableJoin.Size = new System.Drawing.Size(102, 20);
            this.DisableJoin.TabIndex = 174;
            this.DisableJoin.Text = "Disable Join";
            this.DisableJoin.UseVisualStyleBackColor = true;
            // 
            // adminOptionsGroup
            // 
            this.adminOptionsGroup.Controls.Add(this.AdminPIN);
            this.adminOptionsGroup.Controls.Add(this.label9);
            this.adminOptionsGroup.Controls.Add(this.AdminPINTimeout);
            this.adminOptionsGroup.Controls.Add(this.label11);
            this.adminOptionsGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.adminOptionsGroup.Location = new System.Drawing.Point(766, 12);
            this.adminOptionsGroup.Name = "adminOptionsGroup";
            this.adminOptionsGroup.Size = new System.Drawing.Size(166, 128);
            this.adminOptionsGroup.TabIndex = 178;
            this.adminOptionsGroup.TabStop = false;
            this.adminOptionsGroup.Text = "Admin Options";
            // 
            // buildingOptionsGroup
            // 
            this.buildingOptionsGroup.Controls.Add(this.Building);
            this.buildingOptionsGroup.Controls.Add(this.BuildingAccessMode);
            this.buildingOptionsGroup.Controls.Add(this.label3);
            this.buildingOptionsGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buildingOptionsGroup.Location = new System.Drawing.Point(12, 257);
            this.buildingOptionsGroup.Name = "buildingOptionsGroup";
            this.buildingOptionsGroup.Size = new System.Drawing.Size(235, 87);
            this.buildingOptionsGroup.TabIndex = 179;
            this.buildingOptionsGroup.TabStop = false;
            this.buildingOptionsGroup.Text = "Building Options";
            // 
            // printerOptionsGroup
            // 
            this.printerOptionsGroup.Controls.Add(this.PrintMode);
            this.printerOptionsGroup.Controls.Add(this.label2);
            this.printerOptionsGroup.Controls.Add(this.PrintKiosks);
            this.printerOptionsGroup.Controls.Add(this.KiosksToPrintForLabel);
            this.printerOptionsGroup.Controls.Add(this.Printer);
            this.printerOptionsGroup.Controls.Add(this.PrinterLabel);
            this.printerOptionsGroup.Controls.Add(this.KioskName);
            this.printerOptionsGroup.Controls.Add(this.KioskNameLabel);
            this.printerOptionsGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printerOptionsGroup.Location = new System.Drawing.Point(253, 12);
            this.printerOptionsGroup.Name = "printerOptionsGroup";
            this.printerOptionsGroup.Size = new System.Drawing.Size(259, 239);
            this.printerOptionsGroup.TabIndex = 180;
            this.printerOptionsGroup.TabStop = false;
            this.printerOptionsGroup.Text = "Printer Options";
            // 
            // advancedPrinterOptionsGroup
            // 
            this.advancedPrinterOptionsGroup.Controls.Add(this.PageHeightLabel);
            this.advancedPrinterOptionsGroup.Controls.Add(this.PageWidthLabel);
            this.advancedPrinterOptionsGroup.Controls.Add(this.PrinterWidth);
            this.advancedPrinterOptionsGroup.Controls.Add(this.PrinterHeight);
            this.advancedPrinterOptionsGroup.Controls.Add(this.SizeFromPrinter);
            this.advancedPrinterOptionsGroup.Controls.Add(this.AdvancedPageSize);
            this.advancedPrinterOptionsGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.advancedPrinterOptionsGroup.Location = new System.Drawing.Point(253, 257);
            this.advancedPrinterOptionsGroup.Name = "advancedPrinterOptionsGroup";
            this.advancedPrinterOptionsGroup.Size = new System.Drawing.Size(259, 87);
            this.advancedPrinterOptionsGroup.TabIndex = 181;
            this.advancedPrinterOptionsGroup.TabStop = false;
            this.advancedPrinterOptionsGroup.Text = "Advanced Printer Options";
            // 
            // labelOptionsGroup
            // 
            this.labelOptionsGroup.Controls.Add(this.UseOldDatamaxFormat);
            this.labelOptionsGroup.Controls.Add(this.DisableLocationLabels);
            this.labelOptionsGroup.Controls.Add(this.ExtraBlankLabel);
            this.labelOptionsGroup.Controls.Add(this.SecurityLabelPerChild);
            this.labelOptionsGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOptionsGroup.Location = new System.Drawing.Point(518, 12);
            this.labelOptionsGroup.Name = "labelOptionsGroup";
            this.labelOptionsGroup.Size = new System.Drawing.Size(242, 144);
            this.labelOptionsGroup.TabIndex = 182;
            this.labelOptionsGroup.TabStop = false;
            this.labelOptionsGroup.Text = "Label Options";
            // 
            // UseOldDatamaxFormat
            // 
            this.UseOldDatamaxFormat.AutoSize = true;
            this.UseOldDatamaxFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseOldDatamaxFormat.Location = new System.Drawing.Point(16, 114);
            this.UseOldDatamaxFormat.Name = "UseOldDatamaxFormat";
            this.UseOldDatamaxFormat.Size = new System.Drawing.Size(178, 20);
            this.UseOldDatamaxFormat.TabIndex = 176;
            this.UseOldDatamaxFormat.Text = "Use Old Datamax Format";
            this.UseOldDatamaxFormat.UseVisualStyleBackColor = true;
            // 
            // mainOptionsGroup
            // 
            this.mainOptionsGroup.Controls.Add(this.CampusCombo);
            this.mainOptionsGroup.Controls.Add(this.DayOfWeekCombo);
            this.mainOptionsGroup.Controls.Add(this.label13);
            this.mainOptionsGroup.Controls.Add(this.label12);
            this.mainOptionsGroup.Controls.Add(this.label8);
            this.mainOptionsGroup.Controls.Add(this.EarlyHours);
            this.mainOptionsGroup.Controls.Add(this.label7);
            this.mainOptionsGroup.Controls.Add(this.LateMinutes);
            this.mainOptionsGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainOptionsGroup.Location = new System.Drawing.Point(12, 12);
            this.mainOptionsGroup.Name = "mainOptionsGroup";
            this.mainOptionsGroup.Size = new System.Drawing.Size(235, 239);
            this.mainOptionsGroup.TabIndex = 183;
            this.mainOptionsGroup.TabStop = false;
            this.mainOptionsGroup.Text = "Main Options";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(778, 271);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(143, 60);
            this.button1.TabIndex = 184;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.onStartClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.SettingsCombo);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(766, 146);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(166, 105);
            this.groupBox2.TabIndex = 185;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Load/Save";
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(9, 59);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(148, 30);
            this.button2.TabIndex = 1;
            this.button2.Text = "Save";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.onSettingsSave);
            // 
            // SettingsCombo
            // 
            this.SettingsCombo.FormattingEnabled = true;
            this.SettingsCombo.Location = new System.Drawing.Point(9, 22);
            this.SettingsCombo.Name = "SettingsCombo";
            this.SettingsCombo.Size = new System.Drawing.Size(148, 24);
            this.SettingsCombo.TabIndex = 0;
            this.SettingsCombo.SelectedIndexChanged += new System.EventHandler(this.onSettingsNameChanged);
            // 
            // LoginSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(944, 602);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.mainOptionsGroup);
            this.Controls.Add(this.labelOptionsGroup);
            this.Controls.Add(this.advancedPrinterOptionsGroup);
            this.Controls.Add(this.otherOptionsGroup);
            this.Controls.Add(this.printerOptionsGroup);
            this.Controls.Add(this.buildingOptionsGroup);
            this.Controls.Add(this.adminOptionsGroup);
            this.Controls.Add(this.askForOptioonsGroup);
            this.Controls.Add(this.LabelPrinterSize);
            this.Controls.Add(this.SaveLabel);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.LabelList);
            this.Controls.Add(this.LoadLabelList);
            this.Controls.Add(this.LabelFormat);
            this.Controls.Add(this.FormatLabel);
            this.Controls.Add(this.PrintTest);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginSettings";
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.onLoginSettingsClosing);
            this.Load += new System.EventHandler(this.onLoginSettingsLoad);
            this.Move += new System.EventHandler(this.onLoginSettingsMove);
            this.askForOptioonsGroup.ResumeLayout(false);
            this.askForOptioonsGroup.PerformLayout();
            this.otherOptionsGroup.ResumeLayout(false);
            this.otherOptionsGroup.PerformLayout();
            this.adminOptionsGroup.ResumeLayout(false);
            this.adminOptionsGroup.PerformLayout();
            this.buildingOptionsGroup.ResumeLayout(false);
            this.buildingOptionsGroup.PerformLayout();
            this.printerOptionsGroup.ResumeLayout(false);
            this.printerOptionsGroup.PerformLayout();
            this.advancedPrinterOptionsGroup.ResumeLayout(false);
            this.advancedPrinterOptionsGroup.PerformLayout();
            this.labelOptionsGroup.ResumeLayout(false);
            this.labelOptionsGroup.PerformLayout();
            this.mainOptionsGroup.ResumeLayout(false);
            this.mainOptionsGroup.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.TextBox PrintKiosks;
		private System.Windows.Forms.Label KiosksToPrintForLabel;
		public System.Windows.Forms.ComboBox PrintMode;
		public System.Windows.Forms.ComboBox Printer;
		private System.Windows.Forms.Label PrinterLabel;
		public System.Windows.Forms.CheckBox DisableLocationLabels;
		private System.Windows.Forms.Label label2;
		public System.Windows.Forms.CheckBox BuildingAccessMode;
		public System.Windows.Forms.TextBox Building;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button PrintTest;
		private System.Windows.Forms.Label FormatLabel;
		private System.Windows.Forms.TextBox LabelFormat;
		private System.Windows.Forms.Button LoadLabelList;
		private System.Windows.Forms.ComboBox LabelList;
		private System.Windows.Forms.Label NameLabel;
		private System.Windows.Forms.Button SaveLabel;
		private System.Windows.Forms.Label LabelPrinterSize;
		private System.Windows.Forms.Label PageWidthLabel;
		public System.Windows.Forms.TextBox PrinterWidth;
		private System.Windows.Forms.Label PageHeightLabel;
		public System.Windows.Forms.TextBox PrinterHeight;
		private System.Windows.Forms.Button SizeFromPrinter;
		public System.Windows.Forms.CheckBox AdvancedPageSize;
		public System.Windows.Forms.TextBox AdminPIN;
		private System.Windows.Forms.Label label9;
		public System.Windows.Forms.TextBox AdminPINTimeout;
		private System.Windows.Forms.Label label11;
		public System.Windows.Forms.CheckBox ExtraBlankLabel;
		public System.Windows.Forms.CheckBox AskChurchName;
		public System.Windows.Forms.CheckBox AskChurch;
		public System.Windows.Forms.CheckBox AskGrade;
		public System.Windows.Forms.CheckBox AskFriend;
		public System.Windows.Forms.CheckBox SecurityLabelPerChild;
		public System.Windows.Forms.TextBox KioskName;
		private System.Windows.Forms.Label KioskNameLabel;
		public System.Windows.Forms.ComboBox LateMinutes;
		private System.Windows.Forms.Label label7;
		public System.Windows.Forms.ComboBox EarlyHours;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.ComboBox DayOfWeekCombo;
		private System.Windows.Forms.ComboBox CampusCombo;
		private System.Windows.Forms.GroupBox askForOptioonsGroup;
		private System.Windows.Forms.GroupBox otherOptionsGroup;
		public System.Windows.Forms.CheckBox FullScreen;
		public System.Windows.Forms.CheckBox HideCursor;
		public System.Windows.Forms.CheckBox EnableTimer;
		public System.Windows.Forms.CheckBox DisableJoin;
		private System.Windows.Forms.GroupBox adminOptionsGroup;
		private System.Windows.Forms.GroupBox buildingOptionsGroup;
		private System.Windows.Forms.GroupBox printerOptionsGroup;
		private System.Windows.Forms.GroupBox advancedPrinterOptionsGroup;
		private System.Windows.Forms.GroupBox labelOptionsGroup;
		private System.Windows.Forms.GroupBox mainOptionsGroup;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ComboBox SettingsCombo;
		private System.Windows.Forms.CheckBox UseOldDatamaxFormat;
	}
}
