namespace EfesusProfiler;

using MethodDecorator.Fody.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    class ProfileAttribute : Attribute, IMethodDecorator
    {
        private string methodeName;
        private string className;

        // instance, method and args can be captured here and stored in attribute instance fields
        // for future usage in OnEntry/OnExit/OnException
        public void Init(object instance, MethodBase method, object[] args)
        {
            methodeName = method.Name;
            className = method.ReflectedType.Name;
        }

        public void OnEntry()
        {
            ProfilingCollection.add(className + "." + methodeName);
        }

        public void OnExit()
        {
        }

        public void OnException(Exception exception)
        {
        }
    }
