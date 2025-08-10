namespace DeveloperStore.Api.DTOs
{
    public class UsersResultDto
    {
        public IEnumerable<UserDto> Data { get; set; } = new List<UserDto>();
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
