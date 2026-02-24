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
    /// Логика взаимодействия для WindowEditGenres.xaml
    /// </summary>
    public partial class WindowEditGenres : Window
    {
        private Genre _genre;
        private bool _EditGenre;

        public WindowEditGenres()
        {
            InitializeComponent();
            _EditGenre = false;
            _genre = new Genre();
            Title = "Добавление жанра";
        }

        public WindowEditGenres(Genre genre)
        {
            InitializeComponent();
            _EditGenre = true;
            _genre = genre;
            LoadGenreData();
            Title = "Редактирование жанра";
        }

        private void LoadGenreData()
        {
            NameTextBox.Text = _genre.Name;
            DescriptionTextBox.Text = _genre.Description;
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите название жанра", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;
            using (ApplicationContext db = new ApplicationContext())
            {
                _genre.Name = NameTextBox.Text.Trim();
                _genre.Description = DescriptionTextBox.Text?.Trim() ?? "";
                if (_EditGenre == true)
                {
                    db.Genres.Update(_genre);
                    db.SaveChanges();
                }
                else
                {
                    db.Genres.Add(_genre);
                    db.SaveChanges();
                }
            }

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
