using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Thss0.Web.Extensions
{
    public class InitializationHelper
    {
        const ushort RESULT_MIN_GAP = 5;
        private readonly string[] _departmentProperties = { "Name" };
        private readonly string[] _userProperties = { "Name", "DoB", "PoB", "Photo" };
        private readonly string[] _procedureProperties = { "Name", "BeginTime", "EndTime" };
        private readonly string[] _resultProperties = { "ObtainmentTime", "Content" };
        private readonly string[] _roleProperties = { "Name" };

        public void Validation(ModelStateDictionary state, object viewModel)
        {
            var type = viewModel.GetType();
            var propertyNames = Array.Empty<string>();
            string value;
            switch (type.Name)
            {
                case "DepartmentViewModel":
                    propertyNames = _departmentProperties;
                    break;
                case "UserViewModel":
                    propertyNames = _userProperties;
                    break;
                case "ProcedureViewModel":
                    propertyNames = _procedureProperties;
                    break;
                case "ResultViewModel":
                    propertyNames = _resultProperties;
                    break;
                case "IdentityRoleProxy":
                    propertyNames = _roleProperties;// To verify.
                    break;
            }
            var properties = type.GetProperties().Where(p => propertyNames.Contains(p.Name)).ToArray();
            for (ushort i = 0; i < properties.Length; i++)
            {
                value = properties[i].GetValue(viewModel)?.ToString() ?? "";
                if (properties[i].Name.Contains("Time") && value != "")
                {
                    if (DateTime.Parse(value) < DateTime.Now.AddMinutes(RESULT_MIN_GAP))
                    {
                        state.AddModelError(properties[i].Name, $"{Regex.Replace(properties[i].Name, "([a-z])([A-Z])", "$1 $2")} cannot be less than the current time");
                    }
                    if (DateTime.Parse(value).Minute % 15 != 0)
                    {
                        state.AddModelError(properties[i].Name, $"{Regex.Replace(properties[i].Name, "([a-z])([A-Z])", "$1 $2")} minutes must be multiple of 15");
                    }
                }
                else if (!properties[i].Name.Contains("Time") && value == "")
                {
                    state.AddModelError(properties[i].Name, $"{Regex.Replace(properties[i].Name, "([a-z])([A-Z])", "$1 $2")} required");
                }
            }
        }

        public object InitializeEntity(object source, object dest)
        {
            var sourceType = source.GetType();
            var destType = dest.GetType();
            var propertyNames = Array.Empty<string>();
            switch (sourceType.Name)
            {
                case "DepartmentViewModel":
                    propertyNames = _departmentProperties;
                    break;
                case "UserViewModel":
                    propertyNames = _userProperties;
                    break;
                case "ProcedureViewModel":
                    propertyNames = _procedureProperties;
                    break;
                case "ResultViewModel":
                    propertyNames = _resultProperties;
                    break;
                case "IdentityRoleProxy":
                    propertyNames = _roleProperties;// To verify.
                    break;
            }
            var sourceProperties = sourceType.GetProperties()
                                            .Where(p => propertyNames.Contains(p.Name)).ToArray();
            var destProperties = destType.GetProperties()
                                            .Where(p => propertyNames.Contains(p.Name)).ToArray();

            for (ushort i = 0; i < propertyNames.Length; i++)
            {
                if (sourceProperties[i].GetValue(source)?.ToString() != "")
                {
                    if (destProperties[i].PropertyType.Name == "DateTime")
                    {
                        destProperties.FirstOrDefault(p => p.Name == propertyNames[i])
                                ?.SetValue(dest, DateTime.Parse(sourceProperties.FirstOrDefault(p => p.Name == propertyNames[i])
                                                                        ?.GetValue(source)?.ToString() ?? "0"));
                    }
                    else
                    {
                        destProperties.FirstOrDefault(p => p.Name == propertyNames[i])
                                ?.SetValue(dest, sourceProperties.FirstOrDefault(p => p.Name == propertyNames[i])?.GetValue(source));
                    }
                }
            }
            return dest;
        }
        public object InitializeViewModel(object source, object dest)// To verify for roles.
        {
            var sourceType = source.GetType();
            var destType = dest.GetType();
            var propertyNames = Array.Empty<string>();
            string valueToSet;
            switch (sourceType.Name)
            {
                case "DepartmentProxy":
                    propertyNames = _departmentProperties;
                    break;
                case "ApplicationUserProxy":
                    propertyNames = _userProperties;
                    break;
                case "ProcedureProxy":
                    propertyNames = _procedureProperties;
                    break;
                case "ResultProxy":
                    propertyNames = _resultProperties;
                    break;
                case "IdentityRoleProxy":
                    propertyNames = _roleProperties;// To verify.
                    break;
            }
            propertyNames = new[] { "Id" }.Concat(propertyNames).ToArray();
            var sourceProperties = sourceType.GetProperties()
                                            .Where(p => propertyNames.Contains(p.Name)).ToArray();
            var destProperties = destType.GetProperties()
                                            .Where(p => propertyNames.Contains(p.Name)).ToArray();
            for (ushort i = 0; i < propertyNames.Length; i++)
            {
                valueToSet =  sourceProperties[i].GetValue(source)?.ToString() ?? "";
                if (valueToSet != default && valueToSet != default(DateTime).ToString() && sourceProperties[i].Name != "Photo")
                {
                    destProperties.FirstOrDefault(p => p.Name == propertyNames[i])
                            ?.SetValue(dest, valueToSet);
                }
                else if (sourceProperties[i].Name == "Photo")
                {
                    destProperties.FirstOrDefault(p => p.Name == propertyNames[i])
                            ?.SetValue(dest, sourceProperties[i].GetValue(source));
                }
            }
            return dest;
        }
    }
}