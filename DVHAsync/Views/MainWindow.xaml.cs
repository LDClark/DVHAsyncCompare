﻿using System.Windows;
using System.Windows.Interactivity;

namespace DVHAsyncCompare
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Force Interactivity assembly to be included in build files
            Interaction.GetBehaviors(this);

            InitializeComponent();
        }
    }
}
