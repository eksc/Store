using Store.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Core.Specifications
{
    public class ProductWithFiltersForCountSpecification : BaseSpecification<Product>
    {
        public ProductWithFiltersForCountSpecification(ProductSpecificationParams productParams)
            : base(x =>
                (string.IsNullOrEmpty(productParams.Search) || x.Name.ToLower().Contains(productParams.Search)) &&
                (!productParams.BrandId.HasValue || x.ProductBrandId == productParams.BrandId) &&
                (!productParams.TypeId.HasValue || x.ProductTypeId == productParams.TypeId)
            )
        {

        }
    }
}
