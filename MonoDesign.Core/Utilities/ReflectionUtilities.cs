﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MonoDesign.Core.Entity.Reflection;

namespace MonoDesign.Core.Utilities {
	public static class ReflectionUtilities {
		public static void SetPropertyValue(MemberInfo memberInfo, object obj, object propertyValue) {
			switch (memberInfo) {
				case PropertyInfo propertyInfo:
					propertyInfo.SetValue(obj, propertyValue);
					break;
				case FieldInfo fieldInfo:
					fieldInfo.SetValue(obj, propertyValue);
					break;
			}
		}
		public static T GetPropertyValue<T>(object obj, MemberInfo memberInfo) {
			if (memberInfo is PropertyInfo propertyInfo) {
				return (T) propertyInfo.GetValue(obj);
			}
			if (memberInfo is FieldInfo fieldInfo) {
				return (T) fieldInfo.GetValue(obj);
			}
			return default;
		}
		public static MemberInfo GetPropertyInfo<TSource, TProperty>(
			Expression<Func<TSource, TProperty>> propertyLambda) {
			var type = typeof(TSource);
			if (!(propertyLambda.Body is MemberExpression member))
				throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");
			var propInfo = member.Member;
			if (propInfo == null)
				throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
			if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
				throw new ArgumentException(
					$"Expression '{propertyLambda}' refers to a property that is not from type {type}.");
			return propInfo;
		}
		public static IEnumerable<AttributeInfo<T>> GetTypesInfoWithAttribute<T>(params Assembly[] assemblies) where T : Attribute {
			if (assemblies.Length == 0) {
				assemblies = GetAssemblies();
			}
			return assemblies.SelectMany(assembly => assembly.GetTypes())
				.Where(type => type.GetCustomAttribute<T>() != null).Select(type => new AttributeInfo<T>(type.GetCustomAttribute<T>(), type));
		}
		public static IEnumerable<Type> GetTypesWithAttribute<T>(params Assembly[] assemblies) where T : Attribute {
			if (assemblies.Length == 0) {
				assemblies = GetAssemblies();
			}
			return assemblies.SelectMany(assembly => assembly.GetTypes())
			.Where(type => type.GetCustomAttribute<T>() != null);
		}
		public static IEnumerable<Assembly> GetAssemblies<T>() where T: Attribute {
			return GetAssemblies().Where(assembly =>
				assembly.GetCustomAttribute<T>() != null);
		}
		public static Assembly[] GetAssemblies() {
			var currentDomain = AppDomain.CurrentDomain;
			return currentDomain.GetAssemblies();
		}
		public static T GetAttributeValue<T>(this object obj) where T : Attribute {
			var type = obj.GetType();
			return type.GetCustomAttribute<T>();
		}
	}
}