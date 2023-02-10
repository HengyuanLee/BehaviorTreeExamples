using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace GraphProcessor
{
	public delegate void CustomPortIODelegate(BaseNode node, List< SerializableEdge > edges, NodePort outputPort = null);

	public static class CustomPortIO
	{
		class PortIOPerField : Dictionary< string, CustomPortIODelegate > {}
		class PortIOPerNode : Dictionary< Type, PortIOPerField > {}

		static Dictionary< Type, List< Type > >	assignableTypes = new Dictionary< Type, List< Type > >();
		static PortIOPerNode					customIOPortMethods = new PortIOPerNode();

		static CustomPortIO()
		{
			LoadCustomPortMethods();
		}

		static void LoadCustomPortMethods()
		{
			BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			foreach (var type in AppDomain.CurrentDomain.GetAllTypes())
			{
				if (type.IsAbstract || type.ContainsGenericParameters)
					continue ;
				if (!(type.IsSubclassOf(typeof(BaseNode))))
					continue ;

				var methods = type.GetMethods(bindingFlags);

				foreach (var method in methods)
				{
					//�ҳ����нڵ�����CustomPortInput��CustomPortOutput��ǩ�ķ���
					var portInputAttr = method.GetCustomAttribute< CustomPortInputAttribute >();
					var portOutputAttr = method.GetCustomAttribute< CustomPortOutputAttribute >();

					if (portInputAttr == null && portOutputAttr == null)
						continue ;
					
					var p = method.GetParameters();
					bool nodePortSignature = false;

					// ��Ҫ��������
					if (p.Length == 2 && p[1].ParameterType == typeof(NodePort))
						nodePortSignature = true;

					CustomPortIODelegate deleg;
#if ENABLE_IL2CPP
					// IL2CPP doesn't support expression builders
					if (nodePortSignature)
					{
						deleg = new CustomPortIODelegate((node, edges, port) => {
							Debug.Log(port);
							method.Invoke(node, new object[]{ edges, port});
						});
					}
					else
					{
						deleg = new CustomPortIODelegate((node, edges, port) => {
							method.Invoke(node, new object[]{ edges });
						});
					}
#else
					//��������
					var p1 = Expression.Parameter(typeof(BaseNode), "node");
					var p2 = Expression.Parameter(typeof(List< SerializableEdge >), "edges");
					var p3 = Expression.Parameter(typeof(NodePort), "port");

					MethodCallExpression ex;
					if (nodePortSignature)
						//����p1�ڵ㷽��p1.method(p2,p3)
						ex = Expression.Call(Expression.Convert(p1, type), method, p2, p3);
					else
						//����p1�ڵ㷽��p1.method(p2)
						ex = Expression.Call(Expression.Convert(p1, type), method, p2);

					deleg = Expression.Lambda< CustomPortIODelegate >(ex, p1, p2, p3).Compile();
#endif

					if (deleg == null)
					{
						Debug.LogWarning("Can't use custom IO port function " + method + ": The method have to respect this format: " + typeof(CustomPortIODelegate));
						continue ;
					}
					//�ֶ�����
					string fieldName = (portInputAttr == null) ? portOutputAttr.fieldName : portInputAttr.fieldName;
					//�ֶ��Զ������ͣ���float
					Type customType = (portInputAttr == null) ? portOutputAttr.outputType : portInputAttr.inputType;
					//�ֶ���nodeʵ�ʶ��������
					Type fieldType = type.GetField(fieldName, bindingFlags).FieldType;

					//deleg��node�ﶨ���[CustomPort]�ķ���
					AddCustomIOMethod(type, fieldName, deleg);

					//�����以�����ֶ����Ϳ�
					AddAssignableTypes(customType, fieldType);
					AddAssignableTypes(fieldType, customType);
				}
			}
		}
		/// <summary>
		/// ���ݸ�����node���ͺ�ָ�����ֶ����ƣ���ȡ���ֶε��Զ��巽����Ҳ���������[CustomPortOutput]��ǩ�ķ�����
		/// </summary>
		/// <returns>�����[CustomPortOutput]��ǩ����ָ���ֶεķ���</returns>
		public static CustomPortIODelegate GetCustomPortMethod(Type nodeType, string fieldName)
		{
			PortIOPerField			portIOPerField;
			CustomPortIODelegate	deleg;

			customIOPortMethods.TryGetValue(nodeType, out portIOPerField);

			if (portIOPerField == null)
				return null;

			portIOPerField.TryGetValue(fieldName, out deleg);

			return deleg;
		}

		static void AddCustomIOMethod(Type nodeType, string fieldName, CustomPortIODelegate deleg)
		{
			if (!customIOPortMethods.ContainsKey(nodeType))
				customIOPortMethods[nodeType] = new PortIOPerField();

			customIOPortMethods[nodeType][fieldName] = deleg;
		}

		static void AddAssignableTypes(Type fromType, Type toType)
		{
			if (!assignableTypes.ContainsKey(fromType))
				assignableTypes[fromType] = new List< Type >();

			assignableTypes[fromType].Add(toType);
		}

		public static bool IsAssignable(Type input, Type output)
		{
			if (assignableTypes.ContainsKey(input))
				return assignableTypes[input].Contains(output);
			return false;
		}
	}
}