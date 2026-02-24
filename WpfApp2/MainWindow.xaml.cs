using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Database.EnsureCreated();

                var books = db.Books
                    .Include(b => b.Author)
                    .Include(b => b.BookGenres)
                        .ThenInclude(bg => bg.Genre)
                    .ToList();

                var booksView = books.Select(b => new
                {
                    b.Id,
                    b.Title,
                    Author = b.Author,
                    Genres = b.BookGenres != null && b.BookGenres.Any()
                        ? string.Join(", ", b.BookGenres.Select(bg => bg.Genre.Name))
                        : "Нет жанров",
                    b.PublishYear,
                    b.ISBN,
                    QuantityInStock = b.QuantilityInStock
                }).ToList();

                BooksDataGrid.ItemsSource = booksView;

                var genres = db.Genres.ToList();
                genres.Insert(0, new Genre { Id = 0, Name = "Все жанры" });
                GenreFilterComboBox.ItemsSource = genres;
                GenreFilterComboBox.SelectedIndex = 0;

                var authors = db.Authors.ToList();
                authors.Insert(0, new Author { Id = 0, FirstName = "Все", LastName = "авторы" });
                AuthorFilterComboBox.ItemsSource = authors;
                AuthorFilterComboBox.SelectedIndex = 0;

                GenreFilterComboBox.SelectionChanged += FilterChanged;
                AuthorFilterComboBox.SelectionChanged += FilterChanged;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            ApplyFilter();
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var query = db.Books
                        .Include(b => b.Author)
                        .Include(b => b.BookGenres)
                            .ThenInclude(bg => bg.Genre)
                        .AsQueryable();

                    if (GenreFilterComboBox.SelectedItem is Genre selectedGenre && selectedGenre.Id != 0)
                    {
                        query = query.Where(b => b.BookGenres.Any(bg => bg.GenreId == selectedGenre.Id));
                    }

                    if (AuthorFilterComboBox.SelectedItem is Author selectedAuthor && selectedAuthor.Id != 0)
                    {
                        query = query.Where(b => b.AuthorId == selectedAuthor.Id);
                    }

                    if (!string.IsNullOrWhiteSpace(SearchTextBox?.Text))
                    {
                        string searchText = SearchTextBox.Text.Trim();
                        query = query.Where(b => b.Title.Contains(searchText));
                    }

                    var filteredBooks = query.ToList();

                    var booksView = filteredBooks.Select(b => new
                    {
                        b.Id,
                        b.Title,
                        Author = b.Author,
                        Genres = b.BookGenres != null && b.BookGenres.Any()
                            ? string.Join(", ", b.BookGenres.Select(bg => bg.Genre.Name))
                            : "Нет жанров",
                        b.PublishYear,
                        b.ISBN,
                        QuantityInStock = b.QuantilityInStock
                    }).ToList();

                    BooksDataGrid.ItemsSource = booksView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            GenreFilterComboBox.SelectedIndex = 0;
            AuthorFilterComboBox.SelectedIndex = 0;
        }

        private void AddBookButton_Click(object sender, EventArgs e)
        {
            var window = new WindowBook();
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem != null)
            {
                var selectedItem = BooksDataGrid.SelectedItem;
                var idProperty = selectedItem.GetType().GetProperty("Id");

                if (idProperty != null)
                {
                    int bookId = (int)idProperty.GetValue(selectedItem);

                    using (var db = new ApplicationContext())
                    {
                        var book = db.Books
                            .Include(b => b.BookGenres)
                            .FirstOrDefault(b => b.Id == bookId);

                        if (book != null)
                        {
                            var window = new WindowBook(book);
                            if (window.ShowDialog() == true)
                            {
                                LoadData();
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите книгу для редактирования!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem != null)
            {
                dynamic selectedBook = BooksDataGrid.SelectedItem;
                string bookTitle = selectedBook.Title;
                int bookId = selectedBook.Id;

                var result = MessageBox.Show($"Удалить книгу '{bookTitle}'?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var db = new ApplicationContext())
                        {
                            var book = db.Books.Find(bookId);
                            if (book != null)
                            {
                                db.Books.Remove(book);
                                db.SaveChanges();
                            }
                        }

                        LoadData();
                        MessageBox.Show("Книга удалена!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите книгу для удаления!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ManageAuthorsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new WindowAuthor();
            window.ShowDialog();
            LoadData();
        }

        private void ManageGenresButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new WindowGenres();
            window.ShowDialog();
            LoadData();
        }
    }
}