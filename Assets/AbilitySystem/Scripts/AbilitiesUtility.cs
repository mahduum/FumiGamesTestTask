using System;
using System.Linq;

namespace AbilitySystem.Scripts
{
    public static class AbilitiesUtility
    {
        public static string[] GetDerivedTypeNames(Type baseType, bool allowAbstract = false)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => baseType.IsAssignableFrom(p) && p != baseType && (allowAbstract || !p.IsAbstract))
                .Select(t => t.FullName)
                .ToArray();
        }

        public static string[] GetInterfaceImplementors(Type interfaceType, bool allowAbstract = false)
        {
            var typesInAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .ToArray();
            
            var implementingTypes = typesInAssembly
                .Where(t => interfaceType.IsAssignableFrom(t) && t.IsValueType && !t.IsAbstract)
                .ToList();

            var implementingTypeNames = implementingTypes.Select(t => t.FullName).ToArray();

            return implementingTypeNames;
        }
    }
}