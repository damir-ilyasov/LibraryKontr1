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
    /// Логика взаимодействия для WindowEditAuthor.xaml
    /// </summary>
    public partial class WindowEditAuthor : Window
    {
            private Author _author;
            private bool _EditAuthor;

        public WindowEditAuthor()
        {
            InitializeComponent();
            _EditAuthor = false;
            _author = new Author();
            BirthDatePicker.SelectedDate = DateTime.Now;
            Title = "Добавление автора";
        }

        public WindowEditAuthor(Author author)
        {
            InitializeComponent();
            _EditAuthor = true;
            _author = author;
            LoadAuthorData();
            Title = "Редактирование автора";
        }

        private void LoadAuthorData()
        {
            FirstNameTextBox.Text = _author.FirstName;
            LastNameTextBox.Text = _author.LastName;
            BirthDatePicker.SelectedDate = _author.BirthDate;
            CountryTextBox.Text = _author.Country;
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("Введите имя автора!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                MessageBox.Show("Введите фамилию автора!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (BirthDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату рождения!", "Предупреждение",
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
                _author.FirstName = FirstNameTextBox.Text.Trim();
                _author.LastName = LastNameTextBox.Text.Trim();
                var selDate = BirthDatePicker.SelectedDate.Value;
                _author.BirthDate = DateTime.SpecifyKind(selDate, DateTimeKind.Utc);
                _author.Country = CountryTextBox.Text?.Trim() ?? "";
                if (_EditAuthor == true)
                {
                    db.Authors.Update(_author);
                    db.SaveChanges();
                }
                else
                {
                    db.Authors.Add(_author);
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
