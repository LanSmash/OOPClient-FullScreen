namespace OOPClient
{
    partial class GUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUI));
            this.txtAtemAddress = new System.Windows.Forms.TextBox();
            this.btnAtemConnect = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panelATEM = new System.Windows.Forms.GroupBox();
            this.panelPrev = new System.Windows.Forms.GroupBox();
            this.prevBtn8 = new System.Windows.Forms.Button();
            this.prevBtn7 = new System.Windows.Forms.Button();
            this.prevBtn0 = new System.Windows.Forms.Button();
            this.prevBtn6 = new System.Windows.Forms.Button();
            this.prevBtn3 = new System.Windows.Forms.Button();
            this.prevBtn5 = new System.Windows.Forms.Button();
            this.prevBtn1 = new System.Windows.Forms.Button();
            this.prevBtn4 = new System.Windows.Forms.Button();
            this.prevBtn2 = new System.Windows.Forms.Button();
            this.panelProg = new System.Windows.Forms.GroupBox();
            this.progBtn8 = new System.Windows.Forms.Button();
            this.progBtn7 = new System.Windows.Forms.Button();
            this.progBtn0 = new System.Windows.Forms.Button();
            this.progBtn6 = new System.Windows.Forms.Button();
            this.progBtn5 = new System.Windows.Forms.Button();
            this.progBtn4 = new System.Windows.Forms.Button();
            this.progBtn3 = new System.Windows.Forms.Button();
            this.progBtn2 = new System.Windows.Forms.Button();
            this.progBtn1 = new System.Windows.Forms.Button();
            this.slcControlIn = new System.Windows.Forms.ComboBox();
            this.slcControlOut = new System.Windows.Forms.ComboBox();
            this.slcAudioOut = new System.Windows.Forms.ComboBox();
            this.slcAudioIn = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnControlConnect = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnGetMidi = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.panelATEM.SuspendLayout();
            this.panelPrev.SuspendLayout();
            this.panelProg.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtAtemAddress
            // 
            this.txtAtemAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtAtemAddress, "txtAtemAddress");
            this.txtAtemAddress.Name = "txtAtemAddress";
            // 
            // btnAtemConnect
            // 
            this.btnAtemConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.btnAtemConnect, "btnAtemConnect");
            this.btnAtemConnect.Name = "btnAtemConnect";
            this.btnAtemConnect.UseVisualStyleBackColor = false;
            this.btnAtemConnect.Click += new System.EventHandler(this.btnAtemConnect_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnAtemConnect);
            this.groupBox1.Controls.Add(this.txtAtemAddress);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panelATEM
            // 
            this.panelATEM.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.panelATEM.Controls.Add(this.panelPrev);
            this.panelATEM.Controls.Add(this.panelProg);
            resources.ApplyResources(this.panelATEM, "panelATEM");
            this.panelATEM.Name = "panelATEM";
            this.panelATEM.TabStop = false;
            // 
            // panelPrev
            // 
            this.panelPrev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panelPrev.Controls.Add(this.prevBtn8);
            this.panelPrev.Controls.Add(this.prevBtn7);
            this.panelPrev.Controls.Add(this.prevBtn0);
            this.panelPrev.Controls.Add(this.prevBtn6);
            this.panelPrev.Controls.Add(this.prevBtn3);
            this.panelPrev.Controls.Add(this.prevBtn5);
            this.panelPrev.Controls.Add(this.prevBtn1);
            this.panelPrev.Controls.Add(this.prevBtn4);
            this.panelPrev.Controls.Add(this.prevBtn2);
            resources.ApplyResources(this.panelPrev, "panelPrev");
            this.panelPrev.Name = "panelPrev";
            this.panelPrev.TabStop = false;
            // 
            // prevBtn8
            // 
            this.prevBtn8.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.prevBtn8, "prevBtn8");
            this.prevBtn8.Name = "prevBtn8";
            this.prevBtn8.UseVisualStyleBackColor = false;
            this.prevBtn8.Click += new System.EventHandler(this.changePrev);
            // 
            // prevBtn7
            // 
            this.prevBtn7.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.prevBtn7, "prevBtn7");
            this.prevBtn7.Name = "prevBtn7";
            this.prevBtn7.UseVisualStyleBackColor = false;
            this.prevBtn7.Click += new System.EventHandler(this.changePrev);
            // 
            // prevBtn0
            // 
            this.prevBtn0.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.prevBtn0, "prevBtn0");
            this.prevBtn0.Name = "prevBtn0";
            this.prevBtn0.UseVisualStyleBackColor = false;
            this.prevBtn0.Click += new System.EventHandler(this.changePrev);
            // 
            // prevBtn6
            // 
            this.prevBtn6.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.prevBtn6, "prevBtn6");
            this.prevBtn6.Name = "prevBtn6";
            this.prevBtn6.UseVisualStyleBackColor = false;
            this.prevBtn6.Click += new System.EventHandler(this.changePrev);
            // 
            // prevBtn3
            // 
            this.prevBtn3.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.prevBtn3, "prevBtn3");
            this.prevBtn3.Name = "prevBtn3";
            this.prevBtn3.UseVisualStyleBackColor = false;
            this.prevBtn3.Click += new System.EventHandler(this.changePrev);
            // 
            // prevBtn5
            // 
            this.prevBtn5.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.prevBtn5, "prevBtn5");
            this.prevBtn5.Name = "prevBtn5";
            this.prevBtn5.UseVisualStyleBackColor = false;
            this.prevBtn5.Click += new System.EventHandler(this.changePrev);
            // 
            // prevBtn1
            // 
            this.prevBtn1.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.prevBtn1, "prevBtn1");
            this.prevBtn1.Name = "prevBtn1";
            this.prevBtn1.UseVisualStyleBackColor = false;
            this.prevBtn1.Click += new System.EventHandler(this.changePrev);
            // 
            // prevBtn4
            // 
            this.prevBtn4.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.prevBtn4, "prevBtn4");
            this.prevBtn4.Name = "prevBtn4";
            this.prevBtn4.UseVisualStyleBackColor = false;
            this.prevBtn4.Click += new System.EventHandler(this.changePrev);
            // 
            // prevBtn2
            // 
            this.prevBtn2.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.prevBtn2, "prevBtn2");
            this.prevBtn2.Name = "prevBtn2";
            this.prevBtn2.UseVisualStyleBackColor = false;
            this.prevBtn2.Click += new System.EventHandler(this.changePrev);
            // 
            // panelProg
            // 
            this.panelProg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.panelProg.Controls.Add(this.progBtn8);
            this.panelProg.Controls.Add(this.progBtn7);
            this.panelProg.Controls.Add(this.progBtn0);
            this.panelProg.Controls.Add(this.progBtn6);
            this.panelProg.Controls.Add(this.progBtn5);
            this.panelProg.Controls.Add(this.progBtn4);
            this.panelProg.Controls.Add(this.progBtn3);
            this.panelProg.Controls.Add(this.progBtn2);
            this.panelProg.Controls.Add(this.progBtn1);
            resources.ApplyResources(this.panelProg, "panelProg");
            this.panelProg.Name = "panelProg";
            this.panelProg.TabStop = false;
            // 
            // progBtn8
            // 
            this.progBtn8.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.progBtn8, "progBtn8");
            this.progBtn8.Name = "progBtn8";
            this.progBtn8.UseVisualStyleBackColor = false;
            this.progBtn8.Click += new System.EventHandler(this.changeProg);
            // 
            // progBtn7
            // 
            this.progBtn7.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.progBtn7, "progBtn7");
            this.progBtn7.Name = "progBtn7";
            this.progBtn7.UseVisualStyleBackColor = false;
            this.progBtn7.Click += new System.EventHandler(this.changeProg);
            // 
            // progBtn0
            // 
            this.progBtn0.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.progBtn0, "progBtn0");
            this.progBtn0.Name = "progBtn0";
            this.progBtn0.UseVisualStyleBackColor = false;
            this.progBtn0.Click += new System.EventHandler(this.changeProg);
            // 
            // progBtn6
            // 
            this.progBtn6.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.progBtn6, "progBtn6");
            this.progBtn6.Name = "progBtn6";
            this.progBtn6.UseVisualStyleBackColor = false;
            this.progBtn6.Click += new System.EventHandler(this.changeProg);
            // 
            // progBtn5
            // 
            this.progBtn5.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.progBtn5, "progBtn5");
            this.progBtn5.Name = "progBtn5";
            this.progBtn5.UseVisualStyleBackColor = false;
            this.progBtn5.Click += new System.EventHandler(this.changeProg);
            // 
            // progBtn4
            // 
            this.progBtn4.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.progBtn4, "progBtn4");
            this.progBtn4.Name = "progBtn4";
            this.progBtn4.UseVisualStyleBackColor = false;
            this.progBtn4.Click += new System.EventHandler(this.changeProg);
            // 
            // progBtn3
            // 
            this.progBtn3.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.progBtn3, "progBtn3");
            this.progBtn3.Name = "progBtn3";
            this.progBtn3.UseVisualStyleBackColor = false;
            this.progBtn3.Click += new System.EventHandler(this.changeProg);
            // 
            // progBtn2
            // 
            this.progBtn2.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.progBtn2, "progBtn2");
            this.progBtn2.Name = "progBtn2";
            this.progBtn2.UseVisualStyleBackColor = false;
            this.progBtn2.Click += new System.EventHandler(this.changeProg);
            // 
            // progBtn1
            // 
            this.progBtn1.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.progBtn1, "progBtn1");
            this.progBtn1.Name = "progBtn1";
            this.progBtn1.UseVisualStyleBackColor = false;
            this.progBtn1.Click += new System.EventHandler(this.changeProg);
            // 
            // slcControlIn
            // 
            resources.ApplyResources(this.slcControlIn, "slcControlIn");
            this.slcControlIn.FormattingEnabled = true;
            this.slcControlIn.Items.AddRange(new object[] {
            resources.GetString("slcControlIn.Items")});
            this.slcControlIn.Name = "slcControlIn";
            // 
            // slcControlOut
            // 
            resources.ApplyResources(this.slcControlOut, "slcControlOut");
            this.slcControlOut.FormattingEnabled = true;
            this.slcControlOut.Items.AddRange(new object[] {
            resources.GetString("slcControlOut.Items")});
            this.slcControlOut.Name = "slcControlOut";
            // 
            // slcAudioOut
            // 
            resources.ApplyResources(this.slcAudioOut, "slcAudioOut");
            this.slcAudioOut.FormattingEnabled = true;
            this.slcAudioOut.Items.AddRange(new object[] {
            resources.GetString("slcAudioOut.Items")});
            this.slcAudioOut.Name = "slcAudioOut";
            // 
            // slcAudioIn
            // 
            resources.ApplyResources(this.slcAudioIn, "slcAudioIn");
            this.slcAudioIn.FormattingEnabled = true;
            this.slcAudioIn.Items.AddRange(new object[] {
            resources.GetString("slcAudioIn.Items")});
            this.slcAudioIn.Name = "slcAudioIn";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // btnControlConnect
            // 
            this.btnControlConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.btnControlConnect, "btnControlConnect");
            this.btnControlConnect.Name = "btnControlConnect";
            this.btnControlConnect.UseVisualStyleBackColor = false;
            this.btnControlConnect.Click += new System.EventHandler(this.btnControlConnect_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.groupBox2.Controls.Add(this.btnGetMidi);
            this.groupBox2.Controls.Add(this.slcAudioIn);
            this.groupBox2.Controls.Add(this.slcControlIn);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.slcControlOut);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.slcAudioOut);
            this.groupBox2.Controls.Add(this.btnControlConnect);
            this.groupBox2.Controls.Add(this.label4);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // btnGetMidi
            // 
            this.btnGetMidi.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            resources.ApplyResources(this.btnGetMidi, "btnGetMidi");
            this.btnGetMidi.Name = "btnGetMidi";
            this.btnGetMidi.UseVisualStyleBackColor = false;
            this.btnGetMidi.Click += new System.EventHandler(this.btnGetMidi_Click);
            // 
            // GUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panelATEM);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "GUI";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelATEM.ResumeLayout(false);
            this.panelPrev.ResumeLayout(false);
            this.panelProg.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtAtemAddress;
        private System.Windows.Forms.Button btnAtemConnect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox panelATEM;
        private System.Windows.Forms.GroupBox panelPrev;
        private System.Windows.Forms.Button prevBtn6;
        private System.Windows.Forms.Button prevBtn3;
        private System.Windows.Forms.Button prevBtn5;
        private System.Windows.Forms.Button prevBtn1;
        private System.Windows.Forms.Button prevBtn4;
        private System.Windows.Forms.Button prevBtn2;
        private System.Windows.Forms.GroupBox panelProg;
        private System.Windows.Forms.Button progBtn6;
        private System.Windows.Forms.Button progBtn5;
        private System.Windows.Forms.Button progBtn4;
        private System.Windows.Forms.Button progBtn3;
        private System.Windows.Forms.Button progBtn2;
        private System.Windows.Forms.Button progBtn1;
        private System.Windows.Forms.Button prevBtn0;
        private System.Windows.Forms.Button progBtn0;
        private System.Windows.Forms.Button progBtn8;
        private System.Windows.Forms.Button progBtn7;
        private System.Windows.Forms.Button prevBtn8;
        private System.Windows.Forms.Button prevBtn7;
        private System.Windows.Forms.ComboBox slcControlIn;
        private System.Windows.Forms.ComboBox slcControlOut;
        private System.Windows.Forms.ComboBox slcAudioOut;
        private System.Windows.Forms.ComboBox slcAudioIn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnControlConnect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnGetMidi;
    }
}

