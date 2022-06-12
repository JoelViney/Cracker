using System.Linq.Expressions;

namespace Cracker.Rules
{
    internal interface IExceptionRule
    {
        public Type ExceptionType { get; }
        public bool Match(Exception ex);
    }

    internal class ExceptionRule<T> : IExceptionRule where T : Exception
    {
        public Type ExceptionType { get { return typeof(T); } }

        public Expression<Func<T, bool>>? Predicate { get; set; }

        public ExceptionRule()
        {
            Predicate = null;
        }

        public ExceptionRule(Expression<Func<T, bool>> predicate)
        {
            Predicate = predicate;
        }

        public bool Match(Exception ex)
        {
            if (Predicate == null)
            {
                return true;
            }

            var tee = ex as T;

            if (tee == null)
            {
                return false;
            }

            var x = Predicate.Compile();

            var result = x.Invoke(tee);

            return result;
        }
    }
}
