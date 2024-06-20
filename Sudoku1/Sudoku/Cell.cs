using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku1
{
    /// <summary>
    /// Typ wyliczeniowy enum o nazwie EnumCellStatus, zawiera 4 opcje określające stan uzupełnionego pola na planszy.
    /// </summary>
    public enum EnumCellStatus
    {
        TO_GUESS,
        GIVEN,
        CORRECT_GUESS,
        WRONG_GUESS
    }
    /// <summary>
    /// Publiczna Klasa Cell, zawierająca informacje o  polach w planszy oraz metode przysłaniającą ToString
    /// </summary>
    public class Cell
    {
        public int x;   // koordynat x 
        public int y;   // koordynat y
        int _num;   // numer znajdujący się na konkretnym polu
        public int group;   // określa, w którym kwadracie 3x3 znajduje się konkretna komórka
        private EnumCellStatus status;

        public Cell() { }
        /// <summary>
        /// Konstruktor zawierający 3 parametry, który inicjuje położenie x i y, grupę oraz numer początkowy, a następnie sprawdza
        /// czy numer jest podany w startowej planszy, czy jest wolnym polem do uzupełnienia przez użytkownika.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="num"></param>
        public Cell(int x, int y, int num)
        {
            this.x = x;
            this.y = y;
            group = (int)Math.Floor((decimal)y / 3) * 3 + (int)Math.Floor((decimal)x / 3);
            _num = num;
            if (num != 0) { Status = EnumCellStatus.GIVEN; } else { Status = EnumCellStatus.TO_GUESS; }
        }
        /// <summary>
        /// Właściwość Num, która pozwala na zmiane wartości pola w poprawny sposób, czyli kiedy są spełnione wymagania dotyczące
        /// odpowiedniego numeru wpisanywanego w pole oraz czy status jest różny od podanego przy starcie.
        /// </summary>
        public int Num { 
            get => _num; 
            set 
            {
                if (Status != EnumCellStatus.GIVEN)
                {
                    if ((value <= 9) & (value >= 1))
                    {
                        _num = value;
                    }
                    else
                    {
                        Status = EnumCellStatus.TO_GUESS;
                        _num = 0;
                    }
                }
            } 
        }

        public EnumCellStatus Status { get => status; set => status = value; }
        /// <summary>
        /// Publiczna metoda przysłaniająca ToString, która zwraca numer przechowywany przez komórkę
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Num} ";
        }
    }
}
