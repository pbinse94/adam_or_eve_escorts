﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Response
{
    public class UploadedFileResponseModel
    {
        public string FileName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
