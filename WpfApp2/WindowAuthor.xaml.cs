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
    /// Логика взаимодействия для WindowAuthor.xaml
    /// </summary>
    public partial class WindowAuthor : Window
    {
        public WindowAuthor()
        {
            InitializeComponent();
            LoadAuthorData();
        }

        private void LoadAuthorData()
        {
            using (var db = new ApplicationContext())
            {
                AuthorsDataGrid.ItemsSource = db.Authors.ToList();
            }
        }
        private void AddAuthorButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new WindowEditAuthor();
            window.Owner = this;
            if (window.ShowDialog() == true) { LoadAuthorData(); }
        }
        private void EditAuthorButton_Click(Object sender, RoutedEventArgs e)
        {
            if (AuthorsDataGrid.SelectedItem is Author selectedAuthor)
            {
                var window = new WindowEditAuthor(selectedAuthor);
                if (window.ShowDialog() == true) { LoadAuthorData(); }
            }
        }
        private void DeleteAuthorButton_Click(Object sender, RoutedEventArgs e)
        {
            if (AuthorsDataGrid.SelectedItem is Author selectedAuthor)
            {
                var result = MessageBox.Show($"Удалить автора {selectedAuthor.FullName}?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new ApplicationContext())
                    {
                        var hasBooks = db.Books.Any(b => b.AuthorId == selectedAuthor.Id);
                        if (hasBooks)
                        {
                            MessageBox.Show("Нельзя удалить автора, у которого есть книги!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        db.Authors.Remove(selectedAuthor);
                        db.SaveChanges();
                    }
                    LoadAuthorData();
                }
            }
            else
            {
                MessageBox.Show("Выберите автора для удаления", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void CloseButton_Click(Object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
