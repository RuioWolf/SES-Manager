using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SESM;

namespace SESManager
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			Config.Init();
		}
	}
}
