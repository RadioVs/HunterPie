﻿using System;
using System.Windows;
using HunterPie.Core;
using System.Windows.Media;

namespace HunterPie.GUI.Widgets {
    /// <summary>
    /// Interaction logic for MantleTimer.xaml
    /// </summary>
    public partial class MantleTimer : Widget {
        
        private Mantle Context { get; set; }
        private int MantleNumber { get; set; }

        public MantleTimer(int MantleNumber, Mantle context) {
            this.MantleNumber = MantleNumber;
            WidgetType = 2;
            InitializeComponent();
            this.SetContext(context);
            SetWindowFlags();
            ApplySettings();
        }

        public override void EnterWidgetDesignMode() {
            base.EnterWidgetDesignMode();
            RemoveWindowTransparencyFlag();
        }

        public override void LeaveWidgetDesignMode() {
            base.LeaveWidgetDesignMode();
            ApplyWindowTransparencyFlag();
            SaveSettings();
        }

        private void SaveSettings() {
            switch (MantleNumber) {
                case 0:
                    UserSettings.PlayerConfig.Overlay.PrimaryMantle.Position[0] = (int)Left - UserSettings.PlayerConfig.Overlay.Position[0];
                    UserSettings.PlayerConfig.Overlay.PrimaryMantle.Position[1] = (int)Top - UserSettings.PlayerConfig.Overlay.Position[1];
                    UserSettings.PlayerConfig.Overlay.PrimaryMantle.Scale = DefaultScaleX;
                    break;
                case 1:
                    UserSettings.PlayerConfig.Overlay.SecondaryMantle.Position[0] = (int)Left - UserSettings.PlayerConfig.Overlay.Position[0];
                    UserSettings.PlayerConfig.Overlay.SecondaryMantle.Position[1] = (int)Top - UserSettings.PlayerConfig.Overlay.Position[1];
                    UserSettings.PlayerConfig.Overlay.SecondaryMantle.Scale = DefaultScaleX;
                    break;
            }
            
        }

        public void SetContext(Mantle ctx) {
            this.Context = ctx;
            HookEvents();
        }

        private void Dispatch(Action function) {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, function);
        }

        private void HookEvents() {
            Context.OnMantleCooldownUpdate += OnCooldownChange;
            Context.OnMantleTimerUpdate += OnTimerChange;
        }

        public void UnhookEvents() {
            Context.OnMantleCooldownUpdate -= OnCooldownChange;
            Context.OnMantleTimerUpdate -= OnTimerChange;
            Context = null;
        }

        private void OnTimerChange(object source, MantleEventArgs args) {
            if (args.Timer <= 0) {
                Dispatch(() => {
                    this.WidgetHasContent = false;
                    ChangeVisibility(false);
                });
                return;
            }
            string FormatMantleName = $"({(int)args.Timer}) {args.Name.ToUpper()}";
            Dispatch(() => {
                this.WidgetHasContent = true;
                ChangeVisibility(false);
                MantleName.Content = FormatMantleName;
                MantleTimerArc.EndAngle = ConvertPercentageIntoAngle(args.Timer / args.staticTimer);
            });
        }

        private void OnCooldownChange(object source, MantleEventArgs args) {
            if (args.Cooldown <= 0) {
                Dispatch(() => {
                    this.WidgetHasContent = false;
                    ChangeVisibility(false);
                });
                return;
            }
            Dispatch(() => {
                this.WidgetHasContent = true;
                ChangeVisibility(false);
                string FormatMantleName = $"({(int)args.Cooldown}) {args.Name.ToUpper()}";
                MantleName.Content = FormatMantleName;
                MantleTimerArc.EndAngle = ConvertPercentageIntoAngle(args.Cooldown / args.staticCooldown);
            });
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.UnhookEvents();
            this.IsClosed = true;
        }

        public override void ApplySettings(bool FocusTrigger = false) {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => {
                if (!FocusTrigger) {
                    // Changes widget position
                    this.Top = (MantleNumber == 0 ? UserSettings.PlayerConfig.Overlay.PrimaryMantle.Position[1] : UserSettings.PlayerConfig.Overlay.SecondaryMantle.Position[1]) + UserSettings.PlayerConfig.Overlay.Position[1];
                    this.Left = (MantleNumber == 0 ? UserSettings.PlayerConfig.Overlay.PrimaryMantle.Position[0] : UserSettings.PlayerConfig.Overlay.SecondaryMantle.Position[0]) + UserSettings.PlayerConfig.Overlay.Position[0];

                    // Sets widget custom color
                    Color WidgetColor = (Color)ColorConverter.ConvertFromString(MantleNumber == 0 ? UserSettings.PlayerConfig.Overlay.PrimaryMantle.Color : UserSettings.PlayerConfig.Overlay.SecondaryMantle.Color);
                    Brush WidgetColorBrush = new SolidColorBrush(WidgetColor);
                    WidgetColorBrush.Freeze();
                    this.MantleTimerArc.Stroke = WidgetColorBrush;
                    this.MantleBorder.BorderBrush = WidgetColorBrush;
                    double ScaleFactor = MantleNumber == 0 ? UserSettings.PlayerConfig.Overlay.PrimaryMantle.Scale : UserSettings.PlayerConfig.Overlay.SecondaryMantle.Scale;
                    ScaleWidget(ScaleFactor, ScaleFactor);
                    // Sets visibility if enabled/disabled
                    bool IsEnabled = MantleNumber == 0 ? UserSettings.PlayerConfig.Overlay.PrimaryMantle.Enabled : UserSettings.PlayerConfig.Overlay.SecondaryMantle.Enabled;
                    this.WidgetActive = IsEnabled;
                }
                base.ApplySettings();
            }));
        }


        public void ScaleWidget(double NewScaleX, double NewScaleY) {
            if (NewScaleX <= 0.2) return;
            Width = BaseWidth * NewScaleX;
            Height = BaseHeight * NewScaleY;
            this.MantleContainer.LayoutTransform = new ScaleTransform(NewScaleX, NewScaleY);
            this.DefaultScaleX = NewScaleX;
            this.DefaultScaleY = NewScaleY;
        }

        private void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
            this.MouseOver = true;
        }

        private void OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                this.MoveWidget();
                SaveSettings();
            }
        }

        private void OnMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e) {
            if (this.MouseOver) {
                if (e.Delta > 0) {
                    ScaleWidget(DefaultScaleX + 0.05, DefaultScaleY + 0.05);
                } else {
                    ScaleWidget(DefaultScaleX - 0.05, DefaultScaleY - 0.05);
                }
            }
        }

        private void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
            this.MouseOver = false;
        }

        // Helper

        private float ConvertPercentageIntoAngle(float percentage) {
            float max = -269.999f;
            float angle = 90 - (360 * percentage);
            if (angle < max) angle = max;
            return angle;
        }
    }
}
