using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Reflection;

namespace Shared.Extensions
{
    public static class EnumExtensions
    {
        public static List<SelectListItem> EnumValuesAndDescriptionsToList<TEnum>() where TEnum : Enum
        {
            var enumList = new List<SelectListItem>();
            var enumType = typeof(TEnum);

            foreach (var enumValue in Enum.GetValues(enumType))
            {
                FieldInfo? fieldInfo = enumType.GetField(enumValue.ToString() ?? string.Empty);

                // Check for Description attribute
                if (fieldInfo != null)
                {
                    var descriptionAttribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();

                    enumList.Add(new SelectListItem
                    {
                        Value = Convert.ToString((int)enumValue),
                        Text = descriptionAttribute?.Description
                    });
                }

            }

            return enumList;
        }
        public static string GetDescription(Enum value)
        {
            var enumMember = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            var descriptionAttribute =
                enumMember == null
                    ? default(DescriptionAttribute)
                    : enumMember.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            return
                descriptionAttribute == null
                    ? value.ToString()
                    : descriptionAttribute.Description;
        }
    }
}
