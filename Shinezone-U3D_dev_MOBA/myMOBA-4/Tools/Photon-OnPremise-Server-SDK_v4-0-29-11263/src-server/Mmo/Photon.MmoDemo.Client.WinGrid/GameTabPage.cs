// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameTabPage.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The double buffered game tab page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client.WinGrid
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    using ExitGames.Client.Photon;
    using ExitGames.Concurrency.Channels;
    using ExitGames.Concurrency.Core;
    using ExitGames.Concurrency.Fibers;
    using ExitGames.Diagnostics.Counter;

    using ExitGames.Logging;

    using Photon.MmoDemo.Common;

    /// <summary>
    /// The double buffered game tab page.
    /// </summary>
    public class GameTabPage : TabPage, IGameListener
    {
        // The background color.
        internal static readonly Pen LinePen = new Pen(Color.Gray, 1);

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly FormFiber fiber;

        private readonly CountsPerSecondCounter frameCounter = new CountsPerSecondCounter();

        private readonly Channel<MouseEventArgs> movementChannel = new Channel<MouseEventArgs>();

        private readonly List<MyItem> ownedItems = new List<MyItem>();

        private readonly Random random = new Random();

        private int botCounter;

        private float fps;

        private InterestArea mainCamera;

        private bool running;

        public GameTabPage()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            this.MouseMove += this.OnMouseMove;
            this.MouseClick += this.OnMouseClick;

            this.fiber = new FormFiber(this, new DefaultExecutor());
            this.fiber.Start();
        }

        public event Action<GameTabPage> WorldEntered;

        public override Image BackgroundImage
        {
            get
            {
                return Settings.IslandImage;
            }

            set
            {
            }
        }

        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return ImageLayout.Stretch;
            }

            set
            {
            }
        }

        public Game Game { get; private set; }

        public bool IsDebugLogEnabled
        {
            get
            {
                return log.IsDebugEnabled;
            }
        }

        public InterestArea MainCamera
        {
            get
            {
                return this.mainCamera;
            }
        }

        public void AttachInterestAreaToNextItem()
        {
            int nextIndex;
            if (this.mainCamera.AttachedItem == null)
            {
                nextIndex = 0;
            }
            else
            {
                int lastIndex = this.ownedItems.IndexOf(this.mainCamera.AttachedItem);
                nextIndex = lastIndex + 1;
                if (nextIndex >= this.ownedItems.Count)
                {
                    nextIndex = 0;
                }
            }

            this.mainCamera.AttachItem(this.ownedItems[nextIndex]);
        }

        public void AutoMove()
        {
            if (this.Game.WorldEntered)
            {
                this.ownedItems.ForEach(this.AutoMoveStart);
            }
        }

        public void DestroyBot()
        {
            // do not destroy avatar
            if (this.ownedItems.Count == 1)
            {
                return;
            }

            MyItem item = this.ownedItems[this.ownedItems.Count - 1];
            this.ownedItems.RemoveAt(this.ownedItems.Count - 1);
            item.Destroy();

            ////this.LogInfo(this.Game, (this.ownedItems.Count - 1) + " Bots running");
        }

        public void Initialize(Game game)
        {
            this.Text = game.Avatar.Text;
            this.Game = game;
            game.TryGetCamera(0, out this.mainCamera);
            this.ownedItems.Add(game.Avatar);
        }

        public void Run()
        {
            this.running = true;
            var settings = (Settings)this.Game.Settings;
            this.fiber.ScheduleOnInterval(this.GameLoop, settings.DrawInterval, settings.DrawInterval);
            this.fiber.ScheduleOnInterval(this.ReadFps, 1000, 10000);
            this.movementChannel.SubscribeToLast(this.fiber, this.OnMove, settings.SendInterval);
        }

        public void SpawnBot()
        {
            string name = Environment.UserName + " Bot" + this.botCounter++;
            var item = new MyItem(this.Game, Guid.NewGuid().ToString(), ItemType.Bot, name);
            this.Game.AddItem(item);
            item.Spawn(this.GetRandomPosition(), Vector.Zero, this.Game.Avatar.Color, false);
            this.ownedItems.Add(item);

            ////this.LogInfo(this.Game, (this.ownedItems.Count - 1) + " Bots running");
        }

        public void LogDebug(object message)
        {
            log.Debug(message);
        }

        public void LogError(object message)
        {
            log.Error(message);
        }

        public void LogInfo(object message)
        {
            log.Info(message);
        }

        public void OnCameraAttached(string itemId)
        {
        }

        public void OnCameraDetached()
        {
        }

        public void OnConnect()
        {
            this.LogInfo(string.Format("{0}: connected", this.Game.Avatar.Id));
        }

        public void OnDisconnect(StatusCode returnCode)
        {
            this.LogInfo(string.Format("{0}: {1}", this.Game.Avatar.Id, returnCode));
            this.running = false;
        }

        public void OnItemAdded(Item item)
        {
        }

        public void OnItemRemoved(Item item)
        {
        }

        public void OnItemSpawned(string itemId)
        {
            Item item;
            if (this.Game.TryGetItem(itemId, out item))
            {
                if (item.IsMine)
                {
                    this.AutoMoveStart((MyItem)item);
                }
            }
        }

        public void OnRadarUpdate(string itemId, ItemType itemType, Vector position, bool remove)
        {
        }

        public void OnWorldEntered()
        {
            this.LogInfo(string.Format("{0}: entered world {1}", this.Game.Avatar.Id, this.Game.WorldData.Name));
            this.Game.Avatar.MoveAbsolute(this.GetRandomPosition(), Vector.Zero);
            if (((Settings)this.Game.Settings).AutoMove)
            {
                this.AutoMove();
            }

            if (this.WorldEntered != null)
            {
                this.WorldEntered(this);
            }
        }

        internal static void PaintGrid(Graphics grfx, int[] boardSize, WorldData world)
        {
            float width = world.Width;
            float height = world.Height;
            float w = boardSize[0] / width;
            float h = boardSize[1] / height;
            float boxSizeX = w * world.TileDimensions.X;
            float boxSizeY = h * world.TileDimensions.Y;

            for (float y = boxSizeY; y < boardSize[1] - 1; y += boxSizeY)
            {
                grfx.DrawLine(LinePen, new Point(0, (int)y), new Point(boardSize[0], (int)y));
            }

            for (float x = boxSizeX; x < boardSize[0] - 1; x += boxSizeX)
            {
                grfx.DrawLine(LinePen, new Point((int)x, 0), new Point((int)x, boardSize[1]));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.movementChannel.ClearSubscribers();
                this.fiber.Stop();
                this.fiber.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.TopLevelControl == null)
            {
                return;
            }

            if (this.Game.WorldEntered)
            {
                var boardSize = new[] { e.ClipRectangle.Width, e.ClipRectangle.Height };
                PaintGrid(e.Graphics, boardSize, this.Game.WorldData);

                foreach (Item item in this.Game.Items.Values)
                {
                    if (item.IsUpToDate)
                    {
                        this.PaintActor(this.Game, e.Graphics, boardSize, item);
                    }
                }

                PaintCamera(this.Game, e.Graphics, boardSize, this.mainCamera);
                string rttString = string.Format(
                    "RTT/Var: {0}/{1}; FPS {2:0.00}; Bots: {3}",
                    this.Game.Peer.RoundTripTime,
                    this.Game.Peer.RoundTripTimeVariance, 
                    this.fps, 
                    this.ownedItems.Count - 1);
                e.Graphics.DrawString(rttString, this.Font, Settings.IslandTextBrush, 0, 0);
            } else {
                e.Graphics.DrawString(this.Game.WorldEntered ? "World Entered" : "", this.Font, Settings.IslandTextBrush, 0, 0);
                e.Graphics.DrawString("Press + to open new tab", this.Font, Settings.IslandTextBrush, 0, this.Font.SizeInPoints + 4);
                e.Graphics.DrawString("Press - to close tab", this.Font, Settings.IslandTextBrush, 0, (this.Font.SizeInPoints + 4) * 2);
            }
        }

        private static void PaintCamera(Game game, Graphics graphics, int[] boardSize, InterestArea camera)
        {
            Color color = Color.FromArgb(game.Avatar.Color);
            var pen = new Pen(color);

            WorldData world = game.WorldData;

            float width = world.Width;
            float height = world.Height;
            float w = boardSize[0] / width;
            float h = boardSize[1] / height;

            float x = (camera.Position.X * w) - 1;
            float y = ((world.BoundingBox.Max.Y - camera.Position.Y) * h) - 1;

            // draw view distance
            graphics.DrawRectangle(
                pen,
                x - (camera.ViewDistanceEnter.X * w),
                y - (camera.ViewDistanceEnter.Y * h),
                w * 2 * camera.ViewDistanceEnter.X,
                h * 2 * camera.ViewDistanceEnter.Y);

            // draw view distance exit
            graphics.DrawRectangle(
                pen,
                x - (camera.ViewDistanceExit.X * w),
                y - (camera.ViewDistanceExit.Y * h),
                w * 2 * camera.ViewDistanceExit.X,
                h * 2 * camera.ViewDistanceExit.Y);

            // make view distance distance thicker 
            graphics.DrawRectangle(
                pen,
                x - (camera.ViewDistanceEnter.X * w) + 1,
                y - (camera.ViewDistanceEnter.Y * h) + 1,
                (w * camera.ViewDistanceEnter.X * 2) - 2,
                (h * camera.ViewDistanceEnter.Y * 2) - 2);

            // make view distance distance thicker 
            graphics.DrawRectangle(
                pen,
                x - (camera.ViewDistanceExit.X * w) + 1,
                y - (camera.ViewDistanceExit.Y * h) + 1,
                (w * camera.ViewDistanceExit.X * 2) - 2,
                (h * camera.ViewDistanceExit.Y * 2) - 2);
        }

        private void AutoMove(Settings settings, int horizontal, int vertical, Stopwatch t, MyItem item)
        {
            try
            {
                if (item.IsDestroyed)
                {
                    item.IsMoving = false;
                    return;
                }

                if (settings.AutoMove && this.Game.WorldEntered)
                {
                    if (t.ElapsedMilliseconds < settings.AutoMoveInterval)
                    {
                        if (false == item.MoveRelative(new Vector { X = horizontal, Y = vertical }, Vector.Zero))
                        {
                            Vector newPos = this.GetRandomPosition();
                            item.MoveAbsolute(newPos, Vector.Zero);
                        }

                        this.fiber.Schedule(() => this.AutoMove(settings, horizontal, vertical, t, item), settings.SendInterval);
                    }
                    else
                    {
                        item.IsMoving = false;
                        this.fiber.Schedule(() => this.AutoMoveStart(item), settings.SendInterval);
                    }
                }
                else
                {
                    item.IsMoving = false;
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }

        private void AutoMoveStart(MyItem item)
        {
            if (item.IsMoving)
            {
                return;
            }

            if (item.IsDestroyed)
            {
                return;
            }

            item.IsMoving = true;

            Stopwatch t = Stopwatch.StartNew();

            int horizontal;
            int vertical;
            do
            {
                horizontal = this.random.Next(-1, 2);
                vertical = this.random.Next(-1, 2);
            }
            while (horizontal == 0 & vertical == 0);

            var settings = (Settings)this.Game.Settings;
            horizontal *= settings.AutoMoveVelocity;
            vertical *= settings.AutoMoveVelocity;

            this.AutoMove(settings, horizontal, vertical, t, item);
        }

        private void GameLoop()
        {
            if (this.running)
            {
                try
                {
                    this.Game.Update();
                    this.Invalidate();
                    this.frameCounter.Increment();
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            }
        }

        private Vector GetRandomPosition()
        {
            var d = this.Game.WorldData.BoundingBox.Max - this.Game.WorldData.BoundingBox.Min;
            return this.Game.WorldData.BoundingBox.Min + new Vector { X = d.X * (float)this.random.NextDouble(), Y = d.Y * (float)this.random.NextDouble() };
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                this.mainCamera.ResetViewDistance();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (this.mainCamera.AttachedItem != null)
                {
                    this.mainCamera.Detach();
                }
                else
                {
                    this.mainCamera.AttachItem(this.Game.Avatar);
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            this.movementChannel.Publish(e);
        }

        private void OnMove(MouseEventArgs e)
        {
            try
            {
                if (this.Game.WorldEntered)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        float x = e.X * this.Game.WorldData.Width / this.Width;
                        float y = e.Y * this.Game.WorldData.Height / this.Height;

                        if (this.mainCamera.AttachedItem == null)
                        {
                            this.mainCamera.Move(new Vector { X = x, Y = this.Game.WorldData.BoundingBox.Max.Y - y });
                        }
                        else
                        {
                            this.Game.Avatar.MoveAbsolute(new Vector { X = x, Y = this.Game.WorldData.BoundingBox.Max.Y - y }, Vector.Zero);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void PaintActor(Game game, Graphics grfx, int[] boardSize, Item actor)
        {
            WorldData world = game.WorldData;

            float width = world.Width;
            float height = world.Height;
            float w = boardSize[0] / width;
            float h = boardSize[1] / height;
            float minW = Math.Max(4, w);
            float minH = Math.Max(4, h);

            Color color = Color.FromArgb(actor.Color);
            var pen = new Pen(color);

            float x = (actor.Position.X * w) - 1;
            float y = ((world.BoundingBox.Max.Y - actor.Position.Y) * h) - 1;

            if (actor.InterestAreaAttached)
            {
                if (!actor.IsMine)
                {
                    // draw view distance
                    grfx.DrawRectangle(
                        pen, 
                        x - (actor.ViewDistanceEnter.X * w),
                        y - (actor.ViewDistanceEnter.Y * h),
                        w * 2 * actor.ViewDistanceEnter.X,
                        h * 2 * actor.ViewDistanceEnter.Y);

                    // draw view distance exit
                    grfx.DrawRectangle(
                        pen,
                        x - (actor.ViewDistanceExit.X * w),
                        y - (actor.ViewDistanceExit.Y * h),
                        w * 2 * actor.ViewDistanceExit.X,
                        h * 2 * actor.ViewDistanceExit.Y);
                }

                ////if (actor == this.Game.Avatar)
                ////{
                ////    // make view distance distance thicker 
                ////    grfx.DrawRectangle(
                ////        pen,
                ////        x - (actor.ViewDistanceEnter.X * w) + 1,
                ////        y - (actor.ViewDistanceEnter.Y * h) + 1,
                ////        (w * actor.ViewDistanceEnter.X * 2) - 2,
                ////        (h * actor.ViewDistanceEnter.Y * 2) - 2);

                ////    grfx.DrawRectangle(
                ////        pen,
                ////        x - (actor.ViewDistanceExit.X * w) + 1,
                ////        y - (actor.ViewDistanceExit.Y * h) + 1,
                ////        (w * actor.ViewDistanceExit.X * 2) - 2,
                ////        (h * actor.ViewDistanceExit.Y * 2) - 2);
                ////}
            }

            var brush = new SolidBrush(color);
            if (actor.PreviousPosition.HasValue)
            {
                Vector prev = actor.PreviousPosition.Value;
                if ((prev - actor.Position).Len2 > 0) // was exact comparison originally; probbalbly need some threshold instead of 0
                {
                    float lastX = (prev.X * w) - 1;
                    float lastY = ((world.BoundingBox.Max.Y - prev.Y) * h) - 1;

                    float predictedX = x + (x - lastX);
                    float predictedY = y + (y - lastY);

                    // draw movement line
                    grfx.DrawLine(pen, lastX, lastY, predictedX, predictedY);

                    // draw last Pos
                    grfx.FillRectangle(brush, lastX - (minW / 2) + 1, lastY - (minH / 2) + 1, minW - 1, minH - 1);

                    actor.ResetPreviousPosition();
                }
            }

            var txtBrush = new SolidBrush(color);
            if (actor == game.Avatar)
            {
                float localX = (game.Avatar.Position.X * w) - 1;
                float localY = ((world.BoundingBox.Max.Y - game.Avatar.Position.Y) * h) + 1;

                // draw local dot thicker
                grfx.FillRectangle(brush, localX - (minW / 2) + 1, localY - (minH / 2) - 1, minW, minH);

                grfx.DrawString(
                    string.Format("{0} ({1:0},{2:0})", actor.Text, actor.Position.X, world.BoundingBox.Max.Y - actor.Position.Y), 
                    this.Font, 
                    txtBrush, 
                    x + (minW / 2) + 1, 
                    y);
            }
            else
            {
                ////grfx.DrawString(string.Format("{0} (#{1:x})", actor.Name, actor.Color), this.Font, txtBrush, x + (minW / 2) + 1, y);
                grfx.DrawString(actor.Text, this.Font, txtBrush, x + (minW / 2) + 1, y);
            }

            grfx.FillRectangle(brush, x - (minW / 2) + 1, y - (minH / 2) + 1, minW - 1, minH - 1);

            // grfx.DrawString(
            // string.Format("#{3}: {0} ({1},{2})", actor.Name, actor.Position[0], actor.Position.Y, actor.Number), 
            // this.Font, 
            // txtBrush, 
            // actor.Position[0] * w, 
            // actor.Position.Y * h);

            // grfx.FillRectangle(txtBrush, x, y, 1, 1);
        }

        private void ReadFps()
        {
            this.fps = this.frameCounter.GetNextValue();
        }
    }
}