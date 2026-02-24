using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для WindowBook.xaml
    /// </summary>
    public partial class WindowBook : Window
    {
        private Book _Book;
        private bool _Edit;
        public WindowBook()
        {
            InitializeComponent();
            _Edit = false;
            _Book = new Book();
            LoadComboBoxes();
            TitleTextBlock.Text = "Добавление";

        }
        public WindowBook(Book book)
        {
            InitializeComponent();
            _Edit = true;
            _Book = book;
            LoadBookData();
            TitleTextBlock.Text = "Редактирование";
        }

        private void LoadBookData()
        {
            TitleTextBlock.Text = _Book.Title;
            YearTextBox.Text = _Book.PublishYear.ToString();
            IsbnTextBox.Text = _Book.ISBN;
            QuantityTextBox.Text = _Book.QuantilityInStock.ToString();
            AuthorComboBox.SelectedValue = _Book.AuthorId;

            if (_Book.BookGenres != null)
            {
                var bookGenreIds = _Book.BookGenres.Select(bg => bg.GenreId).ToList();

                foreach (Genre genre in GenresListBox.Items)
                {
                    if (bookGenreIds.Contains(genre.Id))
                    {
                        GenresListBox.SelectedItems.Add(genre);
                    }
                }
            }
        }
        private void LoadComboBoxes()
        {
            using (var db = new ApplicationContext())
            {
                var authors = db.Authors.ToList();
                AuthorComboBox.ItemsSource = authors;
                var genres = db.Genres.ToList();
                GenresListBox.ItemsSource = genres;
            }
        }
        private bool ValidatesFields()
        {
            if (string.IsNullOrEmpty(IsbnTextBox.Text))
            {
                MessageBox.Show("Поле ISBN не может быть пустым");
                return false;
            }

            string isbn = IsbnTextBox.Text.Trim();
            string pattern = @"^\d+-\d+-\d+-\d+-\d+$";

            if (!System.Text.RegularExpressions.Regex.IsMatch(isbn, pattern))
            {
                MessageBox.Show("Неверный формат ISBN");
                return false;
            }

            string[] groups = isbn.Split('-');

            foreach (string group in groups)
            {
                if (string.IsNullOrEmpty(group) || !group.All(char.IsDigit))
                {
                    MessageBox.Show("Каждая группа должна содержать только цифры");
                    return false;
                }
            }
            return true;
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidatesFields())
                return;
            using (var db = new ApplicationContext())
            {
                _Book.Title = TitleTextBox.Text.Trim();
                _Book.PublishYear = int.Parse(YearTextBox.Text);
                _Book.ISBN = IsbnTextBox.Text.Trim();
                _Book.QuantilityInStock = int.Parse(QuantityTextBox.Text);
                _Book.AuthorId = (int)AuthorComboBox.SelectedValue;

                if (_Edit)
                {
                    var existingBookGenres = db.BookGenres.Where(bg => bg.BookId == _Book.Id).ToList();

                    db.BookGenres.RemoveRange(existingBookGenres);

                    foreach (Genre selectedGenre in GenresListBox.SelectedItems)
                    {
                        db.BookGenres.Add(new BookGenre
                        {
                            BookId = _Book.Id,
                            GenreId = selectedGenre.Id
                        });
                    }

                    db.Books.Update(_Book);
                    db.SaveChanges();
                }
                else
                {
                    db.Books.Add(_Book);
                    db.SaveChanges();

                    foreach (Genre selectedGenre in GenresListBox.SelectedItems)
                    {
                        db.BookGenres.Add(new BookGenre
                        {
                            BookId = _Book.Id,
                            GenreId = selectedGenre.Id
                        });
                        db.SaveChanges();
                    }
                }
            }

            DialogResult = true;
            Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
