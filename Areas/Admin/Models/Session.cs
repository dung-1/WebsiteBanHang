namespace WebsiteBanHang.Areas.Admin.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Session
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public byte[] Value { get; set; }

        [Required]
        public DateTimeOffset ExpiresAtTime { get; set; }

        public long? SlidingExpirationInSeconds { get; set; }

        public DateTimeOffset? AbsoluteExpiration { get; set; }
    }


}
