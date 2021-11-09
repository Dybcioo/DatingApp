using System.Text.Json;
using DatingApp.Helpers;
using Microsoft.AspNetCore.Http;

namespace DatingApp.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemPerPage,
            int totalPages, int totalItems)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemPerPage, totalItems, totalPages);

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
