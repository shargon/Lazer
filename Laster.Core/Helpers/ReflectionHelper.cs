﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Laster.Core.Helpers
{
    public class ReflectionHelper
    {
        /// <summary>
        /// Redirije un ensamblado
        /// </summary>
        /// <param name="shortName">Nombre corto</param>
        /// <param name="targetVersion">Versión destino</param>
        /// <param name="publicKeyToken">Token</param>
        public static void RedirectAssembly(string shortName, Version targetVersion, string publicKeyToken)
        {
            ResolveEventHandler handler = null;

            handler = (sender, args) =>
            {
                // Use latest strong name & version when trying to load SDK assemblies
                AssemblyName requestedAssembly = new AssemblyName(args.Name);
                if (requestedAssembly.Name != shortName)
                    return null;

#if DEBUG
                Debug.WriteLine("Redirecting assembly load of " + args.Name
                              + ",\tloaded by " + (args.RequestingAssembly == null ? "(unknown)" : args.RequestingAssembly.FullName));
#endif

                requestedAssembly.Version = targetVersion;
                requestedAssembly.SetPublicKeyToken(new AssemblyName("x, PublicKeyToken=" + publicKeyToken).GetPublicKeyToken());
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

                AppDomain.CurrentDomain.AssemblyResolve -= handler;

                return Assembly.Load(requestedAssembly);
            };

            AppDomain.CurrentDomain.AssemblyResolve += handler;
        }
        /// <summary>
        /// Devuelve todos los tipos que implementan el interface
        /// </summary>
        /// <param name="ti">Tipo</param>
        /// <param name="asms">Ensamblados donde comprobar</param>
        public static IEnumerable<Type> GetTypesAssignableFrom(Type ti, params Assembly[] asms)
        {
            foreach (Assembly asm in asms)
                foreach (Type tp in asm.GetTypes())
                {
                    if (ti != tp && ti.IsAssignableFrom(tp))
                    {
                        yield return tp;
                    }
                }
        }
        /// <summary>
        /// Devuelve si tiene algún constructor público y sin parámetros
        /// </summary>
        /// <param name="tp">Tipo</param>
        public static bool HavePublicConstructor(Type tp)
        {
            foreach (ConstructorInfo o in tp.GetConstructors())
            {
                if (!o.IsPublic) continue;
                ParameterInfo[] pi = o.GetParameters();

                if (pi != null && pi.Length > 0) continue;
                return true;
            }
            return false;
        }
    }
}