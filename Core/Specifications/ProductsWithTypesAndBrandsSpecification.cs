using Core.Entities;
using System.Linq.Expressions;

namespace Core.Specifications
{
    public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
    {
        public ProductsWithTypesAndBrandsSpecification(ProductSpecParams productParams)
            : base(GetSearchAndFilterCriteria(productParams))
        {
            AddInclude(x => x.ProductType);
            AddInclude(x => x.ProductBrand);
            AddOrderBy(x => x.Name);
            ApplyPaging((productParams.PageSize * (productParams.PageIndex - 1)), productParams.PageSize);

            if (!string.IsNullOrEmpty(productParams.Sort))
            {
                switch (productParams.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        break;
                }
            }
        }

        public ProductsWithTypesAndBrandsSpecification(int id)
            : base(p => p.Id == id)
        {
            AddInclude(x => x.ProductType);
            AddInclude(x => x.ProductBrand);
        }

        private static Expression<Func<Product, bool>> GetSearchAndFilterCriteria(ProductSpecParams productParams)
        {
            return (p => (string.IsNullOrEmpty(productParams.Search) || p.Name.ToLower().Contains(productParams.Search)) 
                     && (!productParams.BrandId.HasValue || p.ProductBrandId == productParams.BrandId) 
                     && (!productParams.TypeId.HasValue || p.ProductTypeId == productParams.TypeId));
        }
    }
}
