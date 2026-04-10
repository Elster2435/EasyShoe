using EasyShoeDemo.Lib.Data;
using EasyShoeDemo.Lib.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyShoeDemo.Lib.Services
{
    public class MainApp : IMainApp
    {
        public User? Authorize(string login,  string password)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new Exception("Введите логин");
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Введите пароль");
            var db = new ApplicationContext();
            return db.Users.FirstOrDefault(x => x.Login == login &&
            x.PasswordHash == password);
        }
        public string GetRoleName(int roleId)
        {
            using var db = new ApplicationContext();
            var role = db.Roles.FirstOrDefault(x => x.Id == roleId);
            if (role == null)
                throw new Exception("Роль не найдена");
            return role.Name;
        }
        public List<Category> GetCategories()
        {
            using var db = new ApplicationContext();
            return db.Categories
                .OrderBy(x => x.Name)
                .ToList();
        }
        public List<Manufacturer> GetManufacturers()
        {
            using var db = new ApplicationContext();
            return db.Manufacturers
                .OrderBy(x => x.Name)
                .ToList();
        }
        public List<Supplier> GetSuppliers()
        {
            using var db = new ApplicationContext();
            return db.Suppliers
                .OrderBy(x => x.Name)
                .ToList();
        }
        public List<Product> GetProducts()
        {
            using var db = new ApplicationContext();
            return db.Products
                .Include(x => x.Category)
                .Include(x => x.Manufacturer)
                .Include(x => x.Supplier)
                .ToList();
        }
        public List<Product> GetFilteredProducts(string? searchText, int? categoryId, int? manufacturerId, int? supplierId, string? sortType)
        {
            using var db = new ApplicationContext();
            var query = db.Products
                .Include(x => x.Category)
                .Include(x => x.Manufacturer)
                .Include(x => x.Supplier)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim().ToLower();
                query = query.Where(x =>
                x.Article.ToLower().Contains(searchText) ||
                x.Name.ToLower().Contains(searchText) ||
                (x.Description != null && x.Description.ToLower().Contains(searchText)) ||
                x.Unit.ToLower().Contains(searchText) ||
                x.Category.Name.ToLower().Contains(searchText) ||
                x.Manufacturer.Name.ToLower().Contains(searchText) ||
                x.Supplier.Name.ToLower().Contains(searchText));
            }
            if (categoryId.HasValue)
                query = query.Where(x => x.CategoryId == categoryId.Value);
            if (manufacturerId.HasValue)
                query = query.Where(x => x.ManufacturerId == manufacturerId.Value);
            if (supplierId.HasValue)
                query = query.Where(x => x.SupplierId == supplierId.Value);
            if (sortType == "asc")
            {
                query = query.OrderBy(x => x.StockQuantity);
            }
            else if (sortType == "desc")
            {
                query = query.OrderByDescending(x => x.StockQuantity);
            }
            else
            {
                query = query.OrderBy(x => x.Name);
            }
            return query.ToList();
        }
        public Product? GetProductByArticle(string article)
        {
            using var db = new ApplicationContext();
            return db.Products
                .Include(x => x.Category)
                .Include(x => x.Manufacturer)
                .Include(x => x.Supplier)
                .FirstOrDefault(x => x.Article == article);
        }
        public void AddProduct(Product product)
        {
            if (product == null)
                throw new Exception("Товар не передан");
            ValidateProduct(product, false);
            using var db = new ApplicationContext();
            db.Products.Add(product);
            db.SaveChanges();
        }
        public void UpdateProduct(Product product)
        {
            if (product == null)
                throw new Exception("Товар не передан");
            ValidateProduct(product, false);
            using var db = new ApplicationContext();
            var existingProduct = db.Products.FirstOrDefault(x => x.Article == product.Article);
            if (existingProduct == null)
                throw new Exception("Товар не найден");
            existingProduct.Name = product.Name;
            existingProduct.Unit = product.Unit;
            existingProduct.Price = product.Price;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.ManufacturerId = product.ManufacturerId;
            existingProduct.SupplierId = product.SupplierId;
            existingProduct.DiscountPercent = product.DiscountPercent;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.Description = product.Description;
            existingProduct.PhotoPath = product.PhotoPath;
            db.SaveChanges();
        }
        public void DeleteProduct(string article)
        {
            using var db = new ApplicationContext();
            var product = db.Products.FirstOrDefault(x => x.Article == article);
            if (product == null)
                throw new Exception("Товар не найден");
            bool hasOrderitems = db.OrderItems.Any(x => x.ProductArticle == article);
            if (hasOrderitems)
                throw new Exception("Нельзя удалить товар, который используется в заказе");
            db.Products.Remove(product);
            db.SaveChanges();
        }
        private void ValidateProduct(Product product, bool isNew)
        {
            if (string.IsNullOrWhiteSpace(product.Article))
                throw new Exception("Введите артикул товара.");
            if (product.Article.Length != 6)
                throw new Exception("Артикул товара должен содержать ровно 6 символов.");
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new Exception("Введите наименование товара.");
            if (string.IsNullOrWhiteSpace(product.Unit))
                throw new Exception("Введите единицу измерения.");
            if (product.Price < 0)
                throw new Exception("Цена не может быть отрицательной.");
            if (product.StockQuantity < 0)
                throw new Exception("Количество на складе не может быть отрицательным.");
            if (product.DiscountPercent < 0 || product.DiscountPercent > 100)
                throw new Exception("Скидка должна быть в диапазоне от 0 до 100.");
            if (product.CategoryId <= 0)
                throw new Exception("Выберите категорию.");
            if (product.ManufacturerId <= 0)
                throw new Exception("Выберите производителя.");
            if (product.SupplierId <= 0)
                throw new Exception("Выберите поставщика.");
        }
    }
}
