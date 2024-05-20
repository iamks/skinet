using Core.Entities;
using System.Linq.Expressions;

namespace Core.Specifications
{
    public class ProductWithFiltersForCountSpecification : BaseSpecification<Product>
    {
        public ProductWithFiltersForCountSpecification(ProductSpecParams productParams)
            : base(GetSearchAndFilterCriteria(productParams))
        {

        }

        private static Expression<Func<Product, bool>> GetSearchAndFilterCriteria(ProductSpecParams productParams)
        {
            return (p => (string.IsNullOrEmpty(productParams.Search) || p.Name.ToLower().Contains(productParams.Search))
                     && (!productParams.BrandId.HasValue || p.ProductBrandId == productParams.BrandId)
                     && (!productParams.TypeId.HasValue || p.ProductTypeId == productParams.TypeId));
        }
    }
}
