using System;
using System.Collections.Generic;
using VContainer;
using VContainer.Unity;
using VContainerTemplate.Utilities.Assertions;

namespace VContainerTemplate.Utilities.Extensions
{
    public static class VContainerExtensions
    {
        public static IObjectResolver CreateContainer(Action<IContainerBuilder> build)
        {
            var builder = new ContainerBuilder();
            build?.Invoke(builder);

            return builder.Build();
        }

        public static RegistrationBuilder Register(this IContainerBuilder builder, object instance)
        {
            return builder.RegisterInstance(instance.AssertNotNull()).AsImplementedInterfaces();
        }
        
        public static RegistrationBuilder Register<T>(this IContainerBuilder builder)
        {
            return builder.Register<T>(Lifetime.Singleton).AsImplementedInterfaces();
        }
        
        
        public static IObjectResolver CreateContainer(this IInstaller installer)
        {
            if (installer is null)
                throw new ArgumentNullException(nameof(installer));

            var builder = new ContainerBuilder();
            installer.Install(builder);
            return builder.Build();
        }

        public static IObjectResolver CreateContainer(this IEnumerable<IInstaller> installers)
        {
            if (installers is null)
                throw new ArgumentNullException(nameof(installers));

            var builder = new ContainerBuilder();

            foreach (var installer in installers)
            {
                installer.Install(builder);
            }

            return builder.Build();
        }

        public static T Instantiate<T>(this IObjectResolver resolver) where T : class
        {
            var instanceType = typeof(T);
            var constructorInfo = instanceType.GetConstructors()[0];
            var constructorParamsInfo = constructorInfo.GetParameters();
            var constructorParams = new object[constructorParamsInfo.Length];

            for (var i = 0; i < constructorParamsInfo.Length; i++)
            {
                var parameterInfo = constructorParamsInfo[i];
                var type = parameterInfo.ParameterType;
                constructorParams[i] = resolver.Resolve(type);
            }

            var instance = Activator.CreateInstance(instanceType, constructorParams);

            return (T)instance;
        }

        public static T Instantiate<T>(this IObjectResolver resolver, Lifetime lifetime = Lifetime.Singleton)
        {
            var registrationBuilder = new RegistrationBuilder(typeof(T), lifetime);
            var registration = registrationBuilder.Build();
            return (T)resolver.Resolve(registration);
        }
        
        public static T Instantiate<T>(this IObjectResolver resolver, Lifetime lifetime = Lifetime.Singleton, params object[] args)
        {
            var registrationBuilder = new RegistrationBuilder(typeof(T), lifetime);

            foreach (var arg in args)
                registrationBuilder.WithParameter(arg.GetType(), arg);

            var registration = registrationBuilder.Build();
            return (T)resolver.Resolve(registration);
        }

        // public static RegistrationBuilder RegisterFactory<T>(this IContainerBuilder builder, Lifetime lifetime = Lifetime.Singleton) where T : class
        // {
        // return builder.RegisterFactory<T>(resolver => resolver.Instantiate<T>, lifetime);
        // }
    }
}