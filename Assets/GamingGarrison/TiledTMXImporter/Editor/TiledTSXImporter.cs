using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GamingGarrison
{
    public class TiledTSXImporter
    {
        /// <summary>
        /// Uses given pixel values to calculate proportional pivot needed to sit the bottom left corner of the tile at the bottom left corner of the cell
        /// </summary>
        static Vector2 GetPivot(int imageWidth, int imageHeight, int cellWidth, int cellHeight, TSX.TileOffset tileOffset)
        {
            Vector2 cellSize = new Vector2(cellWidth, cellHeight);
            Vector2 OneOverTileSize = new Vector2(1.0f / imageWidth, 1.0f / imageHeight);
            Vector2 pivot01 = Vector2.Scale(cellSize, OneOverTileSize) * 0.5f;
            if (tileOffset != null)
            {
                Vector2 offset = new Vector2(tileOffset.x, tileOffset.y); // In pixels
                Vector2 scaledOffset = (offset * OneOverTileSize);
                //Debug.Log("Offset = " + offset.x + ", " + offset.y + ", scaled offset = " + scaledOffset.x + "," +scaledOffset.y);
                pivot01 = pivot01 + scaledOffset;
            }
            return pivot01;
        }

        static bool CreateTilemapSprite(string targetPath, int cellWidth, int cellHeight, int pixelsPerUnit, TSX.Tileset tileset, string subSpriteNameBase, int realWidth, int realHeight, out Sprite[] tileSprites)
        {
            TextureImporter ti = AssetImporter.GetAtPath(targetPath) as TextureImporter;
            ti.maxTextureSize = 8192; // The max texture size in Unity

            if (ti == null)
            {
                tileSprites = new Sprite[0];
                Debug.LogError ("Cannot find tilemap sprite file at " + targetPath);
                return false;
            }

            TextureImporterSettings textureSettings = new TextureImporterSettings();
            ti.ReadTextureSettings(textureSettings);

            SpriteMeshType meshType = SpriteMeshType.FullRect;
            SpriteAlignment alignment = SpriteAlignment.Custom;
            Vector2 pivot = new Vector2(0.5f, 0.5f); // For the whole tileset sprite, so not important
            FilterMode filterMode = FilterMode.Point;
            SpriteImportMode importMode = SpriteImportMode.Multiple;
            bool fallbackPhysicsShape = true;

            if (textureSettings.spritePixelsPerUnit != pixelsPerUnit
                || textureSettings.spriteMeshType != meshType
                || textureSettings.spriteAlignment != (int)alignment
                || textureSettings.spritePivot != pivot
                || textureSettings.filterMode != filterMode
                || textureSettings.spriteMode != (int)importMode
                || textureSettings.spriteGenerateFallbackPhysicsShape != fallbackPhysicsShape)
            {
                textureSettings.spritePixelsPerUnit = pixelsPerUnit;
                textureSettings.spriteMeshType = meshType;
                textureSettings.spriteAlignment = (int)alignment;
                textureSettings.spritePivot = pivot;
                textureSettings.filterMode = filterMode;
                textureSettings.spriteMode = (int)importMode;
                textureSettings.spriteGenerateFallbackPhysicsShape = fallbackPhysicsShape;

                ti.SetTextureSettings(textureSettings);
                int tileCount = tileset.GetTileCount();

                List<SpriteMetaData> newData = new List<SpriteMetaData>(tileCount);

                int i = 0;
                for (int y = realHeight - tileset.margin; (y - tileset.tileheight >= tileset.margin); y -= tileset.tileheight + tileset.spacing)
                {
                    for (int x = tileset.margin; x + tileset.tilewidth <= realWidth - tileset.margin; x += tileset.tilewidth + tileset.spacing)
                    {
                        SpriteMetaData data = new SpriteMetaData();
                        data.name = subSpriteNameBase + "_" + i;
                        data.alignment = (int)alignment;
                        data.pivot = GetPivot(tileset.tilewidth, tileset.tileheight, cellWidth, cellHeight, tileset.tileoffset);
                        data.rect = new Rect(x, y - tileset.tileheight, tileset.tilewidth, tileset.tileheight);

                        newData.Add(data);
                        i++;
                    }
                }
                if (tileCount != i)
                {
                    Debug.LogWarning("The tileset specifies a tilecount of " + tileCount + " but we actually have " + i + " sprites!");
                }

                //ti.spritesheet = newData.ToArray(); // Can't do this any more

                // Use Unity's new fangled pointlessly overcomplicated provider system instead of a 1 liner
                // Based on logic from https://docs.unity3d.com/Packages/com.unity.2d.sprite@1.0/manual/DataProvider.html
                var factory = new SpriteDataProviderFactories();
                factory.Init();
                var dataProvider = factory.GetSpriteEditorDataProviderFromObject(ti);
                dataProvider.InitSpriteEditorDataProvider();

                // Extra nonsense specific to 2021.2 or newer apparently
                var spriteNameFileIdDataProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();
                List<SpriteNameFileIdPair> nameFileIdPairs = new List<SpriteNameFileIdPair>();

                SpriteRect[] rects = new SpriteRect[newData.Count];
                for (i = 0; i < newData.Count; i++)
                {
                    rects[i] = new SpriteRect();
                    rects[i].spriteID = GUID.Generate();
                    rects[i].name = newData[i].name;
                    rects[i].rect = newData[i].rect;
                    rects[i].border = newData[i].border;
                    rects[i].pivot = newData[i].pivot;
                    rects[i].alignment = (SpriteAlignment)newData[i].alignment;

                    // >=2021.2
                    nameFileIdPairs.Add(new SpriteNameFileIdPair(rects[i].name, rects[i].spriteID));
                }
                dataProvider.SetSpriteRects(rects);

                // >=2021.2
                spriteNameFileIdDataProvider.SetNameFileIdPairs(nameFileIdPairs);

                dataProvider.Apply();


                EditorUtility.SetDirty(ti);
                ti.SaveAndReimport();
            }

            Sprite[] subSprites = AssetDatabase.LoadAllAssetsAtPath(targetPath).OfType<Sprite>().ToArray();
            // For some reason Unity thinks it's smart to return the sub-sprites in random order...
            // ...and provide no API for retrieving sub-sprites by index >_<
            // so we have to manually sort them by the ids in their names
            Array.Sort<Sprite>(subSprites, new SpriteComparer());
            tileSprites = subSprites;
            return true;
        }

        class SpriteComparer : IComparer<Sprite>
        {
            public int Compare(Sprite a, Sprite b)
            {
                // Find last _ we put there
                int aUnderscorePos = a.name.LastIndexOf('_');
                string aNumberString = a.name.Substring(aUnderscorePos + 1);
                int aNumber = int.Parse(aNumberString);

                int bUnderscorePos = b.name.LastIndexOf('_');
                string bNumberString = b.name.Substring(bUnderscorePos + 1);
                int bNumber = int.Parse(bNumberString);

                // Compare the end numbers
                return Mathf.Clamp(aNumber - bNumber, -1, 1);
            }
        }
        

        public static bool CreateTileSprite(string targetPath, int pixelsPerUnit, int cellWidth, int cellHeight, int imageWidth, int imageHeight, TSX.TileOffset tileOffset)
        {
            TextureImporter ti = AssetImporter.GetAtPath(targetPath) as TextureImporter;

            TextureImporterSettings textureSettings = new TextureImporterSettings();
            ti.ReadTextureSettings(textureSettings);

            SpriteMeshType meshType = SpriteMeshType.FullRect;
            SpriteAlignment alignment = SpriteAlignment.Custom;
            Vector2 pivot = GetPivot(imageWidth, imageHeight, cellWidth, cellHeight, tileOffset);
            FilterMode filterMode = FilterMode.Point;
            bool fallbackPhysicsShape = true;

            if (textureSettings.textureType != TextureImporterType.Sprite
                || textureSettings.spritePixelsPerUnit != pixelsPerUnit
                || textureSettings.spriteMeshType != meshType
                || textureSettings.spriteAlignment != (int)alignment
                || textureSettings.spritePivot != pivot
                || textureSettings.filterMode != filterMode
                || textureSettings.spriteGenerateFallbackPhysicsShape != fallbackPhysicsShape)
            {
                textureSettings.textureType = TextureImporterType.Sprite;
                textureSettings.spritePixelsPerUnit = pixelsPerUnit;
                textureSettings.spriteMeshType = meshType;
                textureSettings.spriteAlignment = (int)alignment;
                textureSettings.spritePivot = pivot;
                textureSettings.filterMode = filterMode;
                textureSettings.spriteGenerateFallbackPhysicsShape = fallbackPhysicsShape;

                ti.SetTextureSettings(textureSettings);

                EditorUtility.SetDirty(ti);
                ti.SaveAndReimport();
            }

            return true;
        }

        static ImportedTile CreateAnimatedTileAsset(Sprite tileSprite, Sprite[] animatedSprites, float animationSpeed, string tileAssetPath, Tile.ColliderType colliderType)
        {
            AnimatedTMXTile tile = AssetDatabase.LoadAssetAtPath<AnimatedTMXTile>(tileAssetPath);

            if (tile == null)
            {
                tile = AnimatedTMXTile.CreateInstance<AnimatedTMXTile>();
                tile.m_AnimatedSprites = animatedSprites;
                tile.m_AnimationStartTime = 0.0f;
                tile.m_MinSpeed = animationSpeed;
                tile.m_MaxSpeed = animationSpeed;
                tile.sprite = tileSprite;
                tile.colliderType = colliderType;

                AssetDatabase.CreateAsset(tile, tileAssetPath);
            }
            else if ((tile.m_AnimatedSprites == null) == (animatedSprites == null)
                || tile.m_AnimatedSprites == null
                || animatedSprites == null
                || !tile.m_AnimatedSprites.SequenceEqual<Sprite>(animatedSprites)
                || tile.m_MinSpeed != animationSpeed
                || tile.m_MaxSpeed != animationSpeed
                || tile.sprite != tileSprite
                || tile.colliderType != colliderType)
            {
                tile.m_AnimatedSprites = animatedSprites;
                tile.m_MinSpeed = animationSpeed;
                tile.m_MaxSpeed = animationSpeed;
                tile.sprite = tileSprite;
                tile.colliderType = colliderType;
                EditorUtility.SetDirty(tile);
            }

            return new ImportedTile(tile, tileAssetPath);
        }

        static ImportedTile CreateTileAsset(Sprite tileSprite, string tileAssetPath, Tile.ColliderType colliderType)
        {
            Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(tileAssetPath);

            if (tile == null)
            {
                tile = Tile.CreateInstance<Tile>();
                tile.sprite = tileSprite;
                tile.colliderType = colliderType;

                AssetDatabase.CreateAsset(tile, tileAssetPath);
            }
            else if (tile.sprite != tileSprite || tile.colliderType != colliderType)
            {
                tile.sprite = tileSprite;
                tile.colliderType = colliderType;
                EditorUtility.SetDirty(tile);
            }

            return new ImportedTile(tile, tileAssetPath);
        }
        public static void GetTextureRealWidthAndHeight(TextureImporter texImporter, ref int width, ref int height)
        {
            Debug.Assert(texImporter != null, "Texture Importer is null!  The incoming texture asset is probably nonexistent!");
            System.Type type = typeof(TextureImporter);
            System.Reflection.MethodInfo method = type.GetMethod("GetWidthAndHeight", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var args = new object[] { width, height };
            method.Invoke(texImporter, args);
            width = (int)args[0];
            height = (int)args[1];
        }

        static void CreateSingleImageTilesetPaths(TSX.Tileset tileset, string pathWithoutFile, string tilesetSpriteTargetDir, out string imageSourcePath, out string imageTargetPath)
        {
            imageSourcePath = tileset.image.source;
            imageSourcePath = Path.Combine(pathWithoutFile, imageSourcePath);

            string imageName = Path.GetFileName(imageSourcePath);
            imageTargetPath = Path.Combine(tilesetSpriteTargetDir, imageName);
        }
        static void CreateSingleImageTileTargetPaths(int tileCount, string imageSourcePath, string tilesetTileTargetDir, out string[] tileTargetPaths)
        {
            tileTargetPaths = new string[tileCount];
            for (int i = 0; i < tileCount; i++)
            {
                string tileName = Path.GetFileNameWithoutExtension(imageSourcePath) + "_" + i + ".asset";
                tileTargetPaths[i] = Path.Combine(tilesetTileTargetDir, tileName);
            }
        }

        static void CreateTilePaths(TSX.Tile[] tiles, string pathWithoutFile, string tilesetSpriteTargetDir, string tilesetTileTargetDir, string[] imageSourcePaths, string[] imageTargetPaths, string[] tileTargetPaths)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                TSX.Tile tile = tiles[i];
                string imageSourcePath = tile.image.source;
                imageSourcePath = Path.Combine(pathWithoutFile, imageSourcePath);
                imageSourcePaths[i] = imageSourcePath;

                string imageName = Path.GetFileName(imageSourcePath);
                string imageTargetPath = Path.Combine(tilesetSpriteTargetDir, imageName);
                imageTargetPaths[i] = imageTargetPath;

                string tileName = Path.GetFileNameWithoutExtension(imageSourcePath) + ".asset";
                tileTargetPaths[i] = Path.Combine(tilesetTileTargetDir, tileName);
            }
        }

        public static void CopyImages(string[] imageSourcePaths, string[] imageTargetPaths)
        {
            for (int i = 0; i < imageSourcePaths.Length; i++)
            {
                string imageSourcePath = imageSourcePaths[i];
                string imageTargetPath = imageTargetPaths[i];

                EditorUtility.DisplayProgressBar("Copying...", imageSourcePath + " to " + imageTargetPath, (float)i / (float)imageSourcePath.Length);

                if (!File.Exists(imageSourcePath))
                {
                    throw new Exception("Source image at " + imageSourcePath + " does not exist!!!");
                }
                if (File.Exists(imageSourcePath) && (!File.Exists(imageTargetPath) || (!File.GetLastWriteTime(imageSourcePath).Equals(File.GetLastWriteTime(imageTargetPath)))))
                {
                    File.Copy(imageSourcePath, imageTargetPath, true);
                    AssetDatabase.ImportAsset(imageTargetPath, ImportAssetOptions.ForceSynchronousImport);

                    // Try and make sure we import the texture as a Sprite!
                    TextureImporter importer = AssetImporter.GetAtPath(imageTargetPath) as TextureImporter;
                    if (importer != null && importer.spriteImportMode == SpriteImportMode.None)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        importer.spriteImportMode = SpriteImportMode.Single;
                        EditorUtility.SetDirty(importer);
                        importer.SaveAndReimport();
                    }
                }
            }
        }

        static bool CreateSpriteAssets(TSX.Tileset tileset, int pixelsPerUnit, int cellWidth, int cellHeight, string[] imageTargetPaths, Sprite[] tileSprites)
        {
            TSX.Tile[] tiles = tileset.tiles;
            AssetDatabase.StartAssetEditing();
            for (int i = 0; i < tiles.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Importing " + tileset.name + " tileset sprites", null, (float)i / (float)tiles.Length);
                if (File.Exists(imageTargetPaths[i]))
                {
                    TSX.Tile tile = tiles[i];
                    bool success = CreateTileSprite(imageTargetPaths[i], pixelsPerUnit, cellWidth, cellHeight, tile.image.width, tile.image.height, tileset.tileoffset);
                    if (!success)
                    {
                        return false;
                    }
                    Sprite newSprite = AssetDatabase.LoadAssetAtPath<Sprite>(imageTargetPaths[i]);
                    if (newSprite == null)
                    {
                        // Try again - some changes don't get picked up (i.e. that it got set to be a Sprite asset)
                        AssetDatabase.StopAssetEditing();
                        newSprite = AssetDatabase.LoadAssetAtPath<Sprite>(imageTargetPaths[i]);
                        AssetDatabase.StartAssetEditing();
                    }
                    Debug.Assert(newSprite != null, "The asset at " + imageTargetPaths[i] +" is not a valid sprite!");
                    tileSprites[i] = newSprite;
                }
            }
            AssetDatabase.StopAssetEditing();
            return true;
        }

        static bool CreateTileAssets(TSX.Tile[] tiles, bool singleImageTileset, string tilesetName, int pixelsPerUnit, Sprite[] tileSprites, string[] tileTargetPaths, out ImportedTile[] outputTiles)
        {
            AssetDatabase.StartAssetEditing();
            Debug.Assert(tileSprites != null && tileTargetPaths != null);
            outputTiles = new ImportedTile[tileTargetPaths.Length];
            if (tileSprites.Length != tileTargetPaths.Length)
            {
                Debug.LogWarning("We have " + tileSprites.Length + " sprites but trying to create " + tileTargetPaths.Length + " tiles");
                //AssetDatabase.StopAssetEditing();
                //return false;
            }

            bool success = true;
            for (int i = 0; i < tileTargetPaths.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Importing " + tilesetName + " tileset tiles", null, (float)i / (float)tileTargetPaths.Length);
                if (i < tileSprites.Length && tileSprites[i] != null)
                {
                    Tile.ColliderType colliderType = Tile.ColliderType.None;

                    // Tile Collision
                    if (tiles != null)
                    {
                        TSX.Tile collisionTile = null;
                        if (singleImageTileset)
                        {
                            foreach (TSX.Tile tile in tiles)
                            {
                                if (tile.id == i)
                                {
                                    collisionTile = tile;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            collisionTile = tiles[i];
                        }
                        if (collisionTile != null && collisionTile.HasCollisionData())
                        {
                            if (TiledTMXImporter.s_forceGridColliders)
                            {
                                colliderType = Tile.ColliderType.Grid;
                            }
                            else
                            {
                                colliderType = Tile.ColliderType.Sprite;
                            }
                        }
                    }

                    // Tile Animation
                    TSX.Tile animationTile = null;
                    if (tiles != null)
                    {
                        if (singleImageTileset)
                        {
                            // Have to find relevant tile
                            foreach (TSX.Tile tile in tiles)
                            {
                                if (tile.id == i && tile.animation != null)
                                {
                                    animationTile = tile;
                                    break;
                                }
                            }
                        }
                        else if (tiles[i].animation != null)
                        {
                            animationTile = tiles[i];
                        }
                    }

                    if (animationTile != null)
                    {
                        TSX.Frame[] frames = animationTile.animation.frames;
                        Sprite[] animationSprites = new Sprite[frames.Length];
                        for (int f = 0; f < frames.Length; f++)
                        {
                            animationSprites[f] = tileSprites[frames[f].tileid];
                        }
                        //Just have to assume a constant animation speed ATM due to how Unity's Tilemap animation works :(
                        float animationSpeed = 1.0f / (frames[0].duration / 1000.0f);
                        ImportedTile newTile = CreateAnimatedTileAsset(tileSprites[i], animationSprites, animationSpeed, tileTargetPaths[i], colliderType);
                        success = newTile != null;
                        if (!success)
                        {
                            break;
                        }
                        outputTiles[i] = newTile;
                    }
                    else
                    {
                        ImportedTile newTile = CreateTileAsset(tileSprites[i], tileTargetPaths[i], colliderType);
                        success = newTile != null;
                        if (!success)
                        {
                            break;
                        }
                        outputTiles[i] = newTile;
                    }
                }
                else
                {
                    Debug.LogWarning("Sprite for tile " + tileTargetPaths[i] + " is null when creating Tile");
                }
            }
            AssetDatabase.StopAssetEditing();
            return success;
        }

        public static ImportedTileset ImportFromTilesetReference(TMX.TilesetReference tilesetReference, string baseFolder, string tilesetDir, int cellWidth, int cellHeight, int pixelsPerUnit)
        {
            // The source path in the tileset reference is recorded relative to the tmx
            TSX.Tileset actualTileset;
            ImportedTile[] importedTiles;
            if (tilesetReference.source != null)
            {
                string tsxPath = Path.Combine(baseFolder, tilesetReference.source);
                importedTiles = TiledTSXImporter.ImportTSXFile(tsxPath, tilesetDir, cellWidth, cellHeight, pixelsPerUnit, out actualTileset);
            }
            else
            {
                importedTiles = TiledTSXImporter.ImportEmbeddedTileset(tilesetReference, tilesetDir, baseFolder, cellWidth, cellHeight, pixelsPerUnit, out actualTileset);
            }

            if (importedTiles == null || (importedTiles.Length > 0 && importedTiles[0] == null))
            {
                Debug.LogError("Failed to import tileset " + (tilesetReference.source != null ? tilesetReference.source : tilesetReference.name) + " properly...");
                return null;
            }

            return new ImportedTileset(importedTiles, tilesetReference.firstgid, actualTileset);
        }

        static ImportedTile[] ImportEmbeddedTileset(TMX.TilesetReference embeddedTileset, string tilesetDir, string baseFolder, int cellWidth, int cellHeight, int pixelsPerUnit, out TSX.Tileset tilesetOut)
        {
            TSX.Tileset tileset = new TSX.Tileset(embeddedTileset);
            tilesetOut = tileset;

            if (!ImportUtils.CreateAssetFolderIfMissing(tilesetDir, true))
            {
                return null;
            }

            Debug.Log("Loading embedded tileset = " + tileset.name);

            return ImportTileset(tileset, tilesetDir, baseFolder, cellWidth, cellHeight, pixelsPerUnit);
        }

        static ImportedTile[] ImportTSXFile(string path, string tilesetDir, int cellWidth, int cellHeight, int pixelsPerUnit, out TSX.Tileset tilesetOut)
        {
            tilesetOut = null;
            if (!ImportUtils.CreateAssetFolderIfMissing(tilesetDir, true))
            {
                return null;
            }

            Debug.Log("Loading TSX file from " + path + " into " + tilesetDir);

            TSX.Tileset tileset = ImportUtils.ReadXMLIntoObject<TSX.Tileset>(path);
            if (tileset == null)
            {
                return null;
            }
            tilesetOut = tileset;

            Debug.Log("Loading tileset = " + tileset.name);

            return ImportTileset(tileset, tilesetDir, Path.GetDirectoryName(path), cellWidth, cellHeight, pixelsPerUnit);
        }

        static ImportedTile[] ImportTileset(TSX.Tileset tileset, string tilesetDir, string sourceTilesetDirectory, int cellWidth, int cellHeight, int pixelsPerUnit)
        {
            string tilesetSpriteTargetDir = tilesetDir + Path.DirectorySeparatorChar + tileset.name;
            if (!ImportUtils.CreateAssetFolderIfMissing(tilesetSpriteTargetDir, false))
            {
                return null;
            }
            string tilesetTileTargetDir = tilesetDir + Path.DirectorySeparatorChar + tileset.name + Path.DirectorySeparatorChar + "TileAssets";
            if (!ImportUtils.CreateAssetFolderIfMissing(tilesetTileTargetDir, false))
            {
                return null;
            }

            TSX.Tile[] tiles = tileset.tiles;

            string[] imageTargetPaths = null;
            Sprite[] tileSprites = null;
            string[] tileTargetPaths = null;
            bool singleImageTileset = tileset.IsSingleImageTileset();
            if (singleImageTileset)
            {
                if (tileset.image != null)
                {
                    string imageSourcePath;
                    string imageTargetPath;
                    CreateSingleImageTilesetPaths(tileset, sourceTilesetDirectory, tilesetSpriteTargetDir, out imageSourcePath, out imageTargetPath);
                    CopyImages(new string[] { imageSourcePath }, new string[] { imageTargetPath });
                    TextureImporter texImporter = AssetImporter.GetAtPath(imageTargetPath) as TextureImporter;
                    int realWidth = 0;
                    int realHeight = 0;
                    GetTextureRealWidthAndHeight(texImporter, ref realWidth, ref realHeight);
                    if (realWidth != tileset.image.width)
                    {
                        Debug.LogError("Tileset TSX " + tileset.name + " image width (" + tileset.image.width + ") doesn't match actual image width (" + realWidth + ")");
                        if (ImportUtils.s_validationMode)
                        {
                            return null;
                        }
                    }
                    if (realHeight != tileset.image.height)
                    {
                        Debug.LogError("Tileset TSX " + tileset.name + " image height (" + tileset.image.height + ") doesn't match actual image height (" + realHeight + ")");
                        if (ImportUtils.s_validationMode)
                        {
                            return null;
                        }
                    }
                    if (realWidth > 8192 || realHeight > 8192)
                    {
                        Debug.LogError("The tileset image " + tileset.image.source + " is larger than Unity's maximum texture size!  This will impact tile quality.  Try packing the tiles into a more square texture within 8192x8192");
                        if (ImportUtils.s_validationMode)
                        {
                            return null;
                        }
                    }

                    int tileCount = tileset.GetTileCount();
                    int actualTileCount = tileset.GetTileCountGivenDimensions(realWidth, realHeight);
                    if (tileCount != actualTileCount)
                    {
                        Debug.LogError("Tileset TSX " + tileset.name + " tileCount (" + tileset.GetTileCount() + ") doesn't match actual image tile count (" + actualTileCount + ")");
                        if (ImportUtils.s_validationMode)
                        {
                            return null;
                        }
                    }
                    CreateSingleImageTileTargetPaths(actualTileCount, imageSourcePath, tilesetTileTargetDir, out tileTargetPaths);

                    string subSpriteNameBase = Path.GetFileNameWithoutExtension(imageSourcePath);
                    if (!CreateTilemapSprite(imageTargetPath, cellWidth, cellHeight, pixelsPerUnit, tileset, subSpriteNameBase, realWidth, realHeight, out tileSprites))
                    {
                        return null;
                    }
                }
                else
                {
                    Debug.LogError("Tileset " + tileset.name + " is empty!");
                }
            }
            else
            {
                string[] imageSourcePaths = new string[tiles.Length];
                imageTargetPaths = new string[tiles.Length];
                tileTargetPaths = new string[tiles.Length];
                CreateTilePaths(tiles, sourceTilesetDirectory, tilesetSpriteTargetDir, tilesetTileTargetDir, imageSourcePaths, imageTargetPaths, tileTargetPaths);
                CopyImages(imageSourcePaths, imageTargetPaths);

                tileSprites = new Sprite[tiles.Length];
                CreateSpriteAssets(tileset, pixelsPerUnit, cellWidth, cellHeight, imageTargetPaths, tileSprites);
            }

            if (tileSprites == null)
            {
                Debug.LogError("Tile sprites ended up null when importing tileset: " + tileset.name);
                return null;
            }
            if (tileSprites.Length == 0)
            {
                Debug.LogError("0 tile sprites found from texture assets for tileset: " + tileset.name);
                return null;
            }

            ImportedTile[] tileAssets;
            bool success = CreateTileAssets(tiles, singleImageTileset, tileset.name, pixelsPerUnit, tileSprites, tileTargetPaths, out tileAssets);
            EditorUtility.ClearProgressBar();

            if (!success)
            {
                return null;
            }

            CreatePalette(tileset.name, tilesetTileTargetDir, tileAssets, singleImageTileset, tileset.tilewidth, tileset.tileheight, tileset.columns, cellWidth, cellHeight, tileset.tileoffset);
            return tileAssets;
        }

        static void CreatePalette(string tilesetName, string tilesetTileTargetDir, ImportedTile[] tileAssets, bool singleImageTileset, int tileWidth, int tileHeight, int columns, int cellWidth, int cellHeight, TSX.TileOffset tileOffset)
        {
            GameObject newPaletteGO = new GameObject(tilesetName, typeof(Grid));
            newPaletteGO.GetComponent<Grid>().cellSize = new Vector3(1.0f, 1.0f, 0.0f);
            GameObject paletteTilemapGO = new GameObject("Layer1", typeof(Tilemap), typeof(TilemapRenderer));
            paletteTilemapGO.transform.SetParent(newPaletteGO.transform);

            paletteTilemapGO.GetComponent<TilemapRenderer>().enabled = false;

            Tilemap paletteTilemap = paletteTilemapGO.GetComponent<Tilemap>();
            paletteTilemap.tileAnchor = TiledTSXImporter.GetPivot(tileWidth, tileHeight, cellWidth, cellHeight, tileOffset);
            if (columns <= 0)
            {
                columns = 5;
            }

            if (singleImageTileset)
            {
                for (int i = 0; i < tileAssets.Length; i++)
                {
                    if (tileAssets[i] == null || tileAssets[i].tile == null)
                    {
                        continue;
                    }
                    Sprite sprite = tileAssets[i].tile.sprite;
                    Rect rect = sprite.rect;
                    int x = (int)rect.x / tileWidth;
                    int y = (int)rect.y / tileHeight;
                    paletteTilemap.SetTile(new Vector3Int(x, y, 0), tileAssets[i].tile);
                }
            }
            else
            {
                int x = 0;
                int y = 0;
                for (int i = 0; i < tileAssets.Length; i++)
                {
                    if (tileAssets[i] == null || tileAssets[i].tile == null)
                    {
                        continue;
                    }
                    paletteTilemap.SetTile(new Vector3Int(x, y, 0), tileAssets[i].tile);
                    x++;
                    if (x >= columns)
                    {
                        x = 0;
                        y--;
                    }
                }
            }
            string palettePath = tilesetTileTargetDir + Path.DirectorySeparatorChar + tilesetName + ".prefab";
            palettePath = palettePath.Replace('\\', '/');

            bool createdPrefab = false;
            GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(newPaletteGO, palettePath, out createdPrefab);
            if (!createdPrefab)
            {
                Debug.Log("Failed to create tile palette asset at " + palettePath);
            }

            AssetDatabase.SaveAssets();

            UnityEngine.GameObject.DestroyImmediate(newPaletteGO);

            // Clear out any old subassets
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(palettePath);
            for (int i = 0; i < assets.Length; i++)
            {
                UnityEngine.Object asset = assets[i];
                if (!AssetDatabase.IsMainAsset(asset) && asset is GridPalette)
                {
                    UnityEngine.Object.DestroyImmediate(asset, true);
                }
            }

            GridPalette gridPalette = ScriptableObject.CreateInstance<GridPalette>();
            gridPalette.cellSizing = GridPalette.CellSizing.Automatic;
            gridPalette.name = "Palette Settings";
            AssetDatabase.AddObjectToAsset(gridPalette, newPrefab);
            AssetDatabase.SaveAssets();
        }
    }
}
