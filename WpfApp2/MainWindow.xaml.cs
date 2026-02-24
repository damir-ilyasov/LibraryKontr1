using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Operations;
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
                        .ThenInclude(b => b.Genre)
                    .ToList();

                var booksView = books.Select(b => new
                {
                    b.Id,
                    b.Title,
                    AuthorName = b.Author != null ? $"{b.Author.FirstName} {b.Author.LastName}" : "",
                    Genre = string.Join(", ", b.BookGenres.Select(bg => bg.Genre.Name)),
                    b.PublishYear,
                    b.ISBN,
                    b.QuantilityInStock
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
        private void SearchTextBox_TextChanged(object sender, EventArgs e)
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
                        .Include(b => b.Genre)
                        .AsQueryable();

                    if (GenreFilterComboBox.SelectedItem is Genre selectedGenre && selectedGenre.Id != 0)
                    {
                        query = query.Where(b => b.GenreId == selectedGenre.Id);
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

                    BooksDataGrid.ItemsSource = query.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFilterButton_Click(object sender, EventArgs e)
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
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                var window = new WindowBook(selectedBook);
                if (window.ShowDialog() == true)
                {
                    LoadData();
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
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                var result = MessageBox.Show($"Удалить книгу '{selectedBook.Title}'?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var db = new ApplicationContext())
                        {
                            db.Books.Remove(selectedBook);
                            db.SaveChanges();
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