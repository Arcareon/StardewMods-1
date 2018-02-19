using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewModdingAPI;

namespace Entoarox.CustomBooks
{
    class ImagePage : Page
    {
        private Texture2D Image;
        private string Guid;
        private string Book;
        private string FilePath => Path.Combine(Constants.CurrentSavePath,"Entoarox.CustomBooks",this.Book, this.Guid + ".png");
        public ImagePage(string book)
        {
            this.Image = new Texture2D(Game1.graphics.GraphicsDevice, 140, 245);
            this.Guid = System.Guid.NewGuid().ToString();
            this.Book = book;
        }
        public ImagePage(string book, string guid)
        {
            this.Book = book;
            this.Guid = guid;
            this.Image = Texture2D.FromStream(Game1.graphics.GraphicsDevice, File.OpenRead(this.FilePath));
        }
        public override void Draw(SpriteBatch batch, Rectangle region)
        {
            int w = Math.Min(region.Width, this.Image.Width * 2);
            int h = Math.Min(region.Height, this.Image.Height * 2);
            int x = Math.Max(0, (region.Width - w) / 2);
            int y = Math.Max(0, (region.Height - h) / 2);
            Color c = new Color(221, 148, 84);
            batch.Draw(this.Image, new Rectangle(region.X + x + 2, region.Y + y, w, h), c);
            batch.Draw(this.Image, new Rectangle(region.X + x, region.Y + y + 2, w, h), c);
            batch.Draw(this.Image, new Rectangle(region.X + x + 2, region.Y + y + 2, w, h), c);
            batch.Draw(this.Image, new Rectangle(region.X + x, region.Y + y, w, h), Game1.textColor);
        }
        public void Delete()
        {
            if (File.Exists(this.FilePath))
                File.Delete(this.FilePath);
        }
        public override Bookshelf.Book.Page Serialize()
        {
            using (FileStream stream = File.Create(this.FilePath))
            {
                this.Image.SaveAsPng(stream, 150, 245);
            }
            var page = new Bookshelf.Book.Page
            {
                Type = Bookshelf.Book.PageType.Image,
                Content = this.Guid
            };
            return page;
        }
    }
}
