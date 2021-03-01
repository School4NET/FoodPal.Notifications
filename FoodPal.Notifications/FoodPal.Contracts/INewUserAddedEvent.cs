using System;
namespace FoodPal.Contracts
{
    public interface INewUserAddedEvent
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
    }
}
