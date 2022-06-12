using System.Linq.Expressions;
using Cracker.Rules;

namespace Cracker.Wrappers
{
    /// <summary>
    /// Wrapper Base with Exceptions
    /// </summary>
    internal abstract class WrapperExBase : WrapperBase
    {
        public List<IExceptionRule> UnlessExceptions { get; set; } = new List<IExceptionRule>();
        public List<IExceptionRule> WhenExceptions { get; set; } = new List<IExceptionRule>();
        public List<IResultRule> WhenResults { get; set; } = new List<IResultRule>();
        

        public void AddWhenException<T>() where T : Exception
        {
            var exceptionRule = new ExceptionRule<T>();

            this.WhenExceptions.Add(exceptionRule);
        }

        public void AddWhenException<T>(Expression<Func<T, bool>> predicate) where T : Exception
        {
            var exceptionRule = new ExceptionRule<T>(predicate);

            this.WhenExceptions.Add(exceptionRule);
        }

        protected bool MatchingWhenExceptions(Exception ex)
        {
            var result = this.WhenExceptions.Any(x => x.ExceptionType == ex.GetType() && x.Match(ex));

            return result;
        }

        public void AddUnlessException<T>() where T : Exception
        {
            var exceptionRule = new ExceptionRule<T>();

            this.UnlessExceptions.Add(exceptionRule);
        }

        public void AddUnlessException<T>(Expression<Func<T, bool>> predicate) where T : Exception
        {
            var exceptionRule = new ExceptionRule<T>(predicate);

            this.UnlessExceptions.Add(exceptionRule);
        }

        protected bool MatchingUnlessExceptions(Exception ex)
        {
            var result = this.UnlessExceptions.Any(x => x.ExceptionType == ex.GetType() && x.Match(ex));

            return result;
        }

        protected bool MatchingWhenResult<T>(T result)
        {
            if (this.WhenResults.Count == 0)
            {
                return false;
            }

#pragma warning disable CS8604 // Possible null reference argument.
            var matches = this.WhenResults.Any(x => x.Match(result));
#pragma warning restore CS8604 // Possible null reference argument.

            return matches;
        }

        public void AddWhenResult<T>(Expression<Func<T, bool>> predicate) 
        {
            var resultRule = new ResultRule<T>(predicate);

            this.WhenResults.Add(resultRule);
        }

    }
}
