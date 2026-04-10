using EasyShoeDemo.Lib.Entities;
using EasyShoeDemo.Lib.Services;
using System.Windows;
using System.Windows.Controls;

namespace EasyShoeDemo.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly User? _currentUser;
        private readonly string _roleName;
        private readonly IMainApp _mainApp;
        public MainWindow(User? currentUser, string roleName)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _roleName = roleName;
            _mainApp = new MainApp();
            SetupAccess();
            LoadCategories();
            LoadManufacturers();
            LoadSuppliers();
            LoadProducts();
            LoadUserInfo();
        }
        private void LoadProducts()
        {
            ProductsListView.ItemsSource = _mainApp.GetProducts();
        }
        private void LoadCategories()
        {
            var categories = _mainApp.GetCategories();
            categories.Insert(0, new Category
            {
                Id = 0,
                Name = "Все категории"
            });
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.SelectedValue = 0;
        }
        private void LoadManufacturers()
        {
            var manufacturers = _mainApp.GetManufacturers();
            manufacturers.Insert(0, new Manufacturer
            {
                Id = 0,
                Name = "Все производители"
            });
            ManufacturerComboBox.ItemsSource = manufacturers;
            ManufacturerComboBox.SelectedValue = 0;
        }
        private void LoadSuppliers()
        {
            var suppliers = _mainApp.GetSuppliers();
            suppliers.Insert(0, new Supplier
            {
                Id = 0,
                Name = "Все поставщики"
            });
            SupplierComboBox.ItemsSource = suppliers;
            SupplierComboBox.SelectedValue = 0;
        }
        private void LoadUserInfo()
        {
            if (_currentUser == null)
                UserInfoTextBlock.Text = "Гость";
            else
                UserInfoTextBlock.Text = $"{_currentUser.FullName}, {_roleName}";
        }
        private void SetupAccess()
        {
            bool canFilter = _roleName == "Менеджер" || _roleName == "Администратор";
            SearchTextBox.IsEnabled = canFilter;
            CategoryComboBox.IsEnabled = canFilter;
            ManufacturerComboBox.IsEnabled = canFilter;
            SupplierComboBox.IsEnabled = canFilter;
            SortComboBox.IsEnabled = canFilter;
            bool isAdmin = _roleName == "Администратор";
            ProductsMenuItems.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddEditProductWindow();
            bool? result = window.ShowDialog();
            if (result == true)
                ApplyFilters();
        }
        private void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListView.SelectedItem is not Product selectedProduct)
            {
                MessageBox.Show("Выберите товар для редактирования", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var window = new AddEditProductWindow(selectedProduct.Article);
            bool? result = window.ShowDialog();
            if (result == true)
                ApplyFilters();
        }
        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListView.SelectedItem is not Product selectedProduct)
            {
                MessageBox.Show("Выберите товар для удаления", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show($"Удалить товар - {selectedProduct.Name}?", "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;
            try
            {
                _mainApp.DeleteProduct(selectedProduct.Article);
                MessageBox.Show("Товар успешно удален", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка удаления",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var window = new LoginWindow();
            window.Show();
            Close();
        }
        private void ApplyFilters()
        {
            try
            {
                string searchText = SearchTextBox.Text.Trim();
                int? categoryId = null;
                int? manufacturerId = null;
                int? supplierId = null;
                if (CategoryComboBox.SelectedItem is Category category && category.Id != 0)
                    categoryId = category.Id;
                if (ManufacturerComboBox.SelectedItem is Manufacturer manufacturer && manufacturer.Id != 0)
                    manufacturerId = manufacturer.Id;
                if (SupplierComboBox.SelectedItem is Supplier supplier && supplier.Id != 0)
                    supplierId = supplier.Id;
                string? sortType = null;
                if (SortComboBox.SelectedItem is ComboBoxItem selectedSort)
                    sortType = selectedSort.Tag?.ToString();
                ProductsListView.ItemsSource = _mainApp.GetFilteredProducts(searchText, categoryId, manufacturerId, supplierId, sortType);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка фильтрации",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }
        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                ApplyFilters();
        }
        private void ManufacturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                ApplyFilters();
        }
        private void SupplierComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                ApplyFilters();
        }
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                ApplyFilters();
        }
    }
}