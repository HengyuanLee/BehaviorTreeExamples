using UnityEngine;
using GraphProcessor;
using System;
using NodeGraphProcessor.Examples;
using System.Collections.Generic;

public class CustomConvertions : ITypeAdapter
{
    //----------定义可以兼容，使用自定义方法转换
    public static Vector4 ConvertFloatToVector4(float from) => new Vector4(from, from, from, from);
    public static float ConvertVector4ToFloat(Vector4 from) => from.x;


    //----------定义不兼容类型，使得这两个端口无法连接
    public override IEnumerable<(Type, Type)> GetIncompatibleTypes()
    {
        yield return (typeof(ConditionalLink), typeof(object));
        yield return (typeof(RelayNode.PackedRelayData), typeof(object));
    }
}