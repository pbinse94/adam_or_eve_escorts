using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Shared.ValidationAttributes
{
    public class NotEqualAttribute : ValidationAttribute
    {
        private string OtherProperty { get; set; }

        public NotEqualAttribute(string otherProperty)
        {
            OtherProperty = otherProperty;
        }
#nullable disable
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // get other property value
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
            var otherValue = otherPropertyInfo?.GetValue(validationContext.ObjectInstance);

            // verify values
            if (value?.ToString() == otherValue?.ToString())
            {
                var displayName = validationContext.DisplayName;
                var otherDisplayName = GetDisplayName(otherPropertyInfo) ?? OtherProperty;
                return new ValidationResult($"{displayName} should not be same to the {otherDisplayName}."); 
            }
            else
                return ValidationResult.Success;
        }
        private string GetDisplayName(PropertyInfo propertyInfo)
        {
            var displayAttribute = propertyInfo.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null && !string.IsNullOrWhiteSpace(displayAttribute.Name))
            {
                return displayAttribute.Name;
            }

            return null;
        }
    }
#nullable enable
}
