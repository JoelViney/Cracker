using System.Linq.Expressions;

namespace Cracker.Rules
{
    internal interface IResultRule
    {
        public bool Match(object result);
    }

    internal class ResultRule<T>: IResultRule
    {
        public Expression<Func<T, bool>>? Predicate { get; set; }


        public ResultRule(Expression<Func<T, bool>> predicate)
        {
            this.Predicate = predicate;
        }

        public bool Match(object? result)
        {
            if (Predicate == null)
            {
                return true;
            }

            try
            {
                T? tee = default;

                if (result != null)
                {
                    tee = (T)result;
                }

                var x = this.Predicate.Compile();

#pragma warning disable CS8604 // Possible null reference argument.
                return x.Invoke(tee);
#pragma warning restore CS8604 // Possible null reference argument.
            }
            catch
            {
                return false;
            }
        }
    }
}
