using DeveloperStore.Domain.Entities;
using System.Collections.Generic;

namespace DeveloperStore.Api.DTOs
{
    public class ProductsResultDto
    {
        public List<Product> Data { get; set; } = new();
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}