using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Snblog.Models
{
    public partial class SnPicture
    {
        public int PictureId { get; set; }
        public string PictureUrl { get; set; }
        public string PictureTitle { get; set; }
        public int? PictureTypeId { get; set; }
    }
}
