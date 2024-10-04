namespace _001_DomainDtoExplain.Models.Doamin.ValueObjects
{
    //Value objects are immutable and do not have an identity. They are used to describe certain aspects of your domain entities.
    public class Address
    {
        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string ZipCode { get; }

        public Address(string street, string city, string state, string zipCode)
        {
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
        }
    }

}
