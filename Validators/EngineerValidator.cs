using PFA_WebAPI.Data;
using System.ComponentModel.DataAnnotations;

namespace PFA_WebAPI.Validators
{
    public class UserIsEngineerAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dbContext = (DataContext)validationContext.GetService(typeof(DataContext));
            var userId = (int)value;

            var user = dbContext.Users.FirstOrDefault(u => u.Id == userId && u.Role == "Engineer");

            if (user == null)
            {
                return new ValidationResult("The user must have a role of Engineer.");
            }

            return ValidationResult.Success;
        }
    }
}