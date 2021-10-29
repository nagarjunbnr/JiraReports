using System;
using System.Collections.Generic;
using System.Text;

namespace Loop.Confluence.IoC
{
    public interface IServiceLocator
    {
        void RegisterInstance<ClassType>(ClassType classInstance, Type asType = null) where ClassType : class;
        void RegisterSingleInstance<ClassType>(Type asType = null);
        void RegisterSingleInstance(Type classType, Type asType = null);
        void RegisterInstancePerRequest<ClassType>(Type asType = null);
        void RegisterInstancePerRequest(Type classType, Type asType = null);
        void RegisterSingleInstancePerScope<ClassType>(Type asType = null);
        void RegisterSingleInstancePerScope(Type classType, Type asType = null);

        object GetObject(Type objectType);
        ObjectType GetObject<ObjectType>();
        ObjectType GetObject<ObjectType, ParameterType>(ParameterType parameter);
        ObjectType GetObject<ObjectType, Parameter1Type, Parameter2Type>(Parameter1Type parameter1, Parameter2Type parameter2);
        ObjectType GetObject<ObjectType, Parameter1Type, Parameter2Type, Parameter3Type>(Parameter1Type parameter1, Parameter2Type parameter2, Parameter3Type parameter3);
    }
}
