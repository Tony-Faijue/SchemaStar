namespace SchemaStar.DTOs
{
    public class UserResponseDTO
    {
        //Expose public fields for response from the server
        //For GET Requests

        public Guid PublicId { get; set; } //Map to Guid from byte[]
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; } //From MySQL Current TimeStamp
        public DateTime UpdatedAt { get; set; } //From MySQL Current TimeStamp

    }
}
