using System.Security.Cryptography.X509Certificates;
using Entities.Models;

namespace Entities.Dto
{
 public class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FullAddress { get; set; }
    }
}