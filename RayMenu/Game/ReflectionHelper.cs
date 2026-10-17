using System;
using System.Reflection;

namespace RayMenu.Game
{
    public static class ReflectionHelper
    {
        private const BindingFlags Members =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        private static Assembly _gameAssembly;

        public static Assembly GameAssembly
        {
            get
            {
                if (_gameAssembly != null) return _gameAssembly;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name == "Assembly-CSharp")
                    {
                        _gameAssembly = assembly;
                        break;
                    }
                }
                return _gameAssembly;
            }
        }

        public static Type FindType(string fullName)
        {
            var type = GameAssembly?.GetType(fullName);
            if (type != null) return type;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(fullName);
                if (type != null) return type;
            }
            return null;
        }

        public static object GetStaticMember(Type type, string name)
        {
            if (type == null) return null;

            var property = type.GetProperty(name, Members);
            if (property != null && property.CanRead)
            {
                try { return property.GetValue(null); } catch { }
            }

            var field = type.GetField(name, Members);
            if (field != null)
            {
                try { return field.GetValue(null); } catch { }
            }
            return null;
        }

        public static object GetMember(object target, string name)
        {
            if (target == null) return null;
            var type = target.GetType();

            var property = type.GetProperty(name, Members);
            if (property != null && property.CanRead)
            {
                try { return property.GetValue(target); } catch { }
            }

            var field = type.GetField(name, Members);
            if (field != null)
            {
                try { return field.GetValue(target); } catch { }
            }
            return null;
        }

        public static bool TrySetMember(object target, string name, object value)
        {
            if (target == null) return false;
            var type = target.GetType();

            var property = type.GetProperty(name, Members);
            if (property != null && property.CanWrite)
            {
                try { property.SetValue(target, value); return true; } catch { }
            }

            var field = type.GetField(name, Members);
            if (field != null)
            {
                try { field.SetValue(target, value); return true; } catch { }
            }

            var method = type.GetMethod("set_" + name, Members);
            if (method != null)
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 1)
                {
                    try { method.Invoke(target, new[] { value }); return true; } catch { }
                }
            }
            return false;
        }
    }
}
