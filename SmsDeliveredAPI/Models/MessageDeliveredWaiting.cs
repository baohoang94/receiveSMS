using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmsDeliveredAPI.Models
{
    [Table("MessageDeliveredWaiting")]
    public class MessageDeliveredWaiting: MessageDelivered
    {
        [Key]
        [Column("DeliveredGuid")]
        [Required]
        public Guid DeliveredGuid { get; set; }
    }
}
