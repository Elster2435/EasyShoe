using EasyShoeDemo.Lib.Entities;
using EasyShoeDemo.Lib.Services;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace EasyShoeDemo.Wpf
{
    public partial class AddEditProductWindow : Window
    {
        private readonly IMainApp _mainApp;
        private readonly bool _isEditMode;
        private Product _product;
        public AddEditProductWindow(string? article = null)
        {
            InitializeComponent();
            _mainApp = new MainApp();
            LoadComboBoxes();
            if (string.IsNullOrWhiteSpace(article))
            {
                _isEditMode = false;
                _product = new Product();
                Title = "Добавление товара";
                ArticleTextBox.IsReadOnly = false;
            }
            else
            {
                _isEditMode = true;
                Title = "Редактирование товара";
                ArticleTextBox.IsReadOnly = true;
                _product = _mainApp.GetProductByArticle(article)
                    ?? throw new Exception("Товар не найден");
                LoadProductData();

            }
        }
        private void LoadComboBoxes()
        {
            CategoryComboBox.ItemsSource = _mainApp.GetCategories();
            CategoryComboBox.DisplayMemberPath = "Name";
            CategoryComboBox.SelectedValuePath = "Id";
            ManufacturerComboBox.ItemsSource = _mainApp.GetManufacturers();
            ManufacturerComboBox.DisplayMemberPath = "Name";
            ManufacturerComboBox.SelectedValuePath = "Id";
            SupplierComboBox.ItemsSource = _mainApp.GetSuppliers();
            SupplierComboBox.DisplayMemberPath = "Name";
            SupplierComboBox.SelectedValuePath = "Id";
        }
        private void LoadProductData()
        {
            ArticleTextBox.Text = _product.Article;
            NameTextBox.Text = _product.Name;
            UnitTextBox.Text = _product.Unit;
            PriceTextBox.Text = _product.Price.ToString();
            CategoryComboBox.SelectedValue = _product.CategoryId;
            ManufacturerComboBox.SelectedValue = _product.ManufacturerId;
            SupplierComboBox.SelectedValue = _product.SupplierId;
            DiscountTextBox.Text = _product.DiscountPercent.ToString();
            StockTextBox.Text = _product.StockQuantity.ToString();
            DescriptionTextBox.Text = _product.Description;
            PhotoPathTextBox.Text = _product.PhotoPath ?? "";
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FillProductFromFields();
                if (_isEditMode)
                    _mainApp.UpdateProduct(_product);
                else
                    _mainApp.AddProduct(_product);
                MessageBox.Show(
                    "Товар успешно сохранён.",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка сохранения",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png"
            };
            if (openFileDialog.ShowDialog() != true)
                return;
            try
            {
                string imagesFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);
                string originalExtension = System.IO.Path.GetExtension(openFileDialog.FileName);
                string newFileName = Guid.NewGuid().ToString() + originalExtension;
                string destinationPath = System.IO.Path.Combine(imagesFolder, newFileName);
                File.Copy(openFileDialog.FileName, destinationPath, true);
                PhotoPathTextBox.Text = newFileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при выборе изображения:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private void FillProductFromFields()
        {
            string article = ArticleTextBox.Text.Trim();
            string name = NameTextBox.Text.Trim();
            string unit = UnitTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();
            string photoPath = PhotoPathTextBox.Text.Trim();
            if (!decimal.TryParse(PriceTextBox.Text.Trim(), out decimal price))
                throw new Exception("Введите корректную цену.");
            if (!int.TryParse(DiscountTextBox.Text.Trim(), out int discount))
                throw new Exception("Введите корректную скидку.");
            if (!int.TryParse(StockTextBox.Text.Trim(), out int stock))
                throw new Exception("Введите корректное количество на складе.");
            _product.Article = article;
            _product.Name = name;
            _product.Unit = unit;
            _product.Price = price;
            _product.DiscountPercent = discount;
            _product.StockQuantity = stock;
            _product.Description = description;
            _product.CategoryId = CategoryComboBox.SelectedValue == null
                ? 0
                : (int)CategoryComboBox.SelectedValue;
            _product.ManufacturerId = ManufacturerComboBox.SelectedValue == null
                ? 0
                : (int)ManufacturerComboBox.SelectedValue;
            _product.SupplierId = SupplierComboBox.SelectedValue == null
                ? 0
                : (int)SupplierComboBox.SelectedValue;
            _product.PhotoPath = string.IsNullOrWhiteSpace(photoPath) ? null : photoPath;
        }
    }
}
