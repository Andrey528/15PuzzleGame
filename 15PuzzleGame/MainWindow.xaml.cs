using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _15PuzzleGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int size;
        private Button[,] buttons;
        private int[,] puzzle;
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            // Получаем размер поля от пользователя
            size = int.Parse(tbSize.Text);

            Button button = (Button)sender;
            button.IsEnabled = false;

            // Инициализируем игру
            InitGame();
        }

        private void InitGame() // Функция инициализации игрового поля
        {
            puzzle = new int[size, size]; //
            buttons = new Button[size, size]; // Массив кнопок
            puzzleGrid.Children.Clear(); // Очищаем поле перед его заполнением

            List<int> numbers = new List<int>(); // Массив чисел для игры
            for (int i = 0; i < size * size; i++) // Заполняем массив чисел, числами от 0 до size*size
            {
                numbers.Add(i);
            }

            // Рассчет размера кнопок
            int buttonWidth = (int)puzzleGrid.ActualWidth / size;
            int buttonHeight = (int)puzzleGrid.ActualHeight / size;

            bool isEven = false;
            while (!isEven) // Проверяем четность числа беспорядков
            {
                numbers.Shuffle(); // Перемешиваем числа
                isEven = CheckParity(numbers); // Проверяем четность числа беспорядков
            }

            for (int i = 0; i < size; i++) // Проходим по инициализируемому полю игры
            {
                puzzleGrid.RowDefinitions.Add(new RowDefinition()); // Добавляем строку в сетку
                puzzleGrid.ColumnDefinitions.Add(new ColumnDefinition()); // Добавляем колонку в сетку

                for (int j = 0; j < size; j++)
                {
                    //int index = random.Next(0, numbers.Count); // Берем случайный индекс от 0 до длины массива чисел
                    //puzzle[i, j] = numbers[index]; // По случайному индексу, получаем число и присваиваем его массиву поля игры (это еще массив чисел)

                    //numbers.RemoveAt(index); // Удаляем из массива всех чисел, число которое уже присвоено массиву поля (карте поля)

                    puzzle[i, j] = numbers[i * size + j]; // Присваиваем числа из перемешанного массива

                    Button button = new Button(); // Создаем кнопку
                    button.Content = puzzle[i, j] == 0 ? "" : puzzle[i, j].ToString(); // Если кнопке достанеться значение равное 0, то мы сделаем кнопку пустой, иначе присвоим кнопке значение из массива поля (карте поля)

                    // Задаем кнопкам размеры
                    button.Width = buttonWidth;
                    button.Height = buttonHeight;

                    button.Margin = new Thickness(2); // Добавляем отступы между кнопками

                    button.Click += Btn_Click; // Добавим кнопке логику ее работы (функцию)
                    Grid.SetRow(button, i); // Устанавливаем игровой сетке строки 
                    Grid.SetColumn(button, j); // Устанавливаем игровой сетке колонки
                    puzzleGrid.Children.Add(button); // Помещаем в ячейку сетки кнопку
                    buttons[i, j] = button; // Сааму кнопку помещаем в массив всех кнопок
                }
            }
        }

        private bool CheckParity(List<int> numbers) // Проверка четности числа беспорядков
        {
            int inversions = 0;
            for (int i = 0; i < numbers.Count - 1; i++)
            {
                for (int j = i + 1; j < numbers.Count; j++)
                {
                    if (numbers[i] > numbers[j] && numbers[i] != 0 && numbers[j] != 0)
                    {
                        inversions++;
                    }
                }
            }
            return inversions % 2 == 0; // Возвращаем true, если количество беспорядков четное, иначе false
        }

        private void Swap(int row1, int col1, int row2, int col2) // Логика перемещения значений кнопок
        {
            int temp = puzzle[row1, col1]; // Помещаем текущее значение одной игровой ячейки во временное хранилище
            puzzle[row1, col1] = puzzle[row2, col2]; // Присваиваем значение первой ячейки значение второй ячейки
            puzzle[row2, col2] = temp; // А здесь присваиваем значени второй ячейки значение первой

            // Теперь присваиваем заменные значения ячеек кнопкам
            buttons[row1, col1].Content = puzzle[row1, col1] == 0 ? "" : puzzle[row1, col1].ToString();
            buttons[row2, col2].Content = puzzle[row2, col2] == 0 ? "" : puzzle[row2, col2].ToString();

            // Проверяем победу после каждого перемещения
            if (CheckWin())
            {
                MessageBox.Show("Поздравляем, вы выиграли!");
                Application.Current.Shutdown();
            }
        }

        private bool CheckWin()
        {
            int count = 1;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    // Проверяем, равно ли значение ячейки счетчику
                    if ((i == size - 1) && (j == size - 1))
                    {
                        if (puzzle[i, j] != 0)
                            return false;
                    }
                    else
                    {
                        if (puzzle[i, j] != count)
                        {
                            // Если хотя бы одно значение не равно ожидаемому, игра не завершена
                            return false;
                        }
                    }
                    count++;
                }
            }

            // Если все значения совпадают с ожидаемыми значениями, значит игрок выиграл
            return true;
        }

        private void Btn_Click(object sender, RoutedEventArgs e) // Логика работы кнопок
        {
            Button button = (Button)sender; // Сохраняем ссылку на объект текущей кнопки из поля
            int row = Grid.GetRow(button); // Сохраняем значение строки текущей кнопки
            int col = Grid.GetColumn(button); // Сохраняем значение колонки текущей кнопки

            // В каждом условии, мы проверяем являеться ли соседняя ячейка пустой (без номера на кнопке, т.е. 0 в игровой карте-массиве)
            // Условия включают в себя границы поля
            if (row > 0 && puzzle[row - 1, col] == 0)
            {
                Swap(row, col, row - 1, col);
            }
            else if (row < size - 1 && puzzle[row + 1, col] == 0)
            {
                Swap(row, col, row + 1, col);
            }
            else if (col > 0 && puzzle[row, col - 1] == 0)
            {
                Swap(row, col, row, col - 1);
            }
            else if (col < size - 1 && puzzle[row, col + 1] == 0)
            {
                Swap(row, col, row, col + 1);
            }
        }
    }

    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random random = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}