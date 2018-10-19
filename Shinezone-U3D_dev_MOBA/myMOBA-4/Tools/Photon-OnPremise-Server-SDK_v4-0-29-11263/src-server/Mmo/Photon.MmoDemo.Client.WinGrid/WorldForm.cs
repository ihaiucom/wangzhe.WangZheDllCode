// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorldForm.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The world form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client.WinGrid
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    using ExitGames.Client.Photon;
    using ExitGames.Concurrency.Core;
    using ExitGames.Concurrency.Fibers;

    using ExitGames.Logging;

    using Photon.MmoDemo.Common;

    using ZedGraph;

    public partial class WorldForm : Form, IPhotonPeerListener
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly List<Color> colors = new List<Color> { Color.HotPink, Color.Red, Color.Blue, Color.Green, Color.DarkGray, Color.Cyan, Color.Orange, Color.Purple, Color.Navy, Color.Olive };

        private readonly PhotonPeer diagnosticsPeer;

        private readonly FormFiber fiber;

        private int gameCounter;

        private DateTime? startTime;

        private IDisposable updateTimer;

        public WorldForm()
        {
            this.gameCounter = 0;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_OnUnhandledException;
            Application.ThreadException += Application_OnThreadException;

            this.InitializeComponent();

            this.MouseWheel += this.OnMouseWheel;

            // diagnostics
            this.fiber = new FormFiber(this, new DefaultExecutor());
            this.fiber.Start();
            Settings settings = Program.GetDefaultSettings();
            this.diagnosticsPeer = new PhotonPeer(this, settings.UseTcp ? ConnectionProtocol.Tcp : ConnectionProtocol.Udp);
            this.counterGraph.GraphPane.Title.Text = "Server Performance";
            this.counterGraph.GraphPane.YAxis.Title.Text = "Value";
            this.counterGraph.GraphPane.YAxis.Scale.Min = 0;
            this.counterGraph.GraphPane.XAxis.Title.Text = "Time";
            this.counterGraph.GraphPane.XAxis.Scale.MajorUnit = DateUnit.Second;
        }
        
        public void OnMessage(object messages)
        {
            ///
        }

        void IPhotonPeerListener.DebugReturn(DebugLevel debugLevel, string debug)
        {
            if (log.IsDebugEnabled)
            {
                // we do not use debugLevel here - just log whatever debug we have
                log.DebugFormat(debug);
            }
        }
       
        void IPhotonPeerListener.OnEvent(EventData @event)
        {
            switch (@event.Code)
            {
                case (byte)EventCode.CounterData:
                    {
                        var name = (string)@event.Parameters[(byte)ParameterCode.CounterName];
                        var values = (float[])@event.Parameters[(byte)ParameterCode.CounterValues];
                        var timestamps = (long[])@event.Parameters[(byte)ParameterCode.CounterTimeStamps];
                        var curve = (LineItem)this.counterGraph.GraphPane.CurveList[name];
                        if (curve == null)
                        {
                            if (!this.startTime.HasValue)
                            {
                                this.startTime = DateTime.FromBinary(timestamps[0]);
                            }

                            Color color = this.colors[0];
                            this.colors.RemoveAt(0);
                            curve = this.counterGraph.GraphPane.AddCurve(name, new RollingPointPairList(90), color, SymbolType.Circle);
                            curve.Symbol.Fill = new Fill(curve.Color);
                            curve.Symbol.Size = 5;
                            curve.Line.IsSmooth = true;

                            //// curve.Line.SmoothTension = 0.5f;
                        }

                        var list = (IPointListEdit)curve.Points;
                        for (int index = 0; index < values.Length; index++)
                        {
                            DateTime time = DateTime.FromBinary(timestamps[index]);
                            TimeSpan diff = time.Subtract(this.startTime.GetValueOrDefault());
                            list.Add(diff.TotalSeconds, values[index]);
                        }

                        this.counterGraph.GraphPane.XAxis.Scale.Min = list[0].X;
                        this.counterGraph.GraphPane.XAxis.Scale.Max = list[list.Count - 1].X;

                        this.counterGraph.AxisChange();
                        this.counterGraph.Invalidate();
                        break;
                    }

                case (byte)EventCode.RadarUpdate:
                    {
                        var itemId = (string)@event.Parameters[(byte)ParameterCode.ItemId];
                        var itemType = (ItemType)(byte)@event.Parameters[(byte)ParameterCode.ItemType];
                        var position = (Vector)@event.Parameters[(byte)ParameterCode.Position];
                        var remove = (bool)@event.Parameters[(byte)ParameterCode.Remove];
                        this.tabPageRadar.OnRadarUpdate(itemId, itemType, position, remove);
                        break;
                    }

                default:
                    {
                        log.ErrorFormat("diagnostics: unexpected event {0}", (EventCode)@event.Code);
                        break;
                    }
            }
        }

        void IPhotonPeerListener.OnOperationResponse(OperationResponse operationResponse)
        {
            switch (operationResponse.OperationCode)
            {
                case (byte)OperationCode.SubscribeCounter:
                    {
                        // init first engine
                        var newPage = new GameTabPage { Padding = new Padding(3), UseVisualStyleBackColor = true };
                        this.tabControlTabs.TabPages.Add(newPage);
                        this.StartGame(newPage, Program.GetDefaultSettings());
                        newPage.Run();
                        break;
                    }

                case (byte)OperationCode.RadarSubscribe:
                    {
                        break;
                    }

                default:
                    {
                        log.ErrorFormat("diagnostics: unexpected return {0}", operationResponse.ReturnCode);
                        break;
                    }
            }
        }

        void IPhotonPeerListener.OnStatusChanged(StatusCode returnCode)
        {
            try
            {
                switch (returnCode)
                {
                    case StatusCode.Connect:
                        {
                            log.InfoFormat("diagnostics: connected");
                            Operations.CounterSubscribe(this.diagnosticsPeer, 5000);
                            break;
                        }

                    case StatusCode.Disconnect:
                    case StatusCode.DisconnectByServer:
                    case StatusCode.DisconnectByServerLogic:
                    case StatusCode.DisconnectByServerUserLimit:
                    case StatusCode.TimeoutDisconnect:
                        {
                            log.InfoFormat("diagnostics: {0}", returnCode);
                            break;
                        }

                    default:
                        {
                            log.ErrorFormat("diagnostics: unexpected return {0}", returnCode);
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                // exceptions in this method vanish if not caught here
                log.Error(e);
            }
        }

        private static Vector AddVelocity(Vector input, int velocity)
        {
            return input*velocity;
        }

        private static void Application_OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            log.Error(e.Exception);
        }

        private static void CurrentDomain_OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Error(e.ExceptionObject);
        }

        private static void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private static void OnNumericTextBoxValidating(object sender, CancelEventArgs e)
        {
            int number;
            if (int.TryParse(((TextBox)sender).Text, out number) == false || number < 0)
            {
                e.Cancel = true;
            }
        }

        private GameTabPage GetCurrentGamePage()
        {
            var page = this.tabControlTabs.SelectedTab as GameTabPage;
            if (page == null)
            {
                return this.tabPageSettings.Tag as GameTabPage;
            }

            return page;
        }

        private void HandleGameWorldEntered(GameTabPage page)
        {
            page.WorldEntered -= this.HandleGameWorldEntered;

            if (!this.tabPageRadar.Initialized)
            {
                this.tabPageRadar.Initialize(page.Game.WorldData);
                Operations.RadarSubscribe(this.diagnosticsPeer, page.Game.WorldData.Name);
            }
        }

        /// <summary>
        /// Tag page to settings tab and load values from game engine settings.
        /// </summary>
        private void InitTabSettings(GameTabPage page)
        {
            this.tabPageSettings.Tag = page;
            if (page != null)
            {
                Game game = page.Game;
                var settings = (Settings)game.Settings;

                this.tabPageSettings.Text = game.Avatar.Text;
                this.textBoxSendMovementInterval.Text = settings.SendInterval.ToString();
                this.textBoxAutoMoveInterval.Text = settings.AutoMoveInterval.ToString();
                this.textBoxPlayerText.Text = game.Avatar.Text;
                this.buttonPlayerColor.BackColor = Color.FromArgb(game.Avatar.Color);
                this.checkBoxSendReliable.Checked = settings.SendReliable;
                this.textBoxAutoMoveVelocity.Text = settings.AutoMoveVelocity.ToString();
                this.checkBoxAutoMoveEnabled.Checked = settings.AutoMove;
            }
            else
            {
                this.tabPageSettings.Text = "Settings";
            }
        }

        private void LogText(string text)
        {
            if (this.textBoxLog.InvokeRequired)
            {
                Action a = () => this.LogText(text);
                this.textBoxLog.BeginInvoke(a);
            }
            else
            {
                this.textBoxLog.AppendText(text);
                this.textBoxLog.AppendText("\r\n");
            }
        }

        /// <summary>
        /// Change color on buttonColor click.
        /// </summary>
        private void OnButtonColorClick(object sender, EventArgs e)
        {
            var colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() != DialogResult.Cancel)
            {
                this.buttonPlayerColor.BackColor = colorDialog.Color;
                GameTabPage page = this.GetCurrentGamePage();
                if (page != null)
                {
                    page.Game.Avatar.SetColor(colorDialog.Color.ToArgb());
                }
            }
        }

        /// <summary>
        /// Start/stop auto moving when checkBoxAutoMove changed.
        /// </summary>
        private void OnCheckBoxAutoMoveCheckedChanged(object sender, EventArgs e)
        {
            GameTabPage page = this.GetCurrentGamePage();
            if (page == null)
            {
                return;
            }

            ((Settings)page.Game.Settings).AutoMove = this.checkBoxAutoMoveEnabled.Checked;

            if (this.checkBoxAutoMoveEnabled.Checked)
            {
                page.AutoMove();
            }
        }

        private void OnCheckBoxSendReliableCheckedChanged(object sender, EventArgs e)
        {
            GameTabPage page = this.GetCurrentGamePage();
            if (page == null)
            {
                return;
            }

            page.Game.Settings.SendReliable = this.checkBoxSendReliable.Checked;
        }

        /// <summary>
        /// Stop all engines on form closing.
        /// </summary>
        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.updateTimer != null)
            {
                this.updateTimer.Dispose();
                this.updateTimer = null;
            }

            this.diagnosticsPeer.Disconnect();
            this.fiber.Dispose();

            ////if (this.tabControlTabs.TabCount > 3)
            ////{
            // e.Cancel = true;
            foreach (object control in this.tabControlTabs.TabPages)
            {
                var page = control as GameTabPage;
                if (page != null)
                {
                    this.StopGame(page);
                }
            }

            // e.Cancel = false;
            ////}
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // ignore keyboard when entering text
            if (this.textBoxPlayerText.Focused)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Space:
                    {
                        this.tabControlTabs.SuspendLayout();
                        if (this.tabControlTabs.SelectedTab == this.tabPageSettings)
                        {
                            var t = this.tabPageSettings.Tag as TabPage;
                            this.tabControlTabs.SelectedTab = t;
                        }
                        else
                        {
                            this.tabControlTabs.SelectedTab = this.tabPageSettings;
                        }

                        this.tabControlTabs.ResumeLayout(true);

                        return;
                    }

                case Keys.Oemplus:
                case Keys.Add:
                    {
                        var newPage = new GameTabPage { Padding = new Padding(3), UseVisualStyleBackColor = true };
                        this.tabControlTabs.TabPages.Add(newPage);
                        this.StartGame(newPage, Program.GetDefaultSettings());
                        newPage.Run();
                        return;
                    }
            }

            var page = this.tabControlTabs.SelectedTab as GameTabPage;
            if (page == null)
            {
                return;
            }

            Game game = page.Game;
            var settings = (Settings)game.Settings;

            switch (e.KeyCode)
            {
                case Keys.M:
                    {
                        if (settings.AutoMove)
                        {
                            settings.AutoMove = false;
                        }
                        else
                        {
                            settings.AutoMove = true;
                            page.AutoMove();
                        }

                        break;
                    }

                case Keys.W:
                case Keys.NumPad8:
                    {
                        game.Avatar.MoveRelative(AddVelocity(Game.MoveUp, settings.AutoMoveVelocity), Vector.Zero);
                        break;
                    }

                case Keys.A:
                case Keys.NumPad4:
                    {
                        game.Avatar.MoveRelative(AddVelocity(Game.MoveLeft, settings.AutoMoveVelocity), Vector.Zero);
                        break;
                    }

                case Keys.S:
                case Keys.NumPad2:
                    {
                        game.Avatar.MoveRelative(AddVelocity(Game.MoveDown, settings.AutoMoveVelocity), Vector.Zero);
                        break;
                    }

                case Keys.D:
                case Keys.NumPad6:
                    {
                        game.Avatar.MoveRelative(AddVelocity(Game.MoveRight, settings.AutoMoveVelocity), Vector.Zero);
                        break;
                    }

                case Keys.NumPad5:
                    {
                        game.Avatar.MoveAbsolute(game.WorldData.BoundingBox.Max / 2, Vector.Zero);
                        break;
                    }

                case Keys.NumPad7:
                    {
                        game.Avatar.MoveRelative(AddVelocity(Game.MoveUpLeft, settings.AutoMoveVelocity), Vector.Zero);
                        break;
                    }

                case Keys.NumPad9:
                    {
                        game.Avatar.MoveRelative(AddVelocity(Game.MoveUpRight, settings.AutoMoveVelocity), Vector.Zero);
                        break;
                    }

                case Keys.NumPad1:
                    {
                        game.Avatar.MoveRelative(AddVelocity(Game.MoveDownLeft, settings.AutoMoveVelocity), Vector.Zero);
                        break;
                    }

                case Keys.NumPad3:
                    {
                        game.Avatar.MoveRelative(AddVelocity(Game.MoveDownRight, settings.AutoMoveVelocity), Vector.Zero);
                        break;
                    }

                case Keys.OemMinus:
                case Keys.Subtract:
                    {
                        this.StopGame(page);
                        break;
                    }

                case Keys.Insert:
                    {
                        page.SpawnBot();
                        break;
                    }

                case Keys.Delete:
                    {
                        page.DestroyBot();
                        break;
                    }
                case Keys.OemOpenBrackets:
                    {
                        Vector viewDistance = new Vector(page.MainCamera.ViewDistanceEnter);
                        viewDistance.X = Math.Min(page.Game.WorldData.Width, viewDistance.X - (page.Game.WorldData.TileDimensions.X / 2));
                        viewDistance.Y = Math.Min(page.Game.WorldData.Height, viewDistance.Y - (page.Game.WorldData.TileDimensions.Y / 2));
                        page.MainCamera.SetViewDistance(viewDistance);
                        break;
                    }
                case Keys.OemCloseBrackets:
                    {
                        Vector viewDistance = new Vector(page.MainCamera.ViewDistanceEnter);
                        viewDistance.X = Math.Min(page.Game.WorldData.Width, viewDistance.X + (page.Game.WorldData.TileDimensions.X/2));
                        viewDistance.Y = Math.Min(page.Game.WorldData.Height, viewDistance.Y + (page.Game.WorldData.TileDimensions.Y/2));
                        page.MainCamera.SetViewDistance(viewDistance);
                        break;
                    }
            }
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            var page = this.tabControlTabs.SelectedTab as GameTabPage;
            if (page == null)
            {
                return;
            }

            Game game = page.Game;
            if (game.WorldEntered)
            {
                var offset = game.WorldData.TileDimensions / 2;
                int factor = e.Delta > 0 ? 1 : -1;
                Vector currentViewDistance = page.MainCamera.ViewDistanceEnter;
                var newViewDistance = currentViewDistance + offset * factor;
                page.MainCamera.SetViewDistance(newViewDistance);
            }
        }

        /// <summary>
        /// Tag current game engine to settings tab or apply settings when switching back from settings tab.
        /// </summary>
        private void OnTabControlTabsSelectedIndexChanged(object sender, EventArgs e)
        {
            GameTabPage page = this.GetCurrentGamePage();
            if (page != null)
            {
                this.InitTabSettings(page);
            }
            else if (this.tabControlTabs.SelectedTab == this.tabPageLog)
            {
                this.textBoxLog.ScrollToCaret();
            }
        }

        private void OnTextBoxAutoMoveIntervalLeave(object sender, EventArgs e)
        {
            GameTabPage page = this.GetCurrentGamePage();
            if (page == null)
            {
                return;
            }

            ((Settings)page.Game.Settings).AutoMoveInterval = int.Parse(this.textBoxAutoMoveInterval.Text);
        }

        private void OnTextBoxAutoMoveVelocityLeave(object sender, EventArgs e)
        {
            GameTabPage page = this.GetCurrentGamePage();
            if (page == null)
            {
                return;
            }

            ((Settings)page.Game.Settings).AutoMoveVelocity = int.Parse(this.textBoxAutoMoveVelocity.Text);
        }

        /// <summary>
        /// Change game engine's actor name on TextboxName changed.
        /// </summary>
        private void OnTextBoxNameTextChanged(object sender, EventArgs e)
        {
            GameTabPage page = this.GetCurrentGamePage();
            if (page == null)
            {
                return;
            }

            page.Game.Avatar.SetText(this.textBoxPlayerText.Text);
        }

        private void OnTextBoxSendMovementLeave(object sender, EventArgs e)
        {
            GameTabPage page = this.GetCurrentGamePage();
            if (page == null)
            {
                return;
            }

            ((Settings)page.Game.Settings).SendInterval = int.Parse(this.textBoxSendMovementInterval.Text);
        }

        private void StartGame(GameTabPage page, Settings settings)
        {
            var game = new Game(page, settings, Environment.UserName + ++this.gameCounter);
            var peer = new PhotonPeer(game, settings.UseTcp ? ConnectionProtocol.Tcp : ConnectionProtocol.Udp);

            ////{
            ////    DebugOut = log.IsDebugEnabled ? DE.Exitgames.Neutron.Client.NPeer.DebugLevel.ALL : DE.Exitgames.Neutron.Client.NPeer.DebugLevel.INFO 
            ////};
            if (!this.tabPageRadar.Initialized)
            {
                page.WorldEntered += this.HandleGameWorldEntered;
            }

            game.Initialize(peer);
            page.Initialize(game);
            game.Connect();

            // set focus on game tab
            this.tabControlTabs.SelectedTab = page;
        }

        private void StopGame(GameTabPage page)
        {
            if (this.tabControlTabs.SelectedTab == page)
            {
                // set focus on tab before closing tab
                this.tabControlTabs.SelectedIndex = Math.Max(0, this.tabControlTabs.SelectedIndex - 1);
            }

            // shut down engine (event handlers are removed in OnGameStateChanged handler)
            page.Game.Disconnect();

            // remove tab 
            this.tabControlTabs.Controls.Remove(page);
            page.Dispose();
        }

        private void UpdateDiagnostics()
        {
            try
            {
                this.diagnosticsPeer.Service();
                this.tabPageRadar.Invalidate();
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }

        private void WorldForm_OnLoad(object sender, EventArgs eventArgs)
        {
            LogAppender.OnLog += this.LogText;

            Settings settings = Program.GetDefaultSettings();
            this.diagnosticsPeer.Connect(settings.ServerAddress, settings.ApplicationName);

            this.updateTimer = this.fiber.ScheduleOnInterval(this.UpdateDiagnostics, settings.DrawInterval, settings.DrawInterval);
        }
    }
}