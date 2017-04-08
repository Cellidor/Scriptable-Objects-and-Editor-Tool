using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;
using UnityEditor;
using System.Xml;

public class LevelManager : MonoBehaviour {

    public Transform groundHolder;
    public Transform turretHolder;

    public enum MapDataFormat {
        Base64,
        CSV
    }

    public MapDataFormat mapDataFormat;
    public Texture2D spriteSheetTexture;

    public GameObject tilePrefab;
    public GameObject turretPrefab;
    public List<Sprite> mapSprites;
    public List<GameObject> tiles;
    public List<Vector3> turretPositions;

    Vector3 tileCenterOffset;
    Vector3 mapCenterOffset;

    public string TMXFilename;

    string gameDirectory;
    string dataDirectory;
    string mapsDirectory;
    string spriteSheetFile;
    string TMXFile;

    public int pixelsPerUnit = 32;

    public int tileWidthInPixels;
    public int tileHeightInPixels;
    float tileWidth;
    float tileHeight;

    public int spriteSheetColumns;
    public int spriteSheetRows;

    public int mapColumns;
    public int mapRows;

    public string mapDataString;
    public List<int> mapData;




    // from http://answers.unity3d.com/questions/10580/editor-script-how-to-clear-the-console-output-wind.html
    static void ClearEditorConsole() {
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        Type type = assembly.GetType("UnityEditorInternal.LogEntries");
        MethodInfo method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

    static string GetJoinedUnixPath(string partA, string partB) {
        return Path.GetFullPath(Path.Combine(partA, partB)).Replace("\\", "/");
    }

    static void DestroyChildren(Transform parent) {
        for (int i = parent.childCount - 1; i >= 0; i--) {
            DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }



    // NOTE: CURRENTLY ONLY WORKS WITH A SINGLE TILED TILESET

    public void LoadLevel() {

        ClearEditorConsole();
        DestroyChildren(groundHolder);
        DestroyChildren(turretHolder);

        // 
        {
            gameDirectory = Environment.CurrentDirectory;
            dataDirectory = GetJoinedUnixPath(Directory.GetParent(gameDirectory).FullName, "Data");
            mapsDirectory = GetJoinedUnixPath(dataDirectory, "Maps");
            TMXFile = GetJoinedUnixPath(mapsDirectory, TMXFilename);
        }


        // 
        {
            mapData.Clear();

            string content = File.ReadAllText(TMXFile);

            using (XmlReader reader = XmlReader.Create(new StringReader(content))) {

                reader.ReadToFollowing("map");
                mapColumns = Convert.ToInt32(reader.GetAttribute("width"));
                mapRows = Convert.ToInt32(reader.GetAttribute("height"));

                reader.ReadToFollowing("tileset");
                tileWidthInPixels = Convert.ToInt32(reader.GetAttribute("tilewidth"));
                tileHeightInPixels = Convert.ToInt32(reader.GetAttribute("tileheight"));

                int spriteSheetTileCount = Convert.ToInt32(reader.GetAttribute("tilecount"));
                spriteSheetColumns = Convert.ToInt32(reader.GetAttribute("columns"));
                spriteSheetRows = spriteSheetTileCount / spriteSheetColumns;


                reader.ReadToFollowing("image");
                spriteSheetFile = GetJoinedUnixPath(mapsDirectory, reader.GetAttribute("source"));


                reader.ReadToFollowing("layer");


                reader.ReadToFollowing("data");
                string encodingType = reader.GetAttribute("encoding");

                switch (encodingType) {
                    case "base64":
                        mapDataFormat = MapDataFormat.Base64;
                        break;
                    case "csv":
                        mapDataFormat = MapDataFormat.CSV;
                        break;
                }

                mapDataString = reader.ReadElementContentAsString().Trim();


                turretPositions.Clear();

                if (reader.ReadToFollowing("objectgroup")) {
                    if (reader.ReadToDescendant("object")) {
                        do {
                            float x = Convert.ToSingle(reader.GetAttribute("x")) / (float)pixelsPerUnit;
                            float y = Convert.ToSingle(reader.GetAttribute("y")) / (float)pixelsPerUnit;
                            turretPositions.Add(new Vector3(x, -y, 0));

                        } while (reader.ReadToNextSibling("object"));
                    }
                }

            }


            switch (mapDataFormat) {

                case MapDataFormat.Base64:

                    byte[] bytes = Convert.FromBase64String(mapDataString);
                    int index = 0;
                    while (index < bytes.Length) {
                        int tileID = BitConverter.ToInt32(bytes, index) - 1;
                        mapData.Add(tileID);
                        index += 4;
                    }
                    break;


                case MapDataFormat.CSV:

                    string[] lines = mapDataString.Split(new string[] { " " }, StringSplitOptions.None);
                    foreach (string line in lines) {
                        string[] values = line.Split(new string[] { "," }, StringSplitOptions.None);
                        foreach (string value in values) {
                            int tileID = Convert.ToInt32(value) - 1;
                            mapData.Add(tileID);
                        }
                    }
                    break;

            }

        }


        {
            tileWidth = (tileWidthInPixels / (float)pixelsPerUnit);
            tileHeight = (tileHeightInPixels / (float)pixelsPerUnit);

            tileCenterOffset = new Vector3(.5f * tileWidth, -.5f * tileHeight, 0);
            mapCenterOffset = new Vector3(-(mapColumns * tileWidth) * .5f, (mapRows * tileHeight) * .5f, 0);

        }




        // 
        {
            spriteSheetTexture = new Texture2D(2, 2);
            spriteSheetTexture.LoadImage(File.ReadAllBytes(spriteSheetFile));
            spriteSheetTexture.filterMode = FilterMode.Point;
            spriteSheetTexture.wrapMode = TextureWrapMode.Clamp;
        }


        // 
        {
            mapSprites.Clear();

            for (int y = spriteSheetRows - 1; y >= 0; y--) {
                for (int x = 0; x < spriteSheetColumns; x++) {
                    Sprite newSprite = Sprite.Create(spriteSheetTexture, new Rect(x * tileWidthInPixels, y * tileHeightInPixels, tileWidthInPixels, tileHeightInPixels), new Vector2(0.5f, 0.5f), pixelsPerUnit);
                    mapSprites.Add(newSprite);
                }
            }
        }

        // 
        {
            tiles.Clear();

            for (int y = 0; y < mapRows; y++) {
                for (int x = 0; x < mapColumns; x++) {

                    int mapDatatIndex = x + (y * mapColumns);
                    int tileID = mapData[mapDatatIndex];

                    GameObject tile = Instantiate(tilePrefab, new Vector3(x * tileWidth, -y * tileHeight, 0) + mapCenterOffset + tileCenterOffset, Quaternion.identity) as GameObject;
                    tile.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = mapSprites[tileID];
                    tile.transform.parent = groundHolder;
                    tiles.Add(tile);
                }
            }
        }


        // 
        {
            foreach (Vector3 turretPosition in turretPositions) {
                GameObject turret = Instantiate(turretPrefab, turretPosition + mapCenterOffset, Quaternion.identity) as GameObject;
                turret.name = "Turret";
                turret.transform.parent = turretHolder;
            }
        }


        DateTime localDate = DateTime.Now;
        print("Level loaded at: " + localDate.Hour + ":" + localDate.Minute + ":" + localDate.Second);
    }
}


