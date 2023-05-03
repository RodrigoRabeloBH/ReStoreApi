using System.Diagnostics.CodeAnalysis;

namespace ReStore.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public class Address
    {
        public string FullName { get; set; }
        public string Street { get; set; }
        public string Neighborhood { get; set; }
        public int Number { get; set; }
        public string Complement { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
    }
}
