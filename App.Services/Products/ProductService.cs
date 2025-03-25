using App.Repositories.Products;
using App.Repositories.UnitOfWorks;
using App.Services.Products.Create;
using App.Services.Products.Dtos;
using App.Services.Products.Update;
using App.Services.Products.UpdateStock;
using App.Services.ServiceResults;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace App.Services.Products
{
    public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork) : IProductService
    {
        public async Task<ServiceResult<List<ProductDto>>> GetAllListAsync()
        {
            var products = await productRepository.GetAll().ToListAsync();

            var productsAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();

            return ServiceResult<List<ProductDto>>.Success(productsAsDto);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetPagedAllListAsync(int pageNumber, int pageSize)
        {
            var products = await productRepository.GetAll().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var productsAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();

            return ServiceResult<List<ProductDto>>.Success(productsAsDto);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetTopPriceProductsAsync(int count)
        {
            var products = await productRepository.GetAll().OrderByDescending(p => p.Price).Take(count).ToListAsync();

            var productsAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();

            return ServiceResult<List<ProductDto>>.Success(productsAsDto);
        }

        public async Task<ServiceResult<ProductDto?>> GetByIdAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);

            if (product is null)
                return ServiceResult<ProductDto?>.Failure("Product not found", HttpStatusCode.NotFound);

            var productAsDto = new ProductDto(product.Id, product.Name, product.Price, product.Stock);

            return ServiceResult<ProductDto?>.Success(productAsDto);
        }

        public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
        {
            var isProductExists = await productRepository.Get(p => p.Name == request.Name).AnyAsync();

            if (isProductExists)
                return ServiceResult<CreateProductResponse>.Failure("Product already exists", HttpStatusCode.Conflict);

            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                Stock = request.Stock
            };

            await productRepository.CreateAsync(product);
            await unitOfWork.SaveChangesAsync();

            var productAsDto = new ProductDto(product.Id, product.Name, product.Price, product.Stock);

            return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(productAsDto.Id), $"api/products/{productAsDto.Id}");
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request)
        {
            var product = await productRepository.GetByIdAsync(id);

            if (product is null)
                return ServiceResult.Failure("Product not found", HttpStatusCode.NotFound);

            var isProductNameExists = await productRepository.Get(p => p.Name == request.Name && p.Id != id).AnyAsync();

            if (isProductNameExists)
                return ServiceResult.Failure("Product already exists", HttpStatusCode.Conflict);

            product.Name = request.Name;
            product.Price = request.Price;
            product.Stock = request.Stock;

            productRepository.Update(product);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> UpdateStockAsync(UpdateProductStockRequest request)
        {
            var product = await productRepository.GetByIdAsync(request.ProductId);

            if (product is null)
                return ServiceResult.Failure("Product not found", HttpStatusCode.NotFound);

            product.Stock = request.Quantity;

            productRepository.Update(product);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);

            if (product is null)
                return ServiceResult.Failure("Product not found", HttpStatusCode.NotFound);

            productRepository.Delete(product);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }
    }
}
