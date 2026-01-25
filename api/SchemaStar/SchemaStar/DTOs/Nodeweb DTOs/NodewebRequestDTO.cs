using System.ComponentModel.DataAnnotations;

namespace SchemaStar.DTOs.Nodeweb_DTOs
{
    //DTO used for Creation & Update of NodeWeb
    public class NodewebRequestDTO
    {
        [Required, StringLength(255, MinimumLength = 1)]
        public string NodeWebName { get; set; } = null!;

    }
}
