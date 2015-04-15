namespace CmsCheckin
{
    partial class CommonImageResizer
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Label1 = new System.Windows.Forms.Label();
            this.btnTakePicture = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.picZoomIn = new System.Windows.Forms.PictureBox();
            this.picZoomOut = new System.Windows.Forms.PictureBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.tbResize = new System.Windows.Forms.TrackBar();
            this.pctCamera = new System.Windows.Forms.PictureBox();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.nudCropWidth = new System.Windows.Forms.NumericUpDown();
            this.nudCropHeight = new System.Windows.Forms.NumericUpDown();
            this.Label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.picZoomIn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picZoomOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbResize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctCamera)).BeginInit();
            this.GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCropWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCropHeight)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(88, 37);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(33, 21);
            this.Label1.TabIndex = 57;
            this.Label1.Text = "200%";
            // 
            // btnTakePicture
            // 
            this.btnTakePicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTakePicture.BackColor = System.Drawing.Color.LightSalmon;
            this.btnTakePicture.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTakePicture.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTakePicture.Location = new System.Drawing.Point(646, 11);
            this.btnTakePicture.Name = "btnTakePicture";
            this.btnTakePicture.Size = new System.Drawing.Size(121, 75);
            this.btnTakePicture.TabIndex = 44;
            this.btnTakePicture.Text = "Take Picture";
            this.btnTakePicture.UseVisualStyleBackColor = false;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSave.Enabled = false;
            this.btnSave.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(646, 251);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(121, 73);
            this.btnSave.TabIndex = 47;
            this.btnSave.Text = "Send to Server";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // picZoomIn
            // 
            this.picZoomIn.Image = global::CmsCheckin.Properties.Resources.ZoomIn48;
            this.picZoomIn.Location = new System.Drawing.Point(83, 68);
            this.picZoomIn.Name = "picZoomIn";
            this.picZoomIn.Size = new System.Drawing.Size(32, 32);
            this.picZoomIn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picZoomIn.TabIndex = 55;
            this.picZoomIn.TabStop = false;
            this.picZoomIn.Click += new System.EventHandler(this.picZoomIn_Click);
            // 
            // picZoomOut
            // 
            this.picZoomOut.Image = global::CmsCheckin.Properties.Resources.ZoomOut48;
            this.picZoomOut.Location = new System.Drawing.Point(4, 68);
            this.picZoomOut.Name = "picZoomOut";
            this.picZoomOut.Size = new System.Drawing.Size(32, 32);
            this.picZoomOut.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picZoomOut.TabIndex = 54;
            this.picZoomOut.TabStop = false;
            this.picZoomOut.Click += new System.EventHandler(this.picZoomOut_Click);
            // 
            // Label2
            // 
            this.Label2.Location = new System.Drawing.Point(45, 37);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(33, 21);
            this.Label2.TabIndex = 45;
            this.Label2.Text = "100%";
            // 
            // tbResize
            // 
            this.tbResize.Enabled = false;
            this.tbResize.Location = new System.Drawing.Point(0, -9);
            this.tbResize.Maximum = 200;
            this.tbResize.Minimum = 1;
            this.tbResize.Name = "tbResize";
            this.tbResize.Size = new System.Drawing.Size(120, 56);
            this.tbResize.TabIndex = 50;
            this.tbResize.TickFrequency = 10;
            this.tbResize.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.tbResize.Value = 100;
            this.tbResize.ValueChanged += new System.EventHandler(this.tbResize_ValueChanged);
            // 
            // pctCamera
            // 
            this.pctCamera.Location = new System.Drawing.Point(0, 0);
            this.pctCamera.Name = "pctCamera";
            this.pctCamera.Padding = new System.Windows.Forms.Padding(3);
            this.pctCamera.Size = new System.Drawing.Size(640, 480);
            this.pctCamera.TabIndex = 0;
            this.pctCamera.TabStop = false;
            // 
            // GroupBox1
            // 
            this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox1.Controls.Add(this.nudCropWidth);
            this.GroupBox1.Controls.Add(this.nudCropHeight);
            this.GroupBox1.Location = new System.Drawing.Point(13, 479);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(108, 75);
            this.GroupBox1.TabIndex = 49;
            this.GroupBox1.TabStop = false;
            // 
            // nudCropWidth
            // 
            this.nudCropWidth.Enabled = false;
            this.nudCropWidth.Location = new System.Drawing.Point(44, 13);
            this.nudCropWidth.Name = "nudCropWidth";
            this.nudCropWidth.Size = new System.Drawing.Size(50, 22);
            this.nudCropWidth.TabIndex = 6;
            this.nudCropWidth.ValueChanged += new System.EventHandler(this.nudCrop_ValueChanged);
            // 
            // nudCropHeight
            // 
            this.nudCropHeight.Enabled = false;
            this.nudCropHeight.Location = new System.Drawing.Point(44, 37);
            this.nudCropHeight.Name = "nudCropHeight";
            this.nudCropHeight.Size = new System.Drawing.Size(50, 22);
            this.nudCropHeight.TabIndex = 8;
            this.nudCropHeight.ValueChanged += new System.EventHandler(this.nudCrop_ValueChanged);
            // 
            // Label3
            // 
            this.Label3.Location = new System.Drawing.Point(5, 37);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(21, 21);
            this.Label3.TabIndex = 58;
            this.Label3.Text = "1%";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.picZoomIn);
            this.groupBox2.Controls.Add(this.Label1);
            this.groupBox2.Controls.Add(this.Label3);
            this.groupBox2.Controls.Add(this.Label2);
            this.groupBox2.Controls.Add(this.picZoomOut);
            this.groupBox2.Controls.Add(this.tbResize);
            this.groupBox2.Location = new System.Drawing.Point(646, 115);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(122, 106);
            this.groupBox2.TabIndex = 59;
            this.groupBox2.TabStop = false;
            // 
            // ImageResizer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnTakePicture);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.pctCamera);
            this.Controls.Add(this.GroupBox1);
            this.DoubleBuffered = true;
            this.Name = "ImageResizer";
            this.Size = new System.Drawing.Size(773, 485);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ImageResizer_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EasyImageResizerControl_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EasyImageResizerControl_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EasyImageResizerControl_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.picZoomIn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picZoomOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbResize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctCamera)).EndInit();
            this.GroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudCropWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCropHeight)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Button btnTakePicture;
        internal System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.PictureBox picZoomIn;
        internal System.Windows.Forms.PictureBox picZoomOut;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.TrackBar tbResize;
        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.NumericUpDown nudCropWidth;
        internal System.Windows.Forms.NumericUpDown nudCropHeight;
        internal System.Windows.Forms.Label Label3;
        private System.Windows.Forms.GroupBox groupBox2;
        internal System.Windows.Forms.PictureBox pctCamera;
    }
}
