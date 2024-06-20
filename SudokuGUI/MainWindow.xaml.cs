using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sudoku1;

namespace SudokuGUI
{
    /// <summary>
    /// Publiczna klasa CellButton, dziedzicząca po klasie Button
    /// </summary>
    public class CellButton : Button
    {
        public int x;
        public int y;
        public Cell? cell;
        public bool highlight = false;
        public CellButton() { }
        /// <summary>
        /// Metoda void Update(), która zmienia kolor tła komórki w zależności od poprawności uzupełnienia oraz ustawia zawartość przycisku.
        /// </summary>
        public void Update()
        {
            if(cell.Status == EnumCellStatus.WRONG_GUESS)
            {
                Background = Brushes.PaleVioletRed;
            }            
            else if (cell.Status == EnumCellStatus.CORRECT_GUESS)
            {
                Background = Brushes.Transparent;
            }
            else if (cell.Status == EnumCellStatus.TO_GUESS)
            {
                Background = Brushes.LightSteelBlue;
            }
            Content = cell.Num == 0 ? "" : cell.Num;
        }
    }
    /// <summary>
    /// Publiczna klasa częściowa MainWindow, która zawiera metody dotyczące wyświetlania okna głównego aplikacji
    /// </summary>
    public partial class MainWindow : Window
    {
        
        Sudoku? sudoku;
        Button? PreviousButton;
        List<CellButton> cellButtons = new List<CellButton>();
        public bool completed;
        public MainWindow()
        {
            InitializeComponent();

            NewGame();
            
        }
        /// <summary>
        /// Inicjuje łatwe sudoku oraz tworzy przyciski.
        /// </summary>
        private void NewGame()
        {
            // Poniżej znajduje się przykładowa plansza sudoku
            /*            new int[9, 9]{
                            { 3, 0, 6, 5, 0, 8, 4, 0, 0},
                            { 5, 2, 0, 0, 0, 0, 0, 0, 0},
                            { 0, 8, 7, 0, 0, 0, 0, 3, 1},
                            { 0, 0, 3, 0, 1, 0, 0, 8, 0},
                            { 9, 0, 0, 8, 6, 3, 0, 0, 5},
                            { 0, 5, 0, 0, 9, 0, 6, 0, 0},
                            { 1, 3, 0, 0, 0, 0, 2, 5, 0},
                            { 0, 0, 0, 0, 0, 0, 0, 7, 4},
                            { 0, 0, 5, 2, 0, 6, 3, 0, 0}};

                           {{ 8, 0, 0, 0, 0, 0, 0, 0, 0 },
                            { 0, 0, 3, 6, 0, 0, 0, 0, 0 },
                            { 0, 7, 0, 0, 9, 0, 2, 0, 0 },
                            { 0, 5, 0, 0, 0, 7, 0, 0, 0 },
                            { 0, 0, 0, 0, 4, 5, 7, 0, 0 },
                            { 0, 0, 0, 1, 0, 0, 0, 3, 0 },
                            { 0, 0, 1, 0, 0, 0, 0, 6, 8 },
                            { 0, 0, 8, 5, 0, 0, 0, 1, 0 },
                            { 0, 9, 0, 0, 0, 0, 4, 0, 0 }
                            };  */
            
            sudoku = new EasySudoku(true);
            CreateCellButtons();
        }
        /// <summary>
        /// Metoda void CreateCellButtons() usuwa i tworzy przyciski, które są polami w przedstawionym sudoku, btn zostaje utworzony jako obiekt klasy CellButton
        /// a następnie dodany do listy cellButtons. W następnych krokach ustalane są podstawowe właściwości przycisku, takie jak czcionka, położenie,
        /// czy rozmiar, ponadto ustawiana jest pozycja przycisku poprzez podstawienie wartości i oraz j oraz dodana metoda CBtn_Click
        /// Jeśli przycisk został podany jako uzupełniony na początkowej planszy, jego tło zostaje pomalowane na szary kolor i nie można tego przycisku używać,
        /// w innym przypadku tło jest ustalane na kolor LightSteelBlue.
        /// Dla przycisku jest wywoływana funkcja Update(), która zmienia kolor jego tła, a następnie ustalana pozycja na siatce Grid, taki przycisk dodawany jest do Board.
        /// </summary>
        private void CreateCellButtons()
        {
            completed = false;
            cellButtons.ForEach(c => Board.Children.Remove(c));
            cellButtons.Clear();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    CellButton btn = new CellButton();
                    cellButtons.Add(btn);
                    btn.HorizontalAlignment = HorizontalAlignment.Center;
                    btn.VerticalAlignment = VerticalAlignment.Center;
                    btn.FontWeight = FontWeights.DemiBold;
                    btn.Foreground = Brushes.Black;
                    btn.Width = 50;
                    btn.Height = 50;
                    btn.Click += CBtn_Click;
                    btn.x = i;
                    btn.y = j;
                    btn.cell = sudoku.Cells.First(c => c.x == i & c.y == j);
                    if (btn.cell.Status == EnumCellStatus.GIVEN)
                    {
                        btn.Background = Brushes.LightGray;
                        btn.IsEnabled = false;
                    }
                    else
                    {
                        btn.Background = Brushes.LightSteelBlue;
                    }
                    btn.Update();
                    Grid.SetRow(btn, i + (int)Math.Floor((decimal)i / 3));
                    Grid.SetColumn(btn, j + (int)Math.Floor((decimal)j / 3));
                    Board.Children.Add(btn);
                }
            }
        }
        /// <summary>
        /// Prywatna metoda void CBtn_Click zawierająca 2 parametry. Jeśli completed ma wartość różną od true, czyli dopóki gra nie jest zakończona, funkcja ta wstawia w miejsce przycisku
        /// numer zaznaczony w tym momencie na dolnej części aplikacji, jeśli jest to początek rozgrywki wartość domyślna jest ustawiona jako 0, więc przyciśnięcie pozycji nie zmieni przycisku.
        /// Jeśli rozwiązywanie sudoku jest zakończone, przyciśnięcie dowolnego dostępnego przycisku z planszy spowoduje wyswietlenie komunikatu informującego o wygranej, w komunikacie można
        /// wybrać opcję, aby rozpocząć nową grę, wtedy tworzona jest nowa gra.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!completed)
            {
                int num;
                CellButton cbtn = ((CellButton)sender);
                if (sudoku.CurrentNum == 0) 
                {
                    num = cbtn.cell.Num; 
                    sudoku.SetValue(cbtn.cell);
                    ((CellButton)sender).Update();
                    cellButtons.Where(c => c.cell.Num == num).ToList().ForEach(c => c.Update());
                }
                else
                {
                    sudoku.SetValue(cbtn.cell);
                    ((CellButton)sender).Update();
                    num = cbtn.cell.Num;
                    cellButtons.Where(c => c.cell.Num == num).ToList().ForEach(c => c.Update());
                }
            }
            if (sudoku.IsCompleted())
            {
                completed = true;
                string msg = "Wygrałeś!!! Chesz rozpocząć nową grę?";
                MessageBoxResult result =
                      MessageBox.Show(
                        msg,
                        "Wygrałeś",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                if(result == MessageBoxResult.Yes)
                {
                    NewGame();
                }
            }
        }
        /// <summary>
        /// Prywatna metoda Button_Click z 2 parametrami. Wywoływana jest w niej metoda ChangeHighlight, a następnie podstawiana wartość. Metoda ta dotyczy przycisków w dolnej części
        /// aplikacji (1-9 oraz X), które odpowiadają za uzupełnianie pozostałych pól w tablicy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int num;
            ChangeHighlight(((Button)sender));

            if(Int32.TryParse((string)((Button)sender).Content, out num))
            {
                sudoku.CurrentNum = num;
            } 
            else
            {
                sudoku.CurrentNum = 0;
            }            
        }
        /// <summary>
        /// Prywatna metoda void ChangeHighlight z jednym parametrem. Zmienia tło przycisku.
        /// </summary>
        /// <param name="b"></param>
        private void ChangeHighlight(Button b)
        {
            if (PreviousButton != null)
            {
                PreviousButton.Background = Brushes.LightGray;
            }
            PreviousButton = b;
            b.Background = Brushes.PowderBlue; ;
        }
        /// <summary>
        /// Prywatna metoda New_Game_Click z dwoma parametrami. Jest ona użyta w przypadku naciśnięcia przycisku w dolnej części aplikacji o nazwie "Nowa Gra".Generuje nową grę.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void New_Game_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }
        /// <summary>
        /// Prywatna metoda Solve_Click z dwoma parametrami. Jest ona użyta w przypadku naciśnięcia przycisku w dolnej części aplikacji o nazwie "Rozwiąż". Rozwiazuje sudoku przy użyciu
        /// stworzonej funkcji Solve(), aktualizuje własności przycisków oraz zmienia wartość completed na true, oznacza to, że gra została zakończona.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            Solver.Solve(sudoku.Cells);
            cellButtons.ForEach(c => c.Update());
            completed = true;
        }
        /// <summary>
        /// Prywatna metoda Load_Click z dwoma parametrami. Jest ona użyta w przypadku naciśnięcia przycisku w dolnej części aplikacji o nazwie "Wczytaj". Wczytuje plik "Board.xml" i 
        /// tworzy nową plansze do sudoku, jeśli nie ma dostępnej gry o takiej nazwie zostanie wyświetlony komunikat ostrzegający o braku zapisanej gry.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var newsudoku = Sudoku.LoadEasy("Board.xml");
                sudoku = newsudoku;
                CreateCellButtons();

            }
            catch
            {
                string msg = "Brak zapisanej gry";
                MessageBoxResult result =
                      MessageBox.Show(
                        msg,
                        "Warning",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
            }
        }
        /// <summary>
        /// Prywatna metoda MainWindow_Closing z dwoma parametrami. Wywoływana jest przy zamykaniu okna aplikacji, wyświetlany jest komunikat, czy użytkownik chce zapisać grę, jeśli
        /// użytkownik będzie chciał to zrobić, wtedy gra zostanie zapisana.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!completed)
            {
                string msg = "Chcesz zapisać grę?";
                MessageBoxResult result =
                      MessageBox.Show(
                        msg,
                        "Sudoku",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    sudoku.Save();
                } 
                else if(result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
