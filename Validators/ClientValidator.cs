using PFA_WebAPI.Data;
using System.ComponentModel.DataAnnotations;

namespace PFA_WebAPI.Validators
{
    public class UserIsClientAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dbContext = (DataContext)validationContext.GetService(typeof(DataContext));
            var userId = (int)value;

            var user = dbContext.Users.FirstOrDefault(u => u.Id == userId && u.Role == "Client");

            if (user == null)
            {
                return new ValidationResult("The user must have a role of Client.");
            }

            return ValidationResult.Success;
        }
    }
}