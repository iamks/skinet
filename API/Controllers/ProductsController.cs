using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Specifications;
using API.Dtos;
using AutoMapper;
using API.Errors;
using API.Helpers;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> productRepo;
        
        private readonly IGenericRepository<ProductBrand> productBrandRepo;
        
        private readonly IGenericRepository<ProductType> productTypeRepo;

        private readonly IMapper mapper;

        public ProductsController(
            IGenericRepository<Product> productRepo,
            IGenericRepository<ProductBrand> productBrandRepo,
            IGenericRepository<ProductType> productTypeRepo,
            IMapper mapper)
        {
            this.productRepo = productRepo;
            this.productBrandRepo = productBrandRepo;
            this.productTypeRepo = productTypeRepo;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<ProductDto>>> GetProducts([FromQuery]ProductSpecParams productParams)
        {
            var productWithBrandsAndTypesSpec = new ProductsWithTypesAndBrandsSpecification(productParams);
            var countSpec = new ProductWithFiltersForCountSpecification(productParams);
            var totalItems = await this.productRepo.CountAsync(countSpec);

            var products = await this.productRepo.GetEntitiesAsync(productWithBrandsAndTypesSpec);
            var productsDto = this.mapper.Map<IReadOnlyList<ProductDto>>(products);
            
            return Ok(new Pagination<ProductDto>(productParams.PageIndex, productParams.PageSize, totalItems, productsDto));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            var product = await this.productRepo.GetEntityWithSpecAsync(spec);

            if (product == null)
            {
                return NotFound(new ApiResponse(400));
            }

            return Ok(this.mapper.Map<ProductDto>(product));
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            var productBrands = await this.productBrandRepo.GetAllAsync();

            return Ok(productBrands);
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            var productTypes = await this.productTypeRepo.GetAllAsync();

            return Ok(productTypes);
        }
    }
}