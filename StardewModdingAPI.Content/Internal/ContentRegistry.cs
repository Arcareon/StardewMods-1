﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewModdingAPI.Content.Internal
{
    using Plugins;

    class ContentHelper : IContentHelper
    {
        private IMod Mod;
        private string ModPath;
        public ContentHelper(IMod mod)
        {
            Mod = mod;
            ModPath = Mod.Helper.DirectoryPath.Replace(ExtendibleContentManager.ModContent.RootDirectory,"");
        }
        public string Normalize(string path)
        {
            return path.Replace('/', '\\');
        }
        public object LoadImage(string assetName)
        {
            string file = Path.Combine(ModPath, assetName);
            if (!File.Exists(file))
                return default(Texture2D);
            Texture2D texture = Texture2D.FromStream(StardewValley.Game1.graphics.GraphicsDevice, new FileStream(file, FileMode.Open));
            var data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            Parallel.For(0, data.Length, i => { data[i] = Color.FromNonPremultiplied(data[i].ToVector4()); });

            texture.SetData(data);
            return texture;
        }
        public T Load<T>(string assetName, ContentSource source = ContentSource.ModFolder)
        {
            if (typeof(T) == typeof(Texture2D) && assetName.EndsWith(".png"))
                return (T)LoadImage(assetName);
            if (assetName.EndsWith(".xnb"))
                assetName = assetName.Substring(0, assetName.Length - 4);
            return source == ContentSource.ModFolder ? ExtendibleContentManager.ModContent.Load<T>(Path.Combine(ModPath, assetName)) : StardewValley.Game1.content.Load<T>(assetName);
        }
        public void RegisterContentHandler(IContentHandler handler)
        {
            ExtendibleContentManager.AddContentHandler(handler);
        }
        public void RegisterTexturePatch(string asset, Texture2D patch, Rectangle destination, Rectangle? source)
        {
            asset = Normalize(asset);
            if (!TextureInjector.AssetMap.ContainsKey(asset))
                TextureInjector.AssetMap.Add(asset, new List<(Texture2D Texture,Rectangle Region,Rectangle Source)>());
            TextureInjector.AssetMap[asset].Add((Texture: patch, Region: destination, Source: source ?? new Rectangle(0, 0, patch.Width, patch.Height)));
            if (TextureInjector.AssetCache.ContainsKey(asset))
                TextureInjector.AssetCache.Remove(asset);
        }
        public void RegisterTexturePatch(string asset, string patch, Rectangle destination, Rectangle? source)
        {
            RegisterTexturePatch(asset, Load<Texture2D>(patch), destination, source);
        }
        public void RegisterDictionaryPatch<Tkey,TValue>(string asset, Dictionary<Tkey,TValue> patch)
        {
            asset = Normalize(asset);
            if (!DictionaryInjector.AssetMap.ContainsKey(asset))
                DictionaryInjector.AssetMap.Add(asset, new List<object>());
            DictionaryInjector.AssetMap[asset].Add(patch);
            if (DictionaryInjector.AssetCache.ContainsKey(asset))
                DictionaryInjector.AssetCache.Remove(asset);
        }
        public void RegisterDictionaryPatch<TKey,TValue>(string asset, string patch)
        {
            RegisterDictionaryPatch(asset, Load<Dictionary<TKey, TValue>>(patch));
        }
        public void RegisterXnbReplacement(string asset, string replacement)
        {
            XnbLoader.AssetMap.Add(Normalize(asset), Path.Combine(ModPath,replacement));
        }
        public void RegisterLoader<T>(string asset, AssetLoader<T> loader)
        {
            DelegatedContentHandler.AssetLoadMap.Add(Normalize(asset), loader);
        }
        public void RegisterLoader<T>(AssetLoader<T> loader)
        {
            DelegatedContentHandler.TypeLoadMap.Add(typeof(T), loader);
        }
        public void RegisterInjector<T>(string asset, AssetInjector<T> injector)
        {
            asset = Normalize(asset);
            if (!DelegatedContentHandler.AssetInjectMap.ContainsKey(asset))
                DelegatedContentHandler.AssetInjectMap.Add(asset, new List<Delegate>());
            DelegatedContentHandler.AssetInjectMap[asset].Add(injector);
        }
        public void RegisterInjector<T>(AssetInjector<T> injector)
        {
            if (!DelegatedContentHandler.TypeInjectMap.ContainsKey(typeof(T)))
                DelegatedContentHandler.TypeInjectMap.Add(typeof(T), new List<Delegate>());
            DelegatedContentHandler.TypeInjectMap[typeof(T)].Add(injector);
        }
    }
}