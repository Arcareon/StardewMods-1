﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.Framework.UI
{
    public class ClickableAnimatedComponent : BaseInteractiveMenuComponent
    {
        protected TemporaryAnimatedSprite Sprite;
        protected bool ScaleOnHover;
        public event ClickHandler Handler;
        public ClickableAnimatedComponent(Rectangle area, TemporaryAnimatedSprite sprite, ClickHandler handler = null, bool scaleOnHover = true)
        {
            if (handler != null)
                Handler += handler;
            this.ScaleOnHover = scaleOnHover;
            this.Sprite = sprite;
            SetScaledArea(area);
        }
        public override void HoverIn(Point p, Point o)
        {
            Game1.playSound("Cowboy_Footstep");
            if (!this.ScaleOnHover)
                return;
            this.Area.X -= 2;
            this.Area.Y -= 2;
            this.Area.Width += 4;
            this.Area.Height += 4;
        }
        public override void HoverOut(Point p, Point o)
        {
            if (!this.ScaleOnHover)
                return;
            this.Area.X += 2;
            this.Area.Y += 2;
            this.Area.Width -= 4;
            this.Area.Height -= 4;
        }
        public override void LeftClick(Point p, Point o)
        {
            Game1.playSound("bigDeSelect");
            Handler?.Invoke(this, this.Parent, this.Parent.GetAttachedMenu());
        }
        public override void Update(GameTime t)
        {
            this.Sprite.update(t);
        }
        public override void Draw(SpriteBatch b, Point offset)
        {
            if (this.Visible)
                this.Sprite.draw(b, false, offset.X+ this.Area.X, offset.Y+ this.Area.Y);
        }
    }
}
