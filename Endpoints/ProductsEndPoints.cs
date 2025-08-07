using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SimpleLoginSystem.Objects;
using SimpleLoginSystem.Objects.Requests;
using SimpleLoginSystem.Objects.Responses;
using SimpleLoginSystem.Services;

namespace SimpleLoginSystem.Endpoints
{
    public static class ProductsEndPoints
    {
        public static void MapProductEndpoints(RouteGroupBuilder route)
        {
            route.MapGet("/{id}", GetProduct);
            route.MapGet("/all", GetAllProducts);
            route.MapGet("/category/{category}", GetProductsByCategory);
            route.MapPost("/add", AddProduct).RequireAuthorization("admin");
            route.MapPut("/edit/{id}", EditProduct).RequireAuthorization("admin");
            route.MapDelete("/delete/{id}", DeleteProduct).RequireAuthorization("admin");
        }

        static async Task<Results<Ok<Product>, NotFound>> GetProduct(int id, EcommerceDB db)
        {
            var product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(product);
        }

        static async Task<Results<Ok<List<Product>>, NotFound>> GetAllProducts(EcommerceDB db)
        {
            var products = await db.Products.ToListAsync();
            if (products == null || !products.Any())
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(products);
        }

        static async Task<Results<Ok<List<Product>>, NotFound, BadRequest<string>>> GetProductsByCategory(string? category, EcommerceDB db)
        {
            if (string.IsNullOrEmpty(category))
            {
                return TypedResults.BadRequest("Category cannot be null or empty.");
            }

            var products = await db.Products.Where(p => p.Category == category).ToListAsync();
            if (products == null || !products.Any())
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(products);

        }

        static async Task<Results<Ok<Product>, NotFound, BadRequest<string>>> AddProduct(ProductRequest req, EcommerceDB db)
        {
            if (string.IsNullOrEmpty(req.ProductName) || req.Price <= 0 || string.IsNullOrEmpty(req.Category))
            {
                return TypedResults.BadRequest("Name, Price, and Category are required.");
            }
            Product newProduct = new Product
            {
                ProductName = req.ProductName,
                Price = req.Price,
                Category = req.Category,
                Description = req.Description,
                Stock = req.Stock > 0 ? req.Stock : 0
            };
            db.Products.Add(newProduct);
            await db.SaveChangesAsync();
            return TypedResults.Ok(newProduct);
        }

        static async Task<Results<Ok<Product>, NotFound, BadRequest<string>>> EditProduct(int id, ProductRequest req, EcommerceDB db)
        {
            var product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return TypedResults.NotFound();
            }
            if (string.IsNullOrEmpty(req.ProductName) || req.Price <= 0 || string.IsNullOrEmpty(req.Category))
            {
                return TypedResults.BadRequest("Name, Price, and Category are required.");
            }
            product.ProductName = req.ProductName;
            product.Price = req.Price;
            product.Category = req.Category;
            product.Description = req.Description;
            product.Stock = req.Stock > 0 ? req.Stock : 0;
            db.Products.Update(product);
            await db.SaveChangesAsync();
            return TypedResults.Ok(product);
        }

        static async Task<Results<Ok<Product>, NotFound>> DeleteProduct(int id, EcommerceDB db)
        {
            var product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return TypedResults.NotFound();
            }
            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return TypedResults.Ok(product);
        }
    }
}
