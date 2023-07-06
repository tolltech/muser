using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TolltechCore
{
    public static class ExceptionExtensions
    {
        private static readonly string[] linesDelimeter = { Environment.NewLine };
        private const string indentTemplate = "    ";

        public static string Dump(this Exception e)
        {
            var builder = new StringBuilder();
            e.Dump(builder);
            return builder.ToString();
        }

        public static string DumpStack(this Exception e)
        {
            var builder = new StringBuilder();
            e.DumpStack(builder);
            return builder.ToString();
        }

        public static string DumpMessage(this Exception exception)
        {
            var builder = new StringBuilder();
            DumpMessage(exception, builder, string.Empty, string.Empty);
            return builder.ToString();
        }

        public static Exception[] Unroll(this Exception exception, params Type[] undisclosedExeptionTypes)
        {
            if (exception == null)
            {
                return Array.Empty<Exception>();
            }

            var result = new List<Exception>();

            var queue = new Queue<Exception>();
            queue.Enqueue(exception);
            while (queue.Count > 0)
            {
                var currentExeption = queue.Dequeue();

                if (undisclosedExeptionTypes.Any(type => currentExeption.GetType() == type))
                {
                    result.Add(currentExeption);
                    continue;
                }

                if (currentExeption is AggregateException aggregateException)
                {
                    foreach (var innerExeptions in aggregateException.InnerExceptions)
                    {
                        queue.Enqueue(innerExeptions);
                    }
                }
                else
                {
                    result.Add(currentExeption);
                    if (currentExeption.InnerException != null)
                    {
                        queue.Enqueue(currentExeption.InnerException);
                    }
                }
            }

            return result.ToArray();
        }

        private static void Dump(this Exception e, StringBuilder builder)
        {
            builder.Append($"{e.GetType().FullName}[#{e.GetHashCode()}]");
            var message = e.Message;
            if (!string.IsNullOrEmpty(message))
            {
                builder.Append($": {message}");
            }

            e.DumpStack(builder);
        }

        private static void DumpStack(this Exception e, StringBuilder builder, bool isTopLevel = true)
        {
            var exception = e;

            while (true)
            {
                if (exception.StackTrace != null)
                {
                    if (isTopLevel)
                    {
                        builder.AppendLine(exception.StackTrace);
                    }
                    else
                    {
                        builder.AppendLine($"   --- Start of inner exception stack trace ({exception.GetType().Name}) ---> ");
                        builder.AppendLine(exception.StackTrace);
                        builder.AppendLine("   <--- End of inner exception stack trace ---");
                    }
                }

                exception = exception.InnerException;
                if (exception == null)
                {
                    return;
                }

                isTopLevel = false;
            }
        }

        private static void DumpMessage(Exception ex, StringBuilder builder, string indent, string prefix)
        {
            var message = ex.Message;
#if NET7_0_OR_GREATER
            // в .NET7 текст внутренних исключений стал добавляться в Message основного. Отрезаем его для сохранения старого поведения
            var bracketIndex = ex.Message.IndexOf("(", StringComparison.Ordinal);
            if (bracketIndex > 0)
            {
                message = message.Remove(bracketIndex - 1);
            }
#endif
            var messageLines = message.Split(linesDelimeter, StringSplitOptions.RemoveEmptyEntries);
            if (messageLines.Length > 0)
            {
                builder.Append(indent).Append(prefix).Append(ex.GetType().Name).Append(": ").AppendLine(messageLines[0]);
                for (var i = 1; i < messageLines.Length; i++)
                {
                    builder.AppendLine($"{indent}{messageLines[i]}");
                }
            }

            if (ex.InnerException == null)
            {
                return;
            }

            var innerExceptions = new List<Exception>();
            if (ex is AggregateException aggregateException)
            {
                innerExceptions.AddRange(aggregateException.InnerExceptions);
            }
            else
            {
                innerExceptions.Add(ex.InnerException);
            }

            var nextIndent = $"{indent}{indentTemplate}";
            for (var i = 0; i < innerExceptions.Count; i++)
            {
                DumpMessage(innerExceptions[i], builder, nextIndent, $"{prefix.Trim()}{i + 1}. ");
            }
        }
    }
}