﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace HunterPie.GUIControls.Custom_Controls {
    /// <summary>
    /// Interaction logic for Switcher.xaml
    /// </summary>
    public partial class Switcher : UserControl {



        public string Text {
            get { return GetValue(TextProperty).ToString(); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Switcher));



        public bool RequiresRestart {
            get { return RestartWarning.Visibility == Visibility.Visible; }
            set {
                RestartWarning.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        private bool _isEnabled;
        public new bool IsEnabled {
            get { return _isEnabled; }
            set {
                if (value != _isEnabled) {
                    _isEnabled = value;
                    SwitchAnimation();
                }
            }
        }

        public Switcher() {
            InitializeComponent();
        }

        private void Switcher_OnClick(object sender, MouseButtonEventArgs e) {
            IsEnabled = !IsEnabled;
        }

        public void SwitchAnimation() {
            Storyboard SwitchCircleAnimation = FindResource(IsEnabled ? "SwitchEllipseOn" : "SwitchEllipseOff") as Storyboard;
            Storyboard SwitchBackgroundAnimation = FindResource(IsEnabled ? "BackgroundOn" : "BackgroundOff") as Storyboard;
            Storyboard SwitchForegroundAnimation = FindResource(IsEnabled ? "ForegroundOn" : "ForegroundOff") as Storyboard;
            SwitchForegroundAnimation.Begin(SwitchCircle);
            SwitchCircleAnimation.Begin(SwitchCircle);
            SwitchBackgroundAnimation.Begin(SwitchBackground);
        }
    }
}
