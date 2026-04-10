using EasyShoeDemo.Lib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyShoeDemo.Lib.Services
{
    public interface IMainApp
    {
        User? Authorize(string login, string password);
        string GetRoleName(int roleId);
        List<Category> GetCategories();
        List<Manufacturer> GetManufacturers();
        List<Supplier> GetSuppliers();
        List<Product> GetProducts();
        List<Product> GetFilteredProducts(string? searchText, int? categoryId, int? manufacturerId, int? supplierId, string? sortType);
        Product? GetProductByArticle(string article);
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(string article);
    }
}
