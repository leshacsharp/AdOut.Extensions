using System.ComponentModel.DataAnnotations;

namespace AdOut.Extensions.Repositories
{
    public abstract class PersistentEntity
    {
        [Required]
        public string Creator { get; set; }
    }
}
