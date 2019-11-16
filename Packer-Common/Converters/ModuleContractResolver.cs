using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Phi.Packer.Common.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Phi.Packer.Common.Converters
{
    internal class ModuleContractResolver : DefaultContractResolver
    {
        public static readonly ModuleContractResolver Instance = new ModuleContractResolver();

        private ModuleContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy();
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (prop.PropertyType?.GetInterface(nameof(IEnumerable)) != null)
            {
                prop.ShouldSerialize = self => 
                    self?.GetType().GetProperty(member.Name)?.GetValue(self).As<IEnumerable>()?.Cast<object>().Any() == true;
            }

            return prop;
        }
    }
}
