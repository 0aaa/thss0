using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Thss0.Web.Extensions
{
    public class EntityInitializer
    {
        private readonly string[] _departmentProperties = { "Name" };
        private readonly string[] _userProperties = { "UserName", "DoB", "PoB" };
        private readonly string[] _procedureProperties = { "Name", "RealizationTime", "NextProcedureTime" };
        private readonly string[] _resultProperties = { "Content" };

        public void Validation(ModelStateDictionary state, object viewModel)
        {
            var type = viewModel.GetType();
            var propertyNames = new string[0];
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
            }
            var properties = type.GetProperties().Where(p => propertyNames.Contains(p.Name)).ToArray();
            for (ushort i = 0; i < properties.Length; i++)
            {
                value = properties[i].GetValue(viewModel)?.ToString() ?? "";
                if (properties[i].Name.Contains("Time") && value != "" && DateTime.Parse(value) < DateTime.Now)
                {
                    state.AddModelError(properties[i].Name, $"{Regex.Replace(properties[i].Name, "([a-z])([A-Z])", "$1 $2")} cannot be less than the current time");
                }
                else if (!properties[i].Name.Contains("Time") && value == "")
                {
                    state.AddModelError(properties[i].Name, $"{Regex.Replace(properties[i].Name, "([a-z])([A-Z])", "$1 $2")} required");
                }
            }
        }

        public object InitializeEntity(ModelStateDictionary state, object source, object dest)
        {
            var sourceType = source.GetType();
            var destType = dest.GetType();
            var propertyNames = new string[0];
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
        public object InitializeViewModel(object source, object dest)
        {
            var sourceType = source.GetType();
            var destType = dest.GetType();
            var propertyNames = new string[0];
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
            }
            var sourceProperties = sourceType.GetProperties()
                                            .Where(p => propertyNames.Contains(p.Name)).ToArray();
            var destProperties = destType.GetProperties()
                                            .Where(p => propertyNames.Contains(p.Name)).ToArray();
            string value;
            for (ushort i = 0; i < propertyNames.Length; i++)
            {
                value = sourceProperties[i].GetValue(source)?.ToString() ?? "";
                if (value != default && value != default(DateTime).ToString())
                {
                    destProperties.FirstOrDefault(p => p.Name == propertyNames[i])
                            ?.SetValue(dest, sourceProperties.FirstOrDefault(p => p.Name == propertyNames[i])?.GetValue(source)?.ToString());
                }
            }
            return dest;
        }
    }
}