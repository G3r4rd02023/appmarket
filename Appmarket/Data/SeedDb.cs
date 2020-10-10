using Appmarket.Data.Entities;
using Appmarket.Data.Enums;
using Appmarket.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Appmarket.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper, IBlobHelper blobHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckRolesAsync();
            await CheckUserAsync("0801-1987-13256", "Gerardo", "Lanza", "glanza007@gmail.com", "3307 7964", "Col La Peña", UserType.Admin);
            await CheckCategoriesAsync();
            await CheckProductsAsync();
        }

        private async Task CheckProductsAsync()
        {
            if (!_context.Products.Any())
            {
                User user = await _userHelper.GetUserAsync("glanza007@gmail.com");
                Category mascotas = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Mascotas");
                Category ropa = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Ropa");
                Category tecnologia = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Tecnología");
                string lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris gravida, nunc vel tristique cursus, velit nibh pulvinar enim, non pulvinar lorem leo eget felis. Proin suscipit dignissim nisl, at elementum justo laoreet sed. In tortor nibh, auctor quis est gravida, blandit elementum nulla. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Integer placerat nisi dui, id rutrum nisi viverra at. Interdum et malesuada fames ac ante ipsum primis in faucibus. Pellentesque sodales sollicitudin tempor. Fusce volutpat, purus sit amet placerat gravida, est magna gravida risus, a ultricies augue magna vel dolor. Fusce egestas venenatis velit, a ultrices purus aliquet sed. Morbi lacinia purus sit amet nisi vulputate mollis. Praesent in volutpat tortor. Etiam ac enim id ligula rutrum semper. Sed mattis erat sed condimentum congue. Vestibulum consequat tristique consectetur. Nunc in lorem in sapien vestibulum aliquet a vel leo.";
                await AddProductAsync(mascotas, lorem, "Bulldog Frances", 2500, new string[] { "Bulldog1", "Bulldog2", "Bulldog3", "Bulldog4" }, user);
                await AddProductAsync(ropa, lorem, "Buso GAP Hombre", 850, new string[] { "BusoGAP1", "BusoGAP2" }, user);
                await AddProductAsync(tecnologia, lorem, "iPhone 11", 3500, new string[] { "iPhone1", "iPhone2", "iPhone3", "iPhone4", "iPhone5" }, user);
                await AddProductAsync(tecnologia, lorem, "iWatch \"42", 2100, new string[] { "iWatch" }, user);
                await AddProductAsync(ropa, lorem, "Tennis Adidas", 2500, new string[] { "Adidas" }, user);
                await AddProductAsync(mascotas, lorem, "Collie", 3500, new string[] { "Collie1", "Collie2", "Collie3", "Collie4", "Collie5" }, user);
                await AddProductAsync(tecnologia, lorem, "MacBook Pro 16\" 1TB", 12000, new string[] { "MacBookPro1", "MacBookPro2", "MacBookPro3", "MacBookPro4" }, user);
                await AddProductAsync(ropa, lorem, "Sudadera Mujer", 950, new string[] { "Sudadera1", "Sudadera2", "Sudadera3", "Sudadera4", "Sudadera5" }, user);
                await _context.SaveChangesAsync();
            }
        }

        private async Task AddProductAsync(Category category, string description, string name, decimal price, string[] images, User user)
        {
            Product product = new Product
            {
                Category = category,
                Description = description,
                IsActive = true,
                Name = name,
                Price = price,
                ProductImages = new List<ProductImage>(),              
            };

            foreach (string image in images)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images", $"{image}.png");
                Guid imageId = await _blobHelper.UploadBlobAsync(path, "products");
                product.ProductImages.Add(new ProductImage { ImageId = imageId });
            }

            _context.Products.Add(product);
        }
   

        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                await AddCategoryAsync("Ropa");
                await AddCategoryAsync("Tecnología");
                await AddCategoryAsync("Mascotas");
                await _context.SaveChangesAsync();
            }
        }

        private async Task AddCategoryAsync(string name)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images", $"{name}.png");
            Guid imageId = await _blobHelper.UploadBlobAsync(path, "categories");
            _context.Categories.Add(new Category { Name = name, ImageId = imageId });
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task<User> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            UserType userType)
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    City = _context.Cities.FirstOrDefault(),
                    UserType = userType
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country
                {
                    Name = "Honduras",
                    Cities = new List<City>
                {
                    new City
                    {
                        Name = "Tegucigalpa",
                        
                    },
                    new City
                    {
                        Name = "San Pedro Sula",                     
                    },
                    new City
                    {
                        Name = "La Ceiba",                      
                    }
                }
                });
                _context.Countries.Add(new Country
                {
                    Name = "USA",
                    Cities = new List<City>
                {
                    new City
                    {
                        Name = "Los Angeles",                      
                    },
                    new City
                    {
                        Name = "Chicago",                      
                    }
                }
                });
                await _context.SaveChangesAsync();
            }
        }
    }

}

