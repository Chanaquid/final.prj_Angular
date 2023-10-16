using API.Dtos;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class ProductsController : ControllerBase
    {

        // private readonly IProductRepository _productRepository;
        public IGenericRepository<Product> _productRepository {get;}

        public IGenericRepository<Category> _categoryRepository {get;}
        public IGenericRepository<ProductBrand> _productBrandRepository{get;}
        public IMapper _mapper {get;}
        
        public ProductsController(IGenericRepository<Product> productRepository, IGenericRepository<Category> categoryRepository, IGenericRepository<ProductBrand> productBrandRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productBrandRepository = productBrandRepository;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams productParams)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(productParams);

            var countSpec = new ProductWithFilterForCountSpecification(productParams);

            var totalItems = await _productRepository.CountAsync(countSpec);

            var products = await _productRepository.ListAsync(spec);
            
            var data = _mapper
                .Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            // //Shapping the data
            return Ok(new Pagination<ProductToReturnDto>(productParams.pageIndex, productParams.pageSize, totalItems, data));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);

            try
            {
                var product = await _productRepository.GetEntityWithSpec(spec);

                return _mapper.Map<Product, ProductToReturnDto>(product);
            }
            catch(Exception)
            {
                return StatusCode(500, "Internal server error. Please try again");
            }


        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            try
            {
                if(product == null)
                {
                    return BadRequest("Invalid product data");
                }

                await _productRepository.AddAsync(product);
                await _productRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id}, product);
            }
            catch(Exception)
            {
                return StatusCode(500, "Internal Server error. Could not create product.");
            }
            
           
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            try
            {
                 if(product == null || id != product.Id)
                {
                    return BadRequest("Invalid product data or mismatched IDs");
                }

                var existingProduct = await _productRepository.GetByIdAsync(id);
                
                if(existingProduct == null){

                    return NotFound("Product not found.");

                }

                 //Update properties of the existing product from the provided product
                 existingProduct.Name = product.Name;
                 
                 _productRepository.Update(existingProduct);
                 await _productRepository.SaveChangesAsync();
                 
                
                return Ok(existingProduct);
                
            }
            catch(Exception)
            {
                return StatusCode(500, "Internal Server error. Product could now be updated");
            }
            
           

           
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if(product == null)
                {
                    return NotFound("Product not found");
                }

                _productRepository.Delete(product);
                await _productRepository.SaveChangesAsync();

                return NoContent(); // 204 error response is appropiate for DELETE on success.
            }
            catch(Exception)
            {
                return StatusCode(500, "Internal server error. Could not delete product");
            }
            
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _productBrandRepository.ListAllAsync());
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IReadOnlyList<Category>>> GetProductTypes()
        {
            return Ok(await _categoryRepository.ListAllAsync());
        }


    }
}