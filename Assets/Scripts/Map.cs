﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Map
    {
        public int size = 7;
        public Tile[] tiles;

        private OutlinedTiles outlinedTiles;

        public Map()
        {
            tiles = new Tile[size * size];
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = new Tile(GetTilePos(i), new Vector2Int(i % size, i/size));
            }

            outlinedTiles = new OutlinedTiles();
            Camera.main.transform.position = new Vector3(size / 2f * Tile.size, 7, size / 2f * Tile.size);
        }

        public void SetTileColor(Vector3 worldPos, Color color)
        {
            var tile = GetTileByPosition(worldPos);
            tile.SetColor(color);
        }

        public void SetTilesColor(Vector3 worldPos, Color color)
        {
            foreach (var tile in outlinedTiles)
            {
                tile.SetColor(color);
            }
        }

        private Tile GetTileByPosition(Vector3 worldPos)
        {
            if (worldPos.x > Tile.size * size)
                worldPos.x = Tile.size * (size - 0.5f);
            if (worldPos.z > Tile.size * (size - 0.5f))
                worldPos.z = Tile.size * (size - 0.5f);
            if (worldPos.x < 0)
                worldPos.x = 0;
            if (worldPos.z < 0)
                worldPos.z = 0;

            var x = (int)worldPos.x / Tile.size;
            var z = (int)worldPos.z / Tile.size;
            return tiles[z * size + x];
        }

        private Vector3 GetTilePos(int tileIndex)
        {
            return GetTilePos(new Vector2(tileIndex % size, tileIndex / size));
        }

        public Vector3 GetTilePos(Vector2 posV2)
        {
            return new Vector3(Tile.size * (posV2.x + 0.5f), 0, Tile.size * (posV2.y + 0.5f));
        }

        public Tile GetTile(Vector2Int posV2)
        {
            return tiles[posV2.x + posV2.y * size];
        }
        public void SetTilesOutLine(Vector3 worldPos, Color color, SelectionOrientation orientation, bool activate)
        {
            outlinedTiles.Clear();
            if (!activate)
                return;

            foreach (var selectedTile in GetSelectedTiles(worldPos, orientation))
            {
                outlinedTiles.Add(selectedTile);
            }
           
        }

        public IEnumerable<Tile> GetSelectedTiles(Vector3 worldPos, SelectionOrientation orientation)
        {
            switch (orientation)
            {
                case SelectionOrientation.Horizontal:
                    yield return GetTileByPosition(worldPos + Vector3.right * Tile.size/2);
                    yield return GetTileByPosition(worldPos - Vector3.right * Tile.size/2);
                    break;
                case SelectionOrientation.Vertival:
                    yield return GetTileByPosition(worldPos + Vector3.forward * Tile.size/2);
                    yield return GetTileByPosition(worldPos - Vector3.forward * Tile.size/2);
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        /// <summary>
        /// d6 dice; 5 = 3; 6 = 2
        /// </summary>
        /// <returns></returns>
        public Tile GetNextTile(Tile currentTile, LookingSide lookingSide)
        {
            var d6 = Random.Range(1, 7);
            return GetNextTile(currentTile, d6, lookingSide);
        }

        private Tile GetNextTile(Tile currentTile, int d6, LookingSide lookingSide)
        {
            Debug.Log( new Vector2Int(d6, 0));
            switch (d6)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    return GetTile(currentTile.position + GetDirection(lookingSide) * d6);
                case 5:
                    return GetNextTile(currentTile, 3, lookingSide);
                case 6:
                    return GetNextTile(currentTile, 2, lookingSide);
                default:
                    return currentTile;
            }
        }

        private Vector2Int GetDirection(LookingSide lookingSide)
        {
            switch (lookingSide)
            {
                case LookingSide.up:
                    return new Vector2Int(0, 1);
                case LookingSide.right:
                    return new Vector2Int(1, 0);
                case LookingSide.down:
                    return new Vector2Int(0, -1);
                case LookingSide.left:
                    return new Vector2Int(-1, 0);
                default:
                    return new Vector2Int(0, 1);
            }
        }
    }
}
