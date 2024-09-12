using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Thss0.Web.Extensions
{
    public partial class InitializationHelper
    {
        const int RESULT_MIN_GAP = 5;
        //private static readonly string[] first = ["Id"];
        private readonly string[] _departmentProperties = ["Name"];
        private readonly string[] _userProperties = ["Name", "DoB", "PoB", "Photo"];
        private readonly string[] _procedureProperties = ["Name", "BeginTime", "EndTime"];
        private readonly string[] _resultProperties = ["Name", "ObtainmentTime", "Content"];
        private readonly string[] _roleProperties = ["Name"];

        public void Validation(ModelStateDictionary state, object vm)
        {
            var type = vm.GetType();
            var propNames = Array.Empty<string>();
            string value;
            switch (type.Name)
            {
                case "DepartmentViewModel":
                    propNames = _departmentProperties;
                    break;
                case "UserViewModel":
                    propNames = _userProperties;
                    break;
                case "ProcedureViewModel":
                    propNames = _procedureProperties;
                    break;
                case "ResultViewModel":
                    propNames = _resultProperties;
                    break;
                case "IdentityRoleProxy":
                    propNames = _roleProperties;// To verify.
                    break;
            }
            var props = type.GetProperties().Where(p => propNames.Contains(p.Name)).ToArray();
            for (int i = 0; i < props.Length; i++)
            {
                value = props[i].GetValue(vm)?.ToString() ?? "";
                if (value != "")
                {
                    if (props[i].Name.Contains("Time"))
                    {
                        if (DateTime.Parse(value) < DateTime.Now.AddMinutes(RESULT_MIN_GAP))
                        {
                            state.AddModelError(props[i].Name, $"{PropName().Replace(props[i].Name, "$1 $2")} cannot be less than the current time");
                        }
                        if (DateTime.Parse(value).Minute % 15 != 0)
                        {
                            state.AddModelError(props[i].Name, $"{PropName().Replace(props[i].Name, "$1 $2")} minutes must be multiple of 15");
                        }
                    }
                    else if (type.Name == "ResultViewModel" && props[i].Name == "UserNames" && value.Split('\n').Length < 2)
                    {
                        state.AddModelError(props[i].Name, $"{PropName().Replace(props[i].Name, "$1 $2")} prof and client required");
                    }
                }
                else if (!props[i].Name.Contains("Time") && value == "")
                {
                    state.AddModelError(props[i].Name, $"{PropName().Replace(props[i].Name, "$1 $2")} required");
                }
            }
        }

        public object InitializeEntity(object src, object dest)
        {
            var srcType = src.GetType();
            var destType = dest.GetType();
            var propNames = Array.Empty<string>();
            switch (srcType.Name)
            {
                case "DepartmentViewModel":
                    propNames = _departmentProperties;
                    break;
                case "UserViewModel":
                    propNames = _userProperties;
                    break;
                case "ProcedureViewModel":
                    propNames = _procedureProperties;
                    break;
                case "ResultViewModel":
                    propNames = _resultProperties;
                    break;
                case "IdentityRoleProxy":
                    propNames = _roleProperties;// To verify.
                    break;
            }
            var srcProps = srcType.GetProperties().Where(p => propNames.Contains(p.Name)).OrderBy(p => p.Name).ToArray();
            var destProps = destType.GetProperties().Where(p => propNames.Contains(p.Name)).OrderBy(p => p.Name).ToArray();
            propNames = [.. propNames.Order()];
            for (int i = 0; i < propNames.Length; i++)
            {
                if (srcProps[i].GetValue(src)?.ToString() == "")
                {
                    continue;
                }
                switch (destProps[i].PropertyType.Name)
                {
                    case "DateTime":
                        destProps.FirstOrDefault(p => p.Name == propNames[i])
                                ?.SetValue(dest, DateTime.Parse(srcProps.FirstOrDefault(p => p.Name == propNames[i])
                                                                        ?.GetValue(src)?.ToString() ?? "0"));
                        break;
                    case "Byte[]":
                        destProps.FirstOrDefault(p => p.Name == propNames[i])
                                ?.SetValue(dest, ((sbyte[]?)srcProps.FirstOrDefault(p => p.Name == propNames[i])
                                                                        ?.GetValue(src))?.Select(sb => (byte)(sb + 256)).ToArray());
                        break;
                    default:
                        destProps.FirstOrDefault(p => p.Name == propNames[i])
                                ?.SetValue(dest, srcProps.FirstOrDefault(p => p.Name == propNames[i])
                                                                        ?.GetValue(src));
                        break;
                }
            }
            return dest;
        }

        public object InitializeViewModel(object src, object dest)// To verify for roles.
        {
            var srcType = src.GetType();
            var destType = dest.GetType();
            var propNames = Array.Empty<string>();
            string toSet;
            switch (srcType.Name)
            {
                case "DepartmentProxy":
                    propNames = _departmentProperties;
                    break;
                case "ApplicationUserProxy":
                    propNames = _userProperties;
                    break;
                case "ProcedureProxy":
                    propNames = _procedureProperties;
                    break;
                case "ResultProxy":
                    propNames = _resultProperties;
                    break;
                case "IdentityRoleProxy":
                    propNames = _roleProperties;// To verify.
                    break;
            }
            //propertyNames = [.. first, .. propertyNames];
            var srcProps = srcType.GetProperties().Where(p => propNames.Contains(p.Name)).OrderBy(p => p.Name).ToArray();
            var destProps = destType.GetProperties().Where(p => propNames.Contains(p.Name)).OrderBy(p => p.Name).ToArray();
            propNames = [.. propNames.Order()];
            for (int i = 0; i < propNames.Length; i++)
            {
                toSet = srcProps[i].GetValue(src)?.ToString() ?? "";
                if (toSet != default && toSet != default(DateTime).ToString() && srcProps[i].Name != "Photo")
                {
                    destProps.FirstOrDefault(p => p.Name == propNames[i])
                            ?.SetValue(dest, toSet);
                }
                else if (srcProps[i].Name == "Photo")
                {
                    destProps.FirstOrDefault(p => p.Name == propNames[i])
                            ?.SetValue(dest, srcProps[i].GetValue(src));
                }
            }
            return dest;
        }

        [GeneratedRegex("([a-z])([A-Z])")]
        private static partial Regex PropName();
    }
}