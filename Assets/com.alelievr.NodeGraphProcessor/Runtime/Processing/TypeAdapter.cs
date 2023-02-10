using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace GraphProcessor
{
    /// <summary>
    /// Implement this interface to use the inside your class to define type convertions to use inside the graph.
    /// Example:
    /// <code>
    /// public class CustomConvertions : ITypeAdapter
    /// {
    ///     public static Vector4 ConvertFloatToVector(float from) => new Vector4(from, from, from, from);
    ///     ...
    /// }
    /// </code>
    /// </summary>
    public abstract class ITypeAdapter // TODO: turn this back into an interface when we have C# 8
    {
        public virtual IEnumerable<(Type, Type)> GetIncompatibleTypes() { yield break; }
    }

    public static class TypeAdapter
    {
        //所有继承自ITypeAdapter类的方法，key为方法GetIncompatibleTypes返回的类型对。key对和方法参数自动匹配。
        static Dictionary< (Type from, Type to), Func<object, object> > adapters = new Dictionary< (Type, Type), Func<object, object> >();
        //key对的对应的方法信息
        static Dictionary< (Type from, Type to), MethodInfo > adapterMethods = new Dictionary< (Type, Type), MethodInfo >();
        //两个类型经过自定义可匹配信息
        static List< (Type from, Type to)> incompatibleTypes = new List<( Type from, Type to) >();

        [System.NonSerialized]
        static bool adaptersLoaded = false;

#if !ENABLE_IL2CPP
        static Func<object, object> ConvertTypeMethodHelper<TParam, TReturn>(MethodInfo method)
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            Func<TParam, TReturn> func = (Func<TParam, TReturn>)Delegate.CreateDelegate
                (typeof(Func<TParam, TReturn>), method);

            // Now create a more weakly typed delegate which will call the strongly typed one
            Func<object, object> ret = (object param) => func((TParam)param);
            return ret;
        }
#endif

        static void LoadAllAdapters()
        {
            foreach (Type type in AppDomain.CurrentDomain.GetAllTypes())
            {
                //获取所有继承了ITypeAdapter的类
                if (typeof(ITypeAdapter).IsAssignableFrom(type))
                {
                    //IsAssignableFrom的意思就是，ITypeAdapter可以指向type实例，也就是接口或父类或同类
                    if (type.IsAbstract)
                        continue;
                    
                    var adapter = Activator.CreateInstance(type) as ITypeAdapter;
                    if (adapter != null)
                    {
                        foreach (var types in adapter.GetIncompatibleTypes())
                        {
                            //获取玩家所有的自定义可转换字段的类型，参考CustomConvertions.cs里的定义
                            incompatibleTypes.Add((types.Item1, types.Item2));
                            incompatibleTypes.Add((types.Item2, types.Item1));
                        }
                    }
                    //获取ITypeAdapter实现类里的所有方法
                    foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (method.GetParameters().Length != 1)
                        {
                            Debug.LogError($"Ignoring convertion method {method} because it does not have exactly one parameter");
                            continue;
                        }
                        if (method.ReturnType == typeof(void))
                        {
                            Debug.LogError($"Ignoring convertion method {method} because it does not returns anything");
                            continue;
                        }
                        Type from = method.GetParameters()[0].ParameterType;
                        Type to = method.ReturnType;

                        try {

#if ENABLE_IL2CPP
                            // IL2CPP doesn't suport calling generic functions via reflection (AOT can't generate templated code)
                            Func<object, object> r = (object param) => { return (object)method.Invoke(null, new object[]{ param }); };
#else
                            MethodInfo genericHelper = typeof(TypeAdapter).GetMethod("ConvertTypeMethodHelper", 
                                BindingFlags.Static | BindingFlags.NonPublic);
                            
                            // Now supply the type arguments
                            MethodInfo constructedHelper = genericHelper.MakeGenericMethod(from, to);

                            object ret = constructedHelper.Invoke(null, new object[] {method});
                            var r = (Func<object, object>) ret;
#endif
                            //可调用方法的类型转换方法
                            adapters.Add((method.GetParameters()[0].ParameterType, method.ReturnType), r);
                            //转换的方法反射信息
                            adapterMethods.Add((method.GetParameters()[0].ParameterType, method.ReturnType), method);
                        } catch (Exception e) {
                            Debug.LogError($"Failed to load the type convertion method: {method}\n{e}");
                        }
                    }
                }
            }

            // Ensure that the dictionary contains all the convertions in both ways
            // ex: float to vector but no vector to float
            //单向转换，缺少提醒互相转换的方法
            foreach (var kp in adapters)
            {
                if (!adapters.ContainsKey((kp.Key.to, kp.Key.from)))
                    Debug.LogError($"Missing convertion method. There is one for {kp.Key.from} to {kp.Key.to} but not for {kp.Key.to} to {kp.Key.from}");
            }

            adaptersLoaded = true;
        }
        /// <summary>
        /// 不兼容判断
        /// 数据定义在ITypeAdapter的GetIncompatibleTypes()里。
        /// 判断2个端口是否能连接条件之一，能否从from连到to。
        /// </summary>
        public static bool AreIncompatible(Type from, Type to)
        {
            if (incompatibleTypes.Any((k) => k.from == from && k.to == to))
                return true;
            return false;
        }
        /// <summary>
        /// 可赋值判断
        /// 数据定义在ITypeAdapter的自定义转换方法里。
        /// 判断2个端口是否能连接条件之一。能否从from连到to
        /// </summary>
        public static bool AreAssignable(Type from, Type to)
        {
            if (!adaptersLoaded)
                LoadAllAdapters();
            
            if (AreIncompatible(from, to))
                return false;

            return adapters.ContainsKey((from, to));
        }

        public static MethodInfo GetConvertionMethod(Type from, Type to) => adapterMethods[(from, to)];

        /// <summary>
        /// 将from对象转成targetType，具体定义的方法在实现了ITypeAdapterde的类里。
        /// </summary>
        public static object Convert(object from, Type targetType)
        {
            if (!adaptersLoaded)
                LoadAllAdapters();

            Func<object, object> convertionFunction;
            if (adapters.TryGetValue((from.GetType(), targetType), out convertionFunction))
                return convertionFunction?.Invoke(from);

            return null;
        }
    }
}