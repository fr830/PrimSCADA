﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SUT.PrintEngine.Controls.LoadScreen
{
    public partial class LoadScreen : Form
    {
        public LoadScreen()
        {
            var transparencyKey = Color.White;
            TransparencyKey = transparencyKey;
            BackColor = transparencyKey;
            InitializeComponent();
        }
    }
}
