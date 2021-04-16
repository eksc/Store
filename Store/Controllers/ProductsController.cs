using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Core.Interfaces;
using Store.Core.Models;
using Store.Core.Specifications;
using Store.Infrastructure.Data;
using Store.WebAPI.DTOs;
using Store.WebAPI.Errors;
using Store.WebAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.WebAPI.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductBrand> _productBrandRepository;
        private readonly IGenericRepository<ProductType> _productTypeRepository;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productRepository,
            IGenericRepository<ProductBrand> productBrandRepository,
            IGenericRepository<ProductType> productTypeRepository, 
            IMapper mapper)
        {
            _productRepository = productRepository;
            _productBrandRepository = productBrandRepository;
            _productTypeRepository = productTypeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<ProductDTO>>> GetProducts([FromQuery]ProductSpecificationParams productParams)
        {
            var specification = new ProductsWithTypesAndBrandsSpecification(productParams);

            var countSpecification = new ProductWithFiltersForCountSpecification(productParams);

            var totalItems = await _productRepository.CountAsync(specification);

            var products = await _productRepository.ListAsync(specification);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductDTO>>(products);

            return Ok(new Pagination<ProductDTO>(productParams.PageIndex, productParams.PageSize, totalItems, data));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var specification = new ProductsWithTypesAndBrandsSpecification(id);

            var product = await _productRepository.GetEntityWithSpecification(specification);

            if (product == null)
            {
                return NotFound(new ApiResponse(404));
            }

            return Ok(_mapper.Map<Product, ProductDTO>(product));
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _productBrandRepository.ListAllAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok(await _productTypeRepository.ListAllAsync());
        }
    }
}
