﻿using System;
using System.Windows.Controls;
using Timer = System.Threading.Timer;
using HunterPie.Core;

namespace HunterPie.GUI.Widgets.Monster_Widget.Parts {
    /// <summary>
    /// Interaction logic for MonsterAilment.xaml
    /// </summary>
    public partial class MonsterAilment : UserControl {
        Ailment Context;
        Timer VisibilityTimer;

        public MonsterAilment() {
            InitializeComponent();
        }

        public void SetContext(Ailment ctx, double MaxBarSize) {
            Context = ctx;
            SetAilmentInformation(MaxBarSize);
            HookEvents();
            StartVisibilityTimer();
        }

        private void HookEvents() {
            Context.OnBuildupChange += OnBuildupChange;
            Context.OnDurationChange += OnDurationChange;
            Context.OnCounterChange += OnCounterChange;
        }

        public void UnhookEvents() {
            Context.OnBuildupChange -= OnBuildupChange;
            Context.OnDurationChange -= OnDurationChange;
            Context.OnCounterChange -= OnCounterChange;
            VisibilityTimer?.Dispose();
            Context = null;
        }

        private void Dispatch(Action f) {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, f);
        }

        #region Settings
        public void ApplySettings() {

            System.Windows.Visibility visibility;
            if (UserSettings.PlayerConfig.Overlay.MonstersComponent.EnableMonsterAilments) {
                visibility = System.Windows.Visibility.Visible;
            } else {
                visibility = System.Windows.Visibility.Collapsed;
            }
            if (UserSettings.PlayerConfig.Overlay.MonstersComponent.HidePartsAfterSeconds) visibility = System.Windows.Visibility.Collapsed;
            Dispatch(() => { this.Visibility = visibility; });
        }

        #endregion

        #region Visibility timer
        private void StartVisibilityTimer() {
            if (!UserSettings.PlayerConfig.Overlay.MonstersComponent.HidePartsAfterSeconds) {
                ApplySettings();
                return;
            }
            if (VisibilityTimer == null) {
                VisibilityTimer = new Timer(_ => HideUnactiveBar(), null, 10, 0);
            } else {
                VisibilityTimer.Change(UserSettings.PlayerConfig.Overlay.MonstersComponent.SecondsToHideParts * 1000, 0);
            }
        }

        private void HideUnactiveBar() {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() => {
                this.Visibility = System.Windows.Visibility.Collapsed;
            }));
        }

        #endregion

        private void SetAilmentInformation(double NewSize) {
            this.AilmentName.Text = $"{Context.Name}";
            this.AilmentBar.MaxSize = NewSize - 37;
            this.AilmentCounter.Text = $"{Context.Counter}";
            if (Context.Duration > 0) {
                AilmentBar.MaxHealth = Math.Max(1, Context.MaxDuration);
                AilmentBar.Health = Math.Max(0, Context.Duration);
            } else {
                AilmentBar.MaxHealth = Math.Max(1, Context.MaxBuildup);
                AilmentBar.Health = Math.Max(0, Context.MaxBuildup - Context.Buildup);
            }
            AilmentText.Text = $"{AilmentBar.Health:0}/{AilmentBar.MaxHealth:0}";
        }

        private void OnCounterChange(object source, MonsterAilmentEventArgs args) {
            System.Windows.Visibility visibility;
            if (UserSettings.PlayerConfig.Overlay.MonstersComponent.EnableMonsterAilments) {
                visibility = System.Windows.Visibility.Visible;
            } else {
                visibility = System.Windows.Visibility.Collapsed;
            }
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() => {
                this.AilmentCounter.Text = args.Counter.ToString();
                this.Visibility = visibility;
                StartVisibilityTimer();
            }));
        }

        private void OnDurationChange(object source, MonsterAilmentEventArgs args) {
            if (args.MaxDuration <= 0) { return; }
            System.Windows.Visibility visibility;
            if (UserSettings.PlayerConfig.Overlay.MonstersComponent.EnableMonsterAilments) {
                visibility = System.Windows.Visibility.Visible;
            } else {
                visibility = System.Windows.Visibility.Collapsed;
            }
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() => {
                AilmentBar.MaxHealth = args.MaxDuration;
                AilmentBar.Health = Math.Max(0, args.MaxDuration - args.Duration);
                AilmentText.Text = $"{AilmentBar.Health:0}/{AilmentBar.MaxHealth:0}";
                this.Visibility = visibility;
                StartVisibilityTimer();
            }));
        }

        private void OnBuildupChange(object source, MonsterAilmentEventArgs args) {
            if (args.MaxBuildup <= 0) { return; }
            System.Windows.Visibility visibility;
            if (UserSettings.PlayerConfig.Overlay.MonstersComponent.EnableMonsterAilments) {
                visibility = System.Windows.Visibility.Visible;
            } else {
                visibility = System.Windows.Visibility.Collapsed;
            }
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() => {
                AilmentBar.MaxHealth = Math.Max(1, args.MaxBuildup);
                // Get the min between them so the buildup doesnt overflow
                AilmentBar.Health = Math.Min(args.Buildup, args.MaxBuildup);
                AilmentText.Text = $"{AilmentBar.Health:0}/{AilmentBar.MaxHealth:0}";
                this.Visibility = visibility;
                StartVisibilityTimer();
            }));
        }

        public void UpdateBarSize(double NewSize) {
            if (this.Context == null) return;
            this.AilmentBar.MaxSize = NewSize - 37;
            if (Context.Duration > 0) {
                AilmentBar.MaxHealth = Math.Max(1, Context.MaxDuration);
                AilmentBar.Health = Math.Max(0, Context.Duration);
            } else {
                AilmentBar.MaxHealth = Math.Max(1, Context.MaxBuildup);
                AilmentBar.Health = Math.Max(0, Context.MaxBuildup - Context.Buildup);
            }
            AilmentText.Text = $"{AilmentBar.Health:0}/{AilmentBar.MaxHealth:0}";
        }
    }
}
