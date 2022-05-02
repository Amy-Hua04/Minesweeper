using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Animation2D;

namespace Minesweeper
{
    class TilesManager
    {
        static Random rng = new Random();

        private int[,] mines;
        private int[] mineCol;

        private int flagCount;

        private Tile[,] tiles;

        private int rows;
        private int columns;

        public TilesManager (int rows, int columns, int mineCount)
        {
            this.rows = rows;
            this.columns = columns;

            flagCount = mineCount;

            tiles = new Tile[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    tiles[i, j] = new Tile(i, j);
                }
            }

            int randomRow;
            int randomCol;

            int[,] adjaceTiles;
            mineCol = new int[mineCount];

            mines = new int[rows, columns];

            for (int i = 0; i < mineCount; i++)
            {
                randomRow = rng.Next(0, rows);
                randomCol = rng.Next(0, columns);

                while (tiles[randomRow, randomCol].IsMine() == true)
                {
                    randomRow = rng.Next(0, rows);
                    randomCol = rng.Next(0, columns);
                }

                tiles[randomRow, randomCol].AssignMine(rng.Next(0, 8));
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (tiles[i,j].IsMine())
                    {
                        adjaceTiles = tiles[i, j].GetAdjace();                        
                        for (int k = 0; k < adjaceTiles.GetLength(0); k++)
                        {
                            if (adjaceTiles[k, 0] >= 0 && adjaceTiles[k, 0] < rows && adjaceTiles[k, 1] >= 0 && adjaceTiles[k, 1] < columns)
                            {
                                tiles[adjaceTiles[k, 0], adjaceTiles[k, 1]].AddNum();
                            }

                        }
                    }
                }
            }
        }

        public void ShowTile(int row, int col, int hudHeight, int tileLength, SoundEffect smallClear, SoundEffect largeClear)
        {
            if (tiles[row, col].GetRevealState() == false)
            {
                tiles[row, col].RevealTile();
                
                if (tiles[row, col].GetNum() == 0 && tiles[row, col].IsMine() == false)
                {
                    RevealSurroundingTiles(row, col);
                    largeClear.CreateInstance().Play();
                }
                
                else
                {
                    smallClear.CreateInstance().Play();
                }
            }
        }

        public void RevealSurroundingTiles(int row, int col)
        {
            if (tiles[row, col].GetNum() == 0 && tiles[row, col].IsMine() == false)
            {
                int[,] adjaceTiles = tiles[row, col].GetAdjace();
                for (int k = 0; k < adjaceTiles.GetLength(0); k++)
                {
                    if (adjaceTiles[k, 0] >= 0 && adjaceTiles[k, 0] < rows && adjaceTiles[k, 1] >= 0 && adjaceTiles[k, 1] < columns)
                    {
                        if (tiles[adjaceTiles[k, 0], adjaceTiles[k, 1]].GetRevealState() == false)
                        {
                            tiles[adjaceTiles[k, 0], adjaceTiles[k, 1]].RevealTile();
                            if (tiles[adjaceTiles[k, 0], adjaceTiles[k, 1]].GetNum() == 0)
                            {
                                RevealSurroundingTiles(adjaceTiles[k, 0], adjaceTiles[k, 1]);
                            }
                        }
                    }
                }
            }
        }

        public void DrawTiles(SpriteBatch spriteBatch, Texture2D clearDark, Texture2D clearLight, Texture2D[] minesImgs, Texture2D[] numsImgs, Texture2D flag, int hudHeight, int tileLength)
        {
            for (int i = 0; i < tiles.GetLength(0) ; i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    if (tiles[i,j].GetRevealState() == true)
                    {
                        if (i % 2 == j % 2)
                        {
                            spriteBatch.Draw(clearLight, new Rectangle(j * tileLength, i * tileLength + hudHeight, tileLength, tileLength), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(clearDark, new Rectangle(j * tileLength, i * tileLength + hudHeight, tileLength, tileLength), Color.White);
                        }

                        if (tiles[i, j].GetRevealState() == true)
                        {
                            if (tiles[i, j].IsMine())
                            {
                                spriteBatch.Draw(minesImgs[tiles[i, j].GetCol()], new Rectangle(tileLength * j, tileLength * i + hudHeight, tileLength, tileLength), Color.White);
                            }                            
                            else if(tiles[i, j].GetNum() != 0)
                            {
                                spriteBatch.Draw(numsImgs[tiles[i, j].GetNum() - 1], new Rectangle(tileLength * j, tileLength * i + hudHeight, tileLength, tileLength), Color.White);
                            }
                        }
                    }

                    else if (tiles[i, j].IsFlagged())
                    {
                        spriteBatch.Draw(flag, new Rectangle(j * tileLength, i * tileLength + hudHeight, tileLength, tileLength), Color.White);
                    }
                }
            }            
        }

        public void FlagTile(int row, int col, SoundEffect placeFlag, SoundEffect clearFlag)
        {
            if (tiles[row, col].GetRevealState() == false)
            {
                if (tiles[row, col].IsFlagged() == true)
                {
                    clearFlag.CreateInstance().Play();
                    flagCount++;
                }

                else
                {
                    placeFlag.CreateInstance().Play();
                    flagCount--;
                }

                tiles[row, col].Flag();
            }
        }

        public void RevealMines()
        {
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    if (tiles[i,j].IsMine())
                    {
                        tiles[i, j].RevealTile();
                    }
                }
            }
        }

        public bool IsFlagged(int row, int col)
        {
            return tiles[row, col].IsFlagged();
        }

        public string GetFlagCount()
        {
            return Convert.ToString(flagCount);
        }

        public bool CheckLoss(int row, int col)
        {
            if (tiles[row, col].IsMine())
            {
                return true;
            }

            return false;
        }

        public bool CheckWin()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (tiles[i,j].IsMine() == false && tiles[i,j].GetRevealState() == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
