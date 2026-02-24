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
    /// Логика взаимодействия для WindowGenres.xaml
    /// </summary>
    public partial class WindowGenres : Window
    {
        public WindowGenres()
        {
            InitializeComponent();
            LoadGenres();
        }

        private void LoadGenres()
        {
            using (var db = new ApplicationContext())
            {
                GenresDataGrid.ItemsSource = db.Genres.ToList();
            }
        }

        private void AddGenreButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new WindowEditGenres();
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                LoadGenres();
            }
        }

        private void EditGenreButton_Click(object sender, RoutedEventArgs e)
        {
            if (GenresDataGrid.SelectedItem is Genre selectedGenre)
            {
                var window = new WindowEditGenres(selectedGenre);
                if (window.ShowDialog() == true)
                {
                    LoadGenres();
                }
            }
            else
            {
                MessageBox.Show("Выберите жанр для редактирования", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteGenreButton_Click(object sender, RoutedEventArgs e)
        {
            if (GenresDataGrid.SelectedItem is Genre selectedGenre)
            {
                var result = MessageBox.Show($"Удалить жанр '{selectedGenre.Name}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new ApplicationContext())
                    {
                        var hasBooks = db.Books.Any(b => b.GenreId == selectedGenre.Id);
                        if (hasBooks)
                        {
                            MessageBox.Show("Нельзя удалить жанр, в котором есть книги", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        db.Genres.Remove(selectedGenre);
                        db.SaveChanges();
                    }
                    LoadGenres();
                }
            }
            else
            {
                MessageBox.Show("Выберите жанр для удаления", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
