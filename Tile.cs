using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{    
    class Tile
    {
        private int row;
        private int col;

        private bool reveal;
        private int num;
        private bool isMine;
        private int mineCol;
        private bool flag;

        public Tile(int row, int col)
        {
            this.row = row;
            this.col = col;

            reveal = false;
            num = 0;
            isMine = false;
            flag = false;
        }

        public void RevealTile()
        {
            reveal = true;
        }

        public void AssignNum(int num)
        {
            this.num = num;
        }

        public void AddNum()
        {
            num++;
        }

        public int GetNum()
        {
            return num;
        }

        public void AssignMine(int mineCol)
        {
            isMine = true;
            this.mineCol = mineCol;
        }
        public bool IsMine()
        {
            return isMine;
        }

        public int[,] GetAdjace()
        {
            int[,] adjaceTiles = new int[,]
                { { row - 1, col - 1 }, { row - 1, col }, { row - 1, col + 1 },
                { row, col - 1 }, { row, col + 1 },
                { row + 1, col - 1 }, { row + 1, col }, { row + 1, col + 1 } };

            return adjaceTiles;
        }

        public bool GetRevealState()
        {
            return reveal;
        }

        public int GetCol()
        {
            return mineCol;
        }

        public bool IsFlagged()
        {
            return flag;
        }

        public void Flag()
        {
            flag = !flag;
        }
    }
}
