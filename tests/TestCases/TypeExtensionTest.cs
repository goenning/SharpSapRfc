using SharpSapRfc.Test.Model;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace SharpSapRfc.Test.TestCases
{
    public class TypeExtensionTest
    {
        [Theory, PropertyData("TheseAreNotComplexTypeData")]
        public void TheseAreNotComplexType(object input)
        {
            Assert.False(input.GetType().IsComplexType());
        }

        public static IEnumerable<object[]> TheseAreNotComplexTypeData
        {
            get
            {
                return new[]
                    {
                        new object[] { "a string" },
                        new object[] { 2 },
                        new object[] { DateTime.Now },
                        new object[] { 250m }
                    };
            }
        }

        [Theory, PropertyData("TheseAreComplexTypeData")]
        public void TheseAreComplexType(object input)
        {
            Assert.True(input.GetType().IsComplexType());
        }

        public static IEnumerable<object[]> TheseAreComplexTypeData
        {
            get
            {
                return new[]
                    {
                        new object[] { new ZOrder() },
                        new object[] { new ZOrderLine() },
                        new object[] { new ZMara() }
                    };
            }
        }

        [Theory, PropertyData("TheseAreNotComplexTypeData")]
        public void TheseAreNotEnumerableTypes(object value)
        {
            Assert.False(value.GetType().IsEnumerable());
        }

        [Theory, PropertyData("EnumerableTestData")]
        public void TheseAreEnumerableTypes(Type enumerable, Type dummy)
        {
            Assert.True(enumerable.IsEnumerable());
        }

        [Theory, PropertyData("EnumerableTestData")]
        public void CanGetEnumerableInnerType(Type enumerable, Type innerType)
        {
            Assert.Equal(enumerable.GetEnumerableInnerType(), innerType);
        }

        public static IEnumerable<object[]> EnumerableTestData
        {
            get
            {
                return new[]
                    {
                        new object[] { typeof(IEnumerable<String>), typeof(String) },
                        new object[] { typeof(IEnumerable<ZOrder>), typeof(ZOrder) },
                        new object[] { typeof(String[]), typeof(String) },
                        new object[] { typeof(ZOrder[]), typeof(ZOrder) },
                        new object[] { typeof(List<String>), typeof(String) },
                        new object[] { typeof(List<ZOrder>), typeof(ZOrder) },
                        new object[] { typeof(IList<String>), typeof(String) },
                        new object[] { typeof(IList<ZOrder>), typeof(ZOrder) }
                    };
            }
        }
    }
}
