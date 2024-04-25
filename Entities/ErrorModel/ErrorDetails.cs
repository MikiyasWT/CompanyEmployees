using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ErrorModel
{
    public class ErrorDetails
    {
        public bool Success {get; set;}
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
