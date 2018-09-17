using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;

namespace Entoarox.MorePetsAndAnimals
{
    internal class AdoptQuestion
    {
        /*********
        ** Fields
        *********/
        private readonly bool Cat;
        private readonly int Skin;
        private readonly AnimatedSprite Sprite;
        private Farmer Who;


        /*********
        ** Public methods
        *********/
        public AdoptQuestion(bool cat, int skin)
        {
            this.Cat = cat;
            this.Skin = skin;

            this.Sprite = new AnimatedSprite(ModEntry.Content, $"{(cat ? "cat" : "dog")}_{skin}", 28, 32, 32)
            {
                loop = true
            };
        }

        public static void Show()
        {
            Random random = ModEntry.Random;

            int catLimit = ModEntry.Indexes["cat"].Count;
            int dogLimit = ModEntry.Indexes["dog"].Count;

            bool cat = catLimit != 0 && (dogLimit == 0 || random.NextDouble() < 0.5);
            AdoptQuestion q = new AdoptQuestion(cat, random.Next(1, cat ? catLimit : dogLimit));
            GraphicsEvents.OnPostRenderHudEvent += q.Display;
            Game1.currentLocation.lastQuestionKey = "AdoptPetQuestion";
            Game1.currentLocation.createQuestionDialogue(
                $"Oh dear, it looks like someone has abandoned a poor {(cat ? "Cat" : "Dog")} here! Perhaps you should pay Marnie {ModEntry.Config.AdoptionPrice} gold to give it a checkup so you can adopt it?",
                Game1.player.money < ModEntry.Config.AdoptionPrice
                    ? new[]
                    {
                        new Response("n", $"Unfortunately I do not have the required {ModEntry.Config.AdoptionPrice} gold in order to do this.")
                    }
                    : new[]
                    {
                        new Response("y", "Yes, I really should adopt the poor animal!"),
                        new Response("n", "No, I do not have the space to house it.")
                    },
                q.Resolver);
        }

        public void Display(object o, EventArgs e)
        {
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is DialogueBox)
            {
                DialogueBox box = (DialogueBox)Game1.activeClickableMenu;
                Vector2 c = new Vector2(Game1.viewport.Width / 2 - 128 * Game1.pixelZoom, Game1.viewport.Height - box.height - 56 * Game1.pixelZoom);
                Vector2 p = new Vector2(36 * Game1.pixelZoom + c.X, 32 * Game1.pixelZoom + c.Y);
                IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), (int)c.X, (int)c.Y, 40 * Game1.pixelZoom, 40 * Game1.pixelZoom, Color.White, 1, true);
                Game1.spriteBatch.Draw(this.Sprite.Texture, p, this.Sprite.SourceRect, Color.White, 0, new Vector2(this.Sprite.SpriteWidth, this.Sprite.SpriteHeight), Game1.pixelZoom, SpriteEffects.None, 0.991f);
                this.Sprite.Animate(Game1.currentGameTime, 28, 2, 500);
            }
            else
                GraphicsEvents.OnPostRenderHudEvent -= this.Display;
        }

        public void Resolver(Farmer who, string answer)
        {
            GraphicsEvents.OnPostRenderHudEvent -= this.Display;
            if (answer == "n")
                return;
            this.Who = who;
            Game1.activeClickableMenu = new NamingMenu(this.Namer, "Choose a name");
        }

        public void Namer(string petName)
        {
            Pet pet;
            this.Who.Money -= ModEntry.Config.AdoptionPrice;
            if (this.Cat)
            {
                pet = new Cat((int)Game1.player.position.X, (int)Game1.player.position.Y)
                {
                    Sprite = new AnimatedSprite(ModEntry.Content, $"cat_{this.Skin}", 0, 32, 32)
                };
            }
            else
            {
                pet = new Dog(Game1.player.getTileLocationPoint().X, Game1.player.getTileLocationPoint().Y)
                {
                    Sprite = new AnimatedSprite(ModEntry.Content, $"dog_{this.Skin}", 0, 32, 32)
                };
            }

            pet.Name = petName;
            pet.Manners = this.Skin;
            pet.Age = Game1.year * 1000 + Array.IndexOf(ModEntry.Seasons, Game1.currentSeason) * 100 + Game1.dayOfMonth;
            pet.Position = Game1.player.position;
            Game1.currentLocation.addCharacter(pet);
            pet.warpToFarmHouse(this.Who);
            Game1.drawObjectDialogue($"Marnie will bring {petName} to your house once they have their shots and been given a grooming.");
        }
    }
}
