using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Core.Specifications
{
    public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
    {
        public ProductsWithTypesAndBrandsSpecification(ProductSpecParams productParams)
            : base(x =>
                (string.IsNullOrEmpty(productParams.Search) || x.Name.ToLower().Contains(productParams.Search))&&
                (!productParams.BrandId.HasValue || x.ProductBrandId == productParams.BrandId) &&
                (!productParams.CategoryId.HasValue || x.CategoryId == productParams.CategoryId)
            )
        {
            AddInclude(x => x.category);
            AddInclude(x => x.productBrand);
            AddOrderBy(x => x.Name);
            ApplyPaging(productParams.pageSize * (productParams.pageIndex - 1), productParams.pageSize);

            if(!string.IsNullOrEmpty(productParams.Sort))
            {
                switch (productParams.Sort){
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        AddOrderBy(n => n.Name);
                        break;
                }
            }
        }

        public ProductsWithTypesAndBrandsSpecification(int id)
            : base(x => x.Id == id)
        {   
            AddInclude(x => x.category);
            AddInclude(x => x.productBrand);
        }
    }
}