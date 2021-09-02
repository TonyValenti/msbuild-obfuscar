using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System {
    internal static class StringExtensions {
        public static string Coalesce(this string? This) {
            var ret = This ?? string.Empty;

            return ret;
        }

        public static IEnumerable<T> WhereIsNotNull<T>(this IEnumerable<T?>? This) where T : class {
            return This.Coalesce().Where(x => x.IsNotNull()).OfType<T>();
        }

        public static IEnumerable<T> WhereIsNotNull<T>(this IEnumerable<T?>? This) where T : struct {
            return This.Coalesce().Where(x => x.IsNotNull()).OfType<T>();
        }

        public static IEnumerable<string> WhereIsNotBlank(this IEnumerable<string?>? This) {
            return This.Coalesce<string?>().Where(x => x.IsNotBlank()).OfType<string>();
        }

        public static bool IsNotNull<T>(this T? This) where T : class {
            var ret = This is not null;

            return ret;
        }

        public static bool IsNotNull<T>(this T? This) where T : struct {
            var ret = This is not null;

            return ret;
        }

        public static bool IsNotBlank(this string? This) {
            var ret = !string.IsNullOrEmpty(This);

            return ret;
        }

        public static string Coalesce(this IEnumerable<string?>? This) {
            var ret = This.WhereIsNotNull().FirstOrDefault().Coalesce();

            return ret;
        }

        public static IEnumerable<T> Coalesce<T>(this IEnumerable<T>? This) {
            var ret = This ?? Enumerable.Empty<T>();

            return ret;
        }


        public static IEnumerable<string> SplitComma(this string? This) {
            var ret = This.Coalesce().Split(',');
            return ret;
        }

        public static string JoinComma(this IEnumerable<string?>? This) {
            var ret = string.Join(",", This.Coalesce());
            return ret;
        }

    }


}
