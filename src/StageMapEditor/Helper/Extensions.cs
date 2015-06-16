using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StageMapEditor.Helper
{
    public static class ArrayExtemtions
    {
        public static IEnumerable<TResult> Select<T, TResult>(this T[,] self, Func<T, int, int, TResult> f)
        {
            return from y in Enumerable.Range(0, self.GetLength(0))
                   from x in Enumerable.Range(0, self.GetLength(1))
                   select f(self[y, x], y, x);
        }

        public static IEnumerable<TResult> Select<T, TResult>(this T[,] self, Func<T, TResult> f)
        {
            return from y in Enumerable.Range(0, self.GetLength(0))
                   from x in Enumerable.Range(0, self.GetLength(1))
                   select f(self[y, x]);
        }

        /// <summary>
        /// 二次元配列の行毎に対してコールバックを実行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="self"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> SelectRow<T, TResult>(this T[,] self, Func<IEnumerable<T>, TResult> f)
        {
            var h = self.GetLength(0);
            var w = self.GetLength(1);

            return Enumerable.Range(0, h).Select(r => f(Enumerable.Range(0, w).Select(c => self[r, c])));
        }

        /// <summary>
        /// 二次元配列の行毎に対してコールバックを実行。インデックスを使用。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="self"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> SelectRow<T, TResult>(this T[,] self, Func<IEnumerable<T>, int, TResult> f)
        {
            var h = self.GetLength(0);
            var w = self.GetLength(1);

            return Enumerable.Range(0, h).Select((r, i) => f(Enumerable.Range(0, w).Select(c => self[r, c]), i));
        }

        /// <summary>
        /// 2次元配列に格納されている値を列挙する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        public static IEnumerable<T> Sequential<T>(this T[,] self)
        {
            return from x in Enumerable.Range(0, self.GetLength(0))
                   from y in Enumerable.Range(0, self.GetLength(1))
                   select self[x, y];
        }

        /// <summary>
        /// 2次元配列に格納されている値と座標を引数にとるコールバックを実行する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="f">格納されている値、座標1、座標2を引数に取る関数</param>
        public static void Run<T>(this T[,] self, Action<T, int, int> f)
        {
            var actionList =
                from x in Enumerable.Range(0, self.GetLength(0))
                from y in Enumerable.Range(0, self.GetLength(1))
                select new Action(() => f(self[x, y], x, y));

            foreach (var action in actionList)
            {
                action();
            }
        }

        /// <summary>
        /// 2次元配列に格納されている値を引数にとるコールバックを実行する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="f">格納されている値を引数に取る関数</param>
        public static void Run<T>(this T[,] self, Action<T> f)
        {
            var actionList =
                from x in Enumerable.Range(0, self.GetLength(0))
                from y in Enumerable.Range(0, self.GetLength(1))
                select new Action(() => f(self[x, y]));

            foreach (var action in actionList)
            {
                action();
            }
        }

        /// <summary>
        /// 第一引数と第二引数の組み合わせを引数とする関数をすべてのパターンで実行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="source2"></param>
        /// <param name="action"></param>
        public static void MultiRun<T>(this T[] source, T[] source2, Action<T, T> action)
        {
            foreach (var x in source)
            {
                foreach (var y in source2)
                {
                    action(x, y);
                }
            }
        }

        /// <summary>
        /// 第一引数と第二引数の組み合わせを引数とする関数をすべてのパターンで実行し、その結果を返す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="source2"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> MultiSelect<T, TResult>(this T[] source, T[] source2, Func<T, T, TResult> func)
        {
            return from x in source
                   from y in source2
                   select func(x, y);
        }

        /// <summary>
        /// 文字列に連結します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToJoinedString<T>(this IEnumerable<T> source, string separator)
        {
            var i = 0;
            return source.Aggregate(new System.Text.StringBuilder(), (sb, x) => i++ == 0 ? sb.Append(x) : sb.Append(separator).Append(x)).ToString();
        }

        /// <summary>
        /// 文字列に連結します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToJoinedString<T>(this IEnumerable<T> source)
        {
            return source.ToJoinedString("");
        }

        /// <summary>
        /// 文字列に連結します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="tostring"> </param>
        /// <returns></returns>
        public static string ToJoinedString<T>(this IEnumerable<T> source, Func<T, string> tostring)
        {
            return source.Select(tostring).ToJoinedString("");
        }

        /// <summary>
        /// 文字列に連結します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="separator"> </param>
        /// <param name="tostring"> </param>
        /// <returns></returns>
        public static string ToJoinedString<T>(this IEnumerable<T> source, string separator, Func<T, string> tostring)
        {
            return source.Select(tostring).ToJoinedString(separator);
        }

        public static string ComputeHashString(this System.Security.Cryptography.MD5CryptoServiceProvider provider, string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var hash = provider.ComputeHash(bytes);
            return BitConverter.ToString(hash).ToLower().Replace("-", "");
        }
    }
}
