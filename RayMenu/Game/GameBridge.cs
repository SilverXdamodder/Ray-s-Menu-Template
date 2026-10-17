using System;
using System.Reflection;
using BepInEx.Logging;
using UnityEngine;

namespace RayMenu.Game
{
    public class GameBridge
    {
        private static readonly string[] PlayerTypeNames =
        {
            "GorillaLocomotion.Player",
            "GTPlayer",
            "GorillaTag.GTPlayer",
            "GorillaTag.Player"
        };

        private static readonly string[] VrMenuButtons =
        {
            "leftControllerSecondaryButton",
            "rightControllerPrimaryButton",
            "rightControllerSecondaryButton"
        };

        private readonly ManualLogSource _log;
        private PropertyInfo _playerInstance;
        private float _nextResolve;

        private Type _pollerType;
        private bool _pollerSearched;

        private static Type _xrDeviceType;
        private static Type _xrSettingsType;
        private static bool _xrSearched;

        public GameBridge(ManualLogSource log)
        {
            _log = log;
            TryResolve();
        }

        public object Player
        {
            get
            {
                if (_playerInstance == null) return null;
                try { return _playerInstance.GetValue(null); }
                catch { return null; }
            }
        }

        public void EnsureResolved()
        {
            if (Player != null) return;
            if (Time.time < _nextResolve) return;
            _nextResolve = Time.time + 2f;
            TryResolve();
        }

        public void TryResolve()
        {
            foreach (var name in PlayerTypeNames)
            {
                var type = ReflectionHelper.FindType(name);
                if (type == null) continue;

                var instance = type.GetProperty(
                    "Instance",
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                if (instance == null) continue;

                _playerInstance = instance;
                _log?.LogDebug("GameBridge: bound to " + type.FullName);
                return;
            }
        }

        public bool VrMenuButtonHeld()
        {
            if (!_pollerSearched)
            {
                _pollerSearched = true;
                _pollerType = ReflectionHelper.FindType("ControllerInputPoller")
                    ?? ReflectionHelper.FindType("GorillaTag.ControllerInputPoller");
            }
            if (_pollerType == null) return false;

            var instance = ReflectionHelper.GetStaticMember(_pollerType, "instance");
            if (instance == null) return false;

            foreach (var button in VrMenuButtons)
            {
                var value = ReflectionHelper.GetMember(instance, button);
                if (value is bool pressed && pressed) return true;
            }
            return false;
        }

        public static bool IsVrPresent
        {
            get
            {
                if (!_xrSearched)
                {
                    _xrSearched = true;
                    _xrDeviceType = ReflectionHelper.FindType("UnityEngine.XR.XRDevice");
                    _xrSettingsType = ReflectionHelper.FindType("UnityEngine.XR.XRSettings");
                }

                var present = GetStaticBool(_xrDeviceType, "isPresent");
                if (present.HasValue) return present.Value;

                var active = GetStaticBool(_xrSettingsType, "isDeviceActive");
                return active ?? false;
            }
        }

        private static bool? GetStaticBool(Type type, string name)
        {
            if (type == null) return null;
            var value = ReflectionHelper.GetStaticMember(type, name);
            return value is bool flag ? flag : (bool?)null;
        }
    }
}
