using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sudoku1
{
    /// <summary>
    /// Publiczna klasa wyjątku, dziedzicząca po Exception
    /// </summary>
    public class ZlaNazwaPlikuException : Exception
    {
        public ZlaNazwaPlikuException()
        {
        }
    }
    /// <summary>
    /// Publiczna klasa Sudoku korzystająca z interfejsu ISavable, zawiera metody i właściwości potrzebne do 
    /// tworzenia i obsługi planszy do sudoku.
    /// </summary>
    [XmlInclude(typeof(EasySudoku))]
    [Serializable]
    public class Sudoku: ISavable
    {
        protected List<Cell> cells;
        private int currentNum = 0;

        public int CurrentNum { 
            get => currentNum;
            set {
                if ((value <= 9) & (value >= 1))
                {
                    currentNum = value;
                }
                else
                {
                    currentNum = 0;
                }
            }
        }

        public List<Cell> Cells { get => cells; set => cells = value; }

        public Sudoku()
        {
            cells = new List<Cell>();
        }
        /// <summary>
        /// Chroniona, wirtualna lista obiektów Cell z parametrem, który jest tablicą. Wypełnia listę obiektami Cell o pozycjach i oraz j, 
        /// a także wstawia numery przechowywane przez odpowiednie elementy na konkretne pozycje. Na końcu funkcja zwraca tą listę
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        protected virtual List<Cell> Generate(int[,] board)
        {
            List<Cell> res = new List<Cell>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    res.Add(new Cell(i, j, board[i, j]));
                }
            }
            return res;
        }
        /// <summary>
        /// Chroniona, wirtualna lista obiektów Cell, nieparametryczna. Wypełnia listę obiektami Cell o pozycji i oraz j, a także numerze 0.
        /// </summary>
        /// <returns></returns>
        protected virtual List<Cell> Generate()
        {
            List<Cell> res = new List<Cell>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    res.Add(new Cell(i, j, 0));
                }
            }

            FillValues(res);

            return res;
        }
        /// <summary>
        /// Prywatna metoda FillValues. Korzysta z dwóch opisanych funkcji, aby uzupełnić całe sudoku.
        /// </summary>
        /// <param name="temp"></param>
        private void FillValues(List<Cell> temp)
        {
            FillDiagonal(temp);
            FillRemaining(temp);
        }
        /// <summary>
        /// Prywatna metoda FillDiagonal z parametrem, który jest listą obiektów Cell o nazwie temp. 
        /// Wyszukuje kwadraty 3x3 po przekątnej i uzupełnia je funkcją FillBox.
        /// </summary>
        /// <param name="temp"></param>
        private void FillDiagonal(List<Cell> temp)
        {
            for (int i = 0; i < 9; i += 4)
            {
                var temp1 = temp.FindAll(c => c.group == i);
                FIllBox(temp1);
            }
        }
        /// <summary>
        /// Prywatna metoda FillBox, której parametrem jest lista obiektó Cell o nazwie temp. 
        /// Uzupełnia podaną listę (kwadraty 3x3) numerami od 1 do 9.
        /// </summary>
        /// <param name="temp"></param>
        private void FIllBox(List<Cell> temp)
        {
            Random rnd = new Random();
            int i1;
            int i2;
            List<int> numbers = new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            while (numbers.Count > 0)
            {
                i1 = rnd.Next(0, numbers.Count);
                i2 = rnd.Next(0, numbers.Count);
                temp[i1].Num = numbers[i2];
                temp.RemoveAt(i1);
                numbers.RemoveAt(i2);
            }
        }
        /// <summary>
        /// Prywatna metoda boll FillRemaining. Uzupełnia pozostałe pola sudoku cyframi.
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        private bool FillRemaining(List<Cell> temp)
        {
            Cell? current = FindUnassigned(temp);
            if (current == null)
            {
                return true;
            }

            for (int i = 1; i <= 9; i++)
            {
                if (IsSafe(temp, current, i))
                {
                    current.Num = i;

                    if (FillRemaining(temp))
                    {
                        return true;
                    }
                    current.Num = 0;
                }
            }

            return false;
        }
        /// <summary>
        /// Prywatna metoda dla obiektów klasy Cell o nazwie FindUnassigned. 
        /// Zwraca pierwszą komórkę, która przechowuje numer 0 lub null.
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        private Cell? FindUnassigned(List<Cell> temp)
        {
            return temp.FirstOrDefault(c => c.Num == 0);
        }
        /// <summary>
        /// Prywatna metoda boll IsSafe z 3 parametrami. Sprawdza czy wpisanie numeru do komórki będzie poprawne.
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="cell"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private bool IsSafe(List<Cell> temp, Cell cell, int num)
        {
            return !temp.Where(c => (c.group == cell.group) | (c.y == cell.y) | (c.x == cell.x)).Any(c => (c.Num == num));
        }
        /// <summary>
        /// Usuwa n losowych pozycji (ustawia Num = 0).
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="n"></param>
        protected void RemoveNDigits(List<Cell> temp, int n)
        {
            Random rnd = new Random();
            int i1;
            for (int i = 0; i < n; i++)
            {
                i1 = rnd.Next(0, 81);
                if(temp[i1].Num != 0)
                {
                    temp[i1].Num = 0;
                }
                else
                {
                    i--;
                }

            }
        }
        /// <summary>
        /// Odpowiada za ustalenie numerów i statusów komórki.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="num"></param>
        public void SetValue(Cell cell, int num)
        {
            if (cell.Status != EnumCellStatus.GIVEN)
            {
                cell.Num = num;
                if (num == 0)
                {
                    cell.Status = EnumCellStatus.TO_GUESS;
                    Validate();
                }
                else
                {
                    if (IsCellCorrect(cell))
                    {
                        cell.Status = EnumCellStatus.CORRECT_GUESS;
                    }
                    else
                    {
                        Validate();
                    }
                }
            }
        }
        /// <summary>
        /// Wywołuje funkcję SetValue z parametrami Cell cell i int num.
        /// </summary>
        /// <param name="cell"></param>
        public void SetValue(Cell cell)
        {
            SetValue(cell, currentNum);
        }
        /// <summary>
        /// Znajduje komórkę o konkretnym położeniu i wywołuje funkcję SetValue z parametrami Cell cell i int num
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="num"></param>
        public void SetValue(int x, int y, int num)
        {
            Cell cell = cells.First(c => ((c.x == x) & (c.y == y)));
            SetValue(cell, num);
        }
        /// <summary>
        /// Ustala wartość pola na konkretny zaznaczony numer i wywołuje funkcję SetValue z trzema parametrami
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetValue(int x, int y)
        {
            SetValue(x, y, currentNum);
        }
        /// <summary>
        /// Publiczna metoda boll IsCompleted, która jest prawdziwa, jeśli każda pozycja na liście cells jest poprawna i jest uzupełniona
        /// </summary>
        /// <returns></returns>
        public bool IsCompleted() => !cells.Any(c => c.Status == EnumCellStatus.TO_GUESS | c.Status == EnumCellStatus.WRONG_GUESS);
        /// <summary>
        /// Publiczna metoda bool IsCellCorrect z jednym parametrem, który jest obiektem klasy Cell. Funkcja sprawdza czy komórka może przyjąć wybrany numer,
        /// poprzez sprawdzenie elementów listy cells w pionie, poziomie oraz w konkretnym kwadracie na planszy (group), które mogą zawierać pole o takim samym numerze (Num).
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool IsCellCorrect(Cell cell)
        {
            return !cells.Where(c => ((c.x == cell.x) | (c.y == cell.y) | (c.group == cell.group)) & (c != cell)).Any(c => c.Num == cell.Num);
        }
        /// <summary>
        /// Publiczna metoda boll IsCellCorrect z dwoma argumentami. Pod obiekt klasy Cell o nazwie c podstawiamy pierwszą znalezioną komórkę na liście cells, która spełnia
        /// warunek dotyczący podanej pozycji x i y. Przy zwróceniu wywołana zostaje funkcja IsCellCorrect z jednym parametrem dla podanego wcześniej obiektu o nazwie c.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsCellCorrect(int x, int y)
        {
            Cell c = cells.First(c => ((c.x == x) & (c.y == y)));
            return IsCellCorrect(c);
        }
        /// <summary>
        /// Publiczna metoda Validate. Tworzy listę obiektów typu Cell o nazwie temp, która zawiera elementy listy cells, których status 
        /// jest różny od GIVEN oraz których numer jest różny od 0. Następnie dla każdej komórki z listy temp sprawdza czy jest ona poprawna i ustala odpowiedni 
        /// status komórki (CORRECT_GUESS w przypadku poprawnego numeru oraz WRONG_GUESS w przypadku numeru nieodpowiedniego)
        /// </summary>
        public void Validate()
        {
            List<Cell> temp = cells.Where(c => (c.Status != EnumCellStatus.GIVEN) & (c.Num != 0)).ToList();
            temp.ForEach(c => c.Status = IsCellCorrect(c) ? EnumCellStatus.CORRECT_GUESS : EnumCellStatus.WRONG_GUESS);

        }
        /// <summary>
        /// Publiczna metoda Save, zapisująca plik o nazwie "Board.xml" wykorzystując interfejs ISavable
        /// </summary>
        public void Save()
        {
            Save("Board.xml");
        }
        /// <summary>
        /// Publiczna metoda Save z jednym parametrem, który określa nazwę pliku. Funkcja zapisuje plik, o podanej jako argument nazwie, do formatu xml.
        /// </summary>
        /// <param name="nazwa"></param>
        public void Save(string nazwa)
        {
            using StreamWriter sw = new(nazwa);
            XmlSerializer xs = new(typeof(EasySudoku));
            xs.Serialize(sw, this);
        }
        /// <summary>
        /// Odczytuje sudoku z pliku i zwraca je.
        /// </summary>
        /// <param name="nazwa"></param>
        /// <returns></returns>
        /// <exception cref="ZlaNazwaPlikuException"></exception>
        public static Sudoku? LoadEasy(string nazwa)
        {
            if (!File.Exists(nazwa))
            {
                throw new ZlaNazwaPlikuException();
            }

            using StreamReader sw = new(nazwa);
            XmlSerializer xs = new(typeof(EasySudoku));
            Sudoku es = (Sudoku)xs.Deserialize(sw);

            return es;
        }
        /// <summary>
        /// Publiczna metoda zakrywająca ToString, która wypisuje elementy z listy cells, które mają określoną pozycję x i y równą wartością i oraz j w pętlach for.
        /// Konwertuje ona elementy na typ string i spisuje do pustego stringa o nazwie res, zmieniając linijkę po 9 uzupełnionych polach. Funkcja zwraca takiego stringa.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string res = string.Empty;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    res += cells.First(c => (c.x == i & c.y == j)).ToString();
                }
                res += "\n";
            }
            
            return res;
        }
    }
    /// <summary>
    /// Klasa EasySudoku, reprezentująca łatwy tryb sudoku.
    /// </summary>
    [XmlInclude(typeof(Sudoku))]
    [Serializable]
    public class EasySudoku : Sudoku
    {
        public EasySudoku() : base() 
        {
        }
        public EasySudoku(bool n) : base() 
        {
            cells = Generate();
        }
        public EasySudoku( int[,] board ) : base()
        {
            cells = Generate(board);
        }
        /// <summary>
        /// Metoda przysłaniająca metodę Generate() usuwająca 50 elementów z listy, ustawiająca komórkom pozostawionym status GIVEN. Na końcu zwraca tę listę.
        /// </summary>
        /// <returns></returns>
        protected override List<Cell> Generate()
        {
            List<Cell> temp = base.Generate();
            RemoveNDigits(temp, 50);
            temp.FindAll(c => c.Num != 0).ForEach(c => c.Status = EnumCellStatus.GIVEN);
            return temp;
        }   
    }
    /// <summary>
    /// Klasa HardSudoku, reprezentująca trudny tryb sudoku, do implementacji.
    /// </summary>
    public class HardSudoku : Sudoku
    {
        public HardSudoku() : base()
        {
            cells = Generate();
        }

        protected override List<Cell> Generate()
        {
            return base.Generate();
        }
    }
}
